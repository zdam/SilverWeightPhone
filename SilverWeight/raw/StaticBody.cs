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
using Shape = Silver.Weight.Raw.Shapes.Shape;
namespace Silver.Weight.Raw
{
	
	/// <summary> A body that will not Move
	/// 
	/// </summary>
	public class StaticBody:Body
	{
		/// <summary> Check if this body is static
		/// 
		/// </summary>
		/// <returns> True if this body is static
		/// </returns>
		override public bool Static
		{
			get
			{
				return true;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Body.isResting()">
		/// </seealso>
		override public bool Resting
		{
			get
			{
				return true;
			}
			
		}
		
		/// <summary> Create a static body
		/// 
		/// </summary>
		/// <param name="shape">The shape representing this body
		/// </param>
		public StaticBody(Shape shape):base(shape, Body.INFINITE_MASS)
		{
		}
		
		/// <summary> Create a static body
		/// 
		/// </summary>
		/// <param name="name">The name to assign to the body
		/// </param>
		/// <param name="shape">The shape representing this body
		/// </param>
		public StaticBody(System.String name, Shape shape):base(name, shape, Body.INFINITE_MASS)
		{
		}
		
		/// <seealso cref="Silver.Weight.Raw.Body.IsRotatable()">
		/// </seealso>
		public override bool IsRotatable()
		{
			return false;
		}
		
		/// <seealso cref="Silver.Weight.Raw.Body.IsMoveable()">
		/// </seealso>
		public override bool IsMoveable()
		{
			return false;
		}
	}
}