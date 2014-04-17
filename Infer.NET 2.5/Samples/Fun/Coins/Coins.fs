
(*
  Shows inference of the posterior distribution of two coins,
  given that at least one of them is tails.
*)

/////////////////////////////////////////////////
// Model
/////////////////////////////////////////////////

open MicrosoftResearch.Infer.Fun.FSharp.Syntax

[<ReflectedDefinition>]
let coins p =
    let c1 = random (Bernoulli(p))
    let c2 = random (Bernoulli(p))
    let bothHeads = c1 && c2
    observe (bothHeads = false)
    c1, c2, bothHeads

/////////////////////////////////////////////////
// Sampling
/////////////////////////////////////////////////

// Sampling does not take observations into account.
printf "Sample: %O\n" (coins 0.5)

/////////////////////////////////////////////////
// Inference
/////////////////////////////////////////////////

open MicrosoftResearch.Infer.Fun.FSharp.Inference
open MicrosoftResearch.Infer.Distributions

let model = makeModel <@ coins @>

let (c1D, c2D, bothD): IDistribution<bool> * IDistribution<bool> * IDistribution<bool>
    = inferModel model 0.5
printf "coins distribution with p = 0.5: \n%O\n%O\n%O\n" c1D c2D bothD

let (c1D', c2D', bothD'): IDistribution<bool> * IDistribution<bool> * IDistribution<bool>
    = inferModel model 0.1
printf "coins distribution with p = 0.1: \n%O\n%O\n%O\n" c1D' c2D' bothD'
