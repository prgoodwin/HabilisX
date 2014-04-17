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
	using MicrosoftResearch.Infer.Utils;

	/// <summary>
	/// An example of a Bayes Point Machine (BPM) using sparse features.
	/// </summary>
	public class BPMSparse
	{
		// Training and prediction models.
		private Model trainModel;
		private Model testModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="BPMSparse"/> class.
		/// </summary>
		/// <param name="numClasses">The number of classes.</param>
		/// <param name="noisePrecision">The precision of the noise.</param>
		/// <param name="numFeatures">The number of features.</param>
		public BPMSparse(int numClasses, double noisePrecision, int numFeatures)
		{
			this.trainModel = new Model();
			this.testModel = new Model();

			// Observe the number of classes.
			this.trainModel.numClasses.ObservedValue = numClasses;
			this.testModel.numClasses.ObservedValue = numClasses;

			// Observe the noise precision.
			this.trainModel.noisePrecision.ObservedValue = noisePrecision;
			this.testModel.noisePrecision.ObservedValue = noisePrecision;

			// Observe the number of features.
			this.trainModel.numFeatures.ObservedValue = numFeatures;
			this.testModel.numFeatures.ObservedValue = numFeatures;
		}

		/// <summary>
		/// Trains the BPM using a sparse feature representation.
		/// </summary>
		/// <param name="indices">The indices of the features</param>
		/// <param name="values">The values of the features</param>
		/// <param name="labels">The corresponding labels</param>
		/// <returns>A posterior distribution over weights</returns>
		public Gaussian[][] Train(int[][] indices, double[][] values, int[] labels)
		{
			// Observe features and labels.
			this.trainModel.numItems.ObservedValue = values.Length;
			this.trainModel.numFeaturesPerItem.ObservedValue = Util.ArrayInit(values.Length, i => values[i].Length);
			this.trainModel.values.ObservedValue = values;
			this.trainModel.indices.ObservedValue = indices;
			this.trainModel.y.ObservedValue = labels;

			// Initialize weight prior.
			int numClasses = this.trainModel.numClasses.ObservedValue;
			int numFeatures = this.trainModel.numFeatures.ObservedValue;
			this.trainModel.wPrior.ObservedValue = InitializePrior(numClasses, numFeatures);

			// Infer the weights.
			Gaussian[][] posteriorWeights = this.trainModel.engine.Infer<Gaussian[][]>(this.trainModel.w);

			// Store posterior weights in prior.
			this.trainModel.wPrior.ObservedValue = posteriorWeights;

			return posteriorWeights;
		}

		/// <summary>
		/// Predicts the labels for some given sparse features.
		/// </summary>
		/// <param name="indices">The indices of the features</param>
		/// <param name="values">The values of the features</param>
		/// <returns>A posterior distribution over corresponding labels</returns>
		public Discrete[] Test(int[][] indices, double[][] values)
		{
			// Store weight prior from training as weight prior for prediction.
			this.testModel.wPrior.ObservedValue = this.trainModel.wPrior.ObservedValue;

			// Predict one item after the other.
			Discrete[] predictions = new Discrete[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				// Observe a single sparse feature vector.
				this.testModel.numItems.ObservedValue = 1;
				this.testModel.values.ObservedValue = new double[][] { values[i] };
				this.testModel.numFeaturesPerItem.ObservedValue = new int[] { values[i].Length };
				this.testModel.indices.ObservedValue = new int[][] { indices[i] };

				// Infer the posterior probabilities for its label.
				predictions[i] = this.testModel.engine.Infer<IList<Discrete>>(this.testModel.y)[0];
			}
			return predictions;
		}

		private Gaussian[][] InitializePrior(int numClasses, int numFeatures)
		{
			return Util.ArrayInit(numClasses, c => (c == 0) ?
					Util.ArrayInit(numFeatures, f => Gaussian.PointMass(0.0)) :
					Util.ArrayInit(numFeatures, f => Gaussian.FromMeanAndPrecision(0.0, 1.0)));
		}

		#region Sparse BPM model

		/// <summary>
		/// An Infer.NET model of a BPM using sparse features.
		/// </summary>
		private class Model
		{
			public Variable<int> numClasses;
			public Range c;
			public Variable<int> numFeatures;
			public Range f;
			public Variable<int> numItems;
			public Range i;

			public VariableArray<int> numFeaturesPerItem;
			public Range fItem;

			public VariableArray<VariableArray<Gaussian>, Gaussian[][]> wPrior;
			public VariableArray<VariableArray<double>, double[][]> w;
			public VariableArray<VariableArray<double>, double[][]> values;
			public VariableArray<VariableArray<int>, int[][]> indices;
			public Variable<double> noisePrecision;
			public VariableArray<double> score;
			public VariableArray<int> y;

			public InferenceEngine engine = new InferenceEngine();

			public Model()
			{
				// Classes.
				numClasses = Variable.New<int>().Named("numClasses");
				c = new Range(numClasses).Named("c");

				// Features.
				numFeatures = Variable.New<int>().Named("numFeatures");
				f = new Range(numFeatures).Named("f");

				// Items.
				numItems = Variable.New<int>().Named("numItems");
				i = new Range(numItems).Named("i");
				i.AddAttribute(new Sequential());

				// Features per item.
				numFeaturesPerItem = Variable.Array<int>(i).Named("numFeaturesPerItem");
				fItem = new Range(numFeaturesPerItem[i]).Named("fItem");

				// The prior distribution for weight vector for each class. When
				// <see cref="Test"/> is called, this is set to the posterior weight
				// distributions from <see cref="Train"/>.
				wPrior = Variable.Array(Variable.Array<Gaussian>(f), c).Named("wPrior");

				// The weight vector for each class.
				w = Variable.Array(Variable.Array<double>(f), c).Named("w");
				w[c][f] = Variable<double>.Random(wPrior[c][f]);

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
					score = BPMUtils.ComputeClassScores(w, values[i], indices[i], fItem, noisePrecision);
					y[i] = Variable.DiscreteUniform(c);

					// ...and constrain the output.
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