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
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> The description of any geometry collision resolver. 
	/// 
	/// </summary>
	public interface Collider
	{
		
		/// <summary> Determine is any collisions have occured between the two bodies 
		/// specified.
		/// 
		/// </summary>
		/// <param name="contacts">The contacts array to populate with results
		/// </param>
		/// <param name="bodyA">The first body to check against
		/// </param>
		/// <param name="bodyB">The second body to check against
		/// </param>
		/// <returns> The number of contacts that have been determined and hence
		/// populated in the array.
		/// </returns>
		int Collide(Contact[] contacts, Body bodyA, Body bodyB);
	}
}