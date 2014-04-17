﻿module MicrosoftResearch.Infer.Fun.FSharp.Syntax

open MicrosoftResearch.Infer.Fun.Core.Syntax

open System

///////////////////////////////////////////////////////////////////////////////
// Distributions

open MicrosoftResearch.Infer.Maths

type Distribution<'a>

// floating point distributions

/// true and false count
val Beta: float * float -> Distribution<float>  
val BetaFromMeanAndVariance: float * float -> Distribution<float>  
val GaussianFromMeanAndPrecision: float * float -> Distribution<float>  
val GaussianFromMeanAndVariance: float * float -> Distribution<float>  
val GammaFromShapeAndScale: float * float -> Distribution<float>  
val GammaFromShapeAndRate: float * float -> Distribution<float>  
val GammaFromMeanAndVariance: float * float -> Distribution<float>  


// integer distributions

 /// p(i) = v_i
val Discrete: Vector -> Distribution<int>  
/// discrete uniform on 0..n-1; p(i) = 1/n
val DiscreteUniform: int -> Distribution<int>  
/// subprocess and trial count
val Binomial: int * float -> Distribution<int>  
/// mean
val Poisson: float -> Distribution<int>  

// boolean distributions

/// p(true)
val Bernoulli: float -> Distribution<bool>  

// multivariate (vector) distributions

/// pseudo-counts
val Dirichlet: float[] -> Distribution<Vector>  
/// symmetric Dirichlet
val DirichletSymmetric: int * float -> Distribution<Vector>  
 /// uniform Dirichlet of dimension n
val DirichletUniform: int -> Distribution<Vector>  
/// variance and covariance
val VectorGaussianFromMeanAndVariance: Vector * PositiveDefiniteMatrix -> Distribution<Vector>  
val VectorGaussianFromMeanAndPrecision: Vector * PositiveDefiniteMatrix -> Distribution<Vector>  

// matrix distributions
val WishartFromShapeAndScale: Double * PositiveDefiniteMatrix -> Distribution<PositiveDefiniteMatrix>  

///////////////////////////////////////////////////////////////////////////////
// Language primitives

/// Sampling
val random: Distribution<'a> -> 'a

/// <summary> Observation. </summary>
/// Calling observe in F# directly has no effect. If you would like to sample from a Fun function
/// while respecting observations, use the <see cref="sample"/> function.
val observe: bool -> unit

/// Array range
val range: 'a[] -> int[]

/// Identity in sampling semantics, but suggests to the inference engine 
/// that symmetry of the argument expression needs to be broken. 
/// Put this around a random() call like breakSymmetry(random(Discrete(...))).
val breakSymmetry: 'a -> 'a

/// sameRanges a b returns b and constrains the array ranges of a and b to the be same
/// for the purpose of range inference.
val sameRanges: 'a -> 'b -> 'b

/// hasDimension dim b returns b and constrains the dimension of b to be equal to dim
/// for the purpose of range inference.
/// Example of use:
/// let max = hasDimension (range score) (softmax score)
val hasDimension: 'a -> 'b -> 'b

/// Sparsity annotation
/// Example of use (from LDA):
///    let phi = 
///        [| for i in 0 .. numTopics - 1 -> 
///            sparsity (Sparsity.ApproximateWithTolerance(0.00000000001)) 
///                     (random(DirichletSymmetric(sizeVocab, beta))) |]
val sparsity: MicrosoftResearch.Infer.Maths.Sparsity -> 'a -> 'a

///////////////////////////////////////////////////////////////////////////////
// Sampling semantics of Fun programs.

/// Given a Fun function, sample from it, and return None if an observation fails.
val sample: ('a -> 'b) -> 'a -> 'b option

/// Given a Fun function, sample from it n times, and return the list of successful outcomes 
/// (outcomes in which no observation fails).
val sampleMany: ('a -> 'b) -> int -> 'a -> 'b list

///////////////////////////////////////////////////////////////////////////////
/// Converting F# to Fun expressions

module Reflection = 

    /// Type conversion
    val systemTypeToFunType: System.Type -> FunType

    val unquote : Microsoft.FSharp.Quotations.Expr -> e

    /// unquote a lambda expression
    val unquoteBody: Microsoft.FSharp.Quotations.Expr -> Body

    /// turn a run-time value into an expression of given type
    val unquoteData: FunType -> obj -> e
