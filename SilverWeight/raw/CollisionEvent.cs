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
using ROVector2f = Silver.Weight.Math.ROVector2f;
namespace Silver.Weight.Raw
{
	
	/// <summary> An event describing a collision between two bodies
	/// 
	/// </summary>
	public class CollisionEvent
	{
		/// <summary> Get the time of the collision
		/// 
		/// </summary>
		/// <returns> The time of the collision
		/// </returns>
		virtual public float Time
		{
			get
			{
				return time;
			}
			
		}
		/// <summary> Get the first body in the collision
		/// 
		/// </summary>
		/// <returns> The first body in the collision
		/// </returns>
		virtual public Body BodyA
		{
			get
			{
				return body1;
			}
			
		}
		/// <summary> Get the second body in the collision
		/// 
		/// </summary>
		/// <returns> The second body in the collision
		/// </returns>
		virtual public Body BodyB
		{
			get
			{
				return body2;
			}
			
		}
		/// <summary> Get the normal at the collision point
		/// 
		/// </summary>
		/// <returns> The normal at the collision point
		/// </returns>
		virtual public ROVector2f Normal
		{
			get
			{
				return normal;
			}
			
		}
		/// <summary> Get the point where the collision occured
		/// 
		/// </summary>
		/// <returns> The point where the collision occured
		/// </returns>
		virtual public ROVector2f Point
		{
			get
			{
				return point;
			}
			
		}
		/// <summary> Get the penetration depth caused by the collision
		/// 
		/// </summary>
		/// <returns> The penetration depth caused by the collision
		/// </returns>
		virtual public float PenetrationDepth
		{
			get
			{
				return depth;
			}
			
		}
		/// <summary>The time of the collision detection </summary>
		private float time;
		/// <summary>The first body in the collision </summary>
		private Body body1;
		/// <summary>The second body in the collision </summary>
		private Body body2;
		/// <summary>The point of the collision </summary>
		private ROVector2f point;
		/// <summary>The normal at the collision </summary>
		private ROVector2f normal;
		/// <summary>The penetration caused by the collision </summary>
		private float depth;
		
		/// <summary> Create a new event describing a contact 
		/// 
		/// </summary>
		/// <param name="time">The time of the collision
		/// </param>
		/// <param name="body1">The first body in the collision
		/// </param>
		/// <param name="body2">The second body in the collision
		/// </param>
		/// <param name="point">The point of collision (not always perfect - accepts penetration)
		/// </param>
		/// <param name="normal">The normal of collision
		/// </param>
		/// <param name="depth">The penetration of of the contact
		/// </param>
		public CollisionEvent(float time, Body body1, Body body2, ROVector2f point, ROVector2f normal, float depth)
		{
			this.time = time;
			this.body1 = body1;
			this.body2 = body2;
			this.point = point;
			this.normal = normal;
			this.depth = depth;
		}
		
		/// <seealso cref="java.lang.Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "[Collision \r\n" + " body A: " + body1 + "\r\n" + " body B: " + body2 + "\r\n" + " contact: " + point + "\r\n" + " normal: " + normal + "\r\n" + " penetration: " + depth + "\r\n";
		}
	}
}