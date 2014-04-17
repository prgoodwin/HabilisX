/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.       *
*                                                       *
********************************************************/

namespace BayesPointMachine
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using MicrosoftResearch.Infer.Maths;
	using MicrosoftResearch.Infer.Utils;

	/// <summary>
	/// Reads a file with line-by-line data in the format <classID, feature 1 value, feature 2 value, .... feature N value>.
	/// Dense data can be made sparse, assuming that each feature value which equals 'valueToIgnore' (typically 0) is to be ignored.
	/// </summary>
	public class DataUtils
	{
		public static double valueToIgnore = 0;

		public static Vector[] Read(string filename, bool labelAtEnd, bool addBias, out int[] labels)
		{
			int locationToStart = 0;
			return Read(filename, Int32.MaxValue, labelAtEnd, addBias, out labels, ref locationToStart);
		}

		public static double[][] Read(string filename, bool labelAtEnd, bool addBias, out int[] labels, out int[][] indices)
		{
			int locationToStart = 0;
			return Read(filename, Int32.MaxValue, labelAtEnd, addBias, out labels, out indices, ref locationToStart);
		}

		public static double[][] Read(string filename, int maxItemsInBatch, bool labelAtEnd, bool addBias, out int[] labels, out int[][] indices, ref int locationToStart)
		{
			Vector[] featureVectors = Read(filename, maxItemsInBatch, labelAtEnd, addBias, out labels, ref locationToStart);
			return Sparsify(featureVectors, out indices);
		}

		public static Vector[] Read(string filename, int maxItemsInBatch, bool labelAtEnd, bool addBias, out int[] labels, ref int locationToStart)
		{
			List<Vector> x = new List<Vector>();
			List<int> y = new List<int>();
			Vector[] featureVectors;
			string line;
			double[] values;
			int index;
			int labelIndex;

			// Find start.
			int currentLocation = 0;
			using (StreamReader reader = new StreamReader(filename))
			{
				while (!reader.EndOfStream && currentLocation < locationToStart)
				{
					line = reader.ReadLine();
					currentLocation++;
				}

				// Now read data.
				int readItems = 0;
				while (!reader.EndOfStream && readItems < maxItemsInBatch)
				{
					line = reader.ReadLine();
					string[] pieces = line.Split('\t', ' ', ',');

					int n = pieces.Length - 1;
					if (addBias)
					{
						values = new double[n+1];
						values[n] = 1;
					}
					else
					{
						values = new double[n];
					}

					// Read feature values and labels.
					labelIndex = labelAtEnd ? n : 0;
					for (int i = 0; i < n; i++)
					{
						index = labelAtEnd ? i : i+1;
						values[i] = Double.Parse(pieces[index]);
					}
					x.Add(Vector.FromArray(values));
					y.Add(Int16.Parse(pieces[labelIndex]));

					readItems = readItems + 1;
					currentLocation = currentLocation + 1;
				}
				locationToStart = currentLocation;
			}

			labels = y.ToArray();
			featureVectors = x.ToArray();

			return featureVectors;
		}

		private static double[][] Sparsify(Vector[] featureVectors, out int[][] indices)
		{
			int n = featureVectors.Length;
			double[][] values = new double[n][];
			indices = new int[n][];

			for (int i = 0; i < n; i++)
			{
				List<double> featureValues = new List<double>();
				List<int> featureIndices = new List<int>();
				for (int f = 0; f < featureVectors[i].Count; f++)
				{
					if (featureVectors[i][f] != valueToIgnore)
					{
						featureValues.Add(featureVectors[i][f]);
						featureIndices.Add(f);
					}
				}
				values[i] = featureValues.ToArray();
				indices[i] = featureIndices.ToArray();
			}
			return values;
		}
	}
}