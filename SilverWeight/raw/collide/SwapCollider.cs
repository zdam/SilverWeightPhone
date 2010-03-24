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
using Vector2f = Silver.Weight.Math.Vector2f;
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> A collider wrapper that swaps the collision result of the collider.
	/// This is very useful to get rid of code duplication for colliders like
	/// CircleBoxCollider and BoxCircleCollider.
	/// 
	/// </summary>
	public class SwapCollider : Collider
	{
		
		/// <summary>The wrapped collider of which the result will be swapped </summary>
		private Collider collider;
		
		/// <summary> Create a collider that swaps the result of the wrapped
		/// collider.
		/// 
		/// </summary>
		/// <param name="collider">The collider of which to swap the result
		/// </param>
		public SwapCollider(Collider collider)
		{
			this.collider = collider;
		}
		
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			int count = collider.Collide(contacts, bodyB, bodyA);
			
			// Reverse the collision results by inverting normals
			for (int i = 0; i < count; i++)
			{
				Vector2f vec = MathUtil.Scale(contacts[i].Normal, - 1);
				contacts[i].Normal = vec;
			}
			
			return count;
		}
	}
}