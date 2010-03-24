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
using BodyList = Silver.Weight.Raw.BodyList;
using BroadCollisionStrategy = Silver.Weight.Raw.BroadCollisionStrategy;
using CollisionContext = Silver.Weight.Raw.CollisionContext;
namespace Silver.Weight.Raw.Strategies
{
	
	/// <summary> Brute force collision. Compare every body against every other
	/// 
	/// </summary>
	public class BruteCollisionStrategy : BroadCollisionStrategy
	{
		
		/// <seealso cref="Silver.Weight.Raw.BroadCollisionStrategy.CollideBodies(Silver.Weight.Raw.CollisionContext, Silver.Weight.Raw.BodyList, float)">
		/// </seealso>
		public virtual void  CollideBodies(CollisionContext context, BodyList bodies, float dt)
		{
			context.Resolve(bodies, dt);
		}
	}
}