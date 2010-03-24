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
using Matrix2f = Silver.Weight.Math.Matrix2f;
using ROVector2f = Silver.Weight.Math.ROVector2f;
using Vector2f = Silver.Weight.Math.Vector2f;
namespace Silver.Weight.Raw.Shapes
{
	
	/// <summary> A simple box in the engine - defined by a width and height
	/// 
	/// </summary>
	public class Box:AbstractShape, DynamicShape
	{
		/// <summary> Get the Size of this box
		/// 
		/// </summary>
		/// <returns> The Size of this box
		/// </returns>
		virtual public ROVector2f Size
		{
			get
			{
				return size;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Shapes.Shape.getSurfaceFactor()">
		/// </seealso>
		override public float SurfaceFactor
		{
			get
			{
				float x = size.X;
				float y = size.Y;
				
				return (x * x + y * y);
			}
			
		}
		/// <summary>The Size of the box </summary>
		private Vector2f size = new Vector2f();
		
		/// <summary> Create a box in the simulation 
		/// 
		/// </summary>
		/// <param name="width">The width of a box
		/// </param>
		/// <param name="height">The hieght of the box
		/// </param>
		public Box(float width, float height):base()
		{
			
			size.Reconfigure(width, height);
			bounds = new AABox(size.Length(), size.Length());
		}
		
		/// <summary> Get the current positon of a set of points
		/// 
		/// </summary>
		/// <param name="pos">The centre of the box
		/// </param>
		/// <param name="rotation">The rotation of the box
		/// </param>
		/// <returns> The points building up a box at this position and rotation
		/// </returns>
		public virtual Vector2f[] GetPoints(ROVector2f pos, float rotation)
		{
			Matrix2f R = new Matrix2f(rotation);
			Vector2f[] pts = new Vector2f[4];
			ROVector2f h = MathUtil.Scale(Size, 0.5f);
			
			pts[0] = MathUtil.Mul(R, new Vector2f(- h.X, - h.Y));
			pts[0].Add(pos);
			pts[1] = MathUtil.Mul(R, new Vector2f(h.X, - h.Y));
			pts[1].Add(pos);
			pts[2] = MathUtil.Mul(R, new Vector2f(h.X, h.Y));
			pts[2].Add(pos);
			pts[3] = MathUtil.Mul(R, new Vector2f(- h.X, h.Y));
			pts[3].Add(pos);
			
			return pts;
		}
	}
}