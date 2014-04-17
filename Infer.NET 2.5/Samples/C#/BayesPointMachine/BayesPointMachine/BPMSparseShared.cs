/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.       *
*                                                       *
********************************************************/

namespace BayesPointMachine
{
	using System;
	using System.Collections.Generic;

	using MicrosoftResearch.Infer;
	using MicrosoftResearch.Infer.Distributions;
	using MicrosoftResearch.Infer.Models;
	using MicrosoftResearch.Infer.Maths;
	using MicrosoftResearch.Infer.Utils;

	using GaussianArray2D = MicrosoftResearch.Infer.Distributions.DistributionRefArray<MicrosoftResearch.Infer.Distributions.DistributionStructArray<MicrosoftResearch.Infer.Distributions.Gaussian, double>, double[]>;

	/// <summary>
	/// An example of a Bayes Point Machine (BPM) using sparse features and shared weights.
	/// </summary>
	public class BPMSparseShared
	{
		// Shared weights and their priors.
		private GaussianArray2D weightsPrior;
		private ISharedVariableArray<VariableArray<double>, double[][]> weights;

		// Ranges over classes and features.
		private Range c;
		private Range f;

		// Training and prediction models.
		private Model trainModel;
		private Model testModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="BPMSparseShared"/> class.
		/// </summary>
		/// <param name="numClasses">The number of classes.</param>
		/// <param name="noisePrecision">The precision of the noise.</param>
		/// <param name="numFeatures">The number of features.</param>
		/// <param name="numChunksTraining">The number of training set chunks.</param>
		/// <param name="numChunksTesting">The number of test set chunks.</param>
		public BPMSparseShared(int numClasses, double noisePrecision, int numFeatures, int numChunksTraining, int numChunksTesting)
		{
			// Ranges over classes and features.
			this.c = new Range(numClasses).Named("c");
			this.f = new Range(numFeatures).Named("f");

			// Setup weights and weights' prior.
			this.weightsPrior = InitializePrior(numClasses, numFeatures);
			this.weights = SharedVariable<double>.Random(Variable.Array<double>(this.f), this.c, this.weightsPrior).Named("w"); 

			// Configure models.
			this.trainModel = new Model(this.weights, this.c, numChunksTraining);
			this.testModel = new Model(this.weights, this.c, numChunksTesting);

			// Observe the noise precision.
			this.trainModel.noisePrecision.ObservedValue = noisePrecision;
			this.testModel.noisePrecision.ObservedValue = noisePrecision;
		}

		/// <summary>
		/// Trains the BPM on a given chunk of the training data, using a sparse 
		/// feature representation.
		/// </summary>
		/// <param name="indices">The indices of the features</param>
		/// <param name="values">The values of the features</param>
		/// <param name="labels">The corresponding labels</param>
		/// <param name="chunkNumber">The number of the chunk</param>
		/// <returns>A posterior distribution over weights</returns>
		/// <returns></returns>
		public Gaussian[][] Train(int[][] indices, double[][] values, int[] labels, int chunkNumber)
		{
			// Observe features and labels.
			this.trainModel.numItems.ObservedValue = values.Length;
			this.trainModel.numFeaturesPerItem.ObservedValue = Util.ArrayInit(values.Length, i => values[i].Length);
			this.trainModel.values.ObservedValue = values;
			this.trainModel.indices.ObservedValue = indices;
			this.trainModel.y.ObservedValue = labels;

			// Infer the weights.
			trainModel.model.InferShared(trainModel.engine, chunkNumber);
			Gaussian[][] posteriorWeights = Distribution.ToArray<Gaussian[][]>(this.weights.Marginal<GaussianArray2D>());

			return posteriorWeights;
		}

		/// <summary>
		/// Predicts the labels for a given chunk of sparse features.
		/// </summary>
		/// <param name="indices">The indices of the features</param>
		/// <param name="values">The values of the features</param>
		/// <param name="chunkNumber">The number of the chunk</param>
		/// <returns>A posterior distribution over corresponding labels</returns>
		/// <param name="chunkNumber"></param>
		/// <returns></returns>
		public Discrete[] Test(int[][] indices, double[][] values, int chunkNumber)
		{
			// Predict one item after the other.
			Discrete[] predictions = new Discrete[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				// Observe a single sparse feature vector.
				this.testModel.numItems.ObservedValue = 1;
				this.testModel.values.ObservedValue = new double[][] { values[i] };
				this.testModel.numFeaturesPerItem.ObservedValue = new int[] { values[i].Length };
				this.testModel.indices.ObservedValue = new int[][] { indices[i] };

				// Infer labels.
				this.testModel.model.InferShared(this.testModel.engine, chunkNumber);
				predictions[i] = Distribution.ToArray<Discrete[]>(this.testModel.engine.Infer(this.testModel.y))[0];
			}
			return predictions;
		}

		private GaussianArray2D InitializePrior(int numClasses, int numFeatures)
		{
			return (GaussianArray2D)Distribution<double>.Array(Util.ArrayInit(numClasses, c => (c == 0) ?
					Util.ArrayInit(numFeatures, feat => Gaussian.PointMass(0)) :
					Util.ArrayInit(numFeatures, feat => Gaussian.FromMeanAndPrecision(0.0, 1.0))));
		}

		#region Shared sparse BPM model

		/// <summary>
		/// An Infer.NET model of a BPM using sparse features and shared weights.
		/// </summary>
		private class Model
		{
			public Variable<int> numItems;
			public Range i;

			public VariableArray<int> numFeaturesPerItem;
			public Range fItem;

			public VariableArray<VariableArray<double>, double[][]> values;
			public VariableArray<VariableArray<int>, int[][]> indices;
			public Variable<double> noisePrecision;
			public VariableArray<double> score;
			public VariableArray<int> y;

			public MicrosoftResearch.Infer.Models.Model model;
			public VariableArray<VariableArray<double>, double[][]> wModel;

			public InferenceEngine engine = new InferenceEngine();

			public Model(ISharedVariableArray<VariableArray<double>, double[][]> w, Range c, int numChunks)
			{
				// Items.
				numItems = Variable.New<int>().Named("numItems");
				i = new Range(numItems).Named("i");
				i.AddAttribute(new Sequential());

				// Features per item.
				numFeaturesPerItem = Variable.Array<int>(i).Named("numFeaturesPerItem");
				fItem = new Range(numFeaturesPerItem[i]).Named("fItem");

				// The model identifier for the shared variables.
				model = new MicrosoftResearch.Infer.Models.Model(numChunks).Named("model");
				// The weight vector for each submodel.
				wModel = w.GetCopyFor(model).Named("wModel");

				noisePrecision = Variable.New<double>().Named("noisePrecision");

				// Jagged array of feature values - each item is an array of data values 
				// whose indices are given by the corresponding indices[i].
				values = Variable.Array(Variable.Array<double>(fItem), i).Named("values");

				// Jagged array of indices for the items.
				indices = Variable.Array(Variable.Array<int>(fItem), i).Named("indices");

				// Labels.
				y = Variable.Array<int>(i).Named("y");

				// For all items...
				using (Variable.ForEach(i))
				{
					// ...compute the score of this item across all classes...
					score = BPMUtils.ComputeClassScores(wModel, values[i], indices[i], fItem, noisePrecision);
					y[i] = Variable.DiscreteUniform(c);

					// ... and constrain the output.
					BPMUtils.ConstrainMaximum(y[i], score);
				}

				// Inference engine settings (EP).
				engine.Compiler.UseSerialSchedules = true;
				engine.ShowProgress = false;
			}
		}

		#endregion
	}
}
