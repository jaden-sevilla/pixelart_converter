using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace pixelart_converter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			QuantizerToBox();
			DitherToBox();

		}

		private void OpenFile(object sender, RoutedEventArgs e)
		{
			OpenFileDialog file = new OpenFileDialog();
			bool? opened = file.ShowDialog();
			if (opened == true)
			{
				string path = file.FileName;
				BitmapImage originalBitmap = new BitmapImage(new Uri(path));
				originalImage.Source= originalBitmap;
			}
		}

		private void PixelUp(object sender, RoutedEventArgs e)
		{
			PixelAdd(true);
		}
		private void PixelDown(object sender, RoutedEventArgs e)
		{
			PixelAdd(false);
		}
		private void PixelAdd(bool up)
		{
			int num = int.Parse(pixelNum.Text);
			if (up == true)
			{
				num++;
			}
			else
			{
				if (num > 1)
				{
					num--;
				}
			}
			pixelNum.Text = num.ToString();

		}

		private void ConvertToPixelArt()
		{
			Bitmap originalBitmap = BitmapImageToBitmap((BitmapImage)originalImage.Source);
			int pixelSize = int.Parse(pixelNum.Text);
			Bitmap pixelBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);

			using (Graphics g = Graphics.FromImage(originalBitmap))
			{
				for (int x = 0; x < originalBitmap.Width; x += pixelSize)
				{
					for (int y = 0; y < originalBitmap.Height; y += pixelSize)
					{
						List<System.Drawing.Color> pixelColors = new List<System.Drawing.Color>();
						for (int x2 = 0; x2 < pixelSize; x2++)
						{
							for (int y2 = 0; y2 < pixelSize; y2++)
							{
								int pixelX = x + x2;
								int pixelY = y + y2;

								if (pixelX < originalBitmap.Width && pixelY < originalBitmap.Height)
								{
									pixelColors.Add(originalBitmap.GetPixel(pixelX, pixelY));
								}
							}
						}
						System.Drawing.Color averageColor = FindAverageColor(pixelColors);
						for (int x2 = 0; x2 < pixelSize; x2++)
						{
							for (int y2 = 0; y2 < pixelSize; y2++)
							{
								int pixelX = x + x2;
								int pixelY = y + y2;

								if (pixelX < originalBitmap.Width && pixelY < originalBitmap.Height)
								{
									if (averageColor.A == 0)
									{
										pixelBitmap.SetPixel(pixelX, pixelY, System.Drawing.Color.Transparent);
									}
									else
									{
										pixelBitmap.SetPixel(pixelX, pixelY, averageColor);
									}
								}
							}
						}
					}

					IQuantizer quantizer = SelectQuantizer();
					IDitherer dither = SelectDither();
					if (dither == null)
					{
						pixelBitmap.Quantize(quantizer);
					}
					else
					{
						pixelBitmap.Dither(quantizer, dither);
					}

					pixelImage.Source = BitmapToBitmapSource(pixelBitmap);
				}
			}
		}

		private IQuantizer SelectQuantizer()
		{
			IQuantizer quantizer;
			if (quantizerBox.Text == "RGB 888")
			{
				quantizer = PredefinedColorsQuantizer.Rgb888();
			}
			else if (quantizerBox.Text == "RGB 332")
			{
				quantizer = PredefinedColorsQuantizer.SystemDefault8BppPalette();
			}
			else if (quantizerBox.Text == "Grayscale")
			{
				quantizer = PredefinedColorsQuantizer.Grayscale16();
			}
			else
			{
				quantizer = PredefinedColorsQuantizer.BlackAndWhite();
			}
			return quantizer;
		}
		private IDitherer SelectDither()
		{
			IDitherer dither;
			if (ditherBox.Text == "None")
			{
				return null;
			}
			else if (ditherBox.Text == "Bayer 2x2")
			{
				dither = OrderedDitherer.Bayer2x2;
			}
			else if (ditherBox.Text == "Bayer 8x8")
			{
				dither = OrderedDitherer.Bayer8x8;
			}
			else if (ditherBox.Text == "Dotted Halftone")
			{
				dither = OrderedDitherer.DottedHalftone;
			}
			else if (ditherBox.Text == "Blue Noise")
			{
				dither = OrderedDitherer.BlueNoise;
			}
			else
			{
				dither = ErrorDiffusionDitherer.FloydSteinberg;
			}
			return dither;
		}
		private void QuantizerToBox()
		{
			quantizerBox.Items.Add("RGB 888");
			quantizerBox.Items.Add("RGB 332");
			quantizerBox.Items.Add("Grayscale");
			quantizerBox.Items.Add("Black & White");

			quantizerBox.SelectedIndex = 0;
		}
		private void DitherToBox()
		{
			ditherBox.Items.Add("None");
			ditherBox.Items.Add("Bayer 2x2");
			ditherBox.Items.Add("Bayer 8x8");
			ditherBox.Items.Add("Dotted Halftone");
			ditherBox.Items.Add("Blue Noise");
			ditherBox.Items.Add("Floyd Steinberg");

			ditherBox.SelectedIndex = 0;
		}
		private System.Drawing.Color FindAverageColor(List<System.Drawing.Color> pixels)
		{
			int totalR = 0;
			int totalG = 0;
			int totalB = 0;

			foreach (System.Drawing.Color pixel in pixels)
			{
				totalR += pixel.R;
				totalG += pixel.G;
				totalB += pixel.B;
			}

			int count = pixels.Count;

			int avgR = (int)(totalR / count);
			int avgG = (int)(totalG / count);
			int avgB = (int)(totalB / count);


			return System.Drawing.Color.FromArgb(avgR, avgG, avgB);
		}

		private Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BitmapEncoder encoder = new BmpBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
				encoder.Save(ms);
				return new Bitmap(ms);
			}
		}
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		private BitmapSource BitmapToBitmapSource(Bitmap bitmap)
		{
			IntPtr hBitmap = bitmap.GetHbitmap();
			BitmapSource bitmapSource;

			try
			{
				bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				DeleteObject(hBitmap);
			}

			return bitmapSource;
		}

		private void convertButton_Click(object sender, RoutedEventArgs e)
		{
			if (originalImage.Source !=  null)
			{
				ConvertToPixelArt();
			}
			else
			{
				MessageBox.Show("Please select an image");
			}
		}
		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if(pixelImage.Source != null)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
				saveFileDialog.Title = "Save Image";
				saveFileDialog.FileName = "pixelatedImage.png";

				if (saveFileDialog.ShowDialog() == true)
				{
					string filePath = saveFileDialog.FileName;

					JpegBitmapEncoder encoder = new JpegBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create((InteropBitmap)pixelImage.Source));

					using (FileStream fs = new FileStream(filePath, FileMode.Create))
					{
						encoder.Save(fs);
						MessageBox.Show("Image saved");
					}
				}
			}
			else
			{
				MessageBox.Show("Please convert an image");
			}
		}
	}
}