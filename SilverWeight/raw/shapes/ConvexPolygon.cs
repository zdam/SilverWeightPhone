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
using MathUtil = Silver.Weight.Math.MathUtil;
using ROVector2f = Silver.Weight.Math.ROVector2f;
using Vector2f = Silver.Weight.Math.Vector2f;
namespace Silver.Weight.Raw.Shapes
{
	
	/// <summary> Class representing a convex and closed polygon as a list of vertices
	/// in counterclockwise order. Convexity is maintained by a check in the
	/// constructor after which the polygon becomes immutable.
	/// 
	/// </summary>
	public class ConvexPolygon:Polygon, DynamicShape
	{
		/// <summary> Because convexness is checked at construction
		/// we can always return true here.
		/// </summary>
		/// <seealso cref="Polygon.isConvex()">
		/// </seealso>
		override public bool Convex
		{
			get
			{
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
		
		/// <summary>Construct the convex polygon with a list of vertices
		/// sorted in counterclockwise order.
		/// Note that all the vector values will be copied.
		/// 
		/// Throws an exception when too few vertices are given (< 3)
		/// and when the supplied vertices are not convex.
		/// Polygons with Area = 0, will be reported as non-convex too.
		/// 
		/// </summary>
		/// <param name="vertices">Vertices sorted in counterclockwise order
		/// </param>
		public ConvexPolygon(ROVector2f[] vertices)
		{
			if (vertices.Length < 3)
				throw new System.ArgumentException("A polygon can not have fewer than 3 edges!");
			
			this.vertices = new Vector2f[vertices.Length];
			
			for (int i = 0; i < vertices.Length; i++)
			{
				this.vertices[i] = new Vector2f(vertices[i]);
			}
			
			if (!base.Convex)
				throw new System.ArgumentException("The supplied vertices do not represent a convex polygon!");
			
			float r = ComputeBoundingCircleRadius();
			this.bounds = new AABox(r * 2, r * 2);
			this.area = ComputeArea();
			this.centroid = ComputeCentroid();
		}
		
		/// <summary> Test whether or not the point p is in this polygon in O(n),
		/// where n is the number of vertices in this polygon.
		/// 
		/// </summary>
		/// <param name="p">The point to be tested for inclusion in this polygon
		/// </param>
		/// <returns> true iff the p is in this polygon (not on a border)
		/// </returns>
		public virtual bool Contains(Vector2f p)
		{
			// p is in the polygon if it is left of all the edges
			int l = vertices.Length;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector2f x = vertices[i];
				Vector2f y = vertices[(i + 1) % l];
				Vector2f z = p;
				
				// does the 3d Cross product point up or down?
				if ((z.x - x.x) * (y.y - x.y) - (y.x - x.x) * (z.y - x.y) >= 0)
					return false;
			}
			
			return true;
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
		public override ROVector2f GetNearestPoint(ROVector2f p)
		{
			// TODO: this can be done with a kind of binary search
			float r = System.Single.MaxValue;
			float l;
			Vector2f v;
			int m = - 1;
			
			for (int i = 0; i < vertices.Length; i++)
			{
				v = new Vector2f(vertices[i]);
				v.Sub(p);
				l = v.x * v.x + v.y * v.y;
				
				if (l < r)
				{
					r = l;
					m = i;
				}
			}
			
			// the closest point could be on one of the closest point's edges
			// this happens when the angle between v[m-1]-v[m] and p-v[m] is
			// smaller than 90 degrees, same for v[m+1]-v[m]
			int length = vertices.Length;
			Vector2f pm = new Vector2f(p);
			pm.Sub(vertices[m]);
			Vector2f l1 = new Vector2f(vertices[(m - 1 + length) % length]);
			l1.Sub(vertices[m]);
			Vector2f l2 = new Vector2f(vertices[(m + 1) % length]);
			l2.Sub(vertices[m]);
			
			Vector2f normal;
			if (pm.Dot(l1) > 0)
			{
				normal = MathUtil.GetNormal(vertices[(m - 1 + length) % length], vertices[m]);
			}
			else if (pm.Dot(l2) > 0)
			{
				normal = MathUtil.GetNormal(vertices[m], vertices[(m + 1) % length]);
			}
			else
			{
				return vertices[m];
			}
			
			normal.Scale(- pm.Dot(normal));
			normal.Add(p);
			return normal;
		}
	}
}