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
using System.Collections.Generic;
namespace Silver.Weight.Util
{
	
	/// <summary> Triangulates a polygon into triangles - duh. Doesn't handle
	/// holes in polys
	/// 
	/// </summary>
	/// <author>  Public Source from FlipCode
	/// </author>
	public class Triangulator
	{
		//private void  InitBlock()
		//{
		//    poly = new PointList(this);
		//    tris = new PointList(this);
		//}
		/// <summary> Get a count of the number of triangles produced
		/// 
		/// </summary>
		/// <returns> The number of triangles produced
		/// </returns>
		virtual public int TriangleCount
		{
			get
			{
				if (!tried)
				{
					throw new System.SystemException("Call Triangulate() before accessing triangles");
				}
				return tris.Size() / 3;
			}
			
		}
		/// <summary>The accepted error value </summary>
		private const float EPSILON = 0.0000000001f;
		/// <summary>The list of points to be triangulated </summary>
		private PointList poly = new PointList();
		/// <summary>The list of points describing the triangles </summary>
		private PointList tris = new PointList();
		/// <summary>True if we've tried to Triangulate </summary>
		private bool tried;
		

		
		/// <summary> Add a point describing the polygon to be triangulated
		/// 
		/// </summary>
		/// <param name="x">The x coordinate of the point
		/// </param>
		/// <param name="y">the y coordinate of the point
		/// </param>
		public virtual void  AddPolyPoint(float x, float y)
		{
			poly.Add(new Point(x, y));
		}
		
		/// <summary> Cause the triangulator to split the polygon
		/// 
		/// </summary>
		/// <returns> True if we managed the task
		/// </returns>
		public virtual bool Triangulate()
		{
			tried = true;
			
			bool worked = Process(poly, tris);
			return worked;
		}
		
		/// <summary> Get a point on a specified generated triangle
		/// 
		/// </summary>
		/// <param name="tri">The index of the triangle to interegate
		/// </param>
		/// <param name="i">The index of the point within the triangle to retrieve
		/// (0 - 2)
		/// </param>
		/// <returns> The x,y coordinate pair for the point
		/// </returns>
		public virtual float[] GetTrianglePoint(int tri, int i)
		{
			if (!tried)
			{
				throw new System.SystemException("Call Triangulate() before accessing triangles");
			}
			return tris.Item((tri * 3) + i).ToArray();
		}
		
		/// <summary> Find the Area of a polygon defined by the series of points
		/// in the list
		/// 
		/// </summary>
		/// <param name="contour">The list of points defined the contour of the polygon
		/// (Vector2f)
		/// </param>
		/// <returns> The Area of the polygon defined
		/// </returns>
		private float Area(PointList contour)
		{
			int n = contour.Size();
			
			float A = 0.0f;
			
			for (int p = n - 1, q = 0; q < n; p = q++)
			{
				Point contourP = contour.Item(p);
				Point contourQ = contour.Item(q);
				
				A += contourP.X * contourQ.Y - contourQ.X * contourP.Y;
			}
			return A * 0.5f;
		}
		
		/// <summary> Check if the point P is inside the triangle defined by
		/// the points A,B,C
		/// 
		/// </summary>
		/// <param name="Ax">Point A x-coordinate
		/// </param>
		/// <param name="Ay">Point A y-coordinate
		/// </param>
		/// <param name="Bx">Point B x-coordinate
		/// </param>
		/// <param name="By">Point B y-coordinate
		/// </param>
		/// <param name="Cx">Point C x-coordinate
		/// </param>
		/// <param name="Cy">Point C y-coordinate
		/// </param>
		/// <param name="Px">Point P x-coordinate
		/// </param>
		/// <param name="Py">Point P y-coordinate
		/// </param>
		/// <returns> True if the point specified is within the triangle
		/// </returns>
		private bool InsideTriangle(float Ax, float Ay, float Bx, float By, float Cx, float Cy, float Px, float Py)
		{
			float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
			float cCROSSap, bCROSScp, aCROSSbp;
			
			ax = Cx - Bx;
			ay = Cy - By;
			bx = Ax - Cx;
			by = Ay - Cy;
			cx = Bx - Ax;
			cy = By - Ay;
			apx = Px - Ax;
			apy = Py - Ay;
			bpx = Px - Bx;
			bpy = Py - By;
			cpx = Px - Cx;
			cpy = Py - Cy;
			
			aCROSSbp = ax * bpy - ay * bpx;
			cCROSSap = cx * apy - cy * apx;
			bCROSScp = bx * cpy - by * cpx;
			
			return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
		}
		
		/// <summary> Cut a the contour and Add a triangle into V to describe the 
		/// location of the cut
		/// 
		/// </summary>
		/// <param name="contour">The list of points defining the polygon
		/// </param>
		/// <param name="u">The index of the first point
		/// </param>
		/// <param name="v">The index of the second point
		/// </param>
		/// <param name="w">The index of the third point
		/// </param>
		/// <param name="n">?
		/// </param>
		/// <param name="V">The array to populate with indicies of triangles
		/// </param>
		/// <returns> True if a triangle was found
		/// </returns>
		private bool Snip(PointList contour, int u, int v, int w, int n, int[] V)
		{
			int p;
			float Ax, Ay, Bx, By, Cx, Cy, Px, Py;
			
			Ax = contour.Item(V[u]).X;
			Ay = contour.Item(V[u]).Y;
			
			Bx = contour.Item(V[v]).X;
			By = contour.Item(V[v]).Y;
			
			Cx = contour.Item(V[w]).X;
			Cy = contour.Item(V[w]).Y;
			
			if (EPSILON > (((Bx - Ax) * (Cy - Ay)) - ((By - Ay) * (Cx - Ax))))
			{
				return false;
			}
			
			for (p = 0; p < n; p++)
			{
				if ((p == u) || (p == v) || (p == w))
				{
					continue;
				}
				
				Px = contour.Item(V[p]).X;
				Py = contour.Item(V[p]).Y;
				
				if (InsideTriangle(Ax, Ay, Bx, By, Cx, Cy, Px, Py))
				{
					return false;
				}
			}
			
			return true;
		}
		
		/// <summary> Process a list of points defining a polygon</summary>
		/// <param name="contour">The list of points describing the polygon
		/// </param>
		/// <param name="result">The list of points describing the triangles. Groups
		/// of 3 describe each triangle 
		/// 
		/// </param>
		/// <returns> True if we succeeded in completing triangulation
		/// </returns>
		private bool Process(PointList contour, PointList result)
		{
			/* allocate and initialize list of Vertices in polygon */
			
			int n = contour.Size();
			if (n < 3)
				return false;
			
			int[] V = new int[n];
			
			/* we want a counter-clockwise polygon in V */
			
			if (0.0f < Area(contour))
			{
				for (int v = 0; v < n; v++)
					V[v] = v;
			}
			else
			{
				for (int v = 0; v < n; v++)
					V[v] = (n - 1) - v;
			}
			
			int nv = n;
			
			/*  Remove nv-2 Vertices, creating 1 triangle every time */
			int count = 2 * nv; /* error detection */
			
			for (int m = 0, v = nv - 1; nv > 2; )
			{
				/* if we loop, it is probably a non-simple polygon */
				if (0 >= (count--))
				{
					//** Triangulate: ERROR - probable bad polygon!
					return false;
				}
				
				/* three consecutive vertices in current polygon, <u,v,w> */
				int u = v;
				if (nv <= u)
					u = 0; /* previous */
				v = u + 1;
				if (nv <= v)
					v = 0; /* new v    */
				int w = v + 1;
				if (nv <= w)
					w = 0; /* Next     */
				
				if (Snip(contour, u, v, w, nv, V))
				{
					int a, b, c, s, t;
					
					/* true names of the vertices */
					a = V[u];
					b = V[v];
					c = V[w];
					
					/* output Triangle */
					result.Add(contour.Item(a));
					result.Add(contour.Item(b));
					result.Add(contour.Item(c));
					
					m++;
					
					/* Remove v from remaining polygon */
					for (s = v, t = v + 1; t < nv; s++, t++)
					{
						V[s] = V[t];
					}
					nv--;
					
					/* resest error detection counter */
					count = 2 * nv;
				}
			}
			
			return true;
		}
		
		/// <summary> A single point handled by the triangulator
		/// 
		/// </summary>
		public class Point
		{

			/// <summary> Get the x coordinate of the point
			/// 
			/// </summary>
			/// <returns> The x coordinate of the point
			/// </returns>
			virtual public float X
			{
				get
				{
					return x;
				}
				
			}
			/// <summary> Get the y coordinate of the point
			/// 
			/// </summary>
			/// <returns> The y coordinate of the point
			/// </returns>
			virtual public float Y
			{
				get
				{
					return y;
				}
				
			}

			/// <summary>The x coorindate of this point </summary>
			private float x;
			/// <summary>The y coorindate of this point </summary>
			private float y;
			
			/// <summary> Create a new point
			/// 
			/// </summary>
			/// <param name="x">The x coordindate of the point
			/// </param>
			/// <param name="y">The y coordindate of the point
			/// </param>
			public Point(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
			
			/// <summary> Convert this point into a float array
			/// 
			/// </summary>
			/// <returns> The contents of this point as a float array
			/// </returns>
			public virtual float[] ToArray()
			{
				return new float[]{x, y};
			}
		}
		
		/// <summary> A list of type <code>Point</code>
		/// 
		/// </summary>
		private class PointList
		{

			/// <summary>The list of points </summary>
			private IList<Point> points = new List<Point>();
			
		
			/// <summary> Add a point to the list 
			/// 
			/// </summary>
			/// <param name="point">The point to Add
			/// </param>
			public virtual void  Add(Point point)
			{
				points.Add(point);
			}
			
			/// <summary> Remove a point from the list
			/// 
			/// </summary>
			/// <param name="point">The point to Remove
			/// </param>
			public virtual void  Remove(Point point)
			{
				points.Remove(point);
			}
			
			/// <summary> Get the Size of the list
			/// 
			/// </summary>
			/// <returns> The Size of the list
			/// </returns>
			public virtual int Size()
			{
				return points.Count;
			}
			
			/// <summary> Get a point a specific index in the list
			/// 
			/// </summary>
			/// <param name="i">The index of the point to retrieve
			/// </param>
			/// <returns> The point
			/// </returns>
			public virtual Point Item(int i)
			{
				return (Point) points[i];
			}
		}
	}
}