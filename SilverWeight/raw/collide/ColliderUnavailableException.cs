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
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> An exception that is thrown by the ColliderFactory when no
	/// suitable collider has been found.
	/// The primary reason for this class is to allow us to catch
	/// this exception, which previously was impossible with the
	/// runtime exceptions.
	/// 
	/// A second reason is that a custom factory might overload this
	/// exception.
	/// 
	/// </summary>
	public class ColliderUnavailableException:System.Exception
	{
		
		/// <summary> Constructs an exception in case there is no collider
		/// for a specific combination of two shapes.
		/// 
		/// </summary>
		/// <param name="shapeA">First shape 
		/// </param>
		/// <param name="shapeB">Second shape
		/// </param>
		public ColliderUnavailableException(Shape shapeA, Shape shapeB):base("No collider available for shapes of type " + shapeA.GetType().FullName + " and " + shapeB.GetType().FullName)
		{
		}
		
		
		/// <summary>A constructor that can be used by subclasses. </summary>
		protected internal ColliderUnavailableException()
		{
		}
	}
}