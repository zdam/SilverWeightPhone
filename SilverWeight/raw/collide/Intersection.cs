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
using Vector2f = Silver.Weight.Math.Vector2f;
namespace Silver.Weight.Raw.Collide
{
	/// <summary>Class representing a single intersection.
	/// TODO: this should be merged with the 'Feature' class, it represents the same thing
	/// 
	/// </summary>
	public class Intersection
	{
		/// <summary>The edge of polygon A that intersects </summary>
		public int edgeA;
		/// <summary>The edge of polygon B that intersects </summary>
		public int edgeB;
		
		/// <summary>The position of the intersection in world (absolute) coordinates </summary>
		public Vector2f position;
		
		/// <summary>True iff this is an intersection where polygon A enters B </summary>
		public bool isIngoing;
		
		/// <summary> Construct an intersection with all its attributes set.
		/// 
		/// </summary>
		/// <param name="edgeA">The edge of polygon A that intersects
		/// </param>
		/// <param name="edgeB">The edge of polygon B that intersects
		/// </param>
		/// <param name="position">The position of the intersection in world (absolute) coordinates
		/// </param>
		/// <param name="isIngoing">True iff this is an intersection where polygon A enters B 
		/// </param>
		public Intersection(int edgeA, int edgeB, Vector2f position, bool isIngoing):base()
		{
			this.edgeA = edgeA;
			this.edgeB = edgeB;
			this.position = position;
			this.isIngoing = isIngoing;
		}
	}
}