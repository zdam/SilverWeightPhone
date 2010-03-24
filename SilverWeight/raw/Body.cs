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
using Vector2f = Silver.Weight.Math.Vector2f;
using DynamicShape = Silver.Weight.Raw.Shapes.DynamicShape;
using Shape = Silver.Weight.Raw.Shapes.Shape;
using System.Collections.Generic;
namespace Silver.Weight.Raw
{
	
	/// <summary> A single body within the physics model
	/// 
	/// </summary>
	public class Body
	{
		/// <summary> Attached object which initially
		/// will be null.
		/// </summary>
		virtual public System.Object UserData
		{
			get
			{
				return userData;
			}
			
			set
			{
				userData = value;
			}
			
		}
		/// <summary> Indicate whether this body can come to a resting state. The default is true, but it
		/// is sometimes useful for dynamic objects to prevent them resting.
		/// 
		/// </summary>
		virtual public bool CanRest
		{
			set
			{
				this.canRest = value;
			}
			get { return this.canRest; }			
		}
		/// <summary> Get the list of bodies that this body is touching. Only works if 
		/// resting body detection is turned on
		/// 
		/// </summary>
		virtual public BodyList Touching
		{
			get
			{
				return new BodyList(touching);
			}
			
		}
		/// <summary> Check if this body is static
		/// 
		/// </summary>
		/// <returns> True if this body is static
		/// </returns>
		virtual public bool Static
		{
			get
			{
				return false;
			}
			
		}
		/// <summary> Check if this body has been detected as resting and hence is considered
		/// static.
		/// 
		/// </summary>
		/// <returns> True if the body is considered static since it's resting
		/// </returns>
		virtual public bool Resting
		{
			get
			{
				return isResting;
			}
			
		}
		/// <summary> Force this body into resting or not resting mode.
		/// 
		/// </summary>
		/// <param name="isResting">True if this body should be seen as resting
		/// </param>
		virtual public bool IsResting
		{
			set
			{
				if (this.isResting && !value)
				{
					SetMass(originalMass);
				}
				this.touchingStatic = false;
				this.isResting = value;
			}
			
		}
		/// <summary> Effected by gravity
		/// </summary>
		/// <returns> True if this body is effected by gravity
		/// </returns>
		virtual public bool GravityEffected
		{
			get
			{
				return (gravity) || (I == INFINITE_MASS);
			}
			
			set
			{
				this.gravity = value;
			}
			
		}
		/// <summary> Get the list of bodies that can not Collide with this body
		/// 
		/// </summary>
		/// <returns> The list of bodies that can not Collide with this body
		/// </returns>
		virtual public BodyList ExcludedList
		{
			get
			{
				return excluded;
			}
			
		}
		/// <summary> The "restitution" of the body. Hard bodies transfer
		/// momentum well. A value of 1.0 would be a pool ball. The 
		/// default is 0f
		/// </summary>
		virtual public float Restitution
		{
			get
			{
				return restitution;
			}
			
			set
			{
				this.restitution = value;
			}
			
		}
		/// <summary> The friction on the surface of this body
		/// </summary>
		virtual public float Friction
		{
			get
			{
				return surfaceFriction;
			}
			
			set
			{
				this.surfaceFriction = value;
			}
			
		}
		/// <summary> The rotation in radians of this body
		/// </summary>
		virtual public float Rotation
		{
			get
			{
				return rotation;
			}
			
			set
			{
				this.rotation = value;
			}
			
		}
		/// <summary> The friction of the 'air' on this body
		/// that slows down the object.
		/// </summary>
		/// TODO: check how this works together with the gravity
		/// The friction force F will be
		/// F = -v * damping / m
		virtual public float Damping
		{
			get
			{
				return this.damping;
			}
			
			set
			{
				this.damping = value;
			}
			
		}
		/// <summary> The rotational damping, similar to normal
		/// damping.
		///  Ssimilar to normal
		/// damping. The torque F for this damping would be:
		/// F = -av * damping / m
		/// where av is the angular velocity. 
		/// </summary>
		virtual public float RotDamping
		{
			get
			{
				return this.rotDamping;
			}
			
			set
			{
				this.rotDamping = value;
			}
			
		}
		/// <summary> Get the shape representing this body
		/// 
		/// </summary>
		virtual public Shape Shape
		{
			get
			{
				return shape;
			}
			
		}
		/// <summary> Get the position of this body before it was moved
		/// 
		/// </summary>
		virtual public ROVector2f LastPosition
		{
			get
			{
				return lastPosition;
			}
			
		}
		/// <summary> Get the velocity before the last Update. This is useful
		/// during collisions to determine the change in velocity on impact
		/// 
		/// </summary>
		virtual public ROVector2f LastVelocity
		{
			get
			{
				return lastVelocity;
			}
			
		}
		/// <summary> Get the angular velocity before the last Update. This is useful
		/// during collisions to determine the change in angular velocity on impact
		/// 
		/// </summary>
		virtual public float LastAngularVelocity
		{
			get
			{
				return lastAngularVelocity;
			}
			
		}
		/// <summary> Get the inverse density of this body
		/// 
		/// </summary>
		virtual internal float InvI
		{
			get
			{
				return invI;
			}
			
		}
		/// <summary> The torque applied to this body
		/// </summary>
		virtual public float Torque
		{
			get
			{
				return torque;
			}
			
			set
			{
				torque = value;
			}
			
		}
		/// <summary> Get the velocity of this body
		/// 
		/// </summary>
		virtual public ROVector2f Velocity
		{
			get
			{
				return velocity;
			}
			
		}
		/// <summary> Get the angular velocity of this body
		/// 
		/// </summary>
		virtual public float AngularVelocity
		{
			get
			{
				return angularVelocity;
			}
			
		}
		/// <summary> Get the inverse mass of this body
		/// 
		/// </summary>
		virtual internal float InvMass
		{
			get
			{
				return invMass;
			}
			
		}
		/// <summary> Get the bias velocity of this body
		/// 
		/// </summary>
		virtual public ROVector2f BiasedVelocity
		{
			get
			{
				return biasedVelocity;
			}
			
		}
		/// <summary> Get the bias angular velocity of this body
		/// 
		/// </summary>
		virtual public float BiasedAngularVelocity
		{
			get
			{
				return biasedAngularVelocity;
			}
			
		}
		/// <summary> Get the energy contained in this body
		/// 
		/// </summary>
		virtual public float Energy
		{
			get
			{
				float velEnergy = Mass * Velocity.Dot(Velocity);
				float angEnergy = Inertia * (AngularVelocity * AngularVelocity);
				
				return velEnergy + angEnergy;
			}
			
		}
		/// <summary> This shape's bitmask
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
		/// <summary>The Next ID to be assigned </summary>
		private static int NEXT_ID = 0;
		/// <summary>The maximum value indicating that body won't Move </summary>
		public static readonly float INFINITE_MASS = System.Single.MaxValue;
		
		/// <summary>The current position of this body </summary>
		private Vector2f position = new Vector2f();
		/// <summary>The position of this body in the last frame </summary>
		private Vector2f lastPosition = new Vector2f();
		/// <summary>The current rotation of this body in radians </summary>
		private float rotation;
		
		/// <summary>The velocity of this body </summary>
		private Vector2f velocity = new Vector2f();
		/// <summary>The angular velocity of this body </summary>
		private float angularVelocity;
		/// <summary>The last velocity of this body (before last Update) </summary>
		private Vector2f lastVelocity = new Vector2f();
		/// <summary>The last angular velocity of this body (before last Update) </summary>
		private float lastAngularVelocity;
		
		/// <summary>The velocity of this body </summary>
		private Vector2f biasedVelocity = new Vector2f();
		/// <summary>The angular velocity of this body </summary>
		private float biasedAngularVelocity;
		
		/// <summary>The force being applied to this body - i.e. driving velocity </summary>
		private Vector2f force = new Vector2f();
		/// <summary>The angular force being applied this body - i.e. driving angular velocity </summary>
		private float torque;
		
		/// <summary>The shape representing this body </summary>
		private Shape shape;
		
		/// <summary>The friction on the surface of this body </summary>
		private float surfaceFriction;
		/// <summary>The damping caused by friction of the 'air' on this body. </summary>
		private float damping;
		/// <summary>The rotational damping. </summary>
		private float rotDamping;
		/// <summary>The mass of this body </summary>
		private float mass;
		/// <summary>The inverse mass of this body </summary>
		private float invMass;
		/// <summary>The density of this body </summary>
		private float I;
		/// <summary>The inverse of this density </summary>
		private float invI;
		/// <summary>The name assigned to this body </summary>
		private System.String name;
		/// <summary>The id assigned ot this body </summary>
		private int id;
		/// <summary>The restitution of this body </summary>
		private float restitution = 0f;
		/// <summary>The list of bodies excluded from colliding with this body </summary>
		private BodyList excluded = new BodyList();
		/// <summary>True if this body is effected by gravity </summary>
		private bool gravity = true;
		
		/// <summary>The collision group bitmask </summary>
		//private long bitmask = 0xFFFFFFFFFFFFFFFFL;
		private long bitmask = long.MaxValue;
		/// <summary>A hook for the library's user's data </summary>
		private System.Object userData = null;
		/// <summary>The old position </summary>
		private Vector2f oldPosition;
		/// <summary>The new position </summary>
		private Vector2f newPosition;
		/// <summary>True if we've been hit by another this frame </summary>
		private bool hitByAnother;
		/// <summary>True if we're considered static at the moment </summary>
		private bool isResting;
		/// <summary>The original mass of this object </summary>
		private float originalMass;
		/// <summary>The number of hits this frame </summary>
		private int hitCount = 0;
		/// <summary>True if resting body detection is turned on </summary>
		private bool restingBodyDetection = false;
		/// <summary>The velocity a body hitting a resting body has to have to consider moving it </summary>
		private float hitTolerance;
		/// <summary>The amount a body has to rotate for it to be considered non-resting </summary>
		private float rotationTolerance;
		/// <summary>The amoutn a body has to Move for it to be considered non-resting </summary>
		private float positionTolerance;
		/// <summary>The list of bodies this body Touches </summary>
		private BodyList touching = new BodyList();
		/// <summary>True if this body is touching a static </summary>
		private bool touchingStatic = false;
		/// <summary>Number of bodies we're touching </summary>
		private int touchingCount;
		/// <summary>True if this body is capable of coming to a resting state </summary>
		private bool canRest = true;
		
		/// <summary>True if this body can rotate </summary>
		private bool rotatable = true;
		/// <summary>True if this body can Move </summary>
		private bool moveable = true;
		
		/// <summary> Create a new un-named body
		/// 
		/// </summary>
		/// <param name="shape">The shape describing this body
		/// </param>
		/// <param name="m">The mass of the body
		/// </param>
		public Body(DynamicShape shape, float m):this("UnnamedBody", (Shape) shape, m)
		{
		}
		
		/// <summary> Create a new un-named body
		/// 
		/// </summary>
		/// <param name="shape">The shape describing this body
		/// </param>
		/// <param name="m">The mass of the body
		/// </param>
		protected internal Body(Shape shape, float m):this("UnnamedBody", shape, m)
		{
		}
		
		/// <summary> Create a named body
		/// 
		/// </summary>
		/// <param name="name">The name to assign to the body
		/// </param>
		/// <param name="shape">The shape describing this body
		/// </param>
		/// <param name="m">The mass of the body
		/// </param>
		public Body(System.String name, DynamicShape shape, float m):this(name, (Shape) shape, m)
		{
		}
		
		/// <summary> Create a named body
		/// 
		/// </summary>
		/// <param name="name">The name to assign to the body
		/// </param>
		/// <param name="shape">The shape describing this body
		/// </param>
		/// <param name="m">The mass of the body
		/// </param>
		protected internal Body(System.String name, Shape shape, float m)
		{
			this.name = name;
			
			id = NEXT_ID++;
			position.Reconfigure(0.0f, 0.0f);
			lastPosition.Reconfigure(0.0f, 0.0f);
			rotation = 0.0f;
			velocity.Reconfigure(0.0f, 0.0f);
			angularVelocity = 0.0f;
			force.Reconfigure(0.0f, 0.0f);
			torque = 0.0f;
			surfaceFriction = 0.2f;
			
			//Size.set(1.0f, 1.0f);
			mass = INFINITE_MASS;
			invMass = 0.0f;
			I = INFINITE_MASS;
			invI = 0.0f;
			
			originalMass = m;
			Reconfigure(shape, m);
		}
		
		/// <summary> Check if this body can rotate.
		/// 
		/// </summary>
		/// <returns> True if this body can rotate
		/// </returns>
		public virtual bool IsRotatable()
		{
			return rotatable;
		}
		
		/// <summary> Check if this body can Move
		/// 
		/// </summary>
		/// <returns> True if this body can Move
		/// </returns>
		public virtual bool IsMoveable()
		{
			return moveable;
		}
		
		/// <summary> Indicate whether this body should be able to rotate. Use this feature at
		/// you own risk - other bodies will react as tho this body has rotated when
		/// hit so there may be artefacts involved with it's use.
		/// 
		/// Note also that this only prevents rotations caused by physics model updates. It
		/// is still possible to explicitly set the rotation of the body with 
		/// #setRotation()
		/// 
		/// The default value is true
		/// 
		/// </summary>
		/// <param name="rotatable">True if this body is rotatable
		/// </param>
		public virtual void  SetRotatable(bool rotatable)
		{
			this.rotatable = rotatable;
		}
		
		/// <summary> Indicate whether this body should be able to moe. Use this feature at
		/// you own risk - other bodies will react as tho this body has moved when
		/// hit so there may be artefacts involved with it's use.
		/// 
		/// Note also that this only prevents movement caused by physics model updates. It
		/// is still possible to explicitly set the position of the body with 
		/// #SetPosition()
		/// 
		/// The default value is true
		/// 
		/// </summary>
		/// <param name="moveable">True if this body is rotatable
		/// </param>
		public virtual void  SetMoveable(bool moveable)
		{
			this.moveable = moveable;
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
		internal virtual void  configureRestingBodyDetection(float hitTolerance, float rotationTolerance, float positionTolerance)
		{
			this.hitTolerance = hitTolerance;
			this.rotationTolerance = rotationTolerance;
			this.positionTolerance = positionTolerance;
			restingBodyDetection = true;
		}
		

		/// <summary> Notification that we've started an Update frame/iteration</summary>
		internal virtual void  StartFrame()
		{
			if (!CanRest)
			{
				return ;
			}
			
			oldPosition = new Vector2f(GetPosition());
			hitByAnother = false;
			hitCount = 0;
			touching.Clear();
		}
		
		/// <summary> Notification that this body Collided with another
		/// 
		/// </summary>
		/// <param name="other">The other body that this body Collided with
		/// </param>
		public virtual void  Collided(Body other)
		{
			if (!restingBodyDetection)
			{
				return ;
			}
			
			if (!touching.Contains(other))
			{
				touching.Add(other);
			}
			
			if (Resting)
			{
				if ((!other.Resting))
				{
					if (other.Velocity.LengthSquared() > hitTolerance)
					{
						hitByAnother = true;
						SetMass(originalMass);
					}
				}
			}
			hitCount++;
		}
		
		/// <summary> Notification that we've ended an Update frame/iteration</summary>
		public virtual void  EndFrame()
		{
			if (!CanRest)
			{
				return ;
			}
			
			if ((hitCount == 0) || (touchingCount != touching.Size()))
			{
				isResting = false;
				SetMass(originalMass);
				touchingStatic = false;
				touchingCount = touching.Size();
			}
			else
			{
				newPosition = new Vector2f(GetPosition());
				if (!hitByAnother)
				{
					if (true && (newPosition.DistanceSquared(oldPosition) <= positionTolerance) && (velocity.LengthSquared() <= 0.001f) && (biasedVelocity.LengthSquared() <= 0.001f) && (System.Math.Abs(angularVelocity) <= rotationTolerance))
					{
						if (!touchingStatic)
						{
							touchingStatic = IsTouchingStatic(new List<Body>());
						}
						if (touchingStatic)
						{
							isResting = true;
							SetMass(INFINITE_MASS);
							velocity.Reconfigure(0.0f, 0.0f);
							biasedVelocity.Reconfigure(0, 0);
							angularVelocity = 0.0f;
							biasedAngularVelocity = 0;
							force.Reconfigure(0.0f, 0.0f);
							torque = 0.0f;
						}
					}
				}
				else
				{
					isResting = false;
					SetMass(originalMass);
				}
				
				if ((newPosition.DistanceSquared(oldPosition) > positionTolerance) && (System.Math.Abs(angularVelocity) > rotationTolerance))
				{
					touchingStatic = false;
				}
			}
		}
		
		/// <summary> Check if this body is touching a static body directly or indirectly
		/// 
		/// </summary>
		/// <param name="path">The path of bodies we've used to get here
		/// </param>
		/// <returns> True if we're touching a static body
		/// </returns>
		public virtual bool IsTouchingStatic(IList<Body> path)
		{
			bool result = false;
			
			path.Add(this);
			for (int i = 0; i < touching.Size(); i++)
			{
				Body body = touching.Item(i);
				if (path.Contains(body))
				{
					continue;
				}
				if (body.Static)
				{
					result = true;
					break;
				}
				
				if (body.IsTouchingStatic(path))
				{
					result = true;
					break;
				}
			}
			
			return result;
		}
		
		/// <summary> Get the list of bodies that this body is connected to. Only works if 
		/// resting body detection is turned on
		/// 
		/// </summary>
		/// <returns> The list of bodies this body Touches
		/// </returns>
		public virtual BodyList GetConnected()
		{
			return GetConnected(false);
		}
		
		/// <summary> Get the list of bodies that this body is connected to. Only works if 
		/// resting body detection is turned on
		/// 
		/// </summary>
		/// <param name="stopAtStatic">True if we should stop traversing and looking for elements one you find a static one
		/// </param>
		/// <returns> The list of bodies this body Touches
		/// </returns>
		public virtual BodyList GetConnected(bool stopAtStatic)
		{
			BodyList connected = new BodyList();
			GetConnected(connected, new List<Body>(), stopAtStatic);
			
			return connected;
		}
		/// <summary> Get the bodies connected to this one
		/// 
		/// </summary>
		/// <param name="stopAtStatic">True if we should stop traversing and looking for elements one you find a static one
		/// </param>
		/// <param name="list">The list we're building up
		/// </param>
		/// <param name="path">The list of elements we passed to get here
		/// </param>
		private void  GetConnected(BodyList list, IList<Body> path, bool stopAtStatic)
		{
			path.Add(this);
			for (int i = 0; i < touching.Size(); i++)
			{
				Body body = touching.Item(i);
				if (path.Contains(body))
				{
					continue;
				}
				if (body.Static && stopAtStatic)
				{
					continue;
				}
				
				list.Add(body);
				body.GetConnected(list, path, stopAtStatic);
			}
		}
		
		/// <summary> Add a body that this body is not allowed to Collide with, i.e.
		/// the body specified will Collide with this body
		/// 
		/// </summary>
		/// <param name="other">The body to exclude from collisions with this body
		/// </param>
		public virtual void  AddExcludedBody(Body other)
		{
			if (other.Equals(this))
			{
				return ;
			}
			if (!excluded.Contains(other))
			{
				excluded.Add(other);
				other.AddExcludedBody(this);
			}
		}
		
		/// <summary> Remove a body from the excluded list of this body. i.e. the
		/// body specified will be allowed to Collide with this body again
		/// 
		/// </summary>
		/// <param name="other">The body to Remove from the exclusion list
		/// </param>
		public virtual void  RemoveExcludedBody(Body other)
		{
			if (other.Equals(this))
			{
				return ;
			}
			if (excluded.Contains(other))
			{
				excluded.Remove(other);
				other.RemoveExcludedBody(this);
			}
		}
		
		/// <summary> Get the mass of this body
		/// 
		/// </summary>
		/// <returns> The mass of this body
		/// </returns>
		public virtual float Mass
		{
			get
			{
				return mass;
			}
		}
		
		/// <summary> Get the inertia of this body
		/// 
		/// </summary>
		/// <returns> The inertia of this body 
		/// </returns>
		public virtual float Inertia
		{
			get
			{
				return I;
			}
		}
		
		/// <summary> Reconfigure the body
		/// 
		/// </summary>
		/// <param name="shape">The shape describing this body
		/// </param>
		/// <param name="m">The mass of the body
		/// </param>
		public virtual void  Reconfigure(Shape shape, float m)
		{
			position.Reconfigure(0.0f, 0.0f);
			lastPosition.Reconfigure(0.0f, 0.0f);
			rotation = 0.0f;
			velocity.Reconfigure(0.0f, 0.0f);
			angularVelocity = 0.0f;
			force.Reconfigure(0.0f, 0.0f);
			torque = 0.0f;
			surfaceFriction = 0.2f;
			
			this.shape = shape;
			SetMass(m);
		}
		
		/// <summary> Set the mass of the body
		/// 
		/// </summary>
		/// <param name="m">The new mass of the body
		/// </param>
		private void  SetMass(float m)
		{
			mass = m;
			
			if (mass < INFINITE_MASS)
			{
				invMass = 1.0f / mass;
				//I = mass * (Size.x * Size.x + Size.y * Size.y) / 12.0f;
				
				I = (mass * shape.SurfaceFactor) / 12.0f;
				invI = 1.0f / I;
			}
			else
			{
				invMass = 0.0f;
				I = INFINITE_MASS;
				invI = 0.0f;
			}
		}
		
		/// <summary> Set the position of this body, this will also set the previous position
		/// to the same value.
		/// 
		/// </summary>
		/// <param name="x">The x position of this body
		/// </param>
		/// <param name="y">The y position of this body
		/// </param>
		public virtual void  SetPosition(float x, float y)
		{
			position.Reconfigure(x, y);
			lastPosition.Reconfigure(x, y);
		}
		
		/// <summary> Set the position of this body after it has moved
		/// this means the last position will contain the position
		/// before this function was called.
		/// 
		/// </summary>
		/// <param name="x">The x position of this body
		/// </param>
		/// <param name="y">The y position of this body
		/// </param>
		public virtual void  Move(float x, float y)
		{
			lastPosition.Reconfigure(position);
			position.Reconfigure(x, y);
		}
		
		/// <summary> Get the position of this body
		/// 
		/// </summary>
		/// <returns> The position of this body
		/// </returns>
		public virtual ROVector2f GetPosition()
		{
			return position;
		}
		
		/// <summary> Adjust the position of this body.
		/// The previous position will be set to the position before
		/// this function was called.
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the position by
		/// </param>
		/// <param name="Scale">The amount to Scale the delta by
		/// </param>
		public virtual void  AdjustPosition(ROVector2f delta, float scale)
		{
			lastPosition.Reconfigure(position);
			position.x += delta.X * scale;
			position.y += delta.Y * scale;
		}
		
		/// <summary> Adjust the position of this body
		/// The previous position will be set to the position before
		/// this function was called.
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the position by
		/// </param>
		public virtual void  AdjustPosition(Vector2f delta)
		{
			lastPosition.Reconfigure(position);
			position.Add(delta);
		}
		
		/// <summary> Adjust the rotation of this body
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the rotation by
		/// </param>
		public virtual void  AdjustRotation(float delta)
		{
			rotation += delta;
		}
		
		/// <summary> Set the force being applied to this body
		/// 
		/// </summary>
		/// <param name="x">The x component of the force
		/// </param>
		/// <param name="y">The y component of the force
		/// </param>
		public virtual void  SetForce(float x, float y)
		{
			force.Reconfigure(x, y);
		}
		
		/// <summary> Adjust the velocity of this body
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the velocity by
		/// </param>
		public virtual void  AdjustVelocity(Vector2f delta)
		{
			if (!IsMoveable())
			{
				return ;
			}
			lastVelocity.Reconfigure(velocity);
			velocity.Add(delta);
		}
		
		/// <summary> Adjust the angular velocity of this body
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the velocity by
		/// </param>
		public virtual void  AdjustAngularVelocity(float delta)
		{
			if (!IsRotatable())
			{
				return ;
			}
			lastAngularVelocity = angularVelocity;
			angularVelocity += delta;
		}
		
		/// <summary> Get the force applied to this body
		/// 
		/// </summary>
		/// <returns> The force applied to this body
		/// </returns>
		public virtual ROVector2f GetForce()
		{
			return force;
		}
		
		/// <summary> Add a force to this body
		/// 
		/// </summary>
		/// <param name="f">The force to be applied
		/// </param>
		public virtual void  AddForce(Vector2f f)
		{
			force.Add(f);
		}
		
		/// <seealso cref="java.lang.Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "[Body '" + name + "' id: " + id + " pos: " + position + " vel: " + velocity + " (" + angularVelocity + ")]";
		}
		
		/// <seealso cref="java.lang.Object.hashCode()">
		/// </seealso>
		public override int GetHashCode()
		{
			return id;
		}
		
		/// <seealso cref="java.lang.Object.equals(java.lang.Object)">
		/// </seealso>
		public  override bool Equals(System.Object other)
		{
			if (other.GetType() == GetType())
			{
				return ((Body) other).id == id;
			}
			
			return false;
		}
		
		/// <summary> Adjust the bias velocity of this body
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the velocity by
		/// </param>
		public virtual void  AdjustBiasedVelocity(Vector2f delta)
		{
			if (!IsMoveable())
			{
				return ;
			}
			biasedVelocity.Add(delta);
		}
		
		/// <summary> Adjust the bias angular velocity of this body
		/// 
		/// </summary>
		/// <param name="delta">The amount to change the velocity by
		/// </param>
		public virtual void  AdjustBiasedAngularVelocity(float delta)
		{
			if (!IsRotatable())
			{
				return ;
			}
			biasedAngularVelocity += delta;
		}
		
		/// <summary> Reset the bias velocity (done every time Step)</summary>
		public virtual void  ResetBias()
		{
			biasedVelocity.Reconfigure(0, 0);
			biasedAngularVelocity = 0;
		}
		
		/// <summary> Set one or more individual bits.
		/// 
		/// </summary>
		/// <param name="bitmask">A bitmask with the bits
		/// that will be switched on.
		/// </param>
		public virtual void  AddBit(long bitmask)
		{
			this.bitmask = this.bitmask | bitmask;
		}
		
		/// <summary> Remove one or more individual bits.
		/// The set bits will be removed.
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