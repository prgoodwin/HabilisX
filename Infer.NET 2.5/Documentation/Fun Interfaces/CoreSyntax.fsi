module MicrosoftResearch.Infer.Fun.Core.Syntax

open System

///////////////////////////////////////////////////////////////////////////////
/// Expressions

type FunType = | TSimple of System.Type
               | TUnit
               | TRecord of Map<string, FunType> /// records and tuples 
               | TArray of FunType
               | TDist of FunType

/// Variables.
/// For non-local variables the name includes the module name.
type vname = string

/// Includes type if known.
type v = vname * FunType option

/// Operations
type UnaryOp = Negate | Not (* | Exp | Logistic *)

type BinaryOp = 
            | Plus | Minus | Mult | Div | Max | Mod
            | Or | And
            | Eq | Neq | Lt | Gt | LtEq | GtEq

val (|UNeedsParens|_|): UnaryOp -> UnaryOp option
val (|BNeedsParens|_|): BinaryOp -> BinaryOp option


/// distribution names
type DistName = 
    | Beta | BetaFromMeanAndVariance 
    | GaussianFromMeanAndPrecision | GaussianFromMeanAndVariance
    | GammaFromShapeAndScale | GammaFromMeanAndVariance | GammaFromShapeAndRate
    | Binomial
    | VectorGaussianFromMeanAndVariance | VectorGaussianFromMeanAndPrecision 
    | Discrete | DiscreteUniform
    | Poisson
    | Bernoulli
    | Dirichlet | DirichletUniform | DirichletSymmetric
    | WishartFromShapeAndScale

type CallInfo =
      /// Call a Fun function. 
      /// Function names are bound to bodies in Context (see below).
    | Internal of string
      /// Call an external function that has an associated factor. 
      /// See Lib.fsi for supported external functions and ways to add arbitrary new ones.
    | External of Reflection.MethodInfo

type constant =
    | B of bool
    | I of int
    | F of float
        /// An opaque object (say, Vector, PositiveDefiniteMatrix).
        /// The reason we require a type annotation is that
        /// o.GetType() returns the runtime type which due
        /// to inheritance may be more specific than the declared type 
        /// (DenseVector vs Vector). This leads to problems in the 
        /// interpreter because T :> T' does not imply Variable<T> :> Variable<T'>.
    | O of obj * System.Type


/// Projection/dereference
type selector = Field of string | Index of e

and e = 
        // Values
        | Unit
            /// Variables.
        | V of v
            /// Constants
        | C of constant
            /// Records.
            /// Tuples are represented as records 
            /// with fields Item1, Item2, ...
        | R of Map<string, e>
            /// Arrays.
            /// Contains the element type for the case the array is empty.
            /// An array element must be either an array (A ...) or an expression of non-array type.
            /// Thus arrays of comprehensions are not allowed.
        | A of e list * FunType option
            /// Range e = [| 0 .. e - 1 |].
        | Range of e
            /// RangeOf array = [| 0 .. array.Length - 1 |].
            /// In F# code use range(array).
        | RangeOf of e

        // Operations
            /// Projections: a.[i] or a.fieldName.
            /// Nested projections are allowed.
        | P    of e * selector 
        | UOp  of UnaryOp * e 
        | BOp  of BinaryOp * e * e

        // Control
        | If   of e * e * e
        | Call of CallInfo * e list
        | Let  of v * e * e
        | Seq  of e * e
       
        // Probabilistic expressions
        | D of DistName * e list
        /// Raw distribution object: MicrosoftResearch.Infer.Distributions.IDistribution<'a>.
        /// Contains result type 'a.
        | DRaw of FunType * obj
        | Observe of e
       
        // Array comprehensions
            /// for v in e1 do e2
            /// The iterator name can be arbitrary
        | Iter of v * e * e 
            /// [|for v in e1 -> e2|]
            /// The iterator name can be arbitrary
        | Map of v * e * e
        | Zip of e * e

#if NOFOLD
#else
        // Array fold - only for Filzbach for now
        | Fold of v * v * e * e * e
#endif
        /// annotate(a){e}
        /// In Fun semantics annotate(a){e} is the same as e, 
        /// but annotations are used to drive Infer.NET correctly. 
        /// Most annotations are inserted by program transformations, 
        /// unless explicitly listed as user-provided.
        | Annotation of annotation * e 


and annotation = 
            /// <summary> switch(e){e'}.</summary>
            /// In Infer.NET interpretation switch(e){e'} is interpreted as 
            /// Variable.Switch(interp e){interp e'},
            /// thus allowing e to be used as index in e'.
            /// See Transformations.insertSwitches for details.
        | Switch of e
            /// <summary> Expression copy.</summary>
            /// In Infer.NET interpretation copy{e} is interpreted as Variable.Copy(interpet e).
            /// Making copies is necessary sometimes because Infer.NET assignment (Variable.SetTo()) 
            /// consumes the argument and makes it unusable, so we need to copy the argument first.
            /// See Transformations.insertCopies for details.
        | Copy
            /// <summary> Range annotations, ranges(vr, dr, [r1, ..., rn]){e}.</summary>
            /// Contains information about the value range, the dimension range, and the array ranges
            /// of an expression.
            /// 
            /// For an integer or integer array e, 
            /// vr specifies the value range that each element of e is from.
            ///
            /// A vector or array of vectors of dimension n will be annotated with dr = [0 .. n - 1].
            ///
            /// For any array r1, ..., rn will be the ranges of the array, one for each dimension.
            ///
            /// See Ranges.fsi for more details.
        | Ranges of v option * v option * v list
            /// <summary> User-provided range annotation, sameRanges(e){e'}.
            /// Use (sameRanges e e') in F# code.
            /// The array ranges of e are constrained to be equal to the array ranges of e'.
            /// </summary>
        | SameRanges of e
            /// <summary> User-provided dimension annotation, hasDimension(eR){e}.
            /// Use (hasDimension eR e) in F# code.
            /// The dimension of e is set to be equal to eR which is expected to be a range expression. 
            /// </summary>
            /// <example> let max = hasDimension (range score) (softmax score) </example>
        | HasDimension of e
            /// <summary> Range cloning annotation, clone{e}.
            /// Tells us that the range expression e needs to be cloned. 
            /// </summary>
            /// See Transformations.insertRanges for details.
        | CloneRange
            /// <summary> User-provided symmetry breaking request, breakSymmetry{e}.
            /// Use (breakSymmetry e) in F# code.
            /// Requests that an appropriate InitalizeTo annotation should be inserted.
            /// </summary>
            /// See Transformations.breakSymmetry for details.
        | BreakSymmetry
            /// <summary> Initialization annotation, initializeTo(dist){e}.</summary>
            /// Translates to (interpret e).InitalizeTo(dist).
            /// See Transformations.breakSymmetry for details.
        | InitialiseTo of obj
            /// <summary> User-provided sparsity annotation, sparsity(sp){e}.
            /// Use (sparsity sp e) in F# code.
            /// Translates to (interpret e).SetSparsity(sp).
            /// </summary>
        | Sparsity of obj


type Body =
        | Lambda of v list * e
        | Value  of e

type Context = Map<vname, Body>

////////////////////////////////////////////////////////////////////////////////
/// Typing

/// Type conversion
val systemTypeToFunType: System.Type -> FunType

val funTypeToSystemType: FunType -> System.Type

val getDistributionElementType: DistName -> FunType

/// Fills in missing variable types.
/// Assumes a closed expression where all internal calls have been inlined. 
/// See Transformations.inlineContext.
val inferTypes: e -> e

/// Call inferTypes first.
val getType: e -> FunType

////////////////////////////////////////////////////////////////////////////////
/// Traversal

val children: e -> e list

// val replaceChildren: e list -> e -> e

/// Apply a function to all children
val descend: (e -> e) -> e -> e

////////////////////////////////////////////////////////////////////////////////
/// Helpers

/// Tuple field names
val proj: int -> string

val distName: string -> DistName

val vname: v -> vname



