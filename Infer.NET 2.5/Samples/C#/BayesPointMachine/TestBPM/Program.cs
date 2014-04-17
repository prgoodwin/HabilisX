/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.       *
*                                                       *
********************************************************/

//#define ShowWeights	// Uncomment to plot learnt weights to console.

namespace BayesPointMachine
{
	using System;
	using System.Collections.Generic;

	using MicrosoftResearch.Infer;
	using MicrosoftResearch.Infer.Distributions;
	using MicrosoftResearch.Infer.Maths;
	using MicrosoftResearch.Infer.Utils;

	/// <summary>
	/// A program which runs several Bayes Point Machines on an artificial data set.
	/// </summary>
	class Program
	{
		/// <summary>
		/// The entry point for the program.
		/// </summary>
		/// <param name="args">The array of command-line arguments.</param>
		static void Main(string[] args)
		{
			// Set up example data.
			int numClasses = 3;
			int numItems = 30;
			int numFeatures = 4;
			int maxItemsPerChunk = 10;
			double noisePrecision = 0.1;
			int numChunks = ((numItems - 1) / maxItemsPerChunk) + 1;

			// Training set.
			string trainingSetFile = @"..\..\data\data.txt";

			// If true, a line in the data file ends with a label. Otherwise the
			// label is expected at the beginning of each line.
			bool labelAtEnd = false;

			// If true, a constant feature will be added to each feature vector.
			//
			// N.B. If the data does not contain such a constant feature, it is
			//      important to set addBias to true and add it so that the BPMs 
			//      will learn a bias weight for each class. Note also that you
			//      might need to change the test set below and numFeatures above.
			bool addConstantFeature = false;

			// Test sets (dense and sparse).
			Vector[] testSet = new Vector[2];
			testSet[0] = Vector.FromArray(new double[] { 2.1, 0, 0, 0 });
			testSet[1] = Vector.FromArray(new double[] { 0, 0, 1.3, 0 });

			int[][] testSetIndices = new int[2][];
			testSetIndices[0] = new int[] { 0 };
			testSetIndices[1] = new int[] { 2 };
			double[][] testSetValues = new double[2][];
			testSetValues[0] = new double[] { 2.1 };
			testSetValues[1] = new double[] { 1.3 };

			// Run distinct BPMs.
			RunBPM(numClasses, noisePrecision, trainingSetFile, labelAtEnd, addConstantFeature, testSet);
			RunIncrementalBPM(numClasses, noisePrecision, trainingSetFile, labelAtEnd, addConstantFeature, testSet, maxItemsPerChunk, numChunks);
			RunSharedBPM(numClasses, noisePrecision, numFeatures, trainingSetFile, labelAtEnd, addConstantFeature, testSet, maxItemsPerChunk, numChunks);
			RunSparseBPM(numClasses, noisePrecision, numFeatures, trainingSetFile, labelAtEnd, addConstantFeature, testSetIndices, testSetValues);
			RunSharedSparseBPM(numClasses, noisePrecision, numFeatures, trainingSetFile, labelAtEnd, addConstantFeature, testSetIndices, testSetValues, maxItemsPerChunk, numChunks);

			Console.WriteLine("Press key to quit");
			Console.ReadKey();
		}

		#region BPM tests

		/// <summary>
		/// Runs the BPM using dense features.
		/// </summary>
		/// <param name="numClasses">The number of classes</param>
		/// <param name="noisePrecision">The noise precision</param>
		/// <param name="trainingSetFile">The file containing the training set</param>
		/// <param name="testSet">The test set</param>
		private static void RunBPM(int numClasses, double noisePrecision, string trainingSetFile, bool labelAtEnd, bool addBias, Vector[] testSet)
		{
			Console.WriteLine("------- BPM -------");
			int[] labels;
			Vector[] featureVectors = DataUtils.Read(trainingSetFile, labelAtEnd, addBias, out labels);
			BPM bpm = new BPM(numClasses, noisePrecision);
			VectorGaussian[] posteriorWeights = bpm.Train(featureVectors, labels);

#if ShowWeights
			Console.WriteLine("Weights=" + StringUtil.ArrayToString(posteriorWeights));
#endif

			Console.WriteLine("\nPredictions:");
			Discrete[] predictions = bpm.Test(testSet);
			foreach (Discrete prediction in predictions)
				Console.WriteLine(prediction);
			Console.WriteLine();
		}

		/// <summary>
		/// Runs the BPM using dense features and incremental training.
		/// </summary>
		/// <param name="numClasses">The number of classes</param>
		/// <param name="noisePrecision">The noise precision</param>
		/// <param name="trainingSetFile">The file containing the training set</param>
		/// <param name="testSet">The test set</param>
		/// <param name="maxItemsPerChunk">The maximum number of items per chunk</param>
		/// <param name="numChunks">The total number of chunks</param>
		private static void RunIncrementalBPM(int numClasses, double noisePrecision, string trainingSetFile, bool labelAtEnd, bool addBias, Vector[] testSet, int maxItemsPerChunk, int numChunks)
		{
			Console.WriteLine("\n------- Incremental BPM -------");
			BPM bpm = new BPM(numClasses, noisePrecision);

			int[] labels;
			Vector[] featureVectors;
			VectorGaussian[] posteriorWeights = new VectorGaussian[numClasses];

			int locationToStart = 0;
			for (int c = 0; c < numChunks; c++)
			{
				featureVectors = DataUtils.Read(trainingSetFile, maxItemsPerChunk, labelAtEnd, addBias, out labels, ref locationToStart);
				posteriorWeights = bpm.Train(featureVectors, labels);
			}

#if ShowWeights
			Console.WriteLine("Weights=" + StringUtil.ArrayToString(posteriorWeights));
#endif

			Console.WriteLine("\nPredictions:");
			Discrete[] predictions = bpm.Test(testSet);
			foreach (Discrete pred in predictions)
				Console.WriteLine(pred);
			Console.WriteLine();
		}

		/// <summary>
		/// Runs the BPM using sparse features.
		/// </summary>
		/// <param name="numClasses">The number of classes</param>
		/// <param name="noisePrecision">The noise precision</param>
		/// <param name="numFeatures">The number of features</param>
		/// <param name="trainingSetFile">The file containing the training set</param>
		/// <param name="testSetIndices">The test set feature indices</param>
		/// <param name="testSetValues">The test set feature values</param>
		private static void RunSparseBPM(int numClasses, double noisePrecision, int numFeatures, string trainingSetFile, bool labelAtEnd, bool addBias, int[][] testSetIndices, double[][] testSetValues)
		{
			Console.WriteLine("\n------- Sparse BPM -------");
			int[] labels;
			int[][] indices;
			double[][] values = DataUtils.Read(trainingSetFile, labelAtEnd, addBias, out labels, out indices);
			BPMSparse bpmSparse = new BPMSparse(numClasses, noisePrecision, numFeatures);

			Gaussian[][] posteriorWeights = bpmSparse.Train(indices, values, labels);

#if ShowWeights
			Console.WriteLine("Weights=" + StringUtil.ArrayToString(posteriorWeights));
#endif

			Console.WriteLine("\nPredictions:");
			Discrete[] predictions = bpmSparse.Test(testSetIndices, testSetValues);
			foreach (Discrete prediction in predictions)
				Console.WriteLine(prediction);
			Console.WriteLine();
		}

		/// <summary>
		/// Runs the BPM using dense features and batched training.
		/// </summary>
		/// <param name="numClasses">The number of classes</param>
		/// <param name="noisePrecision">The noise precision</param>
		/// <param name="numFeatures">The number of features</param>
		/// <param name="trainingSetFile">The file containing the training set</param>
		/// <param name="testSet">The test set</param>
		/// <param name="maxItemsPerChunk">The maximum number of items per chunk</param>
		/// <param name="numChunks">The total number of chunks</param>
		private static void RunSharedBPM(int numClasses, double noisePrecision, int numFeatures, string trainingSetFile, bool labelAtEnd, bool addBias, Vector[] testSet, int maxItemsPerChunk, int numChunks)
		{
			Console.WriteLine("\n------- Shared BPM -------");
			BPMShared bpm = new BPMShared(numClasses, noisePrecision, numFeatures, numChunks, 1);

			int[] labels;
			Vector[] featureVectors;
			VectorGaussian[] posteriorWeights = new VectorGaussian[numClasses];

			// Several passes to achieve convergence.
			for (int pass = 0; pass < 15; pass++)
			{
				int locationToStart = 0;
				for (int c = 0; c < numChunks; c++)
				{
					featureVectors = DataUtils.Read(trainingSetFile, maxItemsPerChunk, labelAtEnd, addBias, out labels, ref locationToStart);
					posteriorWeights = bpm.Train(featureVectors, labels, c);
				}
			}

#if ShowWeights
			Console.WriteLine("Weights=" + StringUtil.ArrayToString(posteriorWeights));
#endif

			Console.WriteLine("\nPredictions:");
			Discrete[] predictions = bpm.Test(testSet, 0);
			foreach (Discrete prediction in predictions)
				Console.WriteLine(prediction);
			Console.WriteLine();
		}

		/// <summary>
		/// Runs the BPM using sparse features and batched training.
		/// </summary>
		/// <param name="numClasses">The number of classes</param>
		/// <param name="noisePrecision">The noise precision</param>
		/// <param name="numFeatures">The number of features</param>
		/// <param name="trainingSetFile">The file containing the training set</param>
		/// <param name="testSetIndices">The test set feature indices</param>
		/// <param name="testSetValues">The test set feature values</param>
		/// <param name="maxItemsPerChunk">The maximum number of items per chunk</param>
		/// <param name="numChunks">The total number of chunks</param>
		private static void RunSharedSparseBPM(int numClasses, double noisePrecision, int numFeatures, string trainingSetFile, bool labelAtEnd, bool addBias, int[][] testSetIndices, double[][] testSetValues, int maxItemsPerChunk, int numChunks)
		{
			Console.WriteLine("\n------- Shared Sparse BPM -------");
			BPMSparseShared bpm = new BPMSparseShared(numClasses, noisePrecision, numFeatures, numChunks, 1);

			int[] labels;
			int[][] indices;
			double[][] values;
			Gaussian[][] posteriorWeights = new Gaussian[numClasses][];

			// Several passes to achieve convergence.
			for (int pass = 0; pass < 15; pass++)
			{
				int locationToStart = 0;
				for (int c = 0; c < numChunks; c++)
				{
					values = DataUtils.Read(trainingSetFile, maxItemsPerChunk, labelAtEnd, addBias, out labels, out indices, ref locationToStart);
					posteriorWeights = bpm.Train(indices, values, labels, c);
				}
			}

#if ShowWeights
			Console.WriteLine("Weights=" + StringUtil.ArrayToString(posteriorWeights));
#endif

			Console.WriteLine("\nPredictions:");
			Discrete[] predictions = bpm.Test(testSetIndices, testSetValues, 0);
			foreach (Discrete prediction in predictions)
				Console.WriteLine(prediction);
			Console.WriteLine();
		}

		#endregion
	}
}