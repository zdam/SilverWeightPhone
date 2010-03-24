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
	
	/// <summary> A simple shape describing the Area covered by a body
	/// 
	/// </summary>
	public interface Shape
	{
		/// <summary> Get the box bounds of the shape
		/// 
		/// </summary>
		/// <returns> The bounds of the shape
		/// </returns>
		AABox Bounds
		{
			get;
			
		}
		/// <summary> Some factor based on the edges Length of the shape
		/// 
		/// </summary>
		/// <returns> The factor result - from the original demo
		/// </returns>
		float SurfaceFactor
		{
			get;
			
		}
	}
}