using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using MicrosoftResearch.Infer;

namespace ImageFeatures
{
	class Program
	{
		static void Main(string[] args)
		{
			string folder = @"..\..\..\Images\";
			List<string> filenames = ReadLines(folder + "Images.txt");
			Dictionary<string,Vector> labels = ReadLabels(folder + "Labels.txt");
			Dictionary<string,Vector> features = new Dictionary<string,Vector>();
			foreach (string filename in filenames) {
				Console.WriteLine(filename);
				Bitmap bitmap = new Bitmap(folder + filename);
				Vector f;
				if (false) {
					//int[] distances = new int[] { 1, 3, 5, 7 };
					int[] distances = new int[] { 1 };
					int nBinsPerChannel = 4;
					int[] histogram;
					int[,] count = AutoCorrelogram(Quantize(bitmap, nBinsPerChannel),
						nBinsPerChannel * nBinsPerChannel * nBinsPerChannel, distances, out histogram);
					f = NormalizeAutoCorrelogram(distances, count, histogram);
				} else {
					f = AverageColor(bitmap);
				}
				f = Concatenate(f,labels[filename]);
				f = f.Append(1.0);
				features[filename] = f;
			}
			StreamWriter writer = new StreamWriter(folder + "Features.txt");
			foreach (string filename in filenames) {
				writer.WriteLine(filename+","+Microsoft.Dynamic.StringUtil.CollectionToString(features[filename],","));
			}
			writer.Close();
		}

		public static Dictionary<string, Vector> ReadLabels(string path)
		{
			Dictionary<string, int> keywords = new Dictionary<string, int>();
			List<string[]> lines = new List<string[]>();
			StreamReader reader = new StreamReader(path);
			while (true) {
				string s = reader.ReadLine();
				if (s == null) break;
				string[] items = s.Split(',');
				if (items.Length == 0) continue;
				for (int i = 1; i < items.Length; i++) {
					items[i] = items[i].Trim();
					if (!keywords.ContainsKey(items[i])) keywords[items[i]] = keywords.Count;				
				}
				lines.Add(items);
			}
			reader.Close();
			Dictionary<string, Vector> labels = new Dictionary<string, Vector>();
			foreach(string[] items in lines) {
				Vector v = new Vector(keywords.Count);
				for (int i = 1; i < items.Length; i++) {
					v[keywords[items[i]]] = 1.0;
				}
				labels[items[0]] = v;
			}
			return labels;
		}

		public static Vector Concatenate(Vector a, Vector b)
		{
			Vector result = new Vector(a.Count + b.Count);
			int count = 0;
			for (int i = 0; i < a.Count; i++) {
				result[count++] = a[i];
			}
			for (int i = 0; i < b.Count; i++) {
				result[count++] = b[i];
			}
			return result;
		}

		public static Vector AverageColor(Bitmap bitmap)
		{
			Vector sum = new Vector(2);
			for (int x = 0; x < bitmap.Width; x++) {
				for (int y = 0; y < bitmap.Height; y++) {
					Color pixel = bitmap.GetPixel(x, y);
					// we encode the Hue angle as a 2D vector, scaled by saturation and brightness.
					double hue = pixel.GetHue()/360;
					double saturation = pixel.GetSaturation();
					double brightness = pixel.GetBrightness();
					double vx = brightness*saturation*Math.Cos(hue*2*Math.PI);
					double vy = brightness*saturation*Math.Sin(hue*2*Math.PI);
					sum[0] += vx;
					sum[1] += vy;
					//sum[1] += pixel.GetSaturation();
					//sum[2] += pixel.GetBrightness();
				}
			}
			sum.SetToProduct(sum, 1.0 / ((double)bitmap.Width * bitmap.Height));
			return sum;
		}

		public static int[,] Quantize(Bitmap bitmap, int nBinsPerChannel)
		{
			int[,] result = new int[bitmap.Width, bitmap.Height];
			for (int x = 0; x < bitmap.Width; x++) {
				for (int y = 0; y < bitmap.Height; y++) {
					Color pixel = bitmap.GetPixel(x, y);
					int brightness = (int)(nBinsPerChannel * pixel.GetBrightness());
					if (brightness == nBinsPerChannel) brightness = nBinsPerChannel - 1;
					int saturation = (int)(nBinsPerChannel * pixel.GetSaturation());
					if (saturation == nBinsPerChannel) saturation = nBinsPerChannel - 1;
					int hue = (int)(nBinsPerChannel * pixel.GetHue() / 360.0);
					if (hue == nBinsPerChannel) hue = nBinsPerChannel - 1;
					int bin = brightness + nBinsPerChannel * (saturation + nBinsPerChannel * hue);
					result[x, y] = bin;
				}
			}
			return result;
		}

		/// <summary>
		/// Compute the AutoCorrelogram of an image.
		/// </summary>
		/// <param name="image">Quantized 2D image, each pixel ranging from 0 to nColors-1.</param>
		/// <param name="nColors">The number of distinct colors in image.</param>
		/// <param name="distances">The set of distances to test.</param>
		/// <param name="histogram">Histogram of colors used in the correlogram.</param>
		/// <returns>An array of counts, nColors by distances.Length. result[color,d] is the number of times two pixels of the same color have distance distances[d] in the image.</returns>
		/// <remarks>
		/// Distance is measured via max(xDelta,yDelta).  That is, pixels (1,2) and (3,6) have distance 4.
		/// Pixels close to the boundary are ignored, so the returned histogram is not the same as the overall image histogram.
		/// </remarks>
		public static int[,] AutoCorrelogram(int[,] image, int nColors, int[] distances, out int[] histogram)
		{
			int[,] count = new int[nColors, distances.Length];
			int maxDistance = Maximum(distances);
			Size windowSize = new Size(2 * maxDistance + 1, 2 * maxDistance + 1);
			Size windowIncrement = new Size(1, 1);
			histogram = new int[nColors];
			Size croppedSize = new Size(image.GetLength(0) - windowSize.Width, image.GetLength(1) - windowSize.Height);
			for (int x = 0; x < croppedSize.Width; x += windowIncrement.Width) {
				for (int y = 0; y < croppedSize.Height; y += windowIncrement.Height) {
					int color = image[x, y];
					histogram[color]++;
					// active window is (x,y)(x+windowSize.Width,y+windowSize.Height) which is guaranteed to fit.
					Point center = new Point(x + (windowSize.Width - 1) / 2, y + (windowSize.Height - 1) / 2);
					for (int d = 0; d < distances.Length; d++) {
						// The set of pixels at distance distances[d] forms a square.
						// We loop over the sides of this square and accumulate the number of matches.
						int left = center.X - distances[d];
						int right = center.X + distances[d];
						int top = center.Y - distances[d];
						int bottom = center.Y + distances[d];
						// loop over top and bottom sides of window
						for (int x2 = left; x2 <= right; x2++) {
							count[color, d] += (image[x2, top] == color) ? 1 : 0;
							count[color, d] += (image[x2, bottom] == color) ? 1 : 0;
						}
						// loop over left and right sides of window, 
						// excluding top and bottom sides which were already counted above.
						for (int y2 = top + 1; y2 < bottom; y2++) {
							count[color, d] += (image[left, y2] == color) ? 1 : 0;
							count[color, d] += (image[right, y2] == color) ? 1 : 0;
						}
					}
				}
			}
			return count;
		}

		public static Vector NormalizeAutoCorrelogram(int[] distances, int[,] count, int[] histogram)
		{
			Vector result = new Vector(count.Length);
			int i = 0;
			for (int color = 0; color < count.GetLength(0); color++) {
				for (int d = 0; d < count.GetLength(1); d++) {
					if (histogram[color] > 0) {
						result[i] = count[color, d] / ((double)histogram[color] * 8 * distances[d]);
					} else {
						result[i] = count[color, d];
					}
					i++;
				}
			}
			return result;
		}

		public static int Maximum(int[] array)
		{
			int max = Int32.MinValue;
			for (int i = 0; i < array.Length; i++) {
				if (array[i] > max) max = array[i];
			}
			return max;
		}

		public static List<string> ReadLines(string path)
		{
			List<string> result = new List<string>();
			StreamReader reader = new StreamReader(path);
			while (true) {
				string s = reader.ReadLine();
				if (s == null) break;
				result.Add(s);
			}
			reader.Close();
			return result;
		}
	}
}
