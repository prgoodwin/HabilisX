
(*
  An example of online learning where we have two datasets
  and use the posteriors inferred from the first dataset as priors
  for inference using the second dataset.
*)

open MicrosoftResearch.Infer.Fun.FSharp.Syntax
open MicrosoftResearch.Infer.Fun.FSharp.Inference
open MicrosoftResearch.Infer.Fun.Lib

open MicrosoftResearch.Infer
open MicrosoftResearch.Infer.Maths

/// Parameters of the model, see point(...) function below.
/// The type is generic, so it can represent both specific parameters (Params<float>)
/// and distributions over parameters (Params<IDistribution<float>>).
type Params<'T> = {a: 'T; b: 'T; invNoise: 'T}

/////////////////////////////////////////////////
// Model
/////////////////////////////////////////////////

/// Generates a 'y' from an 'x' using a linear model with stochastic noise. 
/// The parameter invNoise is (1 / noise), also called precision.
[<ReflectedDefinition>]
let point (x: float) {a = a; b = b; invNoise = invNoise}: float * float = 
  let y = random(GaussianFromMeanAndPrecision(a * x + b, invNoise))
  x, y

[<ReflectedDefinition>]
let parameters () =
  let a = random(GaussianFromMeanAndPrecision(0.0, 1.0))
  let b = random(GaussianFromMeanAndPrecision(5.0, 0.3))
  let invNoise = random (GammaFromShapeAndScale(1.0, 1.0))
  {a = a; b = b; invNoise = invNoise}

[<ReflectedDefinition>]
let model (data: (float * float)[], pp) =
  observe (data = [| for (x, _) in data -> point x pp |])
  pp

/////////////////////////////////////////////////
// Data
/////////////////////////////////////////////////

let nPoints = 3

let pp = parameters()

let data1 = 
    [| for x in 1 .. nPoints -> point (float x) pp |]

let data2 = 
    [| for x in 1 .. nPoints -> point (float x) pp |]

let data = (List.ofArray data1) @ (List.ofArray data2) |> List.toArray

/////////////////////////////////////////////////
// Plotting
/////////////////////////////////////////////////

open Microsoft.Office.Interop.Excel

// NB Make sure to plot some data points before calling this
let getMinMax (chart: Chart) = 
    // No way to draw an infinite line, 
    // so drawing explicitly to the bounds of the x axis instead
    let xAxis = chart.Axes(XlAxisType.xlCategory) :?> Axis
    xAxis.MinimumScaleIsAuto <- false
    xAxis.MaximumScaleIsAuto <- false
    xAxis.MinimumScale, xAxis.MaximumScale

let plotLine (chart: Chart) (a: float) (b: float) (name: string) = 

    let series = chart.SeriesCollection() :?> SeriesCollection

    let xMin, xMax = getMinMax chart

    let trueSeries = series.NewSeries()
    trueSeries.XValues <- [| xMin; xMax |]
    trueSeries.Values <- [| a * xMin + b; a * xMax + b |]
    trueSeries.ChartType <- XlChartType.xlXYScatterLinesNoMarkers
    trueSeries.Name <- name

let plotData (chart: Chart) (data: (float*float)[]) 
             (a: float) (b: float) 
             dataName lineName =

    let series = chart.SeriesCollection() :?> SeriesCollection

    let dataSeries = series.NewSeries()
    let dataX, dataY = Array.unzip data
    dataSeries.XValues <- dataX
    dataSeries.Values <- dataY
    dataSeries.ChartType <- XlChartType.xlXYScatter
    dataSeries.Name <- dataName

    let xMin, xMax = getMinMax chart

    plotLine chart a b lineName

let newChart () = 
    let xl = ApplicationClass()
    xl.Visible <- true

    let book = xl.Workbooks.Add() :> _Workbook
    let sheet = book.Sheets.Add() :?> _Worksheet

    let chartObjects = sheet.ChartObjects() :?> ChartObjects
    let chartObject  = chartObjects.Add(100.0, 50.0, 600.0, 600.0)
    chartObject.Chart


/////////////////////////////////////////////////
// Inference
/////////////////////////////////////////////////

open MicrosoftResearch.Infer.Distributions
open System.Diagnostics

let run () =
    printf "true a: %A\n" pp.a
    printf "true b: %A\n" pp.b
    printf "true noise (inverse): %A\n\n" pp.invNoise

    let stopWatch = Stopwatch.StartNew()

    let m = makeModel <@ model @>

    let priors: Params<IDistribution<float>> = infer <@ parameters @> () 
    let pD1: Params<IDistribution<float>> = inferModel m (data1, priors)

    printf "inferred parameters after observing first dataset:\n"
    printf "inferred a: %A\n" pD1.a
    printf "inferred b: %A\n" pD1.b
    printf "inferred noise (inverse): %A\n" pD1.invNoise
    printf "elapsed time: %O\n\n" stopWatch.Elapsed

    (*
        We cannot reuse the model here, as we pass a different distribution as
        parameter (not just a different constant value).

        Calling inferModel would trigger recompilation.
    *)
    let pD2: Params<IDistribution<float>> = infer <@ model @> (data2, pD1)

    printf "Inferred parameters after observing second dataset:\n"
    printf "inferred a: %A\n" pD2.a
    printf "inferred b: %A\n" pD2.b
    printf "inferred noise (inverse): %A\n" pD2.invNoise
    printf "elapsed time: %O\n\n" stopWatch.Elapsed

    (*
        Reusing the same model as for the first dataset here, 
        avoiding recompilation.
    *)
    let pD: Params<IDistribution<float>> = inferModel m (data, priors)


    printf "inferred parameters from observing both datasets simultaneously:\n"
    printf "inferred a: %A\n" pD.a
    printf "inferred b: %A\n" pD.b
    printf "inferred noise (inverse): %A\n" pD.invNoise
    printf "elapsed time: %O\n\n" stopWatch.Elapsed

    let chart = newChart ()
    plotData chart data1 
        ((pD1.a :?> CanGetMean<float>).GetMean()) 
        ((pD1.b :?> CanGetMean<float>).GetMean()) 
        "Data 1" "Inferred with data 1"

    plotData chart data2 
        ((pD2.a :?> CanGetMean<float>).GetMean()) 
        ((pD2.b :?> CanGetMean<float>).GetMean()) 
        "Data 2" "Updated with data 2"

    plotLine chart ((pD.a :?> CanGetMean<float>).GetMean())  
                   ((pD.b :?> CanGetMean<float>).GetMean()) 
                   "Inferred with data 1 and 2 simultaneously"

    plotLine chart pp.a pp.b "True Line"

    // Use layout 3 to see Excel's own linear fit
    chart.ApplyLayout(4)

do run ()
