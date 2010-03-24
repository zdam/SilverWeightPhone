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
	
	/// <summary> A polygon represented by a list of its vertices in counterclockwise
	/// ordering. Note that the term 'counterclockwise' depends on the
	/// orientation of the axes: if x points to the right and y points up,
	/// the vertices are counter clockwise.
	/// This means that on many displays the ordering of vertices will be
	/// clockwise because the y axis is pointing down.
	/// 
	/// TODO: the polygon is immutable but that could be changed
	/// 
	/// </summary>
	public class Polygon:AbstractShape, DynamicShape
	{
		/// <summary> Get the Area of this polygon</summary>
		/// <returns> the Area of this polygon
		/// </returns>
		virtual public float Area
		{
			get
			{
				return area;
			}
			
		}
		/// <summary> Check wether or not the polygon is convex.
		/// 
		/// </summary>
		/// <returns> true iff this polygon is convex
		/// </returns>
		virtual public bool Convex
		{
			get
			{
				// check if all angles are smaller or equal to 180 degrees
				int l = vertices.Length;
				
				for (int i = 0; i < vertices.Length; i++)
				{
					Vector2f x = vertices[i];
					Vector2f y = vertices[(i + 1) % l];
					Vector2f z = vertices[(i + 2) % l];
					
					// does the 3d Cross product point up or down?
					if ((z.x - x.x) * (y.y - x.y) - (y.x - x.x) * (z.y - x.y) >= 0)
						return false;
				}
				
				return true;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Shapes.Shape.getSurfaceFactor()">
		/// </seealso>
		override public float SurfaceFactor
		{
			get
			{
				// TODO: return the real surface factor
				return Area;
			}
			
		}
		
		/// <summary>The vertices of this polygon in counterclockwise order </summary>
		protected internal Vector2f[] vertices;
		/// <summary>The total Area of this polygon </summary>
		protected internal float area;
		/// <summary>The center of mass of this polygon </summary>
		protected internal Vector2f centroid;
		
		/// <summary>Construct the polygon with a list of vertices
		/// sorted in counterclockwise order.
		/// Note that all the vector values will be copied.
		/// 
		/// Throws an exception when too few vertices (&lt;3) are supplied.
		/// TODO: throw an exception when the vertices arent counterclockwise?
		/// 
		/// </summary>
		/// <param name="vertices">Vertices sorted in counterclockwise order
		/// </param>
		public Polygon(ROVector2f[] vertices)
		{
			if (vertices.Length < 3)
				throw new System.ArgumentException("A polygon can not have fewer than 3 edges!");
			
			this.vertices = new Vector2f[vertices.Length];
			
			for (int i = 0; i < vertices.Length; i++)
			{
				this.vertices[i] = new Vector2f(vertices[i]);
			}
			
			
			float r = ComputeBoundingCircleRadius();
			this.bounds = new AABox(r * 2, r * 2);
			this.area = ComputeArea();
			this.centroid = ComputeCentroid();
		}
		
		/// <summary> A constructor that allows for overloading without using
		/// the public constructor. Does absolutely nothing.
		/// </summary>
		protected internal Polygon()
		{
		}
		
		/// <summary> Computes the Area as described by Paul Borke.
		/// See: http://local.wasp.uwa.edu.au/~pbourke/geometry/polyarea/
		/// 
		/// </summary>
		/// <returns> this polygon's computed Area
		/// </returns>
		protected internal virtual float ComputeArea()
		{
			this.area = 0;
			
			Vector2f v1, v2;
			
			for (int i = 0; i < vertices.Length; i++)
			{
				v1 = vertices[i];
				v2 = vertices[(i + 1) % vertices.Length];
				
				this.area += v1.x * v2.y;
				this.area -= v2.x * v1.y;
			}
			
			return System.Math.Abs(this.area / 2f);
		}
		
		/// <summary> Compute the centroid (center of mass) as described by Paul Borke.
		/// See: http://local.wasp.uwa.edu.au/~pbourke/geometry/polyarea/
		/// 
		/// Make sure you have computed the Area before calling this!
		/// 
		/// </summary>
		/// <returns> the computed centroid
		/// </returns>
		protected internal virtual Vector2f ComputeCentroid()
		{
			float x = 0;
			float y = 0;
			
			Vector2f v1, v2;
			
			for (int i = 0; i < vertices.Length; i++)
			{
				v1 = vertices[i];
				v2 = vertices[(i + 1) % vertices.Length];
				
				x += (v1.x + v2.x) * (v1.x * v2.y - v2.x * v1.y);
				y += (v1.y + v2.y) * (v1.x * v2.y - v2.x * v1.y);
			}
			
			return new Vector2f(x / (6 * this.area), y / (6 * this.area));
		}
		
		/// <summary> Computes the radius of an approximation of a minimal bounding circle
		/// which has its origin at (0,0) and sets this.bounds.
		/// 
		/// TODO: this can be done much better
		/// 
		/// </summary>
		/// <returns> The 
		/// </returns>
		protected internal virtual float ComputeBoundingCircleRadius()
		{
			float r = 0;
			float l;
			
			for (int i = 0; i < vertices.Length; i++)
			{
				l = vertices[i].x * vertices[i].x + vertices[i].y * vertices[i].y;
				r = l > r?l:r;
			}
			
			return (float) System.Math.Sqrt(r);
		}
		
		/// <summary> Get the center of mass (aka centroid) for this polygon.</summary>
		/// <returns> the center of mass
		/// </returns>
		public virtual Vector2f GetCentroid()
		{
			return centroid;
		}
		
		/// <summary> Returns a copy of the list of vertices. The vertices are sorted
		/// counterclockwise.
		/// 
		/// </summary>
		/// <returns> this polygons vertices
		/// </returns>
		public virtual ROVector2f[] GetVertices()
		{
			ROVector2f[] roVertices = new ROVector2f[vertices.Length];
			
			for (int i = 0; i < vertices.Length; i++)
				roVertices[i] = vertices[i];
			
			return roVertices;
		}
		
		/// <summary> Returns a translated and rotated copy of this poly's vertices.
		/// The vertices are rotated before they are translated, i.e. they
		/// are rotated around the origin (0,0).
		/// The vertices are sorted counterclockwise.
		/// 
		/// This function is typically used to get the vertices for a 
		/// specific body, for example to Collide it with another body
		/// or draw it.
		/// 
		/// </summary>
		/// <param name="displacement">The displacement with wich all the 
		/// </param>
		/// <param name="rotation">
		/// </param>
		/// <returns> this polygon's vertices translated and rotated
		/// </returns>
		public virtual Vector2f[] GetVertices(ROVector2f displacement, float rotation)
		{
			Vector2f[] retVertices = new Vector2f[vertices.Length];
			
			float cos = (float) System.Math.Cos(rotation);
			float sin = (float) System.Math.Sin(rotation);
			
			for (int i = 0; i < vertices.Length; i++)
			{
				float x = vertices[i].x * cos - vertices[i].y * sin;
				float y = vertices[i].y * cos + vertices[i].x * sin;
				x += displacement.X;
				y += displacement.Y;
				
				retVertices[i] = new Vector2f(x, y);
			}
			
			return retVertices;
		}
		
		/// <summary> Returns a translated and rotated copy of this poly's centroid.
		/// The centroid is rotated before it is translated, i.e. it
		/// is rotated around the origin (0,0).
		/// 
		/// </summary>
		/// <param name="displacement">The displacement with wich all the 
		/// </param>
		/// <param name="rotation">
		/// </param>
		/// <returns> this polygon's vertices translated and rotated
		/// </returns>
		public virtual Vector2f GetCentroid(ROVector2f displacement, float rotation)
		{
			float cos = (float) System.Math.Cos(rotation);
			float sin = (float) System.Math.Sin(rotation);
			
			return new Vector2f(centroid.x * cos - centroid.y * sin + displacement.X, centroid.y * cos + centroid.x * sin + displacement.Y);
		}
		
		/// <summary> Test whether or not the point p is in this polygon in O(n),
		/// where n is the number of vertices in this polygon.
		/// 
		/// </summary>
		/// <param name="p">The point to be tested for inclusion in this polygon
		/// </param>
		/// <returns> true iff the p is in this polygon (not on a border)
		/// </returns>
		public virtual bool Contains(ROVector2f p)
		{
			// TODO: implement this
			
			return false;
		}
		
		/// <summary> Get point on this polygon's hull that is closest to p.
		/// 
		/// TODO: make this thing return a negative value when it is contained in the polygon
		/// 
		/// </summary>
		/// <param name="p">The point to search the closest point for
		/// </param>
		/// <returns> the nearest point on this vertex' hull
		/// </returns>
		public virtual ROVector2f GetNearestPoint(ROVector2f p)
		{
			// TODO: implement this
			
			return null;
		}
	}
}