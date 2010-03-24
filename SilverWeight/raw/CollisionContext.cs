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
namespace Silver.Weight.Raw
{
	
	/// <summary> The description of a class which can compute the collisions of a 
	/// set of bodies. This is part of decoupling the algorithm for broad phase
	/// collisions against from the physics simulation world.
	/// 
	/// </summary>
	public interface CollisionContext
	{
		/// <summary> Resolve and store the collisions betwen each body in the list 
		/// 
		/// </summary>
		/// <param name="bodies">The bodies to be resolved against each other
		/// </param>
		/// <param name="dt">The time thats passed since last collision check
		/// </param>
		void  Resolve(BodyList bodies, float dt);
	}
}