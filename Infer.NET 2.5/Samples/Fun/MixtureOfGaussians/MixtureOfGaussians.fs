
(*
  This corresponds to the C# tutorial MixtureOfGaussians.cs.
*)

open MicrosoftResearch.Infer.Fun.FSharp.Syntax
open MicrosoftResearch.Infer.Fun.FSharp.Inference
open MicrosoftResearch.Infer.Fun.Lib

open MicrosoftResearch.Infer
open MicrosoftResearch.Infer.Maths

/////////////////////////////////////////////////
// Model
/////////////////////////////////////////////////

[<ReflectedDefinition>]
let priors () = 

    let means = [|for i in 0 .. 1 -> random(VectorGaussianFromMeanAndPrecision(VectorFromArray [|0.0; 0.0|], IdentityScaledBy(2,0.01)))|]
    let precs = [|for i in 0 .. 1 -> random(WishartFromShapeAndScale(100.0, IdentityScaledBy(2,0.01)))|]
    let weights = random(Dirichlet([|1.0; 1.0|]))

    (means, precs, weights)

[<ReflectedDefinition>]
let mix(means : Vector[], precs : PositiveDefiniteMatrix[], weights : Vector) = 

    let z = [|for i in 0 .. 300 -> breakSymmetry(random(Discrete(weights)))|]
    let data = [|for zi in z -> random(VectorGaussianFromMeanAndPrecision(means.[zi], precs.[zi]))|]
    data

[<ReflectedDefinition>]
let mixtureModel (data : Vector[]) =
    let (means, precs, weights) = priors ()
    observe(data = mix(means, precs, weights))
    (means, precs, weights)

/////////////////////////////////////////////////
// Data
/////////////////////////////////////////////////


let means = [| Vector.FromArray(2.0, 3.0); Vector.FromArray(7.0, 5.0) |]
let precs = [| new PositiveDefiniteMatrix(array2D [ [ 3.0; 0.2 ]; [ 0.2; 2.0 ] ]);
               new PositiveDefiniteMatrix(array2D [ [ 2.0; 0.4 ]; [ 0.4; 4.0 ] ])|]
let weights = Vector.FromArray(6.0, 4.0)

Rand.Restart(12347)

let data = mix(means, precs, weights)

/////////////////////////////////////////////////
// Inference
/////////////////////////////////////////////////

open MicrosoftResearch.Infer.Distributions

let engine = new InferenceEngine(new VariationalMessagePassing())
setEngine engine

let (meansD: IDistribution<Vector>[]), 
    (precsD: IDistribution<PositiveDefiniteMatrix>[]), 
    (weightsD: IDistribution<Vector>) = infer <@ mixtureModel @> data

printf "means: \n%A\n" meansD
printf "precs: \n%A\n" precsD
printf "weights: \n%A\n" weightsD
