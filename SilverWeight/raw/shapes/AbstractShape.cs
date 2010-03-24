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
	
	/// <summary> Super class of generic shapes in the engine
	/// 
	/// </summary>
	public abstract class AbstractShape : Shape
	{
		/// <seealso cref="Silver.Weight.Raw.Shapes.Shape.getBounds()">
		/// </seealso>
		virtual public AABox Bounds
		{
			get
			{
				return bounds;
			}
			
		}
		public abstract float SurfaceFactor{get;}
		/// <summary>The circular bounds that fit the shape based on the position of the body </summary>
		protected internal AABox bounds;
		
		/// <summary> Construct a new shape as subclas swhich will specified it's
		/// own bounds
		/// </summary>
		protected internal AbstractShape()
		{
		}
		
		/// <summary> Create a shape
		/// 
		/// </summary>
		/// <param name="bounds">The bounds of the shape
		/// </param>
		public AbstractShape(AABox bounds)
		{
			this.bounds = bounds;
		}
	}
}