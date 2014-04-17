using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Utils;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;
using MicrosoftResearch.Infer.Factors;
//[assembly: HasMessageFunctions]

namespace MicrosoftResearch.Infer.Factors
{
	internal static class ExperimentalFactor
	{
		[ParameterNames("items", "array", "indices", "dict")]
		public static T[] GetItemsWithDictionary<T>(IList<T> array, IList<string> indices, IDictionary<string, int> dict)
		{
			T[] result = new T[indices.Count];
			for (int i = 0; i < indices.Count; i++) {
				result[i] = array[dict[indices[i]]];
			}
			return result;
		}
	}

	/// <summary>
	/// Provides outgoing messages for <see cref="Factor.GetItems{T}"/>, given random arguments to the function.
	/// This factor gets a sub-array of (possibly duplicated) items from an array of items
	/// </summary>
	[FactorMethod(typeof(ExperimentalFactor), "GetItemsWithDictionary<>")]
	[Quality(QualityBand.Experimental)]
	[Buffers("marginal")]
	internal static class GetItemsWithDictionaryOp<T>
	{
		public static ArrayType MarginalInit<ArrayType>([SkipIfUniform] ArrayType array)
			where ArrayType : ICloneable
		{
			return (ArrayType)array.Clone();
		}
		[SkipIfAllUniform]
		public static ArrayType Marginal<ArrayType, DistributionType>(ArrayType array, [NoInit] IList<DistributionType> items, IList<string> indices, IDictionary<string, int> dict, ArrayType result)
			where ArrayType : IList<DistributionType>, SettableTo<ArrayType>
			where DistributionType : SettableToProduct<DistributionType>
		{
			result.SetTo(array);
			for (int i = 0; i < indices.Count; i++) {
				int index = dict[indices[i]];
				DistributionType value = result[index];
				value.SetToProduct(value, items[i]);
				result[index] = value;
			}
			return result;
		}
		public static ArrayType MarginalIncrement<ArrayType, DistributionType>(ArrayType result, DistributionType to_item, [SkipIfUniform] DistributionType item, IList<string> indices, IDictionary<string, int> dict, int resultIndex)
			where ArrayType : IList<DistributionType>, SettableTo<ArrayType>
			where DistributionType : SettableToProduct<DistributionType>
		{
			int i = resultIndex;
			int index = dict[indices[i]];
			DistributionType value = result[index];
			value.SetToProduct(to_item, item);
			result[index] = value;
			return result;
		}

		// must have an (unused) 'array' argument to determine the type of 'marginal' buffer
		public static DistributionType ItemsAverageConditional<ArrayType, DistributionType>([Indexed, Cancels] DistributionType items, [IgnoreDependency] ArrayType array, [SkipIfAllUniform] ArrayType marginal, IList<string> indices, IDictionary<string, int> dict, int resultIndex, DistributionType result)
			where ArrayType : IList<DistributionType>
			where DistributionType : SettableToProduct<DistributionType>, SettableToRatio<DistributionType>
		{
			int i = resultIndex;
			int index = dict[indices[i]];
			result.SetToRatio(marginal[index], items);
			return result;
		}

		/// <summary>
		/// EP message to 'array'
		/// </summary>
		/// <param name="items">Incoming message from 'items'. Must be a proper distribution.  If all elements are uniform, the result will be uniform.</param>
		/// <param name="indices">Constant value for 'indices'.</param>
		/// <param name="result">Modified to contain the outgoing message</param>
		/// <returns><paramref name="result"/></returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'array' as the random arguments are varied.
		/// The formula is <c>proj[p(array) sum_(items) p(items) factor(items,array,indices)]/p(array)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="items"/> is not a proper distribution</exception>
		public static ArrayType ArrayAverageConditional<DistributionType, ArrayType>([SkipIfAllUniform] IList<DistributionType> items, IList<string> indices, IDictionary<string, int> dict, ArrayType result)
			where ArrayType : IList<DistributionType>, SettableToUniform
			where DistributionType : SettableToUniform, SettableToProduct<DistributionType>
		{
			result.SetToUniform();
			for (int i = 0; i < indices.Count; i++) {
				int index = dict[indices[i]];
				DistributionType value = result[index];
				value.SetToProduct(value, items[i]);
				result[index] = value;
			}
			return result;
		}

		/// <summary>
		/// EP message to 'array'
		/// </summary>
		/// <param name="items">Incoming message from 'items'. Must be a proper distribution.  If all elements are uniform, the result will be uniform.</param>
		/// <param name="indices">Constant value for 'indices'.</param>
		/// <param name="result">Modified to contain the outgoing message</param>
		/// <returns><paramref name="result"/></returns>
		/// <remarks><para>
		/// The outgoing message is a distribution matching the moments of 'array' as the random arguments are varied.
		/// The formula is <c>proj[p(array) sum_(items) p(items) factor(items,array,indices)]/p(array)</c>.
		/// </para></remarks>
		/// <exception cref="ImproperMessageException"><paramref name="items"/> is not a proper distribution</exception>
		public static ArrayType ArrayAverageConditional<DistributionType, ArrayType>([SkipIfAllUniform] IList<T> items, IList<string> indices, IDictionary<string, int> dict, ArrayType result)
			where ArrayType : IList<DistributionType>, SettableToUniform
			where DistributionType : HasPoint<T>
		{
			result.SetToUniform();
			for (int i = 0; i < indices.Count; i++) {
				int index = dict[indices[i]];
				DistributionType value = result[index];
				value.Point = items[i];
				result[index] = value;
			}
			return result;
		}

	}
}
