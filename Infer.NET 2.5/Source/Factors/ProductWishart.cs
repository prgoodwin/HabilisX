using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;

namespace MicrosoftResearch.Infer.Factors
{
	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.Product(double, PositiveDefiniteMatrix)"/>, given random arguments to the function.
	/// </summary>
	[FactorMethod(typeof(Factor), "Product", typeof(PositiveDefiniteMatrix), typeof(double))]
	[FactorMethod(new string[] { "Product", "b", "a" }, typeof(Factor), "Product", typeof(double), typeof(PositiveDefiniteMatrix))]
	[Quality(QualityBand.Experimental)]
	public static class ProductWishartOp
	{
		[Skip]
		public static double LogEvidenceRatio(Wishart product, Wishart a, double b) { return 0.0; }

		public static double LogEvidenceRatio(PositiveDefiniteMatrix product, Wishart a, double b)
		{
			throw new NotImplementedException();
		}

		public static Wishart ProductAverageConditional([SkipIfUniform] Wishart A, double B, Wishart result)
		{
			if (A.IsPointMass) {
				result.Rate.SetTo(A.Point);
				result.Rate.Scale(B);
			} else {
				result.SetTo(A);
				result.Rate.Scale(1/B);
			}
			return result;
		}

		public static Gamma BAverageConditional([SkipIfUniform] Wishart Product, PositiveDefiniteMatrix A)
		{
			if (Product.IsPointMass) return BAverageLogarithm(Product.Point, A);
			// (ab)^(shape-(d+1)/2) exp(-tr(rate*(ab)))
			return Gamma.FromShapeAndRate(Product.Shape + (1 - (Product.Dimension+1)*0.5), Matrix.TraceOfProduct(Product.Rate, A));
		}

		public static Gamma BAverageConditional(PositiveDefiniteMatrix Product, PositiveDefiniteMatrix A)
		{
			if (Product.Count == 0) return Gamma.Uniform();
			bool allZeroA = true;
			double ratio = 0;
			for (int i = 0; i < Product.Count; i++) {
				if (A[i] != 0) {
					ratio = Product[i]/A[i];
					allZeroA = false;
				}
			}
			if (allZeroA) return Gamma.Uniform();
			for (int i = 0; i < Product.Count; i++) {
				if (Math.Abs(Product[i] - A[i]*ratio) > 1e-15) throw new ConstraintViolatedException("Product is not a multiple of B");
			}
			return Gamma.PointMass(ratio);
		}

		public static Wishart AAverageConditional([SkipIfUniform] Wishart Product, double B, Wishart result)
		{
			result.SetTo(Product);
			result.Rate.Scale(B);
			return result;
		}

		//- VMP ----------------------------------------------------------------------------------------------------------------------

		[Skip]
		public static double AverageLogFactor(Wishart product) { return 0.0; }

		/// <summary>
		/// VMP message to 'product'
		/// </summary>
		/// <param name="B">Incoming message from 'a'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="A">Incoming message from 'b'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'product' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'product' as the random arguments are varied.
		/// The formula is <c>proj[sum_(a,b) p(a,b) factor(product,a,b)]</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="B"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="A"/> is not a proper distribution</exception>
		public static Wishart ProductAverageLogarithm([SkipIfUniform] Wishart A, [SkipIfUniform] Gamma B, Wishart result)
		{
			if (B.IsPointMass) return ProductAverageLogarithm(A, B.Point, result);
			// E[x] = E[a]*E[b]
			// E[log(x)] = E[log(a)]+E[log(b)]
			PositiveDefiniteMatrix m = new PositiveDefiniteMatrix(A.Dimension, A.Dimension);
			A.GetMean(m);
			m.Scale(B.GetMean());
			double meanLogDet = A.Dimension*B.GetMeanLog() + A.GetMeanLogDeterminant();
			if (m.LogDeterminant() < meanLogDet) throw new MatrixSingularException(m);
			return Wishart.FromMeanAndMeanLogDeterminant(m, meanLogDet, result);
		}
		/// <summary>
		/// VMP message to 'product'
		/// </summary>
		/// <param name="B">Constant value for 'a'.</param>
		/// <param name="A">Incoming message from 'b'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'product' argument</returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'product' as the random arguments are varied.
		/// The formula is <c>proj[sum_(b) p(b) factor(product,a,b)]</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="A"/> is not a proper distribution</exception>
		public static Wishart ProductAverageLogarithm([SkipIfUniform] Wishart A, double B, Wishart result)
		{
			return ProductAverageConditional(A, B, result);
		}
		/// <summary>
		/// VMP message to 'a'
		/// </summary>
		/// <param name="Product">Incoming message from 'product'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="A">Incoming message from 'b'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'a' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'a'.
		/// Because the factor is deterministic, 'product' is integrated out before taking the logarithm.
		/// The formula is <c>exp(sum_(b) p(b) log(sum_product p(product) factor(product,a,b)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="Product"/> is not a proper distribution</exception>
		/// <exception cref="ImproperMessageException"><paramref name="A"/> is not a proper distribution</exception>
		public static Gamma BAverageLogarithm([SkipIfUniform] Wishart Product, [Proper] Wishart A)
		{
			if (A.IsPointMass) return BAverageLogarithm(Product, A.Point);
			if (Product.IsPointMass) return BAverageLogarithm(Product.Point, A);
			// (ab)^(shape-(d+1)/2) exp(-tr(rate*(ab)))
			return Gamma.FromShapeAndRate(Product.Shape + (1 - (Product.Dimension+1)*0.5), Matrix.TraceOfProduct(Product.Rate, A.GetMean()));
		}
		public static Gamma BAverageLogarithm([SkipIfUniform] Wishart Product, [Proper] PositiveDefiniteMatrix A)
		{
			return BAverageConditional(Product, A);
		}
		public static Gamma BAverageLogarithm(PositiveDefiniteMatrix Product, PositiveDefiniteMatrix A)
		{
			return BAverageConditional(Product, A);
		}
		public static Wishart AAverageLogarithm([SkipIfUniform] Wishart Product, [Proper] Gamma B, Wishart result)
		{
			if (B.IsPointMass) return AAverageLogarithm(Product, B.Point, result);
			if (Product.IsPointMass) return AAverageLogarithm(Product.Point, B, result);
			// (ab)^(shape-1) exp(-rate*(ab))
			result.Shape = Product.Shape;
			result.Rate.SetToProduct(Product.Rate, B.GetMean());
			return result;
		}
		/// <summary>
		/// VMP message to 'a'
		/// </summary>
		/// <param name="Product">Constant value for 'product'.</param>
		/// <param name="B">Incoming message from 'b'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <returns>The outgoing VMP message to the 'a' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the exponential of the average log-factor value, where the average is over all arguments except 'a'.
		/// The formula is <c>exp(sum_(b) p(b) log(factor(product,a,b)))</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="B"/> is not a proper distribution</exception>
		[NotSupported(GaussianProductVmpOp.NotSupportedMessage)]
		public static Gamma BAverageLogarithm(PositiveDefiniteMatrix Product, [Proper] Wishart A)
		{
			throw new NotSupportedException(GaussianProductVmpOp.NotSupportedMessage);
		}
		[NotSupported(GaussianProductVmpOp.NotSupportedMessage)]
		public static Wishart AAverageLogarithm(PositiveDefiniteMatrix Product, [Proper] Gamma B, Wishart result)
		{
			throw new NotSupportedException(GaussianProductVmpOp.NotSupportedMessage);
		}
		/// <summary>
		/// VMP message to 'a'
		/// </summary>
		/// <param name="Product">Incoming message from 'product'. Must be a proper distribution.  If uniform, the result will be uniform.</param>
		/// <param name="B">Constant value for 'b'.</param>
		/// <returns>The outgoing VMP message to the 'a' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'a' with 'product' integrated out.
		/// The formula is <c>sum_product p(product) factor(product,a,b)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="Product"/> is not a proper distribution</exception>
		public static Wishart AAverageLogarithm([SkipIfUniform] Wishart Product, double B, Wishart result)
		{
			if (Product.IsPointMass) return AAverageLogarithm(Product.Point, B, result);
			return AAverageConditional(Product, B, result);
		}
		/// <summary>
		/// VMP message to 'a'
		/// </summary>
		/// <param name="Product">Constant value for 'product'.</param>
		/// <param name="B">Constant value for 'b'.</param>
		/// <returns>The outgoing VMP message to the 'a' argument</returns>
		/// <remarks><para>
		/// The outgoing message is the factor viewed as a function of 'a' conditioned on the given values.
		/// </para></remarks>
		public static Wishart AAverageLogarithm(PositiveDefiniteMatrix Product, double B, Wishart result)
		{
			result.Point = Product;
			result.Point.Scale(1/B);
			return result;
		}

	}

#if false
	[FactorMethod(typeof(Factor), "Product", typeof(PositiveDefiniteMatrix), typeof(double))]
	[FactorMethod(new string[] { "Product", "b", "a" }, typeof(Factor), "Product", typeof(double), typeof(PositiveDefiniteMatrix))]
	[Buffers("Q")]
	[Quality(QualityBand.Experimental)]
	public static class ProductWishartOp_Laplace
	{
		public static Wishart ProductAverageConditional(Wishart Product, Wishart A, Gamma B, Wishart result)
		{
		}
	}
#endif
}
