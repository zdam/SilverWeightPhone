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
	
	/// <summary> An axis oriented used for shape bounds
	/// 
	/// </summary>
	public class AABox
	{
		/// <summary> Get the width of the box
		/// 
		/// </summary>
		/// <returns> The width of the box
		/// </returns>
		virtual public float Width
		{
			get
			{
				return width;
			}
			
		}
		/// <summary> Get the height of the box
		/// 
		/// </summary>
		/// <returns> The height of the box
		/// </returns>
		virtual public float Height
		{
			get
			{
				return height;
			}
			
		}
		/// <summary> Get the x offset to the body's position of this bounds
		/// 
		/// </summary>
		/// <returns> The x offset to the body's position of this bounds
		/// </returns>
		virtual public float OffsetX
		{
			get
			{
				return offsetx;
			}
			
		}
		/// <summary> Get the y offset to the body's position of this bounds
		/// 
		/// </summary>
		/// <returns> The y offset to this body's position of this bounds
		/// </returns>
		virtual public float OffsetY
		{
			get
			{
				return offsety;
			}
			
		}
		/// <summary>The width of the box </summary>
		private float width;
		/// <summary>The height of the box </summary>
		private float height;
		/// <summary>The x offset to the body's position of the bounds </summary>
		private float offsetx;
		/// <summary>The y offset to the body's position of the bounds </summary>
		private float offsety;
		
		/// <summary> Create a new bounding box
		/// 
		/// </summary>
		/// <param name="width">The width of the box
		/// </param>
		/// <param name="height">The hieght of the box
		/// </param>
		public AABox(float width, float height)
		{
			this.width = width;
			this.height = height;
		}
		
		/// <summary> Create a new AABox
		/// 
		/// </summary>
		/// <param name="offsetx">The x offset to the body's position
		/// </param>
		/// <param name="offsety">The y offset to the body's position
		/// </param>
		/// <param name="width">The width of the box
		/// </param>
		/// <param name="height">The hieght of the box
		/// </param>
		public AABox(float offsetx, float offsety, float width, float height)
		{
			this.width = width;
			this.height = height;
			this.offsetx = offsetx;
			this.offsety = offsety;
		}
		
		/// <summary> Check if this box Touches another
		/// 
		/// </summary>
		/// <param name="x">The x position of this box
		/// </param>
		/// <param name="y">The y position of this box
		/// </param>
		/// <param name="other">The other box to check against  
		/// </param>
		/// <param name="otherx">The other box's x position
		/// </param>
		/// <param name="othery">The other box's y position
		/// </param>
		/// <returns> True if the boxes Touches
		/// </returns>
		public virtual bool Touches(float x, float y, AABox other, float otherx, float othery)
		{
			float totalWidth = (other.width + width) / 2;
			float totalHeight = (other.height + height) / 2;
			
			float dx = System.Math.Abs((x + offsetx) - (otherx + other.offsetx));
			float dy = System.Math.Abs((y + offsety) - (othery + other.offsety));
			
			return (totalWidth > dx) && (totalHeight > dy);
		}
		
		/// <seealso cref="java.lang.Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "[AABox " + width + "x" + height + "]";
		}
	}
}