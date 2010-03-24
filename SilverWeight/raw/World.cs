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
using Vector2f = Silver.Weight.Math.Vector2f;
using BruteCollisionStrategy = Silver.Weight.Raw.Strategies.BruteCollisionStrategy;
using System.Collections;
using System.Collections.Generic;
namespace Silver.Weight.Raw
{
	
	/// <summary> The physics model in which the bodies exist. The world is "stepped"
	/// which causes the bodies to be moved and resulting forces be resolved.
	/// 
	/// </summary>
	public class World:CollisionSpace
	{
		/// <summary> Set the amount of energy retained during collisions
		/// across the system. This might be used to simulate
		/// sound, heat type losses
		/// 
		/// </summary>
		/// <param name="damping">The amount of energy retain across the system
		/// (1 = no loss, 0 = total loss)
		/// </param>
		virtual public float Damping
		{
			set
			{
				this.damping = value;
			}
			
		}
		/// <summary> Retrieve a immutable list of joints in the simulation
		/// 
		/// </summary>
		virtual public JointList Joints
		{
			get
			{
				return joints;
			}
			
		}
		/// <summary> Retrieve a immutable list of arbiters in the simulation
		/// 
		/// </summary>
		virtual public ArbiterList Arbiters
		{
			get
			{
				return arbiters;
			}
			
		}
		/// <summary> Get the list of bodies that should be considered active at this time. Sub-classes
		/// can override to incur spatial culling
		/// 
		/// </summary>
		virtual protected internal BodyList ActiveBodies
		{
			get
			{
				return bodies;
			}
			
		}
		/// <summary> Get the list of joints that should be considered active at this time. Sub-classes
		/// can override to incur spatial culling
		/// 
		/// </summary>
		virtual protected internal JointList ActiveJoints
		{
			get
			{
				return joints;
			}
			
		}
		/// <summary> Get the total energy in the system
		/// 
		/// </summary>
		virtual public float TotalEnergy
		{
			get
			{
				float total = 0;
				
				for (int i = 0; i < bodies.Size(); i++)
				{
					total += bodies.Item(i).Energy;
				}
				
				return total;
			}
			
		}
		/// <summary>The joints contained in the world </summary>
		private JointList joints = new JointList();
		/// <summary>The direction and force of gravity </summary>
		private Vector2f gravity = new Vector2f(0, 0);
		/// <summary>The number of iteration to run each Step </summary>
		private int iterations;
		/// <summary>The damping in effect in the system </summary>
		private float damping = 1;
		/// <summary>True if resting body detection is turned on </summary>
		private bool restingBodyDetection = false;
		/// <summary>The velocity a body hitting a resting body has to have to consider moving it </summary>
		private float hitTolerance;
		/// <summary>The amount a body has to rotate for it to be considered non-resting </summary>
		private float rotationTolerance;
		/// <summary>The amoutn a body has to Move for it to be considered non-resting </summary>
		private float positionTolerance;
		
		/// <summary> Create a new physics model World
		/// 
		/// </summary>
		/// <param name="gravity">The direction and force of gravity
		/// </param>
		/// <param name="iterations">The number of iterations to run each Step. More iteration
		/// is more accurate but slower
		/// </param>
		public World(Vector2f gravity, int iterations):this(gravity, iterations, new BruteCollisionStrategy())
		{
		}
		
		/// <summary> Create a new physics model World
		/// 
		/// </summary>
		/// <param name="gravity">The direction and force of gravity
		/// </param>
		/// <param name="iterations">The number of iterations to run each Step. More iteration
		/// is more accurate but slower
		/// </param>
		/// <param name="strategy">The strategy used to determine which bodies to check detailed
		/// collision on
		/// </param>
		public World(Vector2f gravity, int iterations, BroadCollisionStrategy strategy):base(strategy)
		{
			this.gravity = gravity;
			this.iterations = iterations;
		}
		
		/// <summary> Enable resting body detection.
		/// 
		/// </summary>
		/// <param name="hitTolerance">The velocity a body hitting a resting body has to have to consider moving it
		/// </param>
		/// <param name="rotationTolerance">The amount a body has to rotate for it to be considered non-resting
		/// </param>
		/// <param name="positionTolerance">The amoutn a body has to Move for it to be considered non-resting
		/// </param>
		public virtual void  EnableRestingBodyDetection(float hitTolerance, float rotationTolerance, float positionTolerance)
		{
			this.hitTolerance = hitTolerance;
			this.rotationTolerance = rotationTolerance;
			this.positionTolerance = positionTolerance;
			restingBodyDetection = true;
		}
		
		/// <summary> Disable resting body detection on the world</summary>
		public virtual void  DisableRestingBodyDetection()
		{
			restingBodyDetection = false;
		}
		
		/// <summary> Reset all dynamic bodies to indicate they are no longer resting. Useful when manually
		/// changing the state of the world and then expecting normal results
		/// </summary>
		public virtual void  ClearRestingState()
		{
			for (int i = 0; i < bodies.Size(); i++)
			{
				bodies.Item(i).IsResting = false;
			}
		}
		
		/// <summary> Set the gravity applied in the world
		/// 
		/// </summary>
		/// <param name="x">The x component of the gravity factor
		/// </param>
		/// <param name="y">The y component of the gravity factor
		/// </param>
		public virtual void  SetGravity(float x, float y)
		{
			gravity.x = x;
			gravity.y = y;
		}
		
		/// <summary> Clear any arbiters in place for the given body
		/// 
		/// </summary>
		/// <param name="b">The body whose arbiters should be removed
		/// </param>
		public virtual void  ClearArbiters(Body b)
		{
			for (int i = 0; i < arbiters.size(); i++)
			{
				if (arbiters.Item(i).Concerns(b))
				{
					arbiters.remove(arbiters.Item(i));
					i--;
				}
			}
		}
		
		/// <summary> Add a joint to the simulation
		/// 
		/// </summary>
		/// <param name="joint">The joint to be added 
		/// </param>
		public virtual void  Add(Joint joint)
		{
			joints.Add(joint);
		}
		
		/// <summary> Remove a joint from the simulation
		/// 
		/// </summary>
		/// <param name="joint">The joint to be removed
		/// </param>
		public virtual void  Remove(Joint joint)
		{
			joints.Remove(joint);
		}
		
		/// <summary> Remove all the elements from this world</summary>
		public override void  Clear()
		{
			base.Clear();
			
			joints.Clear();
		}
		
		/// <summary> Step the simulation. Currently anything other than 1/60f as a 
		/// Step leads to unpredictable results - hence the default Step 
		/// fixes us to this Step
		/// </summary>
		public virtual void  Step()
		{
			Step(1 / 60.0f);
		}
		
		/// <summary> Step the simulation. Currently anything other than 1/60f as a 
		/// Step leads to unpredictable results - hence the default Step 
		/// fixes us to this Step
		/// 
		/// </summary>
		/// <param name="dt">The amount of time to Step
		/// </param>
		public virtual void  Step(float dt)
		{
			BodyList bodies = ActiveBodies;
			JointList joints = ActiveJoints;
			
			float invDT = dt > 0.0f?1.0f / dt:0.0f;
			
			if (restingBodyDetection)
			{
				for (int i = 0; i < bodies.Size(); ++i)
				{
					Body b = bodies.Item(i);
					b.StartFrame();
				}
				for (int i = 0; i < joints.Size(); ++i)
				{
					Joint j = joints.Item(i);
					j.Body1.IsResting = false;
					j.Body2.IsResting = false;
				}
			}
			
			BroadPhase(dt);
			
			for (int i = 0; i < bodies.Size(); ++i)
			{
				Body b = bodies.Item(i);
				
				if (b.InvMass == 0.0f)
				{
					continue;
				}
				if (b.Resting && restingBodyDetection)
				{
					continue;
				}
				
				Vector2f temp = new Vector2f(b.GetForce());
				temp.Scale(b.InvMass);
				if (b.GravityEffected)
				{
					temp.Add(gravity);
				}
				temp.Scale(dt);
				
				b.AdjustVelocity(temp);
				
				Vector2f damping = new Vector2f(b.Velocity);
				damping.Scale((- b.Damping) * b.InvMass);
				b.AdjustVelocity(damping);
				
				b.AdjustAngularVelocity(dt * b.InvI * b.Torque);
				b.AdjustAngularVelocity((- b.AngularVelocity) * b.InvI * b.RotDamping);
			}
			
			for (int i = 0; i < arbiters.size(); i++)
			{
				Arbiter arb = arbiters.Item(i);
				if (!restingBodyDetection || !arb.HasRestingPair())
				{
					arb.PreStep(invDT, dt, damping);
				}
			}
			
			for (int i = 0; i < joints.Size(); ++i)
			{
				Joint j = joints.Item(i);
				j.PreStep(invDT);
			}
			
			for (int i = 0; i < iterations; ++i)
			{
				for (int k = 0; k < arbiters.size(); k++)
				{
					Arbiter arb = arbiters.Item(k);
					if (!restingBodyDetection || !arb.HasRestingPair())
					{
						arb.ApplyImpulse();
					}
					else
					{
						arb.Body1.Collided(arb.Body2);
						arb.Body2.Collided(arb.Body1);
					}
				}
				
				for (int k = 0; k < joints.Size(); ++k)
				{
					Joint j = joints.Item(k);
					j.ApplyImpulse();
				}
			}
			
			
			for (int i = 0; i < bodies.Size(); ++i)
			{
				Body b = bodies.Item(i);
				
				if (b.InvMass == 0.0f)
				{
					continue;
				}
				if (restingBodyDetection)
				{
					if (b.Resting)
					{
						continue;
					}
				}
				
				b.AdjustPosition(b.Velocity, dt);
				b.AdjustPosition(b.BiasedVelocity, dt);
				
				b.AdjustRotation(dt * b.AngularVelocity);
				b.AdjustRotation(dt * b.BiasedAngularVelocity);
				
				b.ResetBias();
				b.SetForce(0, 0);
				b.Torque = 0;
			}
			
			if (restingBodyDetection)
			{
				for (int i = 0; i < bodies.Size(); ++i)
				{
					Body b = bodies.Item(i);
					b.EndFrame();
				}
			}
		}
		
		/// <summary> The broad collision phase
		/// 
		/// </summary>
		/// <param name="dt">The amount of time passed since last collision phase
		/// </param>
		internal virtual void  BroadPhase(float dt)
		{
			Collide(dt);
		}
		
		/// <summary> Get a list of collisions at the current time for a given body
		/// 
		/// </summary>
		/// <param name="body">The body to check for
		/// </param>
		/// <returns> The list of collision events describing the current contacts
		/// </returns>
		public virtual CollisionEvent[] GetContacts(Body body)
		{
			List<CollisionEvent> collisions = new List<CollisionEvent>();
			
			for (int i = 0; i < arbiters.size(); i++)
			{
				Arbiter arb = arbiters.Item(i);
				
				if (arb.Concerns(body))
				{
					for (int j = 0; j < arb.NumContacts; j++)
					{
						Contact contact = arb.GetContact(j);
						CollisionEvent newEvent = new CollisionEvent(0, arb.Body1, arb.Body2, contact.Position, contact.Normal, contact.Separation);
						
						collisions.Add(newEvent);
					}
				}
			}

			return collisions.ToArray();
		}
		
		/// <seealso cref="Silver.Weight.Raw.CollisionSpace.Add(Silver.Weight.Raw.Body)">
		/// </seealso>
		public override void  Add(Body body)
		{
			body.configureRestingBodyDetection(hitTolerance, rotationTolerance, positionTolerance);
			base.Add(body);
		}
	}
}