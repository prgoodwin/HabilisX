// (C) Copyright 2009-2010 Microsoft Research Cambridge
using System;
using System.Collections.Generic;
using System.Text;
using MicrosoftResearch.Infer.Maths;
using MicrosoftResearch.Infer.Utils;
using MicrosoftResearch.Infer.Factors;

namespace MicrosoftResearch.Infer.Distributions
{
	/// <summary>
	/// A discrete distribution over characters.
	/// </summary>
	public class DiscreteChar: GenericDiscreteBase<char, DiscreteChar>
	{
		/// <summary>
		/// Creates a uniform distribution over the enum values.
		/// </summary>
		public DiscreteChar() : 
			base(1+(int)char.MaxValue, Sparsity.Piecewise)
		{
		}

		/// <summary>
		/// Converts from an integer to an enum value
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		protected override char ConvertFromInt(int i)
		{
			return (char)i;
		}

		/// <summary>
		/// Converts the enum value to an integer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override int ConvertToInt(char value)
		{
			return (int)value;
		}

		/// <summary>
		/// ToString() method that handles unprintable characters.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		protected override string ToString(char ch)
		{
			if (!Char.IsControl(ch)) return ch.ToString();
			return "#"+(int)ch;
		}

		public override string ToString(string format)
		{
			return base.ToString(format,",");
		}

		/// <summary>
		/// Distribution representing an unknown digit in 0..9
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar Digit() { return UniformInRange('0', '9'); }

		/// <summary>
		/// Distribution representing an unknown lowercase letter in a..z
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar Lower() { return UniformInRange('a', 'z'); }

		/// <summary>
		/// Distribution representing an unknown uppercase letter in A..Z
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar Upper() { return UniformInRange('A', 'Z'); }

		/// <summary>
		/// Distribution representing an unknown letter in a..z, A..Z
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar Letter() {	return UniformInRanges("azAZ"); }

		/// <summary>
		/// Distribution representing an unknown letter or digit
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar LetterOrDigit() { return UniformInRanges("azAZ09"); }

		/// <summary>
		/// Distribution representing an unknown word character.
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar WordChar()  { return UniformInRanges("azAZ09__''"); }

		/// <summary>
		/// Distribution representing an unknown whitespace character
		/// </summary>
		/// <returns></returns>
		public static DiscreteChar Whitespace() { return UniformInRanges("\t\r  "); }

		/// <summary>
		/// Returns a distribution which is uniform over all characters
		/// that have zero probability in this distribution
		/// i.e. that are not 'in' this distribution.
		/// </summary>
		/// <remarks>
		/// This is useful for defining characters that are not in a particular distribution
		/// e.g. not a letter or not a word character.
		/// </remarks>
		/// <param name="d"></param>
		/// <returns></returns>
		public DiscreteChar Complement()
		{
			// This creates a vector whose common value is not zero,
			// but where the pieces are.  This is useful when
			// displaying the distribution (to show that it is a 'complement')
			// but may have unforeseen side effects e.g. on performance.
			// todo: consider revisiting this design.
			PiecewiseVector res;
			if (this.IsPointMass)
			{
				res = PiecewiseVector.Constant(Dimension, 1.0);
				res[this.Point]=0;
			}
			else
			{
				res = PiecewiseVector.Zero(Dimension);
				res.SetToFunction(disc.GetWorkspace(), x => x==0.0 ? 1.0 : 0.0);
			}
			var comp = DiscreteChar.FromVector(res);
			//if (Name!=null) comp.Name="^"+Name;
			return comp;
		}

		/*
		/// <summary>
		/// The name of the distribution, if any.
		/// </summary>
		public string Name { get; set; }
		
		public override string ToString(string format, string delimiter)
		{
			if (Name!=null) return Name;
			return base.ToString(format, delimiter);
		}*/
	}

}
