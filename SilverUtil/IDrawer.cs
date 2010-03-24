using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Silver.Weight.Raw;
using Silver.Weight.Math;
using Silver.Weight.Raw.Shapes;

namespace SilverUtil
{
	public interface IDrawer
	{
		void DrawWorld(World world);
		void DrawBody(Body body);
		void DrawJoint(Joint joint);
		void Clear();
	}

	/// <summary>
	/// Draws using lines
	/// </summary>
	public class BaseDrawer: IDrawer
	{
		protected Canvas canvas;
		public BaseDrawer(Canvas canvas)
		{
			this.canvas = canvas;
		}

		public virtual void PrepForDrawing()
		{
		}

		public void DrawWorld(World world)
		{
			PrepForDrawing();

			BodyList bodies = world.Bodies;

			for (int i = 0; i < bodies.Size(); i++)
			{
				Body body = bodies.Item(i);

				DrawBody(body);
			}

			JointList joints = world.Joints;

			for (int i = 0; i < joints.Size(); i++)
			{
				Joint joint = joints.Item(i);

				DrawJoint(joint);
			}
		}

		public void DrawJoint(Joint joint)
		{
			System.Windows.Shapes.Line e = joint.UserData as System.Windows.Shapes.Line;
			if (e == null)
			{
				if (joint is FixedJoint)
				{
					FixedJoint fixedJoint = (FixedJoint)joint;

					joint.UserData = MakeLine(fixedJoint.Body1.GetPosition(), fixedJoint.Body2.GetPosition(), 1, Colors.Yellow);
					canvas.Children.Add((UIElement)joint.UserData);
				}
				if (joint is BasicJoint)
				{
					joint.UserData = MakeLine(joint.Body1.GetPosition(), joint.Body2.GetPosition(), 2, Colors.Yellow);
					canvas.Children.Add((UIElement)joint.UserData);
				}
			}
			else
			{
				UpdateLinePosition(e, joint.Body1.GetPosition(), joint.Body2.GetPosition());
			}
		}

		public void DrawBody(Body body)
		{
			if (body.Shape is Box)
			{
				DrawBoxBody(body, (Box)body.Shape);
			}
			if (body.Shape is Circle)
			{
				DrawCircleBody(body, (Circle)body.Shape);
			}
			if (body.Shape is Silver.Weight.Raw.Shapes.Line)
			{
				DrawLineBody(body, (Silver.Weight.Raw.Shapes.Line)body.Shape);
			}
			if (body.Shape is Silver.Weight.Raw.Shapes.Polygon)
			{
				DrawPolygonBody(body, (Silver.Weight.Raw.Shapes.Polygon)body.Shape);
			}
		}

		protected virtual void DrawPolygonBody(Body body, Silver.Weight.Raw.Shapes.Polygon polygon)
		{
			System.Windows.Shapes.Polygon poly = body.UserData as System.Windows.Shapes.Polygon;
			ROVector2f[] points = polygon.GetVertices(body.GetPosition(), body.Rotation);
			if (poly == null)
			{
				poly = new System.Windows.Shapes.Polygon();

				poly.StrokeThickness = 2;
				poly.Stroke = new SolidColorBrush(Colors.Yellow);

				PointCollection pts = new PointCollection();

				for (int i = 0; i < points.Length; i++)
				{
					
					pts[i] = new Point((double)points[i].X, (double)points[i].Y);

				}
				poly.Points = pts;
				canvas.Children.Add(poly);
				body.UserData = poly;
			}
			else
			{
				PointCollection pts = new PointCollection();
				for (int i = 0; i < points.Length; i++)
				{
					pts[i] = new Point((double)points[i].X, (double)points[i].Y);
				}
				poly.Points = pts;
			}
		}

		protected virtual void DrawCircleBody(Body body, Circle circle)
		{
			float x = body.GetPosition().X;
			float y = body.GetPosition().Y;
			float r = circle.Radius;
			float rot = body.Rotation;
			float xo = (float)(System.Math.Cos(rot) * r);
			float yo = (float)(System.Math.Sin(rot) * r);

			Ellipse e = body.UserData as Ellipse;
			if (e == null)
			{
				e = new Ellipse();
				e.Stroke = new SolidColorBrush(Colors.Yellow);
				e.StrokeThickness = 1;
				e.Width = r * 2;
				e.Height = r * 2;
				canvas.Children.Add(e);
				body.UserData = e;
			}
			
			var circleSize = new Vector2f(circle.Radius*2, circle.Radius*2);
			UpdateElementPosition(e, Convert.ToInt32(x), Convert.ToInt32(y), circleSize);						
		}

		private void DrawLineBody(Body body, Silver.Weight.Raw.Shapes.Line line)
		{
			System.Windows.Shapes.Line line1 = MakeLine(new Vector2f(line.X1, line.X2), new Vector2f(line.X2,line.Y2), 1, Colors.White);
			canvas.Children.Add(line1);
		}

		protected System.Windows.Shapes.Line MakeLine(ROVector2f pointA, ROVector2f pointB, int thickness, Color color)
		{
			System.Windows.Shapes.Line line1 = new System.Windows.Shapes.Line();
			UpdateLinePosition(line1, pointA, pointB);
			line1.StrokeThickness = thickness;
			line1.Stroke = new SolidColorBrush(color);
			return line1;
		}

		protected void UpdateLinePosition(System.Windows.Shapes.Line line, ROVector2f pointA, ROVector2f pointB)
		{
			line.X1 = pointA.X;
			line.Y1 = pointA.Y;
			line.X2 = pointB.X;
			line.Y2 = pointB.Y;
		}

		protected void UpdateElementPosition(UIElement e, float x, float y, ROVector2f size)
		{
			double left = Convert.ToDouble(x - (size.X / 2));
			double top = Convert.ToDouble(y - (size.Y/2));
			e.SetValue(Canvas.LeftProperty, left);
			e.SetValue(Canvas.TopProperty, top);
		}

		protected virtual void DrawBoxBody(Body body, Box box)
		{
			System.Windows.Shapes.Polygon poly = body.UserData as System.Windows.Shapes.Polygon;
			ROVector2f[] points = box.GetPoints(body.GetPosition(), body.Rotation);
			if (poly == null)
			{
				poly = new System.Windows.Shapes.Polygon();

				poly.StrokeThickness = 1;
				poly.Stroke = new SolidColorBrush(Colors.Yellow);

				PointCollection pts = new PointCollection();
				pts.Add(new Point(points[0].X, points[0].Y));
				pts.Add(new Point(points[1].X, points[1].Y));
				pts.Add(new Point(points[2].X, points[2].Y));
				pts.Add(new Point(points[3].X, points[3].Y));
				poly.Points = pts;
				canvas.Children.Add(poly);
				body.UserData = poly;
			}
			else
			{
				PointCollection pts = new PointCollection();
				pts.Add(new Point(points[0].X, points[0].Y));
				pts.Add(new Point(points[1].X, points[1].Y));
				pts.Add(new Point(points[2].X, points[2].Y));
				pts.Add(new Point(points[3].X, points[3].Y));
				poly.Points = pts;
			}
		}

		public virtual void Clear()
		{
			canvas.Children.Clear();			
		}
	}

	public class SpriteDrawer : BaseDrawer
	{
		string imagePath;
		public SpriteDrawer(Canvas canvas, string imagePath) :base(canvas)
		{
			this.imagePath = imagePath;
		}

		public override void PrepForDrawing()
		{
			// we don't Clear the canvas (we are re-using the already drawn sprites
			//base.PrepForDrawing();
		}

		protected override void DrawBoxBody(Body body, Box box)
		{
			FrameworkElement e = body.UserData as FrameworkElement;
			if (e == null)
			{
				e = new VisualBox(box.Size, imagePath);

				
				canvas.Children.Add(e);
				body.UserData = e;
			}
			ROVector2f pos = body.GetPosition();
			if(Single.IsNaN(pos.X) || Single.IsNaN(pos.Y)) return;
			UpdateElementPosition(e, Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), box.Size);
			ApplyRotationToElement(e,box.Size.X,box.Size.Y,RadToDeg(body.Rotation));
		}

		void ApplyRotationToElement(UIElement element, double controlWidth, double controlHeight, double angleInDegrees)
		{
			// must apply width and height to *element* before renderTransformOrigin will work
			element.SetValue(Canvas.WidthProperty, controlWidth);
			element.SetValue(Canvas.HeightProperty, controlHeight);

			RotateTransform rt = new RotateTransform();
			rt.Angle = angleInDegrees;
			element.RenderTransformOrigin = new Point(0.5, 0.5);
			element.RenderTransform = rt;
		}

		protected override void DrawCircleBody(Body body, Circle circle)
		{
			float x = body.GetPosition().X;
			float y = body.GetPosition().Y;
			float r = circle.Radius;
			float rot = body.Rotation;
			float xo = (float)(System.Math.Cos(rot) * r);
			float yo = (float)(System.Math.Sin(rot) * r);

			var circleSize = new Vector2f(circle.Radius * 2, circle.Radius * 2);

			UIElement e = body.UserData as UIElement;
			if (e == null)
			{
				e = new VisualCircle(circleSize, imagePath);
				canvas.Children.Add(e);
				body.UserData = e;
			}
			
			UpdateElementPosition(e, x, y, circleSize);
			ApplyRotationToElement(e, circle.Radius * 2, circle.Radius * 2, RadToDeg(body.Rotation));
		} 
				
		private const double Pi = 3.14159265358979323846264338327950288419716939937510;

		private double RadToDeg(float radians)
		{
			return radians * (180 / Pi);
		}
	}
}
