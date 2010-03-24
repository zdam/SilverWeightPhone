using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Silver.Weight.Math;

namespace SilverUtil
{
	public partial class VisualCircle : UserControl
	{
		public VisualCircle()
		{
			InitializeComponent();
		}
		public VisualCircle(ROVector2f size, string imagePath) :this()
		{
			Height = size.Y;
			Width = size.X;
			var theCanvas = FindName("TheCanvas") as Canvas;
			theCanvas.Height = size.Y;
			theCanvas.Width = size.X;

			var theEllipse = FindName("TheEllipse") as Ellipse;
			theEllipse.Height = size.Y;
			theEllipse.Width = size.X;

			var brush = new ImageBrush();
			brush.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative));
			theEllipse.Fill = brush;
		}
	}
}
