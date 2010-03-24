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
	
	/// <summary> A tagging interface used for shapes which the current implementation
	/// can use for dynamic bodies - i.e. some shapes arn't implemented to
	/// work for moving bodies.
	/// 
	/// </summary>
	public interface DynamicShape:Shape
	{
	}
}