# region License
/*
* Silver.Weight - a 2D Physics Engine for Microsoft(R) Silverlight (tm)
* Copyright (c) 2007, Adam Webber
*
* For updated news and information please visit http://travellinghead.com/
*
* This is a derivative work based on:
*  Phys2D (http://www.cokeandcode.com/phys2d/)  which is based on
*  the work of Erin Catto (http://www.gphysics.com/)
*
* This source is provided under the terms of the BSD License.
* See License.txt in the root of this distribution for details.
*/
#endregion
using System;
using ROVector2f = Silver.Weight.Math.ROVector2f;
using Vector2f = Silver.Weight.Math.Vector2f;
namespace Silver.Weight.Raw.Shapes
{
	
	/// <summary> Implemenation of a bunch of maths functions to do with lines. Note
	/// that lines can't be used as dynamic shapes right now - also collision 
	/// with the end of a line is undefined.
	/// 
	/// </summary>
	public class Line:AbstractShape, DynamicShape
	{
		/// <summary> Indicate if this line blocks on it's inner edge
		/// 
		/// </summary>
		/// <param name="innerEdge">True if this line blocks on it's inner edge
		/// </param>
		virtual public bool BlocksInnerEdge
		{
			set
			{
				this.innerEdge = value;
			}
			get
			{
				return this.innerEdge;
			}
			
		}

		/// <summary> Indicate if this line blocks on it's outer edge
		/// 
		/// </summary>
		/// <param name="outerEdge">True if this line blocks on it's outer edge
		/// </param>
		virtual public bool BlocksOuterEdge
		{
			set
			{
				this.outerEdge = value;
			}
			get
			{
				return this.outerEdge;
			}
			
		}
		/// <summary> Get the start point of the line
		/// 
		/// </summary>
		/// <returns> The start point of the line
		/// </returns>
		virtual public ROVector2f Start
		{
			get
			{
				return start;
			}
			
		}
		/// <summary> Get the end point of the line
		/// 
		/// </summary>
		/// <returns> The end point of the line
		/// </returns>
		virtual public ROVector2f End
		{
			get
			{
				return end;
			}
			
		}
		/// <summary> Get the x direction of this line
		/// 
		/// </summary>
		/// <returns> The x direction of this line
		/// </returns>
		virtual public float DX
		{
			get
			{
				return end.X - start.X;
			}
			
		}
		/// <summary> Get the y direction of this line
		/// 
		/// </summary>
		/// <returns> The y direction of this line
		/// </returns>
		virtual public float DY
		{
			get
			{
				return end.Y - start.Y;
			}
			
		}
		/// <summary> Get the x coordinate of the start point
		/// 
		/// </summary>
		/// <returns> The x coordinate of the start point
		/// </returns>
		virtual public float X1
		{
			get
			{
				return start.X;
			}
			
		}
		/// <summary> Get the y coordinate of the start point
		/// 
		/// </summary>
		/// <returns> The y coordinate of the start point
		/// </returns>
		virtual public float Y1
		{
			get
			{
				return start.Y;
			}
			
		}
		/// <summary> Get the x coordinate of the end point
		/// 
		/// </summary>
		/// <returns> The x coordinate of the end point
		/// </returns>
		virtual public float X2
		{
			get
			{
				return end.X;
			}
			
		}
		/// <summary> Get the y coordinate of the end point
		/// 
		/// </summary>
		/// <returns> The y coordinate of the end point
		/// </returns>
		virtual public float Y2
		{
			get
			{
				return end.Y;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Shapes.Shape.getSurfaceFactor()">
		/// </seealso>
		override public float SurfaceFactor
		{
			get
			{
				return lengthSquared() / 2;
			}
			
		}
		/// <summary>The start point of the line </summary>
		private ROVector2f start;
		/// <summary>The end point of the line </summary>
		private ROVector2f end;
		/// <summary>The vector between the two points </summary>
		private Vector2f vec;
		/// <summary>The Length of the line squared </summary>
		private float lenSquared;
		
		/// <summary>Temporary storage - declared globally to reduce GC </summary>
		private Vector2f loc = new Vector2f(0, 0);
		/// <summary>Temporary storage - declared globally to reduce GC </summary>
		private Vector2f v = new Vector2f(0, 0);
		/// <summary>Temporary storage - declared globally to reduce GC </summary>
		private Vector2f v2 = new Vector2f(0, 0);
		/// <summary>Temporary storage - declared globally to reduce GC </summary>
		private Vector2f proj = new Vector2f(0, 0);
		
		/// <summary>Temporary storage - declared globally to reduce GC </summary>
		private Vector2f closest = new Vector2f(0, 0);
		/// <summary>Temporary storage - declared globally to reduce GC </summary>
		private Vector2f other = new Vector2f(0, 0);
		
		/// <summary>True if this line blocks on the outer edge </summary>
		private bool outerEdge = true;
		/// <summary>True if this line blocks on the inner edge </summary>
		private bool innerEdge = true;
		
		/// <summary> Create a new line based on the origin and a single point
		/// 
		/// </summary>
		/// <param name="x">The end point of the line
		/// </param>
		/// <param name="y">The end point of the line
		/// </param>
		/// <param name="inner">True if this line blocks on it's inner edge
		/// </param>
		/// <param name="outer">True if this line blocks on it's outer edge
		/// </param>
		public Line(float x, float y, bool inner, bool outer):this(0, 0, x, y)
		{
			
			BlocksInnerEdge = inner;
			BlocksOuterEdge = outer;
		}
		
		/// <summary> Create a new line based on the origin and a single point
		/// 
		/// </summary>
		/// <param name="x">The end point of the line
		/// </param>
		/// <param name="y">The end point of the line
		/// </param>
		public Line(float x, float y):this(x, y, true, true)
		{
		}
		
		/// <summary> Create a new line based on two points
		/// 
		/// </summary>
		/// <param name="x1">The x coordinate of the start point
		/// </param>
		/// <param name="y1">The y coordinate of the start point
		/// </param>
		/// <param name="x2">The x coordinate of the end point
		/// </param>
		/// <param name="y2">The y coordinate of the end point
		/// </param>
		public Line(float x1, float y1, float x2, float y2):this(new Vector2f(x1, y1), new Vector2f(x2, y2))
		{
		}
		
		/// <summary> Create a new line based on two points
		/// 
		/// </summary>
		/// <param name="start">The start point
		/// </param>
		/// <param name="end">The end point
		/// </param>
		public Line(ROVector2f start, ROVector2f end):base()
		{
			
			//		float width = Math.Abs(end.getX()-start.getX());
			//		float height = Math.Abs(end.getY()-start.getY());
			//		float xoffset = width/2;
			//		float yoffset = height/2;
			//		if (width < 10) {
			//			width = 10;
			//		}
			//		if (height < 10) {
			//			height = 50;
			//		}
			//		if (end.getY() < start.getY()) {
			//			yoffset = -yoffset;
			//		}
			//		if (end.getX() < start.getX()) {
			//			xoffset = -xoffset;
			//		}
			//TODO: do this properly!
			float radius = System.Math.Max(start.Length(), end.Length());
			bounds = new AABox(0, 0, radius * 2, radius * 2);
			
			Reconfigure(start, end);
		}
		
		
		/// <summary> Find the Length of the line
		/// 
		/// </summary>
		/// <returns> The the Length of the line
		/// </returns>
		public virtual float length()
		{
			return vec.Length();
		}
		
		/// <summary> Find the Length of the line squared (cheaper and good for comparisons)
		/// 
		/// </summary>
		/// <returns> The Length of the line squared
		/// </returns>
		public virtual float lengthSquared()
		{
			return vec.LengthSquared();
		}
		
		/// <summary> Configure the line
		/// 
		/// </summary>
		/// <param name="start">The start point of the line
		/// </param>
		/// <param name="end">The end point of the line
		/// </param>
		public virtual void  Reconfigure(ROVector2f start, ROVector2f end)
		{
			this.start = start;
			this.end = end;
			
			vec = new Vector2f(end);
			vec.Sub(start);
			
			lenSquared = vec.Length();
			lenSquared *= lenSquared;
		}
		
		/// <summary> Get the shortest Distance from a point to this line
		/// 
		/// </summary>
		/// <param name="point">The point from which we want the Distance
		/// </param>
		/// <returns> The Distance from the line to the point
		/// </returns>
		public virtual float distance(ROVector2f point)
		{
			return (float) System.Math.Sqrt(distanceSquared(point));
		}
		
		/// <summary> Get the shortest Distance squared from a point to this line
		/// 
		/// </summary>
		/// <param name="point">The point from which we want the Distance
		/// </param>
		/// <returns> The Distance squared from the line to the point
		/// </returns>
		public virtual float distanceSquared(ROVector2f point)
		{
			getClosestPoint(point, closest);
			closest.Sub(point);
			
			float result = closest.LengthSquared();
			
			return result;
		}
		
		/// <summary> Get the closest point on the line to a given point
		/// 
		/// </summary>
		/// <param name="point">The point which we want to project
		/// </param>
		/// <param name="result">The point on the line closest to the given point
		/// </param>
		public virtual void  getClosestPoint(ROVector2f point, Vector2f result)
		{
			loc.Reconfigure(point);
			loc.Sub(start);
			
			v.Reconfigure(vec);
			v2.Reconfigure(vec);
			v2.Scale(- 1);
			
			v.Normalise();
			loc.ProjectOntoUnit(v, proj);
			if (proj.LengthSquared() > vec.LengthSquared())
			{
				result.Reconfigure(end);
				return ;
			}
			proj.Add(start);
			
			other.Reconfigure(proj);
			other.Sub(end);
			if (other.LengthSquared() > vec.LengthSquared())
			{
				result.Reconfigure(start);
				return ;
			}
			
			result.Reconfigure(proj);
			return ;
		}
		
		/// <summary> Get a line starting a x,y and ending offset from the current
		/// end point. Curious huh?
		/// 
		/// </summary>
		/// <param name="displacement">The displacement of the line
		/// </param>
		/// <param name="rotation">The rotation of the line in radians
		/// </param>
		/// <returns> The newly created line
		/// </returns>
		public virtual Line getPositionedLine(ROVector2f displacement, float rotation)
		{
			Vector2f[] verts = getVertices(displacement, rotation);
			Line line = new Line(verts[0], verts[1]);
			
			return line;
		}
		
		/// <summary> Return a translated and rotated line.
		/// 
		/// </summary>
		/// <param name="displacement">The displacement of the line
		/// </param>
		/// <param name="rotation">The rotation of the line in radians
		/// </param>
		/// <returns> The two endpoints of this line
		/// </returns>
		public virtual Vector2f[] getVertices(ROVector2f displacement, float rotation)
		{
			float cos = (float) System.Math.Cos(rotation);
			float sin = (float) System.Math.Sin(rotation);
			
			Vector2f[] endPoints = new Vector2f[2];
			endPoints[0] = new Vector2f(X1 * cos - Y1 * sin, Y1 * cos + X1 * sin);
			endPoints[0].Add(displacement);
			endPoints[1] = new Vector2f(X2 * cos - Y2 * sin, Y2 * cos + X2 * sin);
			endPoints[1].Add(displacement);
			
			return endPoints;
		}
		
		/// <summary> Move this line a certain amount
		/// 
		/// </summary>
		/// <param name="v">The amount to Move the line
		/// </param>
		public virtual void  move(ROVector2f v)
		{
			Vector2f temp = new Vector2f(start);
			temp.Add(v);
			start = temp;
			temp = new Vector2f(end);
			temp.Add(v);
			end = temp;
		}
		
		/// <seealso cref="java.lang.Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "[Line " + start + "," + end + "]";
		}
		
		/// <summary> Intersect this line with another
		/// 
		/// </summary>
		/// <param name="other">The other line we should Intersect with
		/// </param>
		/// <returns> The intersection point or null if the lines are parallel
		/// </returns>
		public virtual Vector2f intersect(Line other)
		{
			float dx1 = end.X - start.X;
			float dx2 = other.end.X - other.start.X;
			float dy1 = end.Y - start.Y;
			float dy2 = other.end.Y - other.start.Y;
			float denom = (dy2 * dx1) - (dx2 * dy1);
			
			if (denom == 0)
			{
				return null;
			}
			
			float ua = (dx2 * (start.Y - other.start.Y)) - (dy2 * (start.X - other.start.X));
			ua /= denom;
			float ub = (dx1 * (start.Y - other.start.Y)) - (dy1 * (start.X - other.start.X));
			ub /= denom;
			
			float u = ua;
			
			float ix = start.X + (u * (end.X - start.X));
			float iy = start.Y + (u * (end.Y - start.Y));
			
			return new Vector2f(ix, iy);
		}
	}
}