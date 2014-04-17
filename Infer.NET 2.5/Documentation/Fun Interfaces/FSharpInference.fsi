module MicrosoftResearch.Infer.Fun.FSharp.Inference

open Microsoft.FSharp.Quotations

open MicrosoftResearch.Infer
open MicrosoftResearch.Infer.Distributions
open MicrosoftResearch.Infer.Models

open MicrosoftResearch.Infer.Fun.Core.Syntax

module CoreInference = MicrosoftResearch.Infer.Fun.Core.Inference
module FunToFsharp = MicrosoftResearch.Infer.Fun.Core.FunToFsharp

////////
// Types

type CompoundDistribution = CoreInference.CompoundDistribution

////////
// Setting inference parameters

val setEngine: InferenceEngine -> unit
val getEngine: unit -> InferenceEngine
val setVerbose : bool -> unit
val setShowFactorGraph : bool -> unit

////////
// Compilation of F# expressions

val getSource: Expr -> Body

/// Get the syntax tree by introspection of F# code
val getCoreSyntax: Expr -> e

/// returns compiled versions of all reflected definitions in the currently loaded assembly
val getAssemblyContext: unit -> Context


///////////////////////////
/// Inference

type Model<'a, 'b>

val makeModel: Expr<'a -> 'b> -> Model<'a, 'b>
(*
    Compilation happens on the first run of infer.

    See infer below for the explanation of the relation between 'a and 'aa and 'b and 'bb.
*)
val inferModel: Model<'a, 'b> -> 'aa -> 'bb 

(*
    See Core.Inference.inferModelWithEvidence
*)
val inferModelWithEvidence: Model<'a, 'b> -> 'aa -> float * 'bb 

(*
  <summary> Inference. </summary>

  Provided an expression of type Expr<'a -> 'b> and input of type 'aa
  returns the representation of the distribution of outputs. The value of type 'aa
  may be either a specific value of type 'a, or a distribution 
  over the type 'a (hence a different type variable). It may also be
  a structured value (array, tuple, or record) in which each element is either
  a specific value or a distribution. The type 'bb may either be the type of values
  of type 'b (in which case the inference must produce a PointMass distribution)
  or the type of distributions over the type 'b (or a structured type as above). 

  See LinearRegressionOnline for an example of use.

  Formally, the type 'aa must be <emph>up-convertible</emph> to 'a and 'b must be 
  <emph>down-convertible</emph> to 'bb. These are defined as follows:

  - 'T is down-convertible and up-convertible to 'T for any simple type 'T
    (a type which is not a record, tuple, or array).
  - A type that supports HasPoint<'T> is down-convertible to 'T for any simple type 'T. 
  - A type that supports IDistribution<'T> is up-convertible to 'T for any simple type 'T.
  - A structured type 'T1 is up-convertible/down-convertible to a structured
    type 'T2 if all their component types are up-convertible/down-convertible
    to each other.
*)
val infer: Expr<'a -> 'b> -> 'aa -> 'bb 

(*
    See Core.Inference.inferWithEvidence
*)
val inferWithEvidence: Expr<'a -> 'b> -> 'aa -> float * 'bb 


////////
/// Deprecated Inference

[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val inferVar: IVariable -> IDistribution<'a>


[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val inferDynamic:  Expr<'a -> 'b> -> 'aa -> CompoundDistribution

[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val inferFun1: Expr<'a -> 'b> -> 'aa -> IDistribution<'b> 
[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val inferFun2: Expr<'a -> 'b * 'c> -> 'aa -> (IDistribution<'b> * IDistribution<'c>) 
[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val inferFun3: Expr<'a -> 'b * 'c * 'd> -> 'aa -> (IDistribution<'b> * IDistribution<'c> * IDistribution<'d>)  
[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val inferFun4: Expr<'a -> 'b * 'c * 'd * 'e> -> 'aa -> (IDistribution<'b> * IDistribution<'c> * IDistribution<'d> * IDistribution<'e>) 

[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val interpFun1: Expr<'a -> 'b> -> 'aa -> Variable<'b> 
[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val interpFun2: Expr<'a -> 'b * 'c> -> 'aa -> (Variable<'b> * Variable<'c>) 
[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val interpFun3: Expr<'a -> 'b * 'c * 'd> -> 'aa -> (Variable<'b> * Variable<'c> * Variable<'d>)  
[<System.Obsolete("Use FSharp.Inference.infer instead")>]
val interpFun4: Expr<'a -> 'b * 'c * 'd * 'e> -> 'aa -> (Variable<'b> * Variable<'c> * Variable<'d> * Variable<'e>) 

///////////////////////////
/// Inference Debug

module Debug = 

    type fsharp = string
    type env = FunToFsharp.env

    (*
        Use Core.Inference.transformMinimal.
    *)
    val transformMinimal: Expr<'a -> 'b> -> 'aa -> e

    (*
        Use Core.Inference.inferMinimal.
    *)
    val inferMinimal: Expr<'a -> 'b> -> 'aa -> CompoundDistribution

    val emptyEnv: unit -> env
    val inferenceCode: avoidRecompilation: bool -> Expr<'a -> 'b> -> 'aa -> env -> fsharp
