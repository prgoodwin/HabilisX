// (C) Copyright 2008 Microsoft Research Cambridge
using System;
using System.Collections.Generic;
using System.Text;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;
using MicrosoftResearch.Infer.Utils;

namespace MicrosoftResearch.Infer.Factors
{
	public class GaussianOpBase
	{
		//-- Easy cases ----------------------------------------------------------------------------------------

		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Gaussian SampleAverageConditional(double mean, double precision)
		{
			return Gaussian.FromMeanAndPrecision(mean, precision);
		}
		/// <summary>
		/// EP message to 'mean'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing EP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'mean' conditioned on the given values.
		/// </para></remarks>
		public static Gaussian MeanAverageConditional(double sample, double precision)
		{
			return SampleAverageConditional(sample, precision);
		}

		/// <summary>
		/// EP message to 'precision'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <returns>The outgoing EP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'precision' conditioned on the given values.
		/// </para></remarks>
		public static Gamma PrecisionAverageConditional(double sample, double mean)
		{
			double diff = sample - mean;
			return Gamma.FromShapeAndRate(1.5, 0.5 * diff * diff);
		}

		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'sample' as the random arguments are varied.
		/// The formula is <c>proj[p(sample) sum_(mean) p(mean) factor(sample,mean,precision)]/p(sample)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static Gaussian SampleAverageConditional([SkipIfUniform] Gaussian mean, double precision)
		{
			if (mean.IsPointMass) return SampleAverageConditional(mean.Point, precision);
			// if (precision < 0) throw new ArgumentException("The constant precision given to the Gaussian factor is negative", "precision");
			if (precision == 0) {
				return Gaussian.Uniform();
			} else if (double.IsPositiveInfinity(precision)) {
				return mean;
			} else {
				if (mean.Precision <= -precision) throw new ImproperMessageException(mean);
				// The formula is int_mean N(x;mean,1/prec) p(mean) = N(x; mm, mv + 1/prec)
				// sample.Precision = inv(mv + inv(prec)) = mprec*prec/(prec + mprec)
				// sample.MeanTimesPrecision = sample.Precision*mm = R*(mprec*mm)
				// R = Prec/(Prec + mean.Prec)
				// This code works for mean.IsUniform() since then mean.Precision = 0, mean.MeanTimesPrecision = 0
				Gaussian result = new Gaussian();
				double R = precision / (precision + mean.Precision);
				result.Precision = R * mean.Precision;
				result.MeanTimesPrecision = R * mean.MeanTimesPrecision;
				return result;
			}
		}
		/// <summary>
		/// EP message to 'mean'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing EP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'mean' as the random arguments are varied.
		/// The formula is <c>proj[p(mean) sum_(sample) p(sample) factor(sample,mean,precision)]/p(mean)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageConditional([SkipIfUniform] Gaussian sample, double precision)
		{
			return SampleAverageConditional(sample, precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,precision))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(double sample, double mean, double precision)
		{
			return Gaussian.GetLogProb(sample, mean, 1.0 / precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample,mean) p(sample,mean) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static double LogAverageFactor([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, double precision)
		{
			return GaussianFromMeanAndVarianceOp.LogAverageFactor(sample, mean, 1.0 / precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		public static double LogAverageFactor([SkipIfUniform] Gaussian sample, double mean, double precision)
		{
			return LogAverageFactor(sample, Gaussian.PointMass(mean), precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(mean) p(mean) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static double LogAverageFactor(double sample, [SkipIfUniform] Gaussian mean, double precision)
		{
			//if(mean.IsPointMass) return LogAverageFactor(sample,mean.Point,precision);
			return LogAverageFactor(Gaussian.PointMass(sample), mean, precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(precision) p(precision) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogAverageFactor(double sample, double mean, [SkipIfUniform] Gamma precision)
		{
			if (precision.IsPointMass) return LogAverageFactor(sample, mean, precision.Point);
			if (precision.IsUniform()) return Double.PositiveInfinity;
			return TPdfLn(sample - mean, 2 * precision.Rate, 2 * precision.Shape + 1);
		}

		/// <summary>
		/// Logarithm of Student T density.
		/// </summary>
		/// <param name="x">sample</param>
		/// <param name="v">variance parameter</param>
		/// <param name="n">degrees of freedom plus 1</param>
		/// <returns></returns>
		public static double TPdfLn(double x, double v, double n)
		{
			return MMath.GammaLn(n * 0.5) - MMath.GammaLn((n - 1) * 0.5) - 0.5 * Math.Log(v * Math.PI) - 0.5 * n * Math.Log(1 + x * x / v);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(double sample, double mean, double precision)
		{
			return LogAverageFactor(sample, mean, precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample,mean) p(sample,mean) factor(sample,mean,precision) / sum_sample p(sample) messageTo(sample))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		[Skip]
		public static double LogEvidenceRatio(Gaussian sample, Gaussian mean, double precision)
		{
			return 0.0;
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,mean,precision) / sum_sample p(sample) messageTo(sample))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		[Skip]
		public static double LogEvidenceRatio(Gaussian sample, double mean, double precision)
		{
			return 0.0;
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(mean) p(mean) factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(double sample, [SkipIfUniform] Gaussian mean, double precision)
		{
			return LogAverageFactor(sample, mean, precision);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(precision) p(precision) factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogEvidenceRatio(double sample, double mean, [SkipIfUniform] Gamma precision)
		{
			return LogAverageFactor(sample, mean, precision);
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.Gaussian"/> and <see cref="Gaussian.Sample(double,double)"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Gaussian), "Sample", typeof(double), typeof(double), Default=true)]
	[FactorMethod(new string[] { "sample", "mean", "precision" }, typeof(Factor), "Gaussian", Default=true)]
	[Quality(QualityBand.Mature)]
	public class GaussianOp : GaussianOpBase
	{
		//-- TruncatedGaussian ---------------------------------------------------------------------------------

		public static TruncatedGaussian SampleAverageConditional(double mean, double precision, TruncatedGaussian result)
		{
			return TruncatedGaussian.FromGaussian(Gaussian.FromMeanAndPrecision(mean, precision));
		}
		public static TruncatedGaussian MeanAverageConditional(double sample, double precision, TruncatedGaussian result)
		{
			return SampleAverageConditional(sample, precision, result);
		}

		//-- Gibbs ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Evidence message for Gibbs
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Logarithm of the factor's value at the given arguments</returns>
		/// <remarks><para>
		/// 
		/// </para></remarks>
		public static double LogFactorValue(double sample, double mean, double precision)
		{
			return Gaussian.GetLogProb(sample, mean, 1.0 / precision);
		}

		//-- EP ------------------------------------------------------------------------------------------------
		/// <summary>
		/// Static flag to force a proper distribution
		/// </summary>
		public static bool ForceProper = true;

		/// <summary>
		/// Number of quadrature nodes to use for computing the messages.
		/// Reduce this number to save time in exchange for less accuracy.
		/// </summary>
		public static int QuadratureNodeCount = 50;

		public static bool modified = false;

		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'sample' as the random arguments are varied.
		/// The formula is <c>proj[p(sample) sum_(mean,precision) p(mean,precision) factor(sample,mean,precision)]/p(sample)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian SampleAverageConditional(Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			if (sample.IsUniform() && precision.Shape <= 1.0) sample = Gaussian.FromNatural(1e-20, 1e-20);
			if (precision.IsPointMass) {
				return SampleAverageConditional(mean, precision.Point);
			} else if (sample.IsUniform()) {
				// for large vx, Z =approx N(mx; mm, vx+vm+E[1/prec])
				double mm,mv;
				mean.GetMeanAndVariance(out mm, out mv);
				// NOTE: this error may happen because sample didn't receive any message yet under the schedule.
				// Need to make the scheduler smarter to avoid this.
				if (precision.Shape <= 1.0) throw new ArgumentException("The posterior has infinite variance due to precision distributed as "+precision+" (shape <= 1).  Try using a different prior for the precision, with shape > 1.");
				return Gaussian.FromMeanAndVariance(mm, mv + precision.GetMeanInverse());
			} else if (mean.IsUniform() || precision.IsUniform()) {
				return Gaussian.Uniform();
			} else if (sample.IsPointMass) {
				// The correct answer here is not uniform, but rather a limit.  
				// However it doesn't really matter what we return since multiplication by a point mass 
				// always yields a point mass.
				return Gaussian.Uniform();
			} else if (!precision.IsProper()) {
				throw new ImproperMessageException(precision);
			} else {
				// The formula is int_prec int_mean N(x;mean,1/prec) p(x) p(mean) p(prec) =
				// int_prec N(x; mm, mv + 1/prec) p(x) p(prec) =
				// int_prec N(x; new xm, new xv) N(xm; mm, mv + xv + 1/prec) p(prec)
				// Let R = Prec/(Prec + mean.Prec)
				// new xv = inv(R*mean.Prec + sample.Prec)
				// new xm = xv*(R*mean.PM + sample.PM)

				// In the case where sample and mean are improper distributions, 
				// we must only consider values of prec for which (new xv > 0).
				// This happens when R*mean.Prec > -sample.Prec
				// As a function of Prec, R*mean.Prec has a singularity at Prec=-mean.Prec
				// This function is greater than a threshold when Prec is sufficiently small or sufficiently large.
				// Therefore we construct an interval of Precs to exclude from the integration.
				double xm, xv, mm, mv;
				sample.GetMeanAndVarianceImproper(out xm, out xv);
				mean.GetMeanAndVarianceImproper(out mm, out mv);
				double lowerBound = 0;
				double upperBound = Double.PositiveInfinity;
				bool precisionIsBetween = true;
				if (mean.Precision >= 0) {
					if (sample.Precision < -mean.Precision) throw new ImproperMessageException(sample);
					//lowerBound = -mean.Precision * sample.Precision / (mean.Precision + sample.Precision);
					lowerBound = -1.0 / (xv + mv);
				} else {  // mean.Precision < 0
					if (sample.Precision < 0) {
						precisionIsBetween = true;
						lowerBound = -1.0 / (xv + mv);
						upperBound = -mean.Precision;
					} else if (sample.Precision < -mean.Precision) {
						precisionIsBetween = true;
						lowerBound = 0;
						upperBound = -mean.Precision;
					} else {
						// in this case, the precision should NOT be in this interval.
						precisionIsBetween = false;
						lowerBound = -mean.Precision;
						lowerBound = -1.0 / (xv + mv);
					}
				}
				double[] nodes = new double[QuadratureNodeCount];
				double[] logWeights = new double[nodes.Length];
				Gamma precMarginal = precision*to_precision;
				QuadratureNodesAndWeights(precMarginal, nodes, logWeights);
				if (!to_precision.IsUniform()) {
					// modify the weights
					for (int i = 0; i < logWeights.Length; i++) {
						logWeights[i] += precision.GetLogProb(nodes[i]) - precMarginal.GetLogProb(nodes[i]);
					}
				}
				GaussianEstimator est = new GaussianEstimator();
				double shift = 0;
				for (int i = 0; i < nodes.Length; i++) {
					double newVar, newMean;
					Assert.IsTrue(nodes[i] > 0);
					if ((nodes[i] > lowerBound && nodes[i] < upperBound) != precisionIsBetween) continue;
					// the following works even if sample is uniform. (sample.Precision == 0)
					if (mean.IsPointMass) {
						// take limit mean.Precision -> Inf
						newVar = 1.0 / (nodes[i] + sample.Precision);
						newMean = newVar * (nodes[i] * mean.Point + sample.MeanTimesPrecision);
					} else {
						// mean.Precision < Inf
						double R = nodes[i] / (nodes[i] + mean.Precision);
						newVar = 1.0 / (R * mean.Precision + sample.Precision);
						newMean = newVar * (R * mean.MeanTimesPrecision + sample.MeanTimesPrecision);
					}
					double lp = Gaussian.GetLogProb(xm, mm, xv + mv + 1.0 / nodes[i]);
					if (i == 0) shift = lp;
					double f = Math.Exp(logWeights[i] + lp - shift);
					est.Add(Gaussian.FromMeanAndVariance(newMean, newVar), f);
				}
				double Z = est.mva.Count;
				if (double.IsNaN(Z)) throw new Exception("Z is nan");
				if (Z == 0.0) {
					throw new Exception("Quadrature found zero mass");
				}
				Gaussian result = est.GetDistribution(new Gaussian());
				if (modified && !sample.IsUniform()) {
					// heuristic method to avoid improper messages:
					// the message's mean must be E[mean] (regardless of context) and the variance is chosen to match the posterior mean when multiplied by context
					double sampleMean = result.GetMean();
					if (sampleMean != mm) {
						result.Precision = (sample.MeanTimesPrecision-sampleMean*sample.Precision)/(sampleMean - mm);
						if (result.Precision < 0) throw new Exception("internal: sampleMean is not between sample.Mean and mean.Mean");
						result.MeanTimesPrecision = result.Precision*mm;
					}
				} else {
					if (result.IsPointMass) throw new Exception("Quadrature found zero variance");
					result.SetToRatio(result, sample, ForceProper);
				}
				return result;
			}
		}
		public static Gaussian SampleAverageConditional_slow(Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			if (precision.IsUniform()) return Gaussian.Uniform();
			Gamma to_precision = PrecisionAverageConditional_slow(sample, mean, precision);
			return SampleAverageConditional(sample, mean, precision, to_precision);
		}
		public static Gaussian MeanAverageConditional_slow([SkipIfUniform] Gaussian sample, Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			return SampleAverageConditional_slow(mean, sample, precision);
		}
		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'sample' as the random arguments are varied.
		/// The formula is <c>proj[p(sample) sum_(precision) p(precision) factor(sample,mean,precision)]/p(sample)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian SampleAverageConditional(Gaussian sample, double mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			return SampleAverageConditional(sample, Gaussian.PointMass(mean), precision, to_precision);
		}
		[Skip]
		public static Gaussian SampleAverageConditionalInit()
		{
			return Gaussian.Uniform();
		}
		/// <summary>
		/// EP message to 'mean'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'mean' as the random arguments are varied.
		/// The formula is <c>proj[p(mean) sum_(sample,precision) p(sample,precision) factor(sample,mean,precision)]/p(mean)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageConditional([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			// TM: added SkipIfUniform to mean to encourage good schedules.
			// The result depends on mean, but can be non-uniform even if mean is uniform.
			return SampleAverageConditional(mean, sample, precision, to_precision);
		}
		/// <summary>
		/// EP message to 'mean'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'mean' as the random arguments are varied.
		/// The formula is <c>proj[p(mean) sum_(precision) p(precision) factor(sample,mean,precision)]/p(mean)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageConditional(double sample, Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			return SampleAverageConditional(mean, sample, precision, to_precision);
		}

#if false
		/// <summary>
		/// EP message to 'precision'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'precision' as the random arguments are varied.
		/// The formula is <c>proj[p(precision) sum_(mean) p(mean) factor(sample,mean,precision)]/p(precision)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gamma PrecisionAverageConditional(double sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			return PrecisionAverageConditional(Gaussian.PointMass(sample), mean, precision, to_precision);
		}
		/// <summary>
		/// EP message to 'precision'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'precision' as the random arguments are varied.
		/// The formula is <c>proj[p(precision) sum_(sample) p(sample) factor(sample,mean,precision)]/p(precision)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gamma PrecisionAverageConditional([SkipIfUniform] Gaussian sample, double mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			return PrecisionAverageConditional(sample, Gaussian.PointMass(mean), precision, to_precision);
		}
#endif

		public static Gamma PrecisionAverageConditional_slow([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			Gamma to_precision = Gamma.Uniform();
			for (int iter = 0; iter < 100; iter++) {
				Gamma result = PrecisionAverageConditional(sample, mean, precision, to_precision);
				if (result.MaxDiff(to_precision) < 1e-10) break;
				to_precision = result;
				//Console.WriteLine("[{0}] {1}", iter, to_precision);
			}
			return to_precision;
		}

		/// <summary>
		/// EP message to 'precision'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'precision' as the random arguments are varied.
		/// The formula is <c>proj[p(precision) sum_(sample,mean) p(sample,mean) factor(sample,mean,precision)]/p(precision)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gamma PrecisionAverageConditional([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			if (sample.IsPointMass && mean.IsPointMass)
				return PrecisionAverageConditional(sample.Point, mean.Point);

			Gamma result = new Gamma();
			if (precision.IsPointMass) {
				// The correct answer here is not uniform, but rather a limit.  
				// However it doesn't really matter what we return since multiplication by a point mass 
				// always yields a point mass.
				result.SetToUniform();
			} else if (sample.IsUniform() || mean.IsUniform()) {
				result.SetToUniform();
			} else if (!precision.IsProper()) {
				// improper prior
				throw new ImproperMessageException(precision);
			} else {
				//to_precision.SetToUniform(); // TEMPORARY
				// use quadrature to integrate over the precision
				// see LogAverageFactor
				double xm, xv, mm, mv;
				sample.GetMeanAndVarianceImproper(out xm, out xv);
				mean.GetMeanAndVarianceImproper(out mm, out mv);
				double upperBound = Double.PositiveInfinity;
				if (xv + mv < 0) upperBound = -1.0 / (xv + mv);
				double[] nodes = new double[QuadratureNodeCount];
				double[] logWeights = new double[nodes.Length];
				Gamma precMarginal = precision*to_precision;
				QuadratureNodesAndWeights(precMarginal, nodes, logWeights);
				if (!to_precision.IsUniform()) {
					// modify the weights
					for (int i = 0; i < logWeights.Length; i++) {
						logWeights[i] += precision.GetLogProb(nodes[i]) - precMarginal.GetLogProb(nodes[i]);
					}
				}
				double shift = 0;
				MeanVarianceAccumulator mva = new MeanVarianceAccumulator();
				for (int i = 0; i < nodes.Length; i++) {
					double v = 1.0 / nodes[i] + xv + mv;
					if (v < 0) continue;
					double lp = Gaussian.GetLogProb(xm, mm, v);
					if (shift == 0) shift = lp;
					double f = Math.Exp(logWeights[i] + lp - shift);
					mva.Add(nodes[i], f);
				}

				// Adaptive Clenshaw-Curtis quadrature: gives same results on easy integrals but 
				// still fails ExpFactorTest2
				//Converter<double,double> func = delegate(double y) {
				//    double x = Math.Exp(y); 
				//    double v = 1.0 / x + xv + mv;
				//    if (v < 0) return 0.0;
				//    return Math.Exp(Gaussian.GetLogProb(xm, mm, v) + Gamma.GetLogProb(x, precision.Shape+1, precision.Rate));
				//};
				//double Z2 = BernoulliFromLogOddsOp.AdaptiveClenshawCurtisQuadrature(func, 1, 16, 1e-10);
				//Converter<double, double> func2 = delegate(double y)
				//{
				//    return Math.Exp(y) * func(y); 
				//};
				//double rmean2 = BernoulliFromLogOddsOp.AdaptiveClenshawCurtisQuadrature(func2, 1, 16, 1e-10);
				//Converter<double, double> func3 = delegate(double y)
				//{
				//    return Math.Exp(2*y) * func(y);
				//};
				//double rvariance2 = BernoulliFromLogOddsOp.AdaptiveClenshawCurtisQuadrature(func3, 1, 16, 1e-10);
				//rmean2 = rmean2/ Z2;
				//rvariance2 = rvariance2 / Z2 - rmean2 * rmean2;

				double Z = mva.Count;
				if (double.IsNaN(Z)) throw new Exception("Z is nan");
				if (Z == 0.0) {
					throw new Exception("Quadrature found zero mass");
					//Console.WriteLine("Warning: Quadrature found zero mass.  Results may be inaccurate.");
					result.SetToUniform();
					return result;
				}
				double rmean = mva.Mean;
				double rvariance = mva.Variance;
				if (rvariance == 0) throw new Exception("Quadrature found zero variance");
				if (Double.IsInfinity(rmean)) {
					result.SetToUniform();
				} else {
					result.SetMeanAndVariance(rmean, rvariance);
					result.SetToRatio(result, precision, ForceProper);
				}
			}
#if KeepLastMessage
			if (LastPrecisionMessage != null) {
				if (Stepsize != 1 && Stepsize != 0) {
					LastPrecisionMessage.SetToPower(LastPrecisionMessage, 1 - Stepsize);
					result.SetToPower(result, Stepsize);
					result.SetToProduct(result, LastPrecisionMessage);
				}
			}
			// FIXME: this is not entirely safe since the caller could overwrite result.
			LastPrecisionMessage = result;
#endif
			return result;
		}

		/// <summary>
		/// Quadrature nodes for Gamma expectations
		/// </summary>
		/// <param name="precision">'precision' message</param>
		/// <param name="nodes">Place to put the nodes</param>
		/// <param name="logWeights">Place to put the log-weights</param>
		public static void QuadratureNodesAndWeights(Gamma precision, double[] nodes, double[] logWeights)
		{
#if KeepLastMessage
			if (LastPrecisionMessage != null) {
				Gamma PrecisionPosterior = precision * LastPrecisionMessage;
				Quadrature.GammaNodesAndWeights(PrecisionPosterior.Precision, PrecisionPosterior.PrecisionOverMean, nodes, weights);
				// modify the weights to include q(prec)/Ga(prec;a,b)
				for (int i = 0; i < weights.Length; i++) {
					weights[i] *= Math.Exp(precision.EvaluateLn(nodes[i]) - Gamma.EvaluateLn(nodes[i], PrecisionPosterior.Precision, PrecisionPosterior.PrecisionOverMean));
				}
				return;
			}
#endif
			Quadrature.GammaNodesAndWeights(precision.Shape - 1, precision.Rate, nodes, logWeights);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(mean,precision) p(mean,precision) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogAverageFactor(double sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			if (mean.IsPointMass) return LogAverageFactor(sample, mean.Point, precision);
			if (precision.IsPointMass) return LogAverageFactor(sample, mean, precision.Point);
			if (precision.IsUniform()) return Double.PositiveInfinity;
			return LogAverageFactor(Gaussian.PointMass(sample), mean, precision, to_precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample,precision) p(sample,precision) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogAverageFactor([SkipIfUniform] Gaussian sample, double mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			if (precision.IsPointMass) return LogAverageFactor(sample, mean, precision.Point);
			if (precision.IsUniform()) return Double.PositiveInfinity;
			if (sample.IsPointMass) return LogAverageFactor(sample.Point, mean, precision);
			return LogAverageFactor(sample, Gaussian.PointMass(mean), precision, to_precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample,mean,precision) p(sample,mean,precision) factor(sample,mean,precision))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogAverageFactor([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			if (precision.IsPointMass) return LogAverageFactor(sample, mean, precision.Point);
			if (precision.IsUniform()) return Double.PositiveInfinity;
			if (sample.IsPointMass && mean.IsPointMass) return LogAverageFactor(sample.Point, mean.Point, precision);
			if (sample.IsUniform() || mean.IsUniform()) return 0.0;
			// this code works even if sample and mean are point masses, but not if any variable is uniform.
			double xm, xv, mm, mv;
			sample.GetMeanAndVariance(out xm, out xv);
			mean.GetMeanAndVariance(out mm, out mv);
			// use quadrature to integrate over the precision
			double[] nodes = new double[QuadratureNodeCount];
			double[] logWeights = new double[nodes.Length];
			Gamma precMarginal = precision*to_precision;
			QuadratureNodesAndWeights(precMarginal, nodes, logWeights);
			for (int i = 0; i < nodes.Length; i++) {
				logWeights[i] = logWeights[i] + Gaussian.GetLogProb(xm, mm, xv + mv + 1.0 / nodes[i]);
			}
			if (!to_precision.IsUniform()) {
				// modify the weights
				for (int i = 0; i < logWeights.Length; i++) {
					logWeights[i] += precision.GetLogProb(nodes[i]) - precMarginal.GetLogProb(nodes[i]);
				}
			}
			return MMath.LogSumExp(logWeights);
		}
		public static double LogAverageFactor_slow([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			if (precision.IsUniform()) return Double.PositiveInfinity;
			Gamma to_precision = PrecisionAverageConditional_slow(sample, mean, precision);
			return LogAverageFactor(sample, mean, precision, to_precision);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(mean,precision) p(mean,precision) factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogEvidenceRatio(double sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, Gamma to_precision)
		{
			return LogAverageFactor(sample, mean, precision, to_precision);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="to_sample">Outgoing message to 'sample'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample,mean,precision) p(sample,mean,precision) factor(sample,mean,precision) / sum_sample p(sample) messageTo(sample))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogEvidenceRatio([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, [Fresh] Gaussian to_sample, Gamma to_precision)
		{
			if (precision.IsPointMass) return LogEvidenceRatio(sample, mean, precision.Point);
			//Gaussian to_Sample = SampleAverageConditional(sample, mean, precision);
			return LogAverageFactor(sample, mean, precision, to_precision)
				- sample.GetLogAverageOf(to_sample);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="to_sample">Outgoing message to 'sample'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample,precision) p(sample,precision) factor(sample,mean,precision) / sum_sample p(sample) messageTo(sample))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double LogEvidenceRatio([SkipIfUniform] Gaussian sample, double mean, [SkipIfUniform] Gamma precision, [Fresh] Gaussian to_sample, Gamma to_precision)
		{
			if (precision.IsPointMass) return LogEvidenceRatio(sample, mean, precision.Point);
			//Gaussian to_Sample = SampleAverageConditional(sample, mean, precision);
			return LogAverageFactor(sample, mean, precision, to_precision)
				- sample.GetLogAverageOf(to_sample);
		}

		//-- VMP ------------------------------------------------------------------------------------------------

		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Gaussian SampleAverageLogarithm(double mean, double precision)
		{
			return SampleAverageConditional(mean, precision);
		}

		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'sample'.
		/// The formula is <c>exp(sum_(mean) p(mean) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static Gaussian SampleAverageLogarithm([Proper] Gaussian mean, double precision)
		{
			if (precision < 0.0) throw new ArgumentException("precision < 0 (" + precision + ")");
			Gaussian result = new Gaussian();
			result.Precision = precision;
			result.MeanTimesPrecision = precision * mean.GetMean();
			return result;
		}
		/// <summary>
		/// VMP message to 'mean'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing VMP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'mean' conditioned on the given values.
		/// </para></remarks>
		public static Gaussian MeanAverageLogarithm(double sample, double precision)
		{
			return MeanAverageConditional(sample, precision);
		}
		/// <summary>
		/// VMP message to 'mean'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>The outgoing VMP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'mean'.
		/// The formula is <c>exp(sum_(sample) p(sample) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageLogarithm([Proper] Gaussian sample, double precision)
		{
			return SampleAverageLogarithm(sample, precision);
		}

		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'sample'.
		/// The formula is <c>exp(sum_(mean,precision) p(mean,precision) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian SampleAverageLogarithm([Proper] Gaussian mean, [Proper] Gamma precision)
		{
			// The formula is exp(int_prec int_mean p(mean) p(prec) log N(x;mean,1/prec)) =
			// exp(-0.5 E[prec*(x-mean)^2] + const.) =
			// exp(-0.5 E[prec] (x^2 - 2 x E[mean]) + const.) =
			// N(x; E[mean], 1/E[prec])
			Gaussian result = new Gaussian();
			result.Precision = precision.GetMean();
			result.MeanTimesPrecision = result.Precision * mean.GetMean();
			return result;
		}
		/// <summary>
		/// VMP message to 'mean'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'mean'.
		/// The formula is <c>exp(sum_(sample,precision) p(sample,precision) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageLogarithm([Proper] Gaussian sample, [Proper]Gamma precision)
		{
			return SampleAverageLogarithm(sample, precision);
		}
		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'sample'.
		/// The formula is <c>exp(sum_(precision) p(precision) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian SampleAverageLogarithm(double mean, [Proper] Gamma precision)
		{
			Gaussian result = new Gaussian();
			result.Precision = precision.GetMean();
			result.MeanTimesPrecision = result.Precision * mean;
			return result;
		}
		/// <summary>
		/// VMP message to 'mean'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'mean'.
		/// The formula is <c>exp(sum_(precision) p(precision) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageLogarithm(double sample, [Proper]Gamma precision)
		{
			return SampleAverageLogarithm(sample, precision);
		}

		/// <summary>
		/// VMP message to 'precision'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <returns>The outgoing VMP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'precision' conditioned on the given values.
		/// </para></remarks>
		public static Gamma PrecisionAverageLogarithm(double sample, double mean)
		{
			return PrecisionAverageConditional(sample, mean);
		}
		/// <summary>
		/// VMP message to 'precision'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'precision'.
		/// The formula is <c>exp(sum_(sample,mean) p(sample,mean) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static Gamma PrecisionAverageLogarithm([Proper]Gaussian sample, [Proper]Gaussian mean)
		{
			if (sample.IsUniform()) throw new ImproperMessageException(sample);
			if (mean.IsUniform()) throw new ImproperMessageException(mean);
			// The formula is exp(int_x int_mean p(x) p(mean) log N(x;mean,1/prec)) =
			// exp(-0.5 prec E[(x-mean)^2] + 0.5 log(prec)) =
			// Gamma(prec; 0.5, 0.5*E[(x-mean)^2])
			// E[(x-mean)^2] = E[x^2] - 2 E[x] E[mean] + E[mean^2] = var(x) + (E[x]-E[mean])^2 + var(mean)
			Gamma result = new Gamma();
			result.Shape = 1.5;
			double mx, vx, mm, vm;
			sample.GetMeanAndVariance(out mx, out vx);
			mean.GetMeanAndVariance(out mm, out vm);
			double diff = mx - mm;
			result.Rate = 0.5 * (vx + diff * diff + vm);
			return result;
		}

		/// <summary>
		/// VMP message to 'precision'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <returns>The outgoing VMP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'precision'.
		/// The formula is <c>exp(sum_(sample) p(sample) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		public static Gamma PrecisionAverageLogarithm([Proper]Gaussian sample, double mean)
		{
			if (sample.IsUniform()) throw new ImproperMessageException(sample);
			Gamma result = new Gamma();
			result.Shape = 1.5;
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double diff = mx - mean;
			result.Rate = 0.5 * (vx + diff * diff);
			return result;
		}
		/// <summary>
		/// VMP message to 'precision'
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'precision' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'precision'.
		/// The formula is <c>exp(sum_(mean) p(mean) log(factor(sample,mean,precision)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static Gamma PrecisionAverageLogarithm(double sample, [Proper]Gaussian mean)
		{
			return PrecisionAverageLogarithm(mean, sample);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(sample,mean,precision) p(sample,mean,precision) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double AverageLogFactor([Proper] Gaussian sample, [Proper] Gaussian mean, [Proper] Gamma precision)
		{
			if (sample.IsPointMass)
				return AverageLogFactor(sample.Point, mean, precision);
			if (mean.IsPointMass)
				return AverageLogFactor(sample, mean.Point, precision);
			if (precision.IsPointMass)
				return AverageLogFactor(sample, mean, precision.Point);

			return ComputeAverageLogFactor(sample, mean, precision.GetMeanLog(), precision.GetMean());
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(precision) p(precision) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double AverageLogFactor(double sample, double mean, [Proper] Gamma precision)
		{
			if (precision.IsPointMass)
				return AverageLogFactor(sample, mean, precision.Point);
			else {
				double diff = sample - mean;
				return -MMath.LnSqrt2PI + 0.5 * (precision.GetMeanLog() - precision.GetMean() * diff * diff);
			}
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		public static double AverageLogFactor(double sample, double mean, double precision)
		{
			double diff = sample - mean;
			if (double.IsPositiveInfinity(precision)) return (diff == 0.0) ? 0.0 : double.NegativeInfinity;
			if (precision == 0.0) return 0.0;
			return -MMath.LnSqrt2PI + 0.5 * (Math.Log(precision) - precision * diff * diff);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(sample) p(sample) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		public static double AverageLogFactor([Proper] Gaussian sample, double mean, double precision)
		{
			if (sample.IsPointMass)
				return AverageLogFactor(sample.Point, mean, precision);
			else if (double.IsPositiveInfinity(precision))
				return sample.GetLogProb(mean);
			else if (precision == 0.0)
				return 0.0;
			else
				return ComputeAverageLogFactor(sample, mean, Math.Log(precision), precision);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(mean) p(mean) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static double AverageLogFactor(double sample, [Proper] Gaussian mean, double precision)
		{
			return AverageLogFactor(mean, sample, precision);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(mean,precision) p(mean,precision) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double AverageLogFactor(double sample, [Proper] Gaussian mean, [Proper] Gamma precision)
		{
			return AverageLogFactor(mean, sample, precision);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(sample,precision) p(sample,precision) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static double AverageLogFactor([Proper] Gaussian sample, double mean, [Proper] Gamma precision)
		{
			if (sample.IsPointMass)
				return AverageLogFactor(sample.Point, mean, precision);
			if (precision.IsPointMass)
				return AverageLogFactor(sample, mean, precision.Point);

			return ComputeAverageLogFactor(sample, mean, precision.GetMeanLog(), precision.GetMean());
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Constant value for 'precision'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(sample,mean) p(sample,mean) log(factor(sample,mean,precision))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		public static double AverageLogFactor([Proper] Gaussian sample, [Proper] Gaussian mean, double precision)
		{
			if (sample.IsPointMass)
				return AverageLogFactor(sample.Point, mean, precision);
			else if (mean.IsPointMass)
				return AverageLogFactor(sample, mean.Point, precision);
			else if (double.IsPositiveInfinity(precision))
				return sample.GetLogAverageOf(mean);
			else if (precision == 0.0)
				return 0.0;
			else
				return ComputeAverageLogFactor(sample, mean, Math.Log(precision), precision);
		}

		/// <summary>
		/// Helper method for computing average log factor
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="mean">Incoming message from 'mean'.</param>
		/// <param name="precision_Elogx">Expected log value of the incoming message from 'precision'</param>
		/// <param name="precision_Ex">Expected value of incoming message from 'precision'</param>
		/// <returns>Computed average log factor</returns>
		private static double ComputeAverageLogFactor(Gaussian sample, Gaussian mean, double precision_Elogx, double precision_Ex)
		{
			if (precision_Ex == 0.0) throw new ArgumentException("precision == 0");
			if (double.IsPositiveInfinity(precision_Ex)) throw new ArgumentException("precision is infinite");
			double sampleMean, sampleVariance = 0;
			double meanMean, meanVariance = 0;
			sample.GetMeanAndVariance(out sampleMean, out sampleVariance);
			mean.GetMeanAndVariance(out meanMean, out meanVariance);
			double diff = sampleMean - meanMean;
			return -MMath.LnSqrt2PI + 0.5 * (precision_Elogx
											- precision_Ex * (diff * diff + sampleVariance + meanVariance));
		}

		/// <summary>
		/// Helper method for computing average log factor
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="precision_Elogx">Expected log value of the incoming message from 'precision'</param>
		/// <param name="precision_Ex">Expected value of incoming message from 'precision'</param>
		/// <returns>Computed average log factor</returns>
		private static double ComputeAverageLogFactor(Gaussian sample, double mean, double precision_Elogx, double precision_Ex)
		{
			if (double.IsPositiveInfinity(precision_Ex)) throw new ArgumentException("precision is infinite");
			double sampleMean, sampleVariance;
			sample.GetMeanAndVariance(out sampleMean, out sampleVariance);
			double diff = sampleMean - mean;
			return -MMath.LnSqrt2PI + 0.5 * (precision_Elogx
											- precision_Ex * (diff * diff + sampleVariance));
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.Gaussian"/> and <see cref="Gaussian.Sample(double,double)"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Gaussian), "Sample", typeof(double), typeof(double), Default=false)]
	[FactorMethod(new string[] { "sample", "mean", "precision" }, typeof(Factor), "Gaussian", Default=false)]
	[Quality(QualityBand.Experimental)]
	public class GaussianOp_Slow : GaussianOpBase
	{
		public static Gaussian SampleAverageConditional(Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			if (sample.IsUniform() && precision.Shape <= 1.0) sample = Gaussian.FromNatural(1e-20, 1e-20);
			if (precision.IsPointMass) {
				return SampleAverageConditional(mean, precision.Point);
			} else if (sample.IsUniform()) {
				// for large vx, Z =approx N(mx; mm, vx+vm+E[1/prec])
				double mm,mv;
				mean.GetMeanAndVariance(out mm, out mv);
				// NOTE: this error may happen because sample didn't receive any message yet under the schedule.
				// Need to make the scheduler smarter to avoid this.
				if (precision.Shape <= 1.0) throw new ArgumentException("The posterior has infinite variance due to precision distributed as "+precision+" (shape <= 1).  Try using a different prior for the precision, with shape > 1.");
				return Gaussian.FromMeanAndVariance(mm, mv + precision.GetMeanInverse());
			} else if (mean.IsUniform() || precision.IsUniform()) {
				return Gaussian.Uniform();
			} else if (sample.IsPointMass) {
				// The correct answer here is not uniform, but rather a limit.  
				// However it doesn't really matter what we return since multiplication by a point mass 
				// always yields a point mass.
				return Gaussian.Uniform();
			} else if (!precision.IsProper()) {
				throw new ImproperMessageException(precision);
			} else {
				double mx, vx;
				sample.GetMeanAndVariance(out mx, out vx);
				double mm, vm;
				mean.GetMeanAndVariance(out mm, out vm);
				double m = mx-mm;
				double v = vx+vm;
				if (double.IsPositiveInfinity(v)) return Gaussian.Uniform();
				double m2 = m*m;
				Gamma q = GaussianOp_Laplace.Q(sample, mean, precision, GaussianOp_Laplace.QInit());
				double a = precision.Shape;
				double b = precision.Rate;
				double r = q.GetMean();
				double rmin, rmax;
				GetIntegrationBoundsForPrecision(r, m, v, a, b, out rmin, out rmax);
				int n = 20000;
				double inc = (Math.Log(rmax)-Math.Log(rmin))/(n-1);
				GaussianEstimator est = new GaussianEstimator();
				double shift = 0;
				for (int i = 0; i < n; i++) {
					double prec = rmin*Math.Exp(i*inc);
					double newVar, newMean;
					// the following works even if sample is uniform. (sample.Precision == 0)
					if (mean.IsPointMass) {
						// take limit mean.Precision -> Inf
						newVar = 1.0 / (prec + sample.Precision);
						newMean = newVar * (prec * mean.Point + sample.MeanTimesPrecision);
					} else {
						// mean.Precision < Inf
						double R = prec / (prec + mean.Precision);
						newVar = 1.0 / (R * mean.Precision + sample.Precision);
						newMean = newVar * (R * mean.MeanTimesPrecision + sample.MeanTimesPrecision);
					}
					double logp = -0.5*Math.Log(v+1/prec) -0.5*m2/(v+1/prec) + a*Math.Log(prec) - b*prec;
					if (i == 0) shift = logp;
					est.Add(Gaussian.FromMeanAndVariance(newMean, newVar), Math.Exp(logp-shift));
				}
				if (est.mva.Count == 0) throw new Exception("Quadrature found zero mass");
				if (double.IsNaN(est.mva.Count)) throw new Exception("count is nan");
				Gaussian sampleMarginal = est.GetDistribution(new Gaussian());
				Gaussian result = new Gaussian();
				result.SetToRatio(sampleMarginal, sample, GaussianOp.ForceProper);
				if (double.IsNaN(result.Precision)) throw new Exception("result is nan");
				return result;
			}
		}
		public static Gamma PrecisionAverageConditional([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			if (sample.IsPointMass && mean.IsPointMass) return PrecisionAverageConditional(sample.Point, mean.Point);
			else if (precision.IsPointMass) {
				// The correct answer here is not uniform, but rather a limit.  
				// However it doesn't really matter what we return since multiplication by a point mass 
				// always yields a point mass.
				return Gamma.Uniform();
			} else if (sample.IsUniform() || mean.IsUniform()) {
				return Gamma.Uniform();
			} else if (!precision.IsProper()) {
				// improper prior
				throw new ImproperMessageException(precision);
			} else {
				double mx, vx;
				sample.GetMeanAndVariance(out mx, out vx);
				double mm, vm;
				mean.GetMeanAndVariance(out mm, out vm);
				double m = mx-mm;
				double v = vx+vm;
				if (double.IsPositiveInfinity(v)) return Gamma.Uniform();
				double m2 = m*m;
				Gamma q = GaussianOp_Laplace.Q(sample, mean, precision, GaussianOp_Laplace.QInit());
				double a = precision.Shape;
				double b = precision.Rate;
				double r = q.GetMean();
				double rmin, rmax;
				GetIntegrationBoundsForPrecision(r, m, v, a, b, out rmin, out rmax);
				int n = 20000;
				double inc = (Math.Log(rmax)-Math.Log(rmin))/(n-1);
				MeanVarianceAccumulator mva = new MeanVarianceAccumulator();
				double shift = 0;
				for (int i = 0; i < n; i++) {
					r = rmin*Math.Exp(i*inc);
					double logp = -0.5*Math.Log(v+1/r) -0.5*m2/(v+1/r) + a*Math.Log(r) - b*r;
					if (i == 0) shift = logp;
					mva.Add(r, Math.Exp(logp-shift));
				}
				if (mva.Count == 0) throw new Exception("Quadrature found zero mass");
				if (double.IsNaN(mva.Count)) throw new Exception("count is nan");
				Gamma precMarginal = Gamma.FromMeanAndVariance(mva.Mean, mva.Variance);
				Gamma result = new Gamma();
				result.SetToRatio(precMarginal, precision, GaussianOp.ForceProper);
				if (double.IsNaN(result.Rate)) throw new Exception("result is nan");
				return result;
			}
		}
		public static double LogAverageFactor([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			if (precision.IsPointMass) return LogAverageFactor(sample, mean, precision.Point);
			if (precision.IsUniform()) return Double.PositiveInfinity;
			if (sample.IsPointMass && mean.IsPointMass) return LogAverageFactor(sample.Point, mean.Point, precision);
			if (sample.IsUniform() || mean.IsUniform()) return 0.0;
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			double m = mx-mm;
			double v = vx+vm;
			double m2 = m*m;
			Gamma q = GaussianOp_Laplace.Q(sample, mean, precision, GaussianOp_Laplace.QInit());
			double a = precision.Shape;
			double b = precision.Rate;
			double r = q.GetMean();
			double rmin, rmax;
			GetIntegrationBoundsForPrecision(r, m, v, a, b, out rmin, out rmax);
			int n = 20000;
			double inc = (Math.Log(rmax)-Math.Log(rmin))/(n-1);
			double logZ = double.NegativeInfinity;
			for (int i = 0; i < n; i++) {
				r = rmin*Math.Exp(i*inc);
				double logp = -0.5*Math.Log(v+1/r) -0.5*m2/(v+1/r) + a*Math.Log(r) - b*r;
				logZ = MMath.LogSumExp(logZ, logp);
			}
			logZ += -MMath.LnSqrt2PI - MMath.GammaLn(a) + a*Math.Log(b) + Math.Log(inc);
			return logZ;
		}
		public static double LogEvidenceRatio([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, [Fresh] Gaussian to_sample)
		{
			if (precision.IsPointMass) return LogEvidenceRatio(sample, mean, precision.Point);
			//Gaussian to_Sample = SampleAverageConditional(sample, mean, precision);
			return LogAverageFactor(sample, mean, precision)
				- sample.GetLogAverageOf(to_sample);
		}
		public static double LogEvidenceRatio(double sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			if (precision.IsPointMass) return LogEvidenceRatio(sample, mean, precision.Point);
			//Gaussian to_Sample = SampleAverageConditional(sample, mean, precision);
			return LogAverageFactor(Gaussian.PointMass(sample), mean, precision);
		}
		/// <summary>
		/// EP message to 'mean'
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="mean">Incoming message from 'mean'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="precision">Incoming message from 'precision'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'mean' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'mean' as the random arguments are varied.
		/// The formula is <c>proj[p(mean) sum_(sample,precision) p(sample,precision) factor(sample,mean,precision)]/p(mean)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="mean"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="precision"/> is not a proper distribution</exception>
		public static Gaussian MeanAverageConditional([SkipIfUniform] Gaussian sample, Gaussian mean, [SkipIfUniform] Gamma precision)
		{
			return SampleAverageConditional(mean, sample, precision);
		}

		private static void GetIntegrationBoundsForPrecision(double r, double m, double v, double a, double b, out double rmin, out double rmax)
		{
			// this routine assumes integration is done in log(r), so the Jacobian log(r) is added, turning (a-1)*log(r) into a*log(r)
			// -0.5*log(v+1/rmax) -0.5*m^2/(v+1/rmax) + a*log(rmax) - b*rmax < -0.5*log(v+1/r)-0.5*m^2/(v+1/r)+a*log(r)-b*r - 50
			double m2 = m*m;
			var bound = -0.5*Math.Log(v+1/r) -0.5*m2/(v+1/r) + a*Math.Log(r) - b*r - 50;
			// apply the bound: 1/(v+1/rmax) > (1+log(v+1/r)-log(v+1/rmax))/(v+1/r)
			var shiftmax = -0.5*m2*(1+Math.Log(v+1/r))/(v+1/r);
			// slope1 is coefficient of log(v+1/rmax)
			var slope1 = -0.5 + 0.5*m2/(v+1/r);
			double slopemax;
			if (slope1 < 0) {
				// apply the bound: log(v+1/rmax) > q*log(1/rmax/q) + (1-q)*log(v/(1-q))
				double u = (1/r)/(v+1/r);
				// (1-u)*log(v/(1-u)) = (1-u)*log(v+1/r)
				// adding -u*log(u) gives below
				shiftmax += slope1*(u*Math.Log(r) + Math.Log(v+1/r));
				// slopemax is coefficient of log(rmax)
				slopemax = -u*slope1;
			} else {
				// apply the bound: log(v+1/rmax) < log(v+1/r)  if rmax > r
				shiftmax += slope1*Math.Log(v+1/r);
				slopemax = 0;
			}
			slopemax += a;
			if (slopemax <= 0) {
				// -log(rmax) < -log(r)
				shiftmax += slopemax*Math.Log(r);
				rmax = (shiftmax-bound)/b;
			} else {
				// log(rmax) < (rmax-r2)/r2 + log(r2)
				var r2 = slopemax/(0.1*b);
				shiftmax = shiftmax + slopemax*(Math.Log(r2) - 1);
				// slope is coefficient of rmax
				double linearmax = slopemax/r2 - b;
				rmax = (bound-shiftmax)/linearmax;
			}
			// apply the bound: 1/(v+1/rmin) > 0
			// apply the bound: log(v+1/rmin) > -log(rmin)
			// apply the bound: rmin > 0
			double slopemin = 0.5 + a;
			rmin = Math.Exp(bound/slopemin);
			//if (slopemin > 0) {
			//  // log(rmin) < (rmin-r2)/r2 + log(r2)
			//  var r2 = r/2;
			//  shiftmin += slopemin*(Math.Log(r2)-1);
			//  linearmin += slopemin/r2;
			//  rmin = (bound - shiftmin)/linearmin;
			//} else {
			//  throw new NotImplementedException();
			//}
			if (rmin == 0) throw new Exception("rmin == 0");
			Assert.IsTrue(rmin < rmax);
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.Gaussian"/> and <see cref="Gaussian.Sample(double,double)"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Gaussian), "Sample", typeof(double), typeof(double), Default=false)]
	[FactorMethod(new string[] { "sample", "mean", "precision" }, typeof(Factor), "Gaussian", Default=false)]
	[Buffers("Q")]
	[Quality(QualityBand.Experimental)]
	public class GaussianOp_Laplace : GaussianOpBase
	{
		// Laplace cases -----------------------------------------------------------------------------------------
		public static bool modified = true;
		public static bool modified2 = false;

		public static void LaplaceMoments(Gamma q, double[] g, double[] dlogf, out double m, out double v)
		{
			if (q.IsPointMass) {
				m = g[0];
				v = 0;
				return;
			}
			double a = q.Shape;
			double b = q.Rate;
			double x = a/b;
			double dg = g[1];
			double ddg = g[2];
			double ddlogf = dlogf[1];
			double dddlogf = dlogf[2];
			double dx = dg*x/b;
			double a1 = -2*x*ddlogf - x*x*dddlogf;
			double da = -x*x*ddg + dx*a1;
			m = g[0] + (MMath.Digamma(a)-Math.Log(a))*da;
			if (g.Length > 3) {
				double dddg = g[3];
				double d4logf = dlogf[3];
				double db = -dg+da/x;
				double ddx = (dg + x*ddg)/b*dx - x*dg/(b*b)*db;
				double a2 = -2*ddlogf - 4*x*dddlogf - x*x*d4logf;
				double dda = (-2*x*ddg - x*x*dddg)*dx + a2*dx*dx + a1*ddx;
				v = dg*dx + (MMath.Trigamma(a)-1/a)*da*da + (MMath.Digamma(a)-Math.Log(a))*dda;
				if (v < 0) throw new Exception("v < 0");
			} else {
				v = 0;
			}
		}

		[Skip]
		public static Gamma QInit()
		{
			return Gamma.Uniform();
		}

		// Perform one update of Q
		private static Gamma QUpdate(Gaussian sample, Gaussian mean, Gamma precision, Gamma q)
		{
			if (sample.IsUniform() || mean.IsUniform() || precision.IsPointMass) return precision;
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			double m = mx-mm;
			double v = vx+vm;
			double m2 = m*m;
			double a = q.Shape;
			double b = q.Rate;
			if (b==0 || q.IsPointMass) {
				a = precision.Shape;
				// this guess comes from solving dlogf=0 for x
				double guess = m2-v;
				b = Math.Max(precision.Rate, a*guess);
			}
			double x = a/b;
			double x2 = x*x;
			double x3 = x*x2;
			double x4 = x*x3;
			double p = 1/(v+1/x);
			double dlogf1 = -0.5*p + 0.5*m2*p*p;
			double dlogf = dlogf1*(-1/x2);
			double ddlogf1 = 0.5*p*p - m2*p*p*p;
			double ddlogf = dlogf1*2/x3 + ddlogf1/x4;
			b = precision.Rate - (dlogf + x*ddlogf);
			if (b < 0) {
				if (q.Rate == precision.Rate || true) return QUpdate(sample, mean, precision, Gamma.FromShapeAndRate(precision.Shape, precision.Shape*(m2-v)));
			}
			a = precision.Shape - x2*ddlogf;
			if (a <= 0) a = b*precision.Shape/(precision.Rate - dlogf);
			if (a <= 0 || b <= 0) throw new Exception();
			if (double.IsNaN(a) || double.IsNaN(b)) throw new Exception("result is nan");
			return Gamma.FromShapeAndRate(a, b);
		}
		private static double QReinitialize(Gaussian sample, Gaussian mean, Gamma precision, double x)
		{
			// there can be two local optima for q
			// each time we update q, check the function value near each optimum, and re-initialize q if any is higher
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			double m = mx-mm;
			double v = vx+vm;
			double m2 = m*m;
			double a = precision.Shape;
			double b = precision.Rate;
			//double init0 = precision.GetMean();
			double init0 = (a+0.5)/b;
			double init1 = (a+0.5)/(b + 0.5*(m2+v));
			//double init1 = 1/(m2-v);
			if (v > m2) {
				// if v is large then
				// -0.5*log(v + 1/x) - 0.5*m2/(v + 1/x) + a*log(x) - b*x =approx -0.5/v/x + 0.5*m2/v^2/x + a*log(x) - b*x
				double c = 0.5/v*(m2/v - 1);
				// c < 0 so the Sqrt always succeeds
				init1 = (a + Math.Sqrt(a*a - 4*b*c))/(2*b);
			}
			double logz0 = -0.5*Math.Log(1 + 1/init0/v) - 0.5*m2/(v + 1/init0) + a*Math.Log(init0) - b*init0;
			double logz1 = -0.5*Math.Log(1 + 1/init1/v) - 0.5*m2/(v + 1/init1) + a*Math.Log(init1) - b*init1;
			double logz = -0.5*Math.Log(1 + 1/x/v) - 0.5*m2/(v + 1/x) + a*Math.Log(x) - b*x;
			if (double.IsNaN(logz)) logz = double.NegativeInfinity;
			double logzMax = Math.Max(logz1, logz);
			// must use IsGreater here because round-off errors can cause logz1 > logz even though it is a worse solution according to the gradient
			if (IsGreater(logz0, logzMax)) return init0;
			else if (IsGreater(logz1, logz)) return init1;
			else return x;
		}
		private static bool IsGreater(double a, double b)
		{
			return (a > b) && (MMath.AbsDiff(a, b, 1e-14) > 1e-14);
		}
		public static Gamma Q(Gaussian sample, Gaussian mean, Gamma precision, Gamma q)
		{
			if (precision.IsPointMass || sample.IsUniform() || mean.IsUniform()) return precision;
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			double m = mx-mm;
			double v = vx+vm;
			if (double.IsInfinity(v)) return precision;
			double m2 = m*m;
			double[] dlogfss;
			double x = q.GetMean();
			double a = precision.Shape;
			double b = precision.Rate;
			if (double.IsPositiveInfinity(x)) x = (a + Math.Sqrt(a*a + 2*b/v))/(2*b);
			// TODO: check initialization
			if (q.IsUniform()) x = 0;
			//x = QReinitialize(sample, mean, precision, x);
			for (int iter = 0; iter < 1000; iter++) {
				double oldx = x;
				if (true) {
					// want to find x that optimizes -0.5*log(v + 1/x) - 0.5*m2/(v + 1/x) + a*log(x) - b*x
					// we fit a lower bound, then optimize the bound.
					// this is guaranteed to improve x.
					double logSlope = precision.Shape;
					double slope = -precision.Rate;
					double denom = v*x+1;
					// 1/(v+1/x) <= 1/(v+1/x0) + (x-x0)/(v*x0+1)^2
					slope += -0.5*m2/(denom*denom);
					if (v*x < 1) {
						// log(v+1/x)  = log(v*x+1) - log(x)
						// log(v*x+1) <= log(v*x0+1) + (x-x0)*v/(v*x0+1)
						logSlope += 0.5;
						slope += -0.5*v/denom;
						x = -logSlope/slope;
						// at x=0 the update is x = (a+0.5)/(b + 0.5*(m2+v))
						// at x=inf the update is x = (a+0.5)/b
					} else {
						// if v*x > 1:
						// log(v+1/x) <= log(v+1/x0) + (1/x - 1/x0)/(v + 1/x0)
						double invSlope = -0.5*x/denom;
						// solve for the maximum of logslope*log(r)+slope*r+invslope./r
						// logslope/r + slope - invslope/r^2 = 0
						// logslope*r + slope*r^2 - invslope = 0
						//x = (-logSlope - Math.Sqrt(logSlope*logSlope + 4*invSlope*slope))/(2*slope);
						double c = 0.5*logSlope/slope;
						double d = invSlope/slope;
						// note c < 0 always
						x = Math.Sqrt(c*c + d) - c;
						// at x=inf, invSlope=-0.5/v so the update is x = (ax + sqrt(ax*ax + 2*b/v))/(2*b)
					}
					if (x < 0) throw new Exception("x < 0");
				} else {
					dlogfss = dlogfs(x, m, v);
					if (true) {
						x = (precision.Shape - x*x*dlogfss[1])/(precision.Rate - dlogfss[0] - x*dlogfss[1]);
					} else if (true) {
						double delta = dlogfss[0] - precision.Rate;
						x *= Math.Exp(-(delta*x + precision.Shape)/(delta*x + dlogfss[1]*x));
					} else {
						x = precision.Shape/(precision.Rate - dlogfss[0]);
					}
					if (x < 0) throw new Exception("x < 0");
				}
				if (double.IsNaN(x)) throw new Exception("x is nan");
				//Console.WriteLine("{0}: {1}", iter, x);
				if (MMath.AbsDiff(oldx, x, 1e-10) < 1e-10) {
					x = QReinitialize(sample, mean, precision, x);
					if (MMath.AbsDiff(oldx, x, 1e-10) < 1e-10) break;
				}
				if (iter == 1000-1) throw new Exception("not converging");
			}
			//x = r0;
			dlogfss = dlogfs(x, m, v);
			double dlogf = dlogfss[0];
			double ddlogf = dlogfss[1];
			a = precision.Shape - x*x*ddlogf;
			b = precision.Rate - (dlogf + x*ddlogf);
			if (a <= 0 || b <= 0) throw new Exception();
			if (double.IsNaN(a) || double.IsNaN(b)) throw new Exception("result is nan");
			return Gamma.FromShapeAndRate(a, b);
		}
		public static double r0 = 0.1;

		public static double LogAverageFactor([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, Gamma precision, [Fresh] Gamma q)
		{
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			double m = mx-mm;
			double v = vx+vm;
			double m2 = m*m;
			double x = q.GetMean();
			double logf = -MMath.LnSqrt2PI -0.5*Math.Log(v + 1/x) - 0.5*m2/(v + 1/x);
			double logz = logf + precision.GetLogProb(x) - q.GetLogProb(x);
			return logz;
		}
		public static double LogEvidenceRatio([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, [Fresh] Gaussian to_sample, [Fresh] Gamma q)
		{
			return LogAverageFactor(sample, mean, precision, q)
				- sample.GetLogAverageOf(to_sample);
		}
		public static double LogEvidenceRatio(double sample, [SkipIfUniform] Gaussian mean, [SkipIfUniform] Gamma precision, [Fresh] Gamma q)
		{
			return LogAverageFactor(Gaussian.PointMass(sample), mean, precision, q);
		}
		public static double[] dlogfs(double x, double m, double v)
		{
			if (double.IsPositiveInfinity(v)) return new double[4];
			// log f(x) = -0.5*log(v+1/x) -0.5*m^2/(v+1/x)
			double m2 = m*m;
			double x2 = x*x;
			double x3 = x*x2;
			double x4 = x*x3;
			double p = 1/(v+1/x);
			double p2 = p*p;
			double p3 = p*p2;
			double dlogf1 = -0.5*p + 0.5*m2*p2;
			double dlogf = dlogf1*(-1/x2);
			double ddlogf1 = 0.5*p2 - m2*p3;
			double ddlogf = dlogf1*2/x3 + ddlogf1/x4;
			double dddlogf1 = -p3 + 3*m2*p*p3;
			double	dddlogf = dlogf1*(-6)/x4 +	ddlogf1*(-6)/(x*x4) + dddlogf1*(-1)/(x2*x4);
			double d4logf1 = 3*p*p3 - 12*m2*p2*p3;
			double d4logf = dlogf1*(24)/(x2*x3) + ddlogf1*36/(x3*x3) + dddlogf1*(12)/(x4*x3) + d4logf1/(x4*x4);
			return new double[] { dlogf, ddlogf, dddlogf, d4logf };
		}

		public static Gamma PrecisionAverageConditional([SkipIfUniform] Gaussian sample, [SkipIfUniform] Gaussian mean, [Proper] Gamma precision, [Fresh] Gamma q)
		{
			if (sample.IsUniform() || mean.IsUniform()) return Gamma.Uniform();
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			double m = mx-mm;
			double v = vx+vm;
			double x = q.GetMean();
			double[] g = new double[] { x, 1, 0, 0 };
			double precMean, precVariance;
			LaplaceMoments(q, g, dlogfs(x, m, v), out precMean, out precVariance);
			if (precMean < 0) throw new Exception("internal: precMean < 0");
			Gamma precMarginal = Gamma.FromMeanAndVariance(precMean, precVariance);
			Gamma result = new Gamma();
			result.SetToRatio(precMarginal, precision, true);
			return result;
		}

		public static double[] dlogfxs(double x, Gamma precision)
		{
			if (precision.IsPointMass) throw new Exception("precision is point mass");
			// f(sample,mean) = tpdf(sample-mean, 0, 2b, 2a+1) N(mean; mm, vm)
			// f(x) = tpdf(x; 0, 2b, 2a+1)
			// log f(x) = -(a+0.5)*log(1 + x^2/2/b)  + const.
			double a = precision.Shape;
			double b = precision.Rate;
			double a2 = -(a+0.5);
			double bdenom = b + x*x/2;
			double bdenom2 = bdenom*bdenom;
			double bdenom3 = bdenom*bdenom2;
			double bdenom4 = bdenom*bdenom3;
			double dlogf = x/bdenom;
			double ddlogf = 1/bdenom - x*x/bdenom2;
			double dddlogf = 2*x*x*x/bdenom3 -3*x/bdenom2;
			double d4logf = 12*x*x/bdenom3 - 6*x*x*x*x/bdenom4 - 3/bdenom2;
			return new double[] { a2*dlogf, a2*ddlogf, a2*dddlogf, a2*d4logf };
		}

		public static Gaussian Qx(Gaussian y, Gamma precision, Gaussian qx)
		{
			if (y.IsPointMass) return y;
			double x = QxReinitialize(y, precision, qx.GetMean());
			double r = 0;
			for (int iter = 0; iter < 1000; iter++) {
				double oldx = x;
				double[] dlogfs = dlogfxs(x, precision);
				double ddlogf = dlogfs[1];
				r = Math.Max(0, -ddlogf);
				double t = r*x + dlogfs[0];
				x = (t + y.MeanTimesPrecision)/(r + y.Precision);
				if (Math.Abs(oldx - x) < 1e-10) break;
				//Console.WriteLine("{0}: {1}", iter, x);		
				if (iter == 1000-1) throw new Exception("not converging");
				if (iter % 100 == 99) x = QxReinitialize(y, precision, x);
			}
			return Gaussian.FromMeanAndPrecision(x, r + y.Precision);
		}
		private static double QxReinitialize(Gaussian y, Gamma precision, double x)
		{
			double init0 = 0;
			double init1 = y.GetMean();
			double a = precision.Shape;
			double b = precision.Rate;
			double a2 = -(a+0.5);
			double logz0 = a2*Math.Log(b + init0*init0/2) + y.GetLogProb(init0);
			double logz1 = a2*Math.Log(b + init1*init1/2) + y.GetLogProb(init1);
			double logz = a2*Math.Log(b + x*x/2) + y.GetLogProb(x);
			if (logz0 > Math.Max(logz1, logz)) return init0;
			else if (logz1 > logz) return init1;
			else return x;
		}

		public static void LaplaceMoments(Gaussian q, double[] dlogfx, out double m, out double v)
		{
			double vx = 1/q.Precision;
			double delta = 0.5*dlogfx[2]*vx*vx;
			m = q.GetMean() + delta;
			v = vx + 4*delta*delta + 0.5*dlogfx[3]*vx*vx*vx;
			if (v < 0) throw new Exception();
		}

		public static Gaussian SampleAverageConditional(Gaussian sample, [SkipIfUniform] Gaussian mean, [Proper] Gamma precision, [Fresh] Gamma q)
		{
			if (mean.IsUniform() || sample.IsPointMass) return Gaussian.Uniform();
			if (precision.IsPointMass) return GaussianOp.SampleAverageConditional(mean, precision.Point);
			if (q.IsPointMass) throw new Exception();
			double mm, vm;
			mean.GetMeanAndVariance(out mm, out vm);
			if (sample.IsUniform()) {
				if (precision.Shape <= 1.0) {
					sample = Gaussian.FromNatural(1e-20, 1e-20);  // try to proceed instead of throwing an exception
					//throw new ArgumentException("The posterior has infinite variance due to precision distributed as "+precision+" (shape <= 1).  Try using a different prior for the precision, with shape > 1.");
				} else return Gaussian.FromMeanAndVariance(mm, vm + precision.GetMeanInverse());
			}
			double mx, vx;
			sample.GetMeanAndVariance(out mx, out vx);
			double m = mx-mm;
			double v = vx+vm;
			Gaussian sampleMarginal;
			if (true) {
				Gaussian qx = Qx(Gaussian.FromMeanAndVariance(m, v), precision, Gaussian.FromMeanAndPrecision(0, 1));
				double y = qx.GetMean();
				double[] dlogfx = dlogfxs(y, precision);
				double delta = 0.5*dlogfx[2]/(qx.Precision*qx.Precision);
				// sampleMean can be computed directly as:
				//double dlogz = dlogfx[0] + delta/v;
				//double sampleMean = mx + vx*dlogz;
				double my = y + delta;
				double vy = 1/qx.Precision + 4*delta*delta + 0.5*dlogfx[3]/(qx.Precision*qx.Precision*qx.Precision);
				if (vy < 0) throw new Exception();
				Gaussian yMsg = new Gaussian(my, vy)/(new Gaussian(m, v));
				Gaussian sampleMsg = DoublePlusOp.SumAverageConditional(yMsg, mean);
				sampleMarginal = sampleMsg*sample;
				if (!sampleMarginal.IsProper()) throw new Exception();
				if (sampleMarginal.IsPointMass) throw new Exception();
				//Console.WriteLine("{0} {1}", sampleMean, sampleMarginal);
			} else {
				double a = q.Shape;
				double b = q.Rate;
				double x = a/b;
				if (sample.IsPointMass || mean.IsPointMass) {
					double denom = 1 + x*v;
					double denom2 = denom*denom;
					Gaussian y = sample*mean;
					double my, vy;
					y.GetMeanAndVariance(out my, out vy);
					// 1-1/denom = x*v/denom
					// sampleMean = E[ (x*(mx*vm + mm*vx) + mx)/denom ]
					//            = E[ (mx*vm + mm*vx)/v*(1-1/denom) + mx/denom ]
					//            = E[ (mx/vx + mm/vm)/(1/vx + 1/vm)*(1-1/denom) + mx/denom ]
					// sampleVariance = var((my*(1-1/denom) + mx/denom)) + E[ (r*vx*vm + vx)/denom ]
					//                = (mx-my)^2*var(1/denom) + E[ vx*vm/v*(1-1/denom) + vx/denom ]
					double[] g = new double[] { 1/denom, -v/denom2, 2*v*v/(denom2*denom), -6*v*v*v/(denom2*denom2) };
					double [] dlogf = dlogfs(x, m, v);
					double edenom, vdenom;
					LaplaceMoments(q, g, dlogf, out edenom, out vdenom);
					double sampleMean = mx*edenom + my*(1 - edenom);
					double diff = mx-my;
					double sampleVariance = vx*edenom + vy*(1 - edenom) + diff*diff*vdenom;
					sampleMarginal = Gaussian.FromMeanAndVariance(sampleMean, sampleVariance);
				} else {
					// 1 - samplePrec*mPrec/denom = x*yprec/denom
					// sampleMean = E[ (x*ymprec + sampleMP*mPrec)/denom ]
					//            = E[ (1 - samplePrec*mPrec/denom)*ymprec/yprec + sampleMP*mPrec/denom ]
					// sampleVariance = var((1 - samplePrec*mPrec/denom)*ymprec/yprec + sampleMP*mPrec/denom) + E[ (x+mPrec)/denom ]
					//                = (sampleMP*mPrec - samplePrec*mPrec*ymprec/yprec)^2*var(1/denom) + E[ (1-samplePrec*mPrec/denom)/yprec + mPrec/denom ]
					double yprec = sample.Precision + mean.Precision;
					double ymprec = sample.MeanTimesPrecision + mean.MeanTimesPrecision;
					double denom = sample.Precision*mean.Precision + x*yprec;
					double denom2 = denom*denom;
					double[] g = new double[] { 1/denom, -yprec/denom2, 2*yprec*yprec/(denom2*denom), -6*yprec*yprec*yprec/(denom2*denom2) };
					double [] dlogf = dlogfs(x, m, v);
					double edenom, vdenom;
					LaplaceMoments(q, g, dlogf, out edenom, out vdenom);
					double sampleMean = sample.MeanTimesPrecision*mean.Precision*edenom + ymprec/yprec*(1-sample.Precision*mean.Precision*edenom);
					double diff = sample.MeanTimesPrecision*mean.Precision - sample.Precision*mean.Precision*ymprec/yprec;
					double sampleVariance = mean.Precision*edenom + (1 - sample.Precision*mean.Precision*edenom)/yprec + diff*diff*vdenom;
					sampleMarginal = Gaussian.FromMeanAndVariance(sampleMean, sampleVariance);
				}
			}
			Gaussian result = new Gaussian();
			result.SetToRatio(sampleMarginal, sample, GaussianOp.ForceProper);
			if (modified2) {
				if(!mean.IsPointMass) throw new Exception();
				result.SetMeanAndPrecision(mm, q.GetMean());
			} else if (modified && !sample.IsUniform()) {
				// heuristic method to avoid improper messages:
				// the message's mean must be E[mean] (regardless of context) and the variance is chosen to match the posterior mean when multiplied by context
				double sampleMean = sampleMarginal.GetMean();
				if (sampleMean != mm) {
					result.Precision = (sample.MeanTimesPrecision-sampleMean*sample.Precision)/(sampleMean - mm);
					if (result.Precision < 0) throw new Exception("internal: sampleMean is not between sample.Mean and mean.Mean");
					result.MeanTimesPrecision = result.Precision*mm;
				}
			}
			//if (!result.IsProper()) throw new Exception();
			if (result.Precision < -0.001) throw new Exception();
			if (double.IsNaN(result.MeanTimesPrecision) || double.IsNaN(result.Precision)) throw new Exception("result is nan");
			return result;
		}

		public static Gaussian MeanAverageConditional([SkipIfUniform] Gaussian sample, Gaussian mean, [Proper] Gamma precision, [Fresh] Gamma q)
		{
			return SampleAverageConditional(mean, sample, precision, q);
		}
	}
}
