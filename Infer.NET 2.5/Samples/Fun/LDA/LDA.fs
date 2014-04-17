
(*
    An implementation of Latent Dirichlet Allocation as described the C# example.
    This is a simple version that does not use repeat blocks yet and so is (much) less efficient.
*)

module LDA

open MicrosoftResearch.Infer.Maths
open MicrosoftResearch.Infer.Models
open MicrosoftResearch.Infer.Distributions

open MicrosoftResearch.Infer.Fun.FSharp.Syntax
open MicrosoftResearch.Infer.Fun.Core.Inference
open MicrosoftResearch.Infer.Fun.FSharp.Inference

open System
open System.Collections.Generic


/////////////////////////////////////////////////
// Types
/////////////////////////////////////////////////

type Token =
    | Word of string
    | NotWord of string

    override this.ToString() =
        match this with
        | Word s -> sprintf "Word \"%s\"" s
        | NotWord s -> sprintf "NotWord \"%s\"" s

type Doc<'T> = {
    // Could tokenize the title and use it for learning as well.
    title: string;
    text: Token[];
    words: int[];
    topics: 'T
}


/// A topic is a distribution over words
type Topic = Dirichlet // IDistribution<Vector>


/////////////////////////////////////////////////
// Model
/////////////////////////////////////////////////

(*
    Theta gives a distribution of topics in each document.
    Such a distribution is a vector whose all components are positive and sum up to 1, 
    in other words, a pie chart. The Dirichlet distribution is a distribution over pie charts.

    Similarly phi gives the distritubution of words in each topic.

    Alpha and beta are concentration parameters that govern the distribution of piecharts. 
    Values < 1.0 prefer concentrated distributions with a single prominent component. 
*)

[<ReflectedDefinition>]
let priors sizeVocab numTopics (alpha: float) (beta: float) (docs: Doc<unit>[]): Vector[] * Vector[] =

    let theta = [| for d in docs -> breakSymmetry(random(DirichletSymmetric(numTopics, alpha))) |]
    let phi = 
        [| for i in 0 .. numTopics - 1 -> 
            sparsity (Sparsity.ApproximateWithTolerance(0.00000000001)) 
                     (random(DirichletSymmetric(sizeVocab, beta))) |]
    theta, phi

[<ReflectedDefinition>]
let generateWords (theta: Vector[], phi: Vector[], docs: Doc<'T>[]): Doc<'T>[] =
    [| for d in range(docs) -> 
        let words = 
            [| for w in docs.[d].words -> 
                let topic = random(Discrete(theta.[d])) in 
                random(Discrete(phi.[topic])) |] 
        {docs.[d] with words = words} |]

(*
  Unfortunately record update is not polymorphic, so we have to use an explicit update function.
*)
[<ReflectedDefinition>]
let setTopics (doc: Doc<'T1>) (topics: 'T2) =
    {title = doc.title; text = doc.text; words = doc.words; topics = topics}


[<ReflectedDefinition>]
let model (sizeVocab: int, nTopics: int, alpha: float, beta: float, docs: Doc<unit>[]): 
          Doc<Vector>[] * Vector[] =

    let theta, phi = priors sizeVocab nTopics alpha beta docs
    let docsRandom = generateWords(theta, phi, docs)
    observe(docs = docsRandom)
    let docs = [| for d, t in Array.zip docs theta -> setTopics d t |]
    docs, phi



/////////////////////////////////////////////////
// Tokenizing text
/////////////////////////////////////////////////

let private isNonWord c = 
    (System.Char.IsWhiteSpace c || System.Char.IsPunctuation c) && not (System.Char.IsSymbol c) && not (c = '#')

let private stopWords = 
    [| "of"; "and"; "the"; "a"; "an"; "in"; "to"; "for"; "we"; "on"; "from"; "is"; "new";
        "that"; "with"; "can"; "will"; "as"; "such"; "s"; "we"; "are"; "you"; "these"; "so"; "this";
        "which"; "also"; "use"; "used"; "have"; "up"; "or"; "be"; "it"; "i"; "our"; "used"; "by"; "being";
        "what"; "do"; "as"; "they"; "way"; "how"; "your"; "now"; "more"; "than"; "their" |]

let private chars2token (cs: char list) = 
    let word = String.Concat cs
    if Array.exists (fun w -> w = word.ToLower()) stopWords then
        NotWord word
    else
        Word word

// may generate empty words, the caller must filter
let rec private tokenize_: char list -> char list -> Token list = fun word text ->
    match word, text with
    | word, c :: cs when isNonWord c -> 
        chars2token word :: NotWord (string c) :: tokenize_ [] cs
    | word, c :: cs -> 
        tokenize_ (word @ [c]) cs
    | word, [] -> [chars2token word]

let tokenize (text: string): Token[] =
    text.ToCharArray() |> List.ofArray |> tokenize_ [] |> List.filter (function | Word "" -> false | _ -> true) |> Array.ofList 
    // text.Split( [| ' '; '\n'; '\t'; '\r'; '.'; ','; ';'; '\"'|], StringSplitOptions.RemoveEmptyEntries )   


/////////////////////////////////////////////////
// Vocabulary
// A mapping between words and integers
/////////////////////////////////////////////////

type Vocabulary = Dictionary<string, int> * Dictionary<int, string>

/// Words that differ in case map to the same id.
let word2id (v: Vocabulary) (word: string): int =
    let ivocab, vocab = v
    let word = word.ToLower()
    if not (ivocab.ContainsKey(word)) then
        ivocab.[word] <- vocab.Count
        vocab.[vocab.Count] <- word
    ivocab.[word]

let id2word (v: Vocabulary) (id: int): string = 
    let ivocab, vocab = v in vocab.[id] 

let wordIds (v: Vocabulary) (tokens: Token[]): int[] =
    
    let getId = function
        | Word "" -> None
        | NotWord _ -> None
        | Word word -> Some (word2id v word)

    Array.choose getId tokens

let vocabularySize (v: Vocabulary): int =
    let ivocab, vocab = v
    vocab.Count

/// Fill in the words field from the text field.
let encode (docs: Doc<'T>[]): Doc<'T>[] * Vocabulary =
    let vocab = new Dictionary<string, int>(), new Dictionary<int, string>()
    let docs' = [| for doc in docs -> {doc with words = wordIds vocab doc.text} |]
    let (v, iv) = vocab

    // debug output
    use sw = new System.IO.StreamWriter("words.out")
    for word in v.Keys do
        fprintf sw "%s\n" word

    docs', vocab

/// Fill in the text field from the words field.
let decode docs vocab =         
    [| for doc in docs -> {doc with text = Array.map (function id -> Word (id2word vocab id)) doc.words } |]



/////////////////////////////////////////////////
// Inference
/////////////////////////////////////////////////

(*
    Return the distribution of topics in documents,
    the distribution of words in topics,
    and model evidence
*)

(*
let infer (numTopics: int) (sizeVocab: int)
          (alpha: float) (beta: float) (docs: Doc<unit>[]): Doc<Dirichlet>[] * Topic[] * float = 

    Console.WriteLine("************************************")
    Console.WriteLine("Vocabulary size = " + string sizeVocab)
    Console.WriteLine("Number of documents = " + string docs.Length)
    Console.WriteLine("Number of topics = " + string numTopics)
    Console.WriteLine("alpha = " + string alpha)
    Console.WriteLine("beta = " + string beta)
    Console.WriteLine("************************************")

    let evidence, 
        ((postDocs: Doc<Dirichlet>[]), (postPhi: Dirichlet[]))
            = inferWithEvidence <@ model @> (sizeVocab, numTopics, alpha, beta, docs)

    postDocs, postPhi, evidence
*)

let infer (numTopics: int) (sizeVocab: int)
          (alpha: float) (beta: float) (docs: Doc<unit>[]): Doc<Dirichlet>[] * Topic[] = 

    Console.WriteLine("************************************")
    Console.WriteLine("Vocabulary size = " + string sizeVocab)
    Console.WriteLine("Number of documents = " + string docs.Length)
    Console.WriteLine("Number of topics = " + string numTopics)
    Console.WriteLine("alpha = " + string alpha)
    Console.WriteLine("beta = " + string beta)
    Console.WriteLine("************************************")

    let ((postDocs: Doc<Dirichlet>[]), (postPhi: Dirichlet[]))
            = infer <@ model @> (sizeVocab, numTopics, alpha, beta, docs)

    postDocs, postPhi


/////////////////////////////////////////////////
// Generation
/////////////////////////////////////////////////

let generate (docs: Doc<Dirichlet>[]) (topics: Topic[]): Doc<Dirichlet>[] = 

    let theta = [| for doc in docs -> doc.topics.GetMean() |]
    let phi = [| for t in topics -> t.GetMean() |]
    let docs, vocab = encode docs

    let randomDocs = generateWords(theta, phi, docs)

    decode randomDocs vocab

