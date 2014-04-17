// (C) Copyright 2009-2010 Microsoft Research Cambridge
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;

namespace MicrosoftResearch.Infer.Factors
{
	/// <summary>
	/// Provides outgoing messages for <see cref="Wishart.SampleFromShapeAndScale"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Wishart), "SampleFromShapeAndScale")]
	[Quality(QualityBand.Stable)]
	public static class WishartFromShapeAndScaleOp
	{
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,shape,scale))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(PositiveDefiniteMatrix sample, double shape, PositiveDefiniteMatrix scale)
		{
			Wishart to_sample = SampleAverageConditional(shape, scale);
			return to_sample.GetLogProb(sample);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,shape,scale))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(PositiveDefiniteMatrix sample, double shape, PositiveDefiniteMatrix scale) { return LogAverageFactor(sample, shape, scale); }
		[Skip]
		public static double LogEvidenceRatio(Wishart sample, double shape, PositiveDefiniteMatrix scale) { return 0.0; }
		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,shape,scale))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		public static double AverageLogFactor(PositiveDefiniteMatrix sample, double shape, PositiveDefiniteMatrix scale) { return LogAverageFactor(sample, shape, scale); }
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="to_sample">Outgoing message to 'sample'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,shape,scale))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(Wishart sample, [Fresh] Wishart to_sample)
		{
			return to_sample.GetLogAverageOf(sample);
		}

		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Wishart SampleAverageLogarithm(double shape, PositiveDefiniteMatrix scale)
		{
			return Wishart.FromShapeAndScale(shape, scale);
		}

		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Wishart SampleAverageConditional(double shape, PositiveDefiniteMatrix scale)
		{
			return Wishart.FromShapeAndScale(shape, scale);
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Wishart.SampleFromShapeAndRate"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Wishart), "SampleFromShapeAndRate", typeof(double), typeof(PositiveDefiniteMatrix))]
	[Quality(QualityBand.Preview)]
	public static class WishartFromShapeAndRateOp
	{
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,shape,rate))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(PositiveDefiniteMatrix sample, double shape, PositiveDefiniteMatrix rate)
		{
			int dimension = sample.Rows;
			Wishart to_sample = SampleAverageConditional(shape, rate, new Wishart(dimension));
			return to_sample.GetLogProb(sample);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(PositiveDefiniteMatrix sample, double shape, PositiveDefiniteMatrix rate) { return LogAverageFactor(sample, shape, rate); }
		[Skip]
		public static double LogEvidenceRatio(Wishart sample, double shape, PositiveDefiniteMatrix rate) { return 0.0; }
		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		public static double AverageLogFactor(PositiveDefiniteMatrix sample, double shape, PositiveDefiniteMatrix rate) { return LogAverageFactor(sample, shape, rate); }
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="to_sample">Outgoing message to 'sample'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,shape,scale))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(Wishart sample, [Fresh] Wishart to_sample)
		{
			return to_sample.GetLogAverageOf(sample);
		}

		public static double AverageLogFactor([Proper] Wishart sample, double shape, [Proper] Wishart rate)
		{
			// factor = (a-(d+1)/2)*logdet(X) -tr(X*B) + a*logdet(B) - GammaLn(a,d)
			int dimension = sample.Dimension;
			return (shape-(dimension+1)*0.5)*sample.GetMeanLogDeterminant() - Matrix.TraceOfProduct(sample.GetMean(), rate.GetMean()) + shape*rate.GetMeanLogDeterminant() - MMath.GammaLn(shape, dimension);
		}

		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'rate'.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Wishart SampleAverageLogarithm(double shape, PositiveDefiniteMatrix rate, Wishart result)
		{
			result.Shape = shape;
			result.Rate.SetTo(rate);
			return result;
		}

		public static Wishart SampleAverageLogarithm(double shape, [SkipIfUniform] Wishart rate, Wishart result)
		{
			result.Shape = shape;
			rate.GetMean(result.Rate);
			return result;
		}

		public static Wishart RateAverageLogarithm([SkipIfUniform] Wishart sample, double shape, Wishart result)
		{
			int dimension = result.Dimension;
			result.Shape = shape + 0.5*(dimension+1);
			sample.GetMean(result.Rate);
			return result;
		}

		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'rate'.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Wishart SampleAverageConditional(double shape, PositiveDefiniteMatrix rate, Wishart result)
		{
			return SampleAverageLogarithm(shape, rate, result);
		}
	}
}
