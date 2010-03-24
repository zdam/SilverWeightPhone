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
namespace Silver.Weight.Raw.Shapes
{
	
	/// <summary> A simple Circle within the simulation, defined by its radius and the
	/// position of the body to which it belongs
	/// 
	/// </summary>
	public class Circle:AbstractShape, DynamicShape
	{
		/// <summary> Get the radius of the circle
		/// 
		/// </summary>
		/// <returns> The radius of the circle
		/// </returns>
		virtual public float Radius
		{
			get
			{
				return radius;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Shapes.Shape.getSurfaceFactor()">
		/// </seealso>
		override public float SurfaceFactor
		{
			get
			{
				float circ = (float) (2 * System.Math.PI * radius);
				circ /= 2;
				
				return circ * circ;
			}
			
		}
		/// <summary>The radius of the circle </summary>
		private float radius;
		
		/// <summary> Create a new circle based on its radius
		/// 
		/// </summary>
		/// <param name="radius">The radius of the circle
		/// </param>
		public Circle(float radius):base(new AABox(radius * 2, radius * 2))
		{
			
			this.radius = radius;
		}
		
		/// <summary> Check if this circle Touches another
		/// 
		/// </summary>
		/// <param name="x">The x position of this circle
		/// </param>
		/// <param name="y">The y position of this circle
		/// </param>
		/// <param name="other">The other circle
		/// </param>
		/// <param name="ox">The other circle's x position
		/// </param>
		/// <param name="oy">The other circle's y position
		/// </param>
		/// <returns> True if they touch
		/// </returns>
		public virtual bool Touches(float x, float y, Circle other, float ox, float oy)
		{
			float totalRad2 = Radius + other.Radius;
			
			if (System.Math.Abs(ox - x) > totalRad2)
			{
				return false;
			}
			if (System.Math.Abs(oy - y) > totalRad2)
			{
				return false;
			}
			
			totalRad2 *= totalRad2;
			
			float dx = System.Math.Abs(ox - x);
			float dy = System.Math.Abs(oy - y);
			
			return totalRad2 >= ((dx * dx) + (dy * dy));
		}
	}
}