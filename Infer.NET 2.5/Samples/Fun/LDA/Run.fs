module Run

open LDA
open InOut

open System.IO
open System.Collections.Generic

open MicrosoftResearch.Infer

let me = System.Reflection.Assembly.GetExecutingAssembly().Location
let dir = System.IO.Path.GetDirectoryName(me)
let path = Path.GetFullPath (dir + @"..\..\..\Test\")



let runCatsDogs() =

    let engine = new InferenceEngine(new VariationalMessagePassing())
    MicrosoftResearch.Infer.Fun.Core.Inference.setEngine engine

    // The results even for cats and dogs are rather sensitive to initialisation
    MicrosoftResearch.Infer.Maths.Rand.Restart(128)

    let numTopics = 2
    let alpha = 0.5
    let beta = 0.1

    let docs = readExcel (path + "CatsDogs.xlsx") "A1:A3" "B1:B3"

    let docs, vocab = encode docs
    let sizeVocab = vocabularySize vocab

    let docs, topics = infer numTopics sizeVocab alpha beta docs
    
    writeHTML path "CatsDogs" docs topics vocab

    let randomDocs = generate docs topics
    writeHTML path "CatsDogsRandom" randomDocs topics vocab

    printf "output written to %A\n" path

    // if on Windows, open explorer with path
    if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT
    then try
          System.Diagnostics.Process.Start("explorer.exe",path) |> ignore
         with _ -> ()
   


let runBlei () =

    let engine = new InferenceEngine(new VariationalMessagePassing())

    engine.NumberOfIterations <- 10

    MicrosoftResearch.Infer.Fun.Core.Inference.setEngine engine
    MicrosoftResearch.Infer.Fun.Core.Inference.setVerbose true
    MicrosoftResearch.Infer.Fun.Core.Inference.setShowFactorGraph true

    MicrosoftResearch.Infer.Maths.Rand.Restart(6)

    let docs = readWordCountsAsDoc (path + "ap.txt")

    let sizeVocab = vocabularySizeFromDocs docs

    let numTopics = 10

    let alpha = 150.0 / (float) numTopics
    let beta = 0.1

    let docs, topics = infer numTopics sizeVocab alpha beta docs

    let numWordsToPrint = 20
    let vocabulary = readVocabulary (path + "apvocab.txt")

    for i in 0 .. topics.Length - 1 do
        let pc = topics.[i].PseudoCount.ToArray()
        let wordIndices = [| for j in 0 .. pc.Length - 1 -> j |]

        System.Array.Sort(pc, wordIndices)

        System.Console.WriteLine("\nTop {0} words in topic {1}:\n", numWordsToPrint, i)

        for j in 1 .. numWordsToPrint do
            System.Console.Write("\t{0}\n", vocabulary.[wordIndices.[wordIndices.Length - j]])    


do runCatsDogs()
// do runBlei()

