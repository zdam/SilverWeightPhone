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
	
	/// <summary> A description of class that can recieve notifications of collisions
	/// within a <code>CollisionSpace</code>
	/// 
	/// </summary>
	public interface CollisionListener
	{
		/// <summary> Notification that a collision occured
		/// 
		/// </summary>
		/// <param name="event">The describing the collision
		/// </param>
		void  CollisionOccured(CollisionEvent theEvent);
	}
}