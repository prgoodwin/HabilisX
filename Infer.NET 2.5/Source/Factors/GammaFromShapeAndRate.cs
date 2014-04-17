// (C) Copyright 2009-2010 Microsoft Research Cambridge
using System;
using System.Collections.Generic;
using System.Text;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;

namespace MicrosoftResearch.Infer.Factors
{
	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.GammaFromShapeAndRate"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Factor), "GammaFromShapeAndRate", Default=true)]
	[Quality(QualityBand.Mature)]
	public static class GammaFromShapeAndRateOp
	{
		public static int QuadratureNodeCount = 1000000;

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(y,shape,rate))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(double sample, double shape, double rate)
		{
			return Gamma.GetLogProb(sample, shape, rate);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(y,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(double sample, double shape, double rate) { return LogAverageFactor(sample, shape, rate); }

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'.</param>
		/// <param name="to_sample">Outgoing message to 'y'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(y) p(y) factor(y,shape,rate))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(Gamma sample, [Fresh] Gamma to_sample)
		{
			return to_sample.GetLogAverageOf(sample);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(y) p(y) factor(y,shape,rate) / sum_y p(y) messageTo(y))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		[Skip]
		public static double LogEvidenceRatio(Gamma sample, double shape, double rate)
		{
			return 0.0;
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Incoming message from 'rate'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(rate) p(rate) factor(y,shape,rate))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(double sample, double shape, Gamma rate)
		{
			// f(y,a,b) = y^(a-1) b^a/Gamma(a) exp(-by) = y^(-2) Gamma(a+1)/Gamma(a) Ga(b; a+1, y)
			Gamma to_rate = RateAverageConditional(sample, shape);
			return rate.GetLogAverageOf(to_rate) - 2*Math.Log(sample) + Math.Log(shape);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Incoming message from 'rate'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(rate) p(rate) factor(y,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(double sample, double shape, Gamma rate)
		{
			return LogAverageFactor(sample, shape, rate);
		}

		/// <summary>
		/// EP message to 'y'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>The outgoing EP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'y' conditioned on the given values.
		/// </para></remarks>
		public static Gamma SampleAverageConditional(double shape, double rate)
		{
			return Gamma.FromShapeAndRate(shape, rate);
		}

		/// <summary>
		/// EP message to 'rate'
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <returns>The outgoing EP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'rate' conditioned on the given values.
		/// </para></remarks>
		public static Gamma RateAverageConditional(double sample, double shape)
		{
			return Gamma.FromShapeAndRate(shape+1, sample);
		}

		const string NotSupportedMessage = "Expectation Propagation does not support Gamma variables with stochastic shape or rate";

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(y,shape,rate) p(y,shape,rate) factor(y,shape,rate))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static double LogAverageFactor([SkipIfUniform] Gamma sample, [SkipIfUniform] Gamma shape, [SkipIfUniform] Gamma rate)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(y,shape) p(y,shape) factor(y,shape,rate))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static double LogAverageFactor([SkipIfUniform] Gamma sample, [SkipIfUniform] Gamma shape, double rate)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		public static double LogAverageFactor([SkipIfUniform] Gamma sample, double shape, double rate)
		{
			return sample.GetLogAverageOf(Gamma.FromShapeAndRate(shape, rate));
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(y,rate) p(y,rate) factor(y,shape,rate))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		public static double LogAverageFactor([SkipIfUniform] Gamma sample, double shape, [SkipIfUniform] Gamma rate)
		{
			if (rate.IsPointMass) return LogAverageFactor(sample, shape, rate.Point);
			if (sample.IsPointMass) return LogAverageFactor(sample.Point, shape, rate);
			if (false) {
				// f(y,b) = y^(a-1) b^a exp(-by)/Gamma(a)
				// q(b) f(y,b) = y^(a-1) b^(a+a2-1) exp(-b (y+b2)) b2^a2/Gamma(a)/Gamma(a2)
				// int over b = y^(a-1) / (y + b2)^(a+a2) b2^a2 Gamma(a+a2)/Gamma(a)/Gamma(a2)
				double max = shape/rate.GetMean()*10;
				int n = 1000;
				double inc = max/n;
				double logz = double.NegativeInfinity;
				for (int i = 0; i < n; i++) {
					double x = (i+1)*inc;
					double logp = (shape + sample.Shape - 2)*Math.Log(x) - x*sample.Rate - (shape + rate.Shape)*Math.Log(x + rate.Rate);
					logz = MMath.LogSumExp(logz, logp);
				}
				logz = logz + Math.Log(inc) + rate.Shape*Math.Log(rate.Rate) + MMath.GammaLn(shape+rate.Shape) - MMath.GammaLn(shape) - MMath.GammaLn(rate.Shape) + sample.Shape*Math.Log(sample.Rate) - MMath.GammaLn(sample.Shape);
				return logz;
			} else {
				double shape1 = AddShapesMinus1(shape, rate.Shape);
				double shape2 = AddShapesMinus1(shape, sample.Shape);
				double r, rmin, rmax;
				GetIntegrationBoundsForRate(sample, shape, rate, out r, out rmin, out rmax);
				int n = QuadratureNodeCount;
				double inc = (rmax-rmin)/(n-1);
				double logz = double.NegativeInfinity;
				for (int i = 0; i < n; i++) {
					double x = rmin + i*inc;
					double logp = shape1*Math.Log(x) - shape2*Math.Log(x+sample.Rate) - x*rate.Rate;
					logz = MMath.LogSumExp(logz, logp);
				}
				logz += Math.Log(inc) + MMath.GammaLn(shape2) - MMath.GammaLn(shape) - rate.GetLogNormalizer() - sample.GetLogNormalizer();
				return logz;
			}
	
		}
		public static double LogEvidenceRatio([SkipIfUniform] Gamma sample, double shape, [SkipIfUniform] Gamma rate, Gamma to_sample)
		{
			return LogAverageFactor(sample, shape, rate) - to_sample.GetLogAverageOf(sample);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(shape,rate) p(shape,rate) factor(y,shape,rate))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static double LogAverageFactor(double sample, [SkipIfUniform] Gamma shape, [SkipIfUniform] Gamma rate)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(shape) p(shape) factor(y,shape,rate))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static double LogAverageFactor(double sample, [SkipIfUniform] Gamma shape, double rate)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'y'
		/// </summary>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'y' as the random arguments are varied.
		/// The formula is <c>proj[p(y) sum_(shape,rate) p(shape,rate) factor(y,shape,rate)]/p(y)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma SampleAverageConditional([SkipIfUniform] Gamma shape, [SkipIfUniform] Gamma rate)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}
		[Skip]
		public static Gamma SampleAverageConditionalInit()
		{
			return Gamma.Uniform();
		}

		/// <summary>
		/// EP message to 'y'
		/// </summary>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>The outgoing EP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'y' as the random arguments are varied.
		/// The formula is <c>proj[p(y) sum_(shape) p(shape) factor(y,shape,rate)]/p(y)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma SampleAverageConditional([SkipIfUniform] Gamma shape, double rate)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'y'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'y' as the random arguments are varied.
		/// The formula is <c>proj[p(y) sum_(rate) p(rate) factor(y,shape,rate)]/p(y)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		//[NotSupported(NotSupportedMessage)]
		[Quality(QualityBand.Experimental)]
		public static Gamma SampleAverageConditional(Gamma sample, double shape, [SkipIfUniform] Gamma rate)
		{
			if (sample.IsPointMass || rate.Rate == 0) return Gamma.Uniform();
			if (rate.IsPointMass) return SampleAverageConditional(shape, rate.Point);
			double shape1 = AddShapesMinus1(shape, rate.Shape);
			double shape2 = AddShapesMinus1(shape, sample.Shape);
			double sampleMean, sampleVariance;
			if (sample.Rate == 0) sample = Gamma.FromShapeAndRate(sample.Shape, 1e-20);
			if (sample.Rate == 0) {
				sampleMean = shape2*rate.GetMeanInverse();
				sampleVariance = shape2*(1+shape2)*rate.GetMeanPower(-2) - sampleMean*sampleMean;
			} else if(true) {
				// quadrature over sample
				double y, ymin, ymax;
				GetIntegrationBoundsForSample(sample, shape, rate, out y, out ymin, out ymax);
				int n = QuadratureNodeCount;
				double inc = (ymax-ymin)/(n-1);
				shape1 = sample.Shape+shape-2;
				shape2 = shape+rate.Shape;
				double shift = shape1*Math.Log(y) - shape2*Math.Log(y+rate.Rate) - y*sample.Rate;
				MeanVarianceAccumulator mva = new MeanVarianceAccumulator();
				for (int i = 0; i < n; i++) {
					double x = ymin + i*inc;
					double logp = shape1*Math.Log(x) - shape2*Math.Log(x+rate.Rate) - x*sample.Rate;
					//if (i == 0 || i == n-1) Console.WriteLine(logp-shift);
					if ((i == 0 || i == n-1) && (logp-shift > -50)) throw new Exception("invalid integration bounds");
					double p = Math.Exp(logp - shift);
					mva.Add(x, p);
				}
				sampleMean = mva.Mean;
				sampleVariance = mva.Variance;
			} else {
				// quadrature over rate
				// sampleMean = E[ shape2/(sample.Rate + r) ]
				// sampleVariance = var(shape2/(sample.Rate + r)) + E[ shape2/(sample.Rate+r)^2 ]
				//                = shape2^2*var(1/(sample.Rate + r)) + shape2*(var(1/(sample.Rate+r)) + (sampleMean/shape2)^2)
				double r, rmin, rmax;
				GetIntegrationBoundsForRate(sample, shape, rate, out r, out rmin, out rmax);
				int n = QuadratureNodeCount;
				double inc = (rmax-rmin)/(n-1);
				double shift = shape1*Math.Log(r) - shape2*Math.Log(r+sample.Rate) - r*rate.Rate;
				MeanVarianceAccumulator mva = new MeanVarianceAccumulator();
				for (int i = 0; i < n; i++) {
					double x = rmin + i*inc;
					double logp = shape1*Math.Log(x) - shape2*Math.Log(x+sample.Rate) - x*rate.Rate;
					//if (i == 0 || i == n-1) Console.WriteLine(logp-shift);
					if ((i == 0 || i == n-1) && (logp-shift > -50)) throw new Exception("invalid integration bounds");
					double p = Math.Exp(logp - shift);
					double f = 1/(x + sample.Rate);
					mva.Add(f, p);
				}
				sampleMean = shape2*mva.Mean;
				sampleVariance = shape2*(1+shape2)*mva.Variance + shape2*mva.Mean*mva.Mean;
			}
			Gamma sampleMarginal = Gamma.FromMeanAndVariance(sampleMean, sampleVariance);
			Gamma result = new Gamma();
			result.SetToRatio(sampleMarginal, sample, true);
			if (double.IsNaN(result.Shape) || double.IsNaN(result.Rate)) throw new Exception("result is nan");
			return result;
		}

		private static void GetIntegrationBoundsForSample(Gamma sample, double shape, Gamma rate, out double y, out double ymin, out double ymax)
		{
			// f(y,r) = y^(s-1) r^s exp(-r y)/Gamma(s)
			// q(r) f(y,r) = y^(s-1) r^(s+ar-1) exp(-r (y+br)) br^ar/Gamma(ar)/Gamma(s)
			// int over r = y^(s-1) / (y + br)^(s+ar) br^ar Gamma(s+ar)/Gamma(ar)/Gamma(s)
			// compare to r^s / (r + by)^(ay+s-1)
			GetIntegrationBoundsForRate(Gamma.FromShapeAndRate(rate.Shape+2,rate.Rate), shape-1, sample, out y, out ymin, out ymax);
		}

		private static void GetIntegrationBoundsForRate(Gamma sample, double shape, Gamma rate, out double r, out double rmin, out double rmax)
		{
			if (sample.Rate == 0) throw new ArgumentException("sample.Rate == 0");
			double shape1 = AddShapesMinus1(shape, rate.Shape);
			double shape2 = AddShapesMinus1(shape, sample.Shape);
			// q(r) = r^(ar-1) exp(-br r)
			// q(y) = y^(ay-1) exp(-by y)
			// f(y,r) = y^(s-1) r^s exp(-ry)
			// q(y) f(y,r) = y^(ay+s-2) r^s exp(-y (r+by))
			// int over y = r^s / (r+by)^(ay+s-1)
			// logp = (s+ar-1)*log(r) - (ay+s-1)*log(r+by) - r*br
			// shape1*log(rmax) - shape2*log(rmax+by) - rmax*br < shape1*log(r) - shape2*log(r+by) - r*br - 50
			r = GammaFromShapeAndRateOp_Laplace.Q_slow(sample, shape, rate).GetMean();
			double bound = shape1*Math.Log(r) - shape2*Math.Log(r+sample.Rate) - r*rate.Rate - 50;
			if (shape1 >= 0) {
				if (shape2 >= 0) {
					double slope, offset;
					// shape1*log(rmax) -shape2*log(rmax+by) - rmax*br < bound
					// log(rmax) < rmax/u + log(u)-1  for any u
					// log(rmax+by) > q*log(rmax/q) + (1-q)*log(by/(1-q))
					// slope*rmax + offset < bound
					// must have slope < 0, u > (shape1-shape2*q)/br
					double q = r/(r + sample.Rate);
					if (shape1 - shape2*q > 0) {
						if (true) {
							double u = r + 50/rate.Rate;
							slope = (shape1 - shape2*q)/u - rate.Rate;
							offset = (shape1-shape2*q)*(Math.Log(u)-1) - shape2*((1-q)*Math.Log(sample.Rate/(1-q)) - q*Math.Log(q));
							rmax = (bound - offset)/slope;
						} else {
							// rmax = exp(log(rmax)) > 1 + log(rmax) + 0.5*log(rmax)^2
							// (shape1-shape2*q)*log(rmax) - br*(1 + log(rmax) + 0.5*log(rmax)^2)
							double a = -rate.Rate/2;
							double b = shape1 - shape2*q - rate.Rate;
							double c = -rate.Rate - bound - shape2*((1-q)*Math.Log(sample.Rate/(1-q)) - q*Math.Log(q));
							rmax = Math.Exp((-b - Math.Sqrt(b*b - 4*a*c))/(2*a));
							Console.WriteLine(a*Math.Log(rmax)*Math.Log(rmax) + b*Math.Log(rmax) + c);
						}
					} else {
						// try two bounds and see which gives the smaller rmax
						// case 1: log term dominates the linear term
						// shape1*log(rmax) - shape2*(q*log(rmax/q) + (1-q)*log(by/(1-q))) -r*br < bound
						slope = shape1 - shape2*q;
						offset = -shape2*((1-q)*Math.Log(sample.Rate/(1-q)) - q*Math.Log(q)) -r*rate.Rate;
						double rmax1 = Math.Exp((bound - offset)/slope);
						// case 2: linear term dominates the log term
						slope = -rate.Rate;
						offset = shape1*Math.Log(r) - shape2*Math.Log(r+sample.Rate);
						rmax = (bound - offset)/slope;
						rmax = Math.Min(rmax, rmax1);
					}
					if (double.IsPositiveInfinity(rmax)) throw new Exception("rmax is infinity");
					// shape1*log(rmin) -shape2*log(rmin+by) - rmin*br < bound
					// rmin > u*log(rmin) - u*log(u) + u  for any u
					// rmin > 0
					// log(rmin+by) > log(by)
					// slope*log(rmin) + offset < bound
					slope = shape1;
					offset = -shape2*Math.Log(sample.Rate);
					rmin = Math.Exp((bound - offset)/slope);
				} else throw new NotImplementedException();
			} else throw new NotImplementedException();
			//Console.WriteLine("rmin = {0}, rmax = {1}", rmin, rmax);
		}

		private static double AddShapesMinus1(double a1, double a2)
		{
			if (a1 < a2)
				return a1 + (a2 - 1);
			else
				return a2 + (a1 - 1);
		}

		/// <summary>
		/// EP message to 'rate'
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'rate' as the random arguments are varied.
		/// The formula is <c>proj[p(rate) sum_(y,shape) p(y,shape) factor(y,shape,rate)]/p(rate)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma RateAverageConditional([SkipIfUniform] Gamma sample, [SkipIfUniform] Gamma shape)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'rate'
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing EP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'rate' as the random arguments are varied.
		/// The formula is <c>proj[p(rate) sum_(shape) p(shape) factor(y,shape,rate)]/p(rate)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma RateAverageConditional(double sample, [SkipIfUniform] Gamma shape)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'rate'
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <returns>The outgoing EP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'rate' as the random arguments are varied.
		/// The formula is <c>proj[p(rate) sum_(y) p(y) factor(y,shape,rate)]/p(rate)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		//[NotSupported(NotSupportedMessage)]
		[Quality(QualityBand.Experimental)]
		public static Gamma RateAverageConditional([SkipIfUniform] Gamma sample, double shape, Gamma rate)
		{
			if (rate.IsPointMass) return Gamma.Uniform();
			if (sample.IsPointMass) return RateAverageConditional(sample.Point, shape);
			if (sample.Rate == 0) return Gamma.Uniform();
			double shape1 = AddShapesMinus1(shape, rate.Shape);
			double shape2 = AddShapesMinus1(shape, sample.Shape);
			double rateMean, rateVariance;
			double r, rmin, rmax;
			GetIntegrationBoundsForRate(sample, shape, rate, out r, out rmin, out rmax);
			int n = QuadratureNodeCount;
			double inc = (rmax-rmin)/(n-1);
			double shift = shape1*Math.Log(r) - shape2*Math.Log(r+sample.Rate) - r*rate.Rate;
			MeanVarianceAccumulator mva = new MeanVarianceAccumulator();
			for (int i = 0; i < n; i++) {
				double x = rmin + i*inc;
				double logp = shape1*Math.Log(x) - shape2*Math.Log(x+sample.Rate) - x*rate.Rate;
				if ((i == 0 || i == n-1) && (logp-shift > -50)) throw new Exception("invalid integration bounds");
				double p = Math.Exp(logp - shift);
				mva.Add(x, p);
			}
			rateMean = mva.Mean;
			rateVariance = mva.Variance;
			Gamma rateMarginal = Gamma.FromMeanAndVariance(rateMean, rateVariance);
			Gamma result = new Gamma();
			result.SetToRatio(rateMarginal, rate, true);
			if (double.IsNaN(result.Shape) || double.IsNaN(result.Rate)) throw new Exception("result is nan");
			return result;
		}

		/// <summary>
		/// EP message to 'shape'
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <param name="result">Modified to contain the outgoing message</param>
		/// <returns><paramref name="result"/></returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'shape' as the random arguments are varied.
		/// The formula is <c>proj[p(shape) sum_(y) p(y) factor(y,shape,rate)]/p(shape)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma ShapeAverageConditional([SkipIfUniform] Gamma sample, [SkipIfUniform] Gamma shape, double rate, [SkipIfUniform] Gamma result)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'shape'
		/// </summary>
		/// <param name="y">Constant value for 'y'.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="result">Modified to contain the outgoing message</param>
		/// <returns><paramref name="result"/></returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'shape' as the random arguments are varied.
		/// The formula is <c>proj[p(shape) sum_(rate) p(rate) factor(y,shape,rate)]/p(shape)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma ShapeAverageConditional(double sample, [SkipIfUniform] Gamma shape, [SkipIfUniform] Gamma rate, [SkipIfUniform] Gamma result)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'shape'
		/// </summary>
		/// <param name="y">Constant value for 'y'.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <param name="result">Modified to contain the outgoing message</param>
		/// <returns><paramref name="result"/></returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'shape' conditioned on the given values.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma ShapeAverageConditional(double sample, [SkipIfUniform] Gamma shape, double rate, [SkipIfUniform] Gamma result)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		/// <summary>
		/// EP message to 'shape'
		/// </summary>
		/// <param name="y">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="result">Modified to contain the outgoing message</param>
		/// <returns><paramref name="result"/></returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'shape' as the random arguments are varied.
		/// The formula is <c>proj[p(shape) sum_(y,rate) p(y,rate) factor(y,shape,rate)]/p(shape)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="y"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[NotSupported(NotSupportedMessage)]
		public static Gamma ShapeAverageConditional([SkipIfUniform] Gamma sample, [SkipIfUniform] Gamma shape, [SkipIfUniform] Gamma rate, [SkipIfUniform] Gamma result)
		{
			throw new NotSupportedException(NotSupportedMessage);
		}

		// VMP ----------------------------------------------------------------------------------------

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(y,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		public static double AverageLogFactor(double sample, double shape, double rate)
		{
			return LogAverageFactor(sample, shape, rate);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(y,shape,rate) p(y,shape,rate) log(factor(y,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[Quality(QualityBand.Experimental)]
		public static double AverageLogFactor([Proper] Gamma sample, [Proper] Gamma shape, [Proper] Gamma rate)
		{
			double a = shape.GetMean();
			return (a-1)*sample.GetMeanLog() + a*rate.GetMeanLog() - rate.GetMean()*sample.GetMean() - ELogGamma(shape);
		}

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>sum_(y) p(y) log(factor(y,shape,rate))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		public static double AverageLogFactor([Proper] Gamma sample, double shape, double rate)
		{
			Gamma to_sample = SampleAverageLogarithm(shape, rate);
			return sample.GetAverageLog(to_sample);
		}

		/// <summary>
		/// VMP message to 'y'
		/// </summary>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'y'.
		/// The formula is <c>exp(sum_(shape,rate) p(shape,rate) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[Quality(QualityBand.Experimental)]
		public static Gamma SampleAverageLogarithm([SkipIfUniform] Gamma shape, [SkipIfUniform] Gamma rate)
		{
			return Gamma.FromShapeAndRate(shape.GetMean(), rate.GetMean());
		}

		/// <summary>
		/// VMP message to 'y'
		/// </summary>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Constant value for 'rate'.</param>
		/// <returns>The outgoing VMP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'y'.
		/// The formula is <c>exp(sum_(shape) p(shape) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		[Quality(QualityBand.Experimental)]
		public static Gamma SampleAverageLogarithm([SkipIfUniform] Gamma shape, double rate)
		{
			return Gamma.FromShapeAndRate(shape.GetMean(), rate);
		}

		/// <summary>
		/// VMP message to 'y'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'y'.
		/// The formula is <c>exp(sum_(rate) p(rate) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		public static Gamma SampleAverageLogarithm(double shape, [SkipIfUniform] Gamma rate)
		{
			return Gamma.FromShapeAndRate(shape, rate.GetMean());
		}

		/// <summary>
		/// VMP message to 'y'
		/// </summary>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="rate">Constant value for 'rate'..</param>
		/// <returns>The outgoing VMP message to the 'y' argument</returns>
		/// <remarks><para>
		/// The message is simply the factor itself.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		public static Gamma SampleAverageLogarithm(double shape, double rate)
		{
			return Gamma.FromShapeAndRate(shape, rate);
		}

		/// <summary>
		/// VMP message to 'rate'
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'rate'.
		/// The formula is <c>exp(sum_(y,shape) p(y,shape) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		public static Gamma RateAverageLogarithm([SkipIfUniform] Gamma sample, [SkipIfUniform] Gamma shape)
		{
			// factor = rate^shape exp(-y*rate)
			// log(factor) = shape*log(rate) - y*rate
			// E[log(factor)] = E[shape]*log(rate) - E[y]*rate
			return Gamma.FromShapeAndRate(shape.GetMean() + 1, sample.GetMean());
		}

		/// <summary>
		/// VMP message to 'rate'
		/// </summary>
		/// <param name="sample">Constant value for 'y'.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'rate'.
		/// The formula is <c>exp(sum_(shape) p(shape) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		public static Gamma RateAverageLogarithm(double sample, [SkipIfUniform] Gamma shape)
		{
			return Gamma.FromShapeAndRate(shape.GetMean() + 1, sample);
		}

		/// <summary>
		/// VMP message to 'rate'
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <returns>The outgoing VMP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'rate'.
		/// The formula is <c>exp(sum_(y) p(y) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		public static Gamma RateAverageLogarithm([SkipIfUniform] Gamma sample, double shape)
		{
			return Gamma.FromShapeAndRate(shape + 1, sample.GetMean());
		}

		/// <summary>
		/// VMP message to 'rate'
		/// </summary>
		/// <param name="sample">Constant value for 'y'..</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <returns>The outgoing VMP message to the 'rate' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'rate'.
		/// The formula is <c>exp(sum_(y) p(y) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		public static Gamma RateAverageLogarithm(double sample, double shape)
		{
			return Gamma.FromShapeAndRate(shape + 1, sample);
		}

		/// <summary>
		/// VMP message to 'shape'
		/// </summary>
		/// <param name="sample">Incoming message from 'y'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="shape">Incoming message from 'shape'. Must be a proper distribution.  If uniform, the result will be uniform. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="rate">Incoming message from 'rate'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="to_shape">Previous outgoing message to 'shape'.</param>
		/// <returns>The outgoing VMP message to the 'shape' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'shape'.
		/// The formula is <c>exp(sum_(y,rate) p(y,rate) log(factor(y,shape,rate)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="sample"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="shape"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="rate"/> is not a proper distribution</exception>
		[Quality(QualityBand.Experimental)]
		public static Gamma ShapeAverageLogarithm([SkipIfUniform] Gamma sample, [Proper] Gamma shape, [Proper] Gamma rate, Gamma to_shape)
		{
			//if (to_shape.IsUniform())
			//	to_shape.SetShapeAndRate(1, 1);
			//if (y.Rate == 0 || Double.IsInfinity(y.Rate))
			//	y.SetShapeAndRate(1, 1);
			//if (rate.Rate==0 || Double.IsInfinity(rate.Rate))
			//	rate.SetShapeAndRate(1, 1);
			double ElogYR = sample.GetMeanLog() + rate.GetMeanLog();
			double a, b;
			a = shape.Shape;
			b = shape.Rate;

			// Find required expectations using quadrature
			Vector gradElogGamma = CalculateDerivatives(shape);

			// Calculate gradients
			Vector gradS = -gradElogGamma;
			gradS[0] += ElogYR / b;
			gradS[1] += -a * ElogYR / (b * b);

			// Calculate the required message to match these gradients
			Gamma approximateFactor = NonconjugateProjection(shape, gradS);

			double damping = 0.0;
			if (damping == 0.0)
				return approximateFactor;
			else
				return (approximateFactor^(1-damping)) * (to_shape ^ damping);
		}

		/// <summary>
		/// Project the standard VMP message onto a gradient matched Gamma message. 
		/// </summary>
		/// <param name="context">Incoming message.</param>
		/// <param name="gradS">Gradient of S=int factor(x) p(x) dx</param>
		/// <returns>Projected gamma message</returns>
		internal static Gamma NonconjugateProjection(Gamma context, Vector gradS)
		{
			Matrix mat = new Matrix(2, 2);
			mat[0, 0] = MMath.Trigamma(context.Shape);
			mat[1, 0] = mat[0, 1] = -1 / context.Rate;
			mat[1, 1] = context.Shape / (context.Rate * context.Rate);
			Vector v = twoByTwoInverse(mat) * gradS;
			return Gamma.FromShapeAndRate(v[0] + 1, v[1]);
		}

		/// <summary>
		/// Two by two matrix inversion. 
		/// </summary>
		/// <param name="a">Matrix to invert</param>
		/// <returns>Inverted matrix</returns>
		internal static Matrix twoByTwoInverse(Matrix a)
		{
			Matrix result = new Matrix(2, 2);
			double det = a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
			result[0, 0] = a[1, 1] / det;
			result[0, 1] = -a[0, 1] / det;
			result[1, 0] = -a[1, 0] / det;
			result[1, 1] = a[0, 0] / det;
			return result;
		}

		/// <summary>
		/// Calculate derivatives of \int G(x;a,b) LogGamma(x) dx wrt (a,b)
		/// </summary>
		/// <param name="q">The Gamma distribution G(x;a,b).</param>
		/// <returns>A 2-vector containing derivatives of \int G(x;a,b) LogGamma(x) dx wrt (a,b).</returns>
		/// <remarks><para>
		///  Calculates expectations in x=log(s) space using Gauss-Hermite quadrature. 
		///  For each integral the behaviour as x->0 is subtracted from the integrand 
		///  before performing quadrature to remove the singularity there. 
		/// </para></remarks>
		public static Vector CalculateDerivatives(Gamma q)
		{
			Vector gradElogGamma = Vector.Zero(2);
			// Get shape and scale of the distribution
			double a = q.Shape;
			double b = q.Rate;
			// Mean in the transformed domain
			double proposalMean = q.GetMeanLog();
			// Laplace approximation of variance in transformed domain 
			double proposalVariance = 1 / a;
			//double proposalVariance = Math.Exp(-proposalMean) / b; 

			// Quadrature coefficient
			int n = 11;
			Vector nodes = Vector.Zero(n);
			Vector weights = Vector.Zero(n);
			Quadrature.GaussianNodesAndWeights(proposalMean, proposalVariance, nodes, weights);

			double EXDigamma = 0;
			double ELogGam = 0;
			double ELogXLogGamma = 0;

			// Calculate expectations in x=log(s) space using Gauss-Hermite quadrature
			double logZ = MMath.GammaLn(a) - a * Math.Log(b);
			for (int i = 0; i < n; i++) {
				double x = nodes[i];
				double expx = Math.Exp(x);
				double p = a * x - b * expx - logZ - Gaussian.GetLogProb(x, proposalMean, proposalVariance);
				p = Math.Exp(p) * weights[i];
				EXDigamma += p * (expx * MMath.Digamma(expx) + 1);
				ELogGam += p * (MMath.GammaLn(expx) + x);
				ELogXLogGamma += p * (x * MMath.GammaLn(expx) + x * x);
			}

			// Normalise and add removed components
			ELogGam = ELogGam - proposalMean;
			ELogXLogGamma = ELogXLogGamma - (MMath.Trigamma(a) + proposalMean * proposalMean);
			EXDigamma = EXDigamma - 1;

			// Calculate derivatives
			gradElogGamma[0] = ELogXLogGamma - proposalMean * ELogGam;
			gradElogGamma[1] = -1.0 / b * EXDigamma;
			//gradElogGamma[1] = (ELogGamma(q) - ELogGamma(new Gamma(a + 1, b))) * a / b; 
			return gradElogGamma;
		}

		public static Vector CalculateDerivativesTrapezoid(Gamma q)
		{
			Vector gradElogGamma = Vector.Zero(2);
			// Get shape and scale of the distribution
			double a = q.Shape;
			double b = q.Rate;
			double mean, variance;
			q.GetMeanAndVariance(out mean, out variance);
			double upperBound = 10;

			int n = 10000;
			double ELogGamma = 0, ELogXLogGamma = 0, ExDigamma = 0;
			double inc = upperBound/n;
			for (int i = 0; i < n; i++) {
				double x = inc * (i+1);
				double logp = q.GetLogProb(x);
				double p = Math.Exp(logp);
				double f = p * MMath.GammaLn(x);
				ELogGamma += f;
				ELogXLogGamma += Math.Log(x)*f;
				ExDigamma += x*MMath.Digamma(x)*p;
			}
			ELogGamma *= inc;
			ELogXLogGamma *= inc;
			ExDigamma *= inc;
			gradElogGamma[0] = ELogXLogGamma + (Math.Log(b) - MMath.Digamma(a))*ELogGamma;
			gradElogGamma[1] = -ExDigamma/b;
			return gradElogGamma;
		}

		public static Vector CalculateDerivativesNaive(Gamma q)
		{
			Vector gradElogGamma = Vector.Zero(2);
			// Get shape and scale of the distribution
			double a = q.Shape;
			double b = q.Rate;
			// Mean in the transformed domain
			double proposalMean = q.GetMeanLog();
			// Laplace approximation of variance in transformed domain 
			double proposalVariance = 1 / a;

			// Quadrature coefficient
			int n = 11;
			Vector nodes = Vector.Zero(n);
			Vector weights = Vector.Zero(n);
			Quadrature.GaussianNodesAndWeights(proposalMean, proposalVariance, nodes, weights);

			double EXDigamma = 0;
			double ELogGam = 0;
			double ELogXLogGamma = 0;

			// Calculate expectations in x=log(s) space using Gauss-Hermite quadrature
			double logZ = MMath.GammaLn(a) - a * Math.Log(b);
			for (int i = 0; i < n; i++) {
				double x = nodes[i];
				double expx = Math.Exp(x);
				double p = a * x - b * expx - logZ - Gaussian.GetLogProb(x, proposalMean, proposalVariance);
				p = Math.Exp(p) * weights[i];
				EXDigamma += p * (expx * MMath.Digamma(expx));
				ELogGam += p * (MMath.GammaLn(expx));
				ELogXLogGamma += p * (x * MMath.GammaLn(expx));
			}

			// Calculate derivatives
			gradElogGamma[0] = ELogXLogGamma - proposalMean * ELogGam;
			gradElogGamma[1] = -1.0 / b * EXDigamma;
			//gradElogGamma[1] = (ELogGamma(q) - ELogGamma(new Gamma(a + 1, b))) * a / b; 
			return gradElogGamma;
		}

		/// <summary>
		/// Calculates \int G(x;a,b) LogGamma(x) dx
		/// </summary>
		/// <param name="q">G(x;a,b)</param>
		/// <returns>\int G(x;a,b) LogGamma(x) dx</returns>
		public static double ELogGamma(Gamma q)
		{
			if (q.IsPointMass)
				return MMath.GammaLn(q.Point);
			double a = q.Shape;
			double b = q.Rate;
			// Mean in the transformed domain
			double proposalMean = q.GetMeanLog();
			// Laplace approximation of variance in transformed domain 
			double proposalVariance = 1 / a;

			// Quadrature coefficient
			int n = 11;
			Vector nodes = Vector.Zero(n);
			Vector weights = Vector.Zero(n);
			Quadrature.GaussianNodesAndWeights(proposalMean, proposalVariance, nodes, weights);

			double ELogGamma = 0;
			double logZ = -a * Math.Log(b) + MMath.GammaLn(a);
			// Calculate expectations in x=log(s) space using Gauss-Hermite quadrature
			for (int i = 0; i < n; i++) {
				double x = nodes[i];
				double expx = Math.Exp(x);
				double p = a * x - b * expx - Gaussian.GetLogProb(x, proposalMean, proposalVariance) - logZ;
				p = Math.Exp(p + Math.Log(weights[i]));
				ELogGamma += p * (MMath.GammaLn(expx) + x);
			}

			// Add removed components
			return ELogGamma - proposalMean;
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.GammaFromShapeAndRate"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Factor), "GammaFromShapeAndRate", Default=false)]
	[Buffers("Q")]
	[Quality(QualityBand.Experimental)]
	public static class GammaFromShapeAndRateOp_Laplace
	{
		private static double GetShape2(Gamma sample, double shape)
		{
			if (sample.IsPointMass) throw new Exception();
			if (shape < sample.Shape)
				return shape + (sample.Shape - 1);
			else
				return sample.Shape + (shape - 1);
		}

		private static double[] dlogfs(double r, double shape, Gamma y)
		{
			if (y.IsPointMass) {
				// logf = s*log(r) - y*r
				double p = 1/r;
				double p2 = p*p;
				double dlogf = shape*p - y.Point;
				double ddlogf = -shape*p2;
				double dddlogf = 2*shape*p*p2;
				double d4logf = -6*shape*p2*p2;
				return new double[] { dlogf, ddlogf, dddlogf, d4logf };
			} else {
				// logf = s*log(r) - (s+ya-1)*log(r + yb)
				double p = 1/(r + y.Rate);
				double p2 = p*p;
				double r2 = r*r;
				double shape2 = GetShape2(y, shape);
				double dlogf = shape/r - shape2*p;
				double ddlogf = -shape/r2 + shape2*p2;
				double dddlogf = 2*shape/(r*r2) - 2*shape2*p*p2;
				double d4logf = -6*shape/(r2*r2) + 6*shape2*p2*p2;
				return new double[] { dlogf, ddlogf, dddlogf, d4logf };
			}
		}

		[Skip]
		public static Gamma QInit()
		{
			return Gamma.Uniform();
		}

		// Perform one update of Q
		public static Gamma Q(Gamma sample, double shape, Gamma rate, Gamma q)
		{
			if (sample.IsUniform() || rate.IsPointMass) return rate;
			double a = q.Shape;
			double b = q.Rate;
			if (b==0) {
				a = rate.Shape;
				b = rate.Rate;
				if (shape > 0) {
					// this guess comes from solving dlogf=0 for x
					double guess = shape*sample.GetMeanInverse();
					b = Math.Max(rate.Rate, a/guess);
				}
			}
			double x = a/b;
			double x2 = x*x;
			double[] dlogfss = dlogfs(x, shape, sample);
			double dlogf = dlogfss[0];
			double ddlogf = dlogfss[1];
			b = rate.Rate - (dlogf + x*ddlogf);
			a = rate.Shape - x2*ddlogf;
			if (a <= 0) a = b*rate.Shape/(rate.Rate - dlogf);
			if (a <= 0 || b <= 0) throw new Exception();
			if (double.IsNaN(a) || double.IsNaN(b)) throw new Exception("result is nan");
			return Gamma.FromShapeAndRate(a, b);
		}
		public static Gamma Q_slow(Gamma sample, double shape, Gamma rate)
		{
			Gamma q = QInit();
			for (int iter = 0; iter < 1000; iter++) {
				Gamma oldq = q;
				q = Q(sample, shape, rate, q);
				if (oldq.MaxDiff(q) < 1e-10) break;
			}
			return q;
		}

		public static double LogAverageFactor(Gamma sample, double shape, Gamma rate, [Fresh] Gamma q)
		{
			double x = q.GetMean();
			double shape2 = GetShape2(sample, shape);
			double logf = shape*Math.Log(x) - shape2*Math.Log(x + sample.Rate) + 
				MMath.GammaLn(shape2) - MMath.GammaLn(shape) - sample.GetLogNormalizer();
			double logz = logf + rate.GetLogProb(x) - q.GetLogProb(x);
			return logz;
		}
		private static double LogAverageFactor_slow(Gamma sample, double shape, Gamma rate)
		{
			return LogAverageFactor(sample, shape, rate, Q_slow(sample, shape, rate));
		}

		public static double LogEvidenceRatio(Gamma sample, double shape, Gamma rate, Gamma to_sample, [Fresh] Gamma q)
		{
			if (sample.IsPointMass) return LogEvidenceRatio(sample.Point, shape, rate);
			return LogAverageFactor(sample, shape, rate, q) - to_sample.GetLogAverageOf(sample);
		}
		public static double LogEvidenceRatio(double sample, double shape, Gamma rate)
		{
			return GammaFromShapeAndRateOp.LogEvidenceRatio(sample, shape, rate);
		}

		public static Gamma RateAverageConditional([SkipIfUniform] Gamma sample, double shape, Gamma rate, [Fresh] Gamma q)
		{
			if (sample.IsPointMass) return GammaFromShapeAndRateOp.RateAverageConditional(sample.Point, shape);
			double x = q.GetMean();
			double[] g = new double[] { x, 1, 0, 0 };
			double rateMean, rateVariance;
			GaussianOp_Laplace.LaplaceMoments(q, g, dlogfs(x, shape, sample), out rateMean, out rateVariance);
			Gamma rateMarginal = Gamma.FromMeanAndVariance(rateMean, rateVariance);
			Gamma result = new Gamma();
			result.SetToRatio(rateMarginal, rate, true);
			return result;
		}

		public static Gamma SampleAverageConditional(Gamma sample, double shape, [SkipIfUniform] Gamma rate, [Fresh] Gamma q)
		{
			if (sample.IsPointMass) return Gamma.Uniform();
			if (rate.IsPointMass) return GammaFromShapeAndRateOp.SampleAverageConditional(shape, rate.Point);
			if (q.IsPointMass) throw new Exception();
			double shape2 = GetShape2(sample, shape); // sample.Shape+shape-1
			double sampleMean, sampleVariance;
			if (sample.Rate == 0) sample = Gamma.FromShapeAndRate(sample.Shape, 1e-10);
			if (sample.Rate == 0) {
				// result.Shape > 1 iff sampleMean^2 > sampleVariance iff shape*(rate.Shape-2) > shape+rate.Shape-1
				// e.g. shape=4, rate.Shape=4
				sampleMean = shape2*rate.GetMeanInverse();
				sampleVariance = shape2*(1+shape2)*rate.GetMeanPower(-2) - sampleMean*sampleMean;
			} else if(true) {
				// Laplace for sample
				Gamma temp = Gamma.FromShapeAndRate(rate.Shape+2, rate.Rate);
				Gamma qy = Q_slow(temp, shape-1, sample);
				double x = qy.GetMean();
				double[] g = new double[] { x, 1, 0, 0 };
				GaussianOp_Laplace.LaplaceMoments(qy, g, dlogfs(x, shape-1, temp), out sampleMean, out sampleVariance);
			} else {
				// Laplace for rate
				// tends to be less accurate than above
				double x = q.GetMean();
				double x2 = x*x;
				double p = 1/(x + sample.Rate);
				double p2 = p*p;
				if (sample.Rate < x) {
					// another approach might be to write 1/(sample.Rate + r) = 1/r - sample.Rate/r/(sample.Rate + r)
					//double a1 = q.Shape - x2*p2;
					//double b1 = q.Rate + sample.Rate*p2;
					double logz = LogAverageFactor_slow(sample, shape, rate) + sample.GetLogNormalizer();
					Gamma sample1 = Gamma.FromShapeAndRate(sample.Shape+1, sample.Rate);
					double logz1 = LogAverageFactor_slow(sample1, shape, rate) + sample1.GetLogNormalizer();
					double pMean = Math.Exp(logz1 - logz);
					sampleMean = shape2*pMean;
					Gamma sample2 = Gamma.FromShapeAndRate(sample.Shape+2, sample.Rate);
					double logz2 = LogAverageFactor_slow(sample2, shape, rate) + sample2.GetLogNormalizer();
					double pMean2 = Math.Exp(logz2 - logz);
					double pVariance = pMean2 - pMean*pMean;
					sampleVariance = shape2*shape2*pVariance + shape2*(pVariance + pMean*pMean);
				} else {
					// sampleMean = E[ shape2/(sample.Rate + r) ]
					// sampleVariance = var(shape2/(sample.Rate + r)) + E[ shape2/(sample.Rate+r)^2 ]
					//                = shape2^2*var(1/(sample.Rate + r)) + shape2*(var(1/(sample.Rate+r)) + (sampleMean/shape2)^2)
					// Note: this is not a good approximation if sample.Rate is small
					double[] g = new double[] { p, -p2, 2*p2*p, -6*p2*p2 };
					double pMean, pVariance;
					GaussianOp_Laplace.LaplaceMoments(q, g, dlogfs(x, shape, sample), out pMean, out pVariance);
					sampleMean = shape2*pMean;
					sampleVariance = shape2*shape2*pVariance + shape2*(pVariance + pMean*pMean);
				}
			}
			Gamma sampleMarginal = Gamma.FromMeanAndVariance(sampleMean, sampleVariance);
			Gamma result = new Gamma();
			result.SetToRatio(sampleMarginal, sample, true);
			if (double.IsNaN(result.Shape) || double.IsNaN(result.Rate)) throw new Exception("result is nan");
			return result;
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Gamma.SampleFromMeanAndVariance"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(new string[] { "sample", "mean", "variance" }, typeof(Gamma), "SampleFromMeanAndVariance")]
	[Quality(QualityBand.Stable)]
	public static class GammaFromMeanAndVarianceOp
	{
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="variance">Constant value for 'variance'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,variance))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(double sample, double mean, double variance)
		{
			return Gamma.FromShapeAndScale(mean, variance).GetLogProb(sample);
		}
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="variance">Constant value for 'variance'.</param>
		/// <returns>Logarithm of the factor's contribution the EP model evidence</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,variance))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for EP.
		/// </para></remarks>
		public static double LogEvidenceRatio(double sample, double mean, double variance) { return LogAverageFactor(sample, mean, variance); }
		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="variance">Constant value for 'variance'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,variance))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		public static double AverageLogFactor(double sample, double mean, double variance) { return LogAverageFactor(sample, mean, variance); }
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message from 'sample'.</param>
		/// <param name="to_sample">Outgoing message to 'sample'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,mean,variance))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(Gamma sample, [Fresh] Gamma to_sample)
		{
			return to_sample.GetLogAverageOf(sample);
		}

		/// <summary>
		/// VMP message to 'sample'
		/// </summary>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="variance">Constant value for 'variance'.</param>
		/// <returns>The outgoing VMP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Gamma SampleAverageLogarithm(double mean, double variance)
		{
			return Gamma.FromMeanAndVariance(mean, variance);
		}

		/// <summary>
		/// EP message to 'sample'
		/// </summary>
		/// <param name="mean">Constant value for 'mean'.</param>
		/// <param name="variance">Constant value for 'variance'.</param>
		/// <returns>The outgoing EP message to the 'sample' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'sample' conditioned on the given values.
		/// </para></remarks>
		public static Gamma SampleAverageConditional(double mean, double variance)
		{
			return Gamma.FromMeanAndVariance(mean, variance);
		}
	}

	/// <summary>
	/// Provides just the operators for constructing Gamma distributions with this parameterisation.
	/// </summary>
	[FactorMethod(typeof(Gamma), "Sample", typeof(double), typeof(double))]
	[Quality(QualityBand.Stable)]
	public static class GammaFromShapeAndScaleOp
	{
		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,mean,variance))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(double sample, double shape, double scale)
		{
			return Gamma.FromShapeAndScale(shape, scale).GetLogProb(sample);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,mean,variance))</c>.
		/// </para></remarks>
		public static double LogEvidenceRatio(double sample, double shape, double scale) { return LogAverageFactor(sample, shape, scale); }

		[Skip]
		public static double LogEvidenceRatio(Gamma sample, double shape, double scale) { return 0.0; }

		/// <summary>
		/// Evidence message for VMP
		/// </summary>
		/// <param name="sample">Constant value for 'sample'.</param>
		/// <param name="shape">Constant value for 'shape'.</param>
		/// <param name="scale">Constant value for 'scale'.</param>
		/// <returns>Average of the factor's log-value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(factor(sample,mean,variance))</c>.
		/// Adding up these values across all factors and variables gives the log-evidence estimate for VMP.
		/// </para></remarks>
		public static double AverageLogFactor(double sample, double shape, double scale) { return LogAverageFactor(sample, shape, scale); }

		public static double AverageLogFactor([Proper] Gamma sample, double shape, double scale)
		{
			return GammaFromShapeAndRateOp.AverageLogFactor(sample, shape, 1/scale);
		}

		/// <summary>
		/// Evidence message for EP
		/// </summary>
		/// <param name="sample">Incoming message to 'sample'.</param>
		/// <param name="to_sample">Message sent to 'sample'</param>
		/// <returns>Logarithm of the factor's average value across the given argument distributions</returns>
		/// <remarks><para>
		/// The formula for the result is <c>log(sum_(sample) p(sample) factor(sample,mean,variance))</c>.
		/// </para></remarks>
		public static double LogAverageFactor(Gamma sample, [Fresh] Gamma to_sample)
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
		public static Gamma SampleAverageLogarithm(double shape, double scale)
		{
			return Gamma.FromShapeAndScale(shape, scale);
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
		public static Gamma SampleAverageConditional(double shape, double scale)
		{
			return Gamma.FromShapeAndScale(shape, scale);
		}
	}
}
