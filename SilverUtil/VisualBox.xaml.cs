using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Silver.Weight.Math;

namespace SilverUtil
{
	public partial class VisualBox : UserControl
	{
		public VisualBox()
		{
			InitializeComponent();
		}

		public VisualBox(ROVector2f size, string imagePath) : this()
		{
			Height = size.Y;
			Width = size.X;
			Canvas theCanvas = FindName("TheCanvas") as Canvas;
			theCanvas.Height = size.Y;
			theCanvas.Width = size.X;
			Image theImage = FindName("TheImage") as Image;
			theImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
			theImage.Width = size.X;
			theImage.Height = size.Y;
		}
	}
}
