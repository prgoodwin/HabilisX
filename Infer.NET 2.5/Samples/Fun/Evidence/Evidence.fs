
(*
    Example of computing model evidence.
    See "Computing model evidence" in Infer.NET Fun documentation.
*)

open MicrosoftResearch.Infer.Fun.FSharp.Syntax
open MicrosoftResearch.Infer.Fun.FSharp.Inference

/////////////////////////////////////////////////
// Model
/////////////////////////////////////////////////

[<ReflectedDefinition>]
let model () =
    let evidence = random(Bernoulli(0.5))

    if evidence then
        let coin = random(Bernoulli(0.6))
        observe(coin = true)

    evidence

[<ReflectedDefinition>]
let basicModel () =
    let coin = random(Bernoulli(0.6))
    observe(coin = true)
    coin


/////////////////////////////////////////////////
// Inference
/////////////////////////////////////////////////

open MicrosoftResearch.Infer.Distributions

let evidenceD: Bernoulli = infer <@ model @> () 
printf "evidence: \n%O\n" evidenceD
printf "log odds: \n%O\n" evidenceD.LogOdds
printf "probability of model being true: \n%O\n" (exp evidenceD.LogOdds)

(*
    If you want to compute both the model value and the model evidence, you 
    need to use inferWithEvidence:
*)
let evidence, (coinD: Bernoulli) = inferWithEvidence <@ basicModel @> () 
printf "\ncoin distribution: \n%O\n" coinD
printf "probability of model being true: \n%O\n" evidence
