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
	
	/// <summary> A description of any strategy for determining which bodies should
	/// be compared against each other for collision - some times referred to
	/// as the broad phase. For example the default implementation simply
	/// compares every body against every other. Another implementation might
	/// spatially partition the bodies into areas and only Resolve collisions
	/// between those in the same Area.
	/// 
	/// </summary>
	public interface BroadCollisionStrategy
	{
		
		/// <summary> Perform the broad phase strategy. The implementation of this method
		/// is expected to determine a set of lists of bodies to Collided against
		/// each other and then pass these lists back through the context for
		/// collision detection and response.
		/// 
		/// </summary>
		/// <param name="context">The context that can actually perform the collision
		/// checking.
		/// </param>
		/// <param name="bodies">The complete list of bodies to be computed
		/// </param>
		/// <param name="dt">The amount of time passed since last collision
		/// </param>
		void  CollideBodies(CollisionContext context, BodyList bodies, float dt);
	}
}