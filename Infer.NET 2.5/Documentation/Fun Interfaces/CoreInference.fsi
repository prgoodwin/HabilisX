///////////////////////////////////////////////////////////////////////////////
/// Inference functions
module MicrosoftResearch.Infer.Fun.Core.Inference

open Syntax

open MicrosoftResearch.Infer
open MicrosoftResearch.Infer.Distributions
open MicrosoftResearch.Infer.Models

type isTuple = bool

type CompoundDistribution =
    | Unit
    | Prod   of isTuple * Map<string, CompoundDistribution>
    | Array  of FunType * CompoundDistribution[] // The type of the array, for the case it is empty
    | Simple of obj // IDistribution does not have a parameterless base class.

    with
      member proj: int -> CompoundDistribution
      member Item: int -> CompoundDistribution with get
      member (?):  string -> CompoundDistribution
      /// For (Simple o) this is o.GetType(), and it distributes over the other constructors in the natural way
      member GetDistType: unit -> FunType 
      (* member simple: obj *)
 
val (|Tuple|_|):  CompoundDistribution -> CompoundDistribution[] option
val (|SimpleOrArray|_|): CompoundDistribution -> obj option
val (|Record|_|): string[] -> CompoundDistribution -> (CompoundDistribution[]) option
val (|Map|_|): 'Key[] -> Map<'Key,'Item> -> ('Item[]) option


(**
    Turn a CompoundDistribution into native distribution object.

    The input distribution d must be down-convertible to type 'a as follows:
    - Simple (PointMass(o: 'a)) is down-convertible to 'a.
    - Simple (d: 'a) is down-convertible to 'a.
    - Unit is down-convertible to unit, PointMass<unit>, and IDistribution<unit>
    - A structured distribution is down-convertible to a structured type if
      the components are down-convertible to component types.

    Cf. FSharp.Inference.infer.
*)
val fromCompound: CompoundDistribution -> 'a

////////
/// Setting inference parameters

val setEngine: InferenceEngine -> unit
val getEngine: unit -> InferenceEngine
val setVerbose : bool -> unit
val setShowFactorGraph : bool -> unit

///////////////////////////
/// Inference

type Model

val makeModel: Context -> main: string -> Model
(*
    Compilation happens on the first run of infer.
*)
val inferModel: Model -> args: e list -> CompoundDistribution

(**
    Additionally returns the probability of the model being true.

    Switching between inferModel and inferModelWithEvidence triggers recompilation.
*)
val inferModelWithEvidence: Model -> args: e list -> float * CompoundDistribution

val infer: Context -> e -> CompoundDistribution

val inferWithEvidence: Context -> e -> float * CompoundDistribution

///////////////////////////
/// Deprecated Inference


[<System.Obsolete("Use Core.Inference.infer instead")>]
val inferDynamic: Context -> e -> CompoundDistribution

[<System.Obsolete("Use Core.Inference.infer instead")>]
val inferVar: Variable<'a> -> IDistribution<'a>

[<System.Obsolete("Use Core.Inference.infer instead")>]
val interpFun1:     Context -> e -> Variable<'T>
[<System.Obsolete("Use Core.Inference.infer instead")>]
val interpFun2:     Context -> e -> Variable<'T1> * Variable<'T2>
[<System.Obsolete("Use Core.Inference.infer instead")>]
val interpFun3:     Context -> e -> Variable<'T1> * Variable<'T2> * Variable<'T3>
[<System.Obsolete("Use Core.Inference.infer instead")>]
val interpFun4:     Context -> e -> Variable<'T1> * Variable<'T2> * Variable<'T3> * Variable<'T4>

[<System.Obsolete("Use Core.Inference.infer instead")>]
val inferFun1:     Context -> e -> IDistribution<'T>
[<System.Obsolete("Use Core.Inference.infer instead")>]
val inferFun2:     Context -> e -> IDistribution<'T1> * IDistribution<'T2>
[<System.Obsolete("Use Core.Inference.infer instead")>]
val inferFun3:     Context -> e -> IDistribution<'T1> * IDistribution<'T2> * IDistribution<'T3>
[<System.Obsolete("Use Core.Inference.infer instead")>]
val inferFun4:     Context -> e -> IDistribution<'T1> * IDistribution<'T2> * IDistribution<'T3> * IDistribution<'T4>

///////////////////////////
/// Inference Debugging

module Debug = 

    type env = FunToFsharp.env
    type fsharp = string

    val transform: Context -> e -> e * FunType

    (*
        Perform only minimal transformations. 
    *)
    val transformMinimal: Context -> e -> e

    (*
        Infer with minimal transformations. 
    *)
    val inferMinimal: Context -> e -> CompoundDistribution

    val emptyEnv: unit -> env
    val inferenceCode: avoidRecompilation: bool -> e -> env -> fsharp
