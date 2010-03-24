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
namespace Silver.Weight.Math
{
	
	/// <summary> A readonly two dimensional vector
	/// 
	/// </summary>
	public interface ROVector2f
	{
		/// <summary> Get the X component of this vector
		/// 
		/// </summary>
		/// <returns> The X component of this vector
		/// </returns>
		float X
		{
			get;
			
		}
		/// <summary> Get the Y component of this vector
		/// 
		/// </summary>
		/// <returns> The Y component of this vector
		/// </returns>
		float Y
		{
			get;
			
		}
		
		/// <summary> Get the Length of this vector
		/// 
		/// </summary>
		/// <returns> The Length of this vector
		/// </returns>
		float Length();
		
		/// <summary> Get the Dot product of this vector and another
		/// 
		/// </summary>
		/// <param name="other">The other vector to Dot against
		/// </param>
		/// <returns> The Dot product of the two vectors
		/// </returns>
		float Dot(ROVector2f other);
		
		/// <summary> Project this vector onto another
		/// 
		/// </summary>
		/// <param name="b">The vector to project onto
		/// </param>
		/// <param name="result">The projected vector
		/// </param>
		void  ProjectOntoUnit(ROVector2f b, Vector2f result);
		
		/// <summary> The Length of the vector squared
		/// 
		/// </summary>
		/// <returns> The Length of the vector squared
		/// </returns>
		float LengthSquared();
		
		/// <summary> Get the Distance from this point to another
		/// 
		/// </summary>
		/// <param name="other">The other point we're measuring to
		/// </param>
		/// <returns> The Distance to the other point
		/// </returns>
		float Distance(ROVector2f other);
		
		/// <summary> Get the Distance squared from this point to another
		/// 
		/// </summary>
		/// <param name="other">The other point we're measuring to
		/// </param>
		/// <returns> The Distance to the other point
		/// </returns>
		float DistanceSquared(ROVector2f other);
	}
}