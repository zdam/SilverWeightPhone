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
using System.Collections.Generic;
namespace Silver.Weight.Raw
{
	
	/// <summary> A space that will Resolve collisions and report them to registered 
	/// listeners.
	/// 
	/// </summary>
	public class CollisionSpace : CollisionContext
	{
		/// <summary> Retrieve a immutable list of bodies in the simulation
		/// 
		/// </summary>
		/// <returns> The list of bodies
		/// </returns>
		virtual public BodyList Bodies
		{
			get
			{
				return bodies;
			}
			
		}
		/// <summary> Set the strategy used to determine the bodies for collision in the
		/// broad phase.
		/// 
		/// </summary>
		/// <param name="strategy">The strategy used to determine which bodies to check detailed
		/// collision on
		/// </param>
		virtual public BroadCollisionStrategy CollisionStrategy
		{
			set
			{
				this.collisionStrategy = value;
			}
			
		}
		/// <summary> The bitmask used to determine which
		/// bits are allowed to Collide.
		/// </summary>
		virtual public long Bitmask
		{
			get
			{
				return bitmask;
			}
			
			set
			{
				this.bitmask = value;
			}
			
		}
		/// <summary>The bodies contained in the world </summary>
		protected internal BodyList bodies = new BodyList();
		/// <summary>The arbiters that have been required in the world </summary>
		protected internal ArbiterList arbiters = new ArbiterList();
		/// <summary>The broad phase collision strategy we're using </summary>
		protected internal BroadCollisionStrategy collisionStrategy;
		/// <summary>The list of listeners that should be notified of collisions </summary>
		protected internal IList<CollisionListener> listeners = new List<CollisionListener>();
		/// <summary>The total time passed </summary>
		protected internal float totalTime;

		/// <summary>The bitmask that determine which bits are used for collision detection </summary>
		//private long bitmask = 0xFFFFFFFFFFFFFFFFL;
		private long bitmask = long.MaxValue;
		
		/// <summary> Create a new collision space based on a given strategy for 
		/// partioning the space
		/// 
		/// </summary>
		/// <param name="strategy">The strategy to use to partion the collision space
		/// </param>
		public CollisionSpace(BroadCollisionStrategy strategy)
		{
			this.collisionStrategy = strategy;
		}
		
		/// <summary> Add a listener to be notified of collisions
		/// 
		/// </summary>
		/// <param name="listener">The listener to be notified of collisions
		/// </param>
		public virtual void  AddListener(CollisionListener listener)
		{
			listeners.Add(listener);
		}
		
		/// <summary> Remove a listener from the space
		/// 
		/// </summary>
		/// <param name="listener">The listener to be removed
		/// </param>
		public virtual void  RemoveListener(CollisionListener listener)
		{
			listeners.Remove(listener);
		}
		
		/// <summary> Cause collision to occur and be reported.
		/// 
		/// </summary>
		/// <param name="dt">The amount of time since last collision. This may be used
		/// for swept collision in some future implementation
		/// </param>
		public virtual void  Collide(float dt)
		{
			totalTime += dt;
			collisionStrategy.CollideBodies(this, bodies, dt);
		}
		
		/// <summary> Remove all the elements from this space</summary>
		public virtual void  Clear()
		{
			bodies.Clear();
			arbiters.Clear();
		}
		
		/// <summary> Add a body to the simulation
		/// 
		/// </summary>
		/// <param name="body">The body to be added
		/// </param>
		public virtual void  Add(Body body)
		{
			bodies.Add(body);
		}
		
		/// <summary> Remove a body from the simulation
		/// 
		/// </summary>
		/// <param name="body">The body to be removed
		/// </param>
		public virtual void  Remove(Body body)
		{
			bodies.Remove(body);
		}
		
		/// <summary> Notify listeners of a collision
		/// 
		/// </summary>
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
		private void  NotifyCollision(Body body1, Body body2, ROVector2f point, ROVector2f normal, float depth)
		{
			if (listeners.Count == 0)
			{
				return ;
			}
			
			CollisionEvent newEvent = new CollisionEvent(totalTime, body1, body2, point, normal, depth);
			
			for (int i = 0; i < listeners.Count; i++)
			{
				((CollisionListener) listeners[i]).CollisionOccured(newEvent);
			}
		}
		
		/// <seealso cref="Silver.Weight.Raw.CollisionContext.Resolve(Silver.Weight.Raw.BodyList, float)">
		/// </seealso>
		public virtual void  Resolve(BodyList bodyList, float dt)
		{
			for (int i = 0; i < bodyList.Size(); ++i)
			{
				Body bi = bodyList.Item(i);
				
				for (int j = i + 1; j < bodyList.Size(); ++j)
				{
					Body bj = bodyList.Item(j);
					if ((bitmask & bi.Bitmask & bj.Bitmask) == 0)
					{
						continue;
					}
					if (bi.ExcludedList.Contains(bj))
					{
						continue;
					}
					if (bi.InvMass == 0.0f && bj.InvMass == 0.0f)
					{
						continue;
					}
					if (!bi.Shape.Bounds.Touches(bi.GetPosition().X, bi.GetPosition().Y, bj.Shape.Bounds, bj.GetPosition().X, bj.GetPosition().Y))
					{
						
						arbiters.remove(new Arbiter(bi, bj));
						continue;
					}
					
					Arbiter newArb = new Arbiter(bi, bj);
					newArb.Collide(dt);
					
					if (newArb.NumContacts > 0)
					{
						bi.Collided(bj);
						bj.Collided(bi);
						
						if (arbiters.Contains(newArb))
						{
							int index = arbiters.indexOf(newArb);
							Arbiter arb = arbiters.Item(index);
							arb.Update(newArb.Contacts, newArb.NumContacts);
						}
						else
						{
							Contact c = newArb.GetContact(0);
							
							NotifyCollision(bi, bj, c.Position, c.Normal, c.Separation);
							arbiters.add(newArb);
							newArb.Init();
						}
					}
					else
					{
						arbiters.remove(newArb);
					}
				}
			}
		}
		
		/// <summary> Set one or more individual bits of
		/// the bitmask used to determine which
		/// bits are allowed to Collide.
		/// 
		/// </summary>
		/// <param name="bitmask">A bitmask with the bits
		/// that will be switched on.
		/// </param>
		public virtual void  AddBit(long bitmask)
		{
			this.bitmask = this.bitmask | bitmask;
		}
		
		/// <summary> Remove one or more individual bits of
		/// the bitmask used to determine which
		/// bits are allowed to Collide.
		/// 
		/// </summary>
		/// <param name="bitmask">A bitmask with the bits
		/// that will be switched off.
		/// </param>
		public virtual void  RemoveBit(long bitmask)
		{
			this.bitmask -= (bitmask & this.bitmask);
		}
	}
}