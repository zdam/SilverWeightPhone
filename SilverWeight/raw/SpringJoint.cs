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
using Matrix2f = Silver.Weight.Math.Matrix2f;
using ROVector2f = Silver.Weight.Math.ROVector2f;
using Vector2f = Silver.Weight.Math.Vector2f;
namespace Silver.Weight.Raw
{
	
	/// <summary> A joint representing a spring. The spring can have different constants for 
	/// the stretched and compressed states. It also has a maximum and minimum
	/// compression and stretch. If the spring is compressed or stretched beyond
	/// those points, the connected bodies will push or pull each other directly.
	/// 
	/// </summary>
	/// <author>  Gideon Smeding
	/// </author>
	public class SpringJoint : Joint
	{
		/// <summary> Retrieve the anchor for the first body attached
		/// 
		/// </summary>
		/// <returns> The anchor for the first body
		/// </returns>
		virtual public ROVector2f LocalAnchor1
		{
			get
			{
				return localAnchor1;
			}
			
		}

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
		/// <summary>A hook for the library's user's data </summary>
		private System.Object userData = null;

		/// <summary> Retrieve the anchor for the second body attached
		/// 
		/// </summary>
		/// <returns> The anchor for the second body
		/// </returns>
		virtual public ROVector2f LocalAnchor2
		{
			get
			{
				return localAnchor2;
			}
			
		}
		/// <summary> Get the first body attached to this joint
		/// 
		/// </summary>
		/// <returns> The first body attached to this joint
		/// </returns>
		virtual public Body Body1
		{
			get
			{
				return body1;
			}
			
		}
		/// <summary> Get the second body attached to this joint
		/// 
		/// </summary>
		/// <returns> The second body attached to this joint
		/// </returns>
		virtual public Body Body2
		{
			get
			{
				return body2;
			}
			
		}
		/// <summary> This function is disabled for this joint
		/// because this class allows a far more precise
		/// control over the relaxation through the various
		/// spring constants and the spring's Size.
		/// 
		/// </summary>
		/// <param name="relaxation">useless parameter of a useless function
		/// </param>
		virtual public float Relaxation
		{
			set
			{
			}
			
		}
		/// <summary> The spring constant of Hooke's law, when the spring is out of 
		/// the bounds determined by min and maxSpringsize.
		/// This is especially useful when either the compressed or stretched 
		/// spring constants are zero.
		/// 
		/// </summary>
		virtual public float BrokenSpringConst
		{
			get
			{
				return brokenSpringConst;
			}
			
			set
			{
				this.brokenSpringConst = value;
			}
			
		}
		/// <summary> The spring constant of Hooke's law, when the spring is compressed.
		/// 
		/// </summary>
		virtual public float CompressedSpringConst
		{
			get
			{
				return compressedSpringConst;
			}
			
			set
			{
				this.compressedSpringConst = value;
			}
			
		}
		/// <summary> The maximum Size of the spring, if it stretches beyond this Size
		/// the string is considered 'broken'. This means the string will start
		/// acting more or less like a rope (the impulses from the connected bodies
		/// will be transferred directly).
		/// </summary>
		virtual public float MaxSpringSize
		{
			get
			{
				return maxSpringSize;
			}
			
			set
			{
				this.maxSpringSize = value;
				springSize = springSize > value?value:springSize;
			}
			
		}
		/// <summary> The minimum Size of the spring, if it compressed beyond this Size
		/// the string is considered 'broken'. This means the string will start
		/// acting more or less as if the bodies have direct contact at the axes.
		/// </summary>
		virtual public float MinSpringSize
		{
			get
			{
				return minSpringSize;
			}
			
			set
			{
				this.minSpringSize = value;
				springSize = springSize < value?value:springSize;
			}
			
		}
		/// <summary>The spring's Size.
		/// </summary>
		virtual public float SpringSize
		{
			get
			{
				return springSize;
			}
			
			set
			{
				this.springSize = value;
				maxSpringSize = value > maxSpringSize?value:maxSpringSize;
				minSpringSize = value < minSpringSize?value:minSpringSize;
			}
			
		}
		/// <summary> The spring constant of Hooke's law, when the spring is streched.
		/// </summary>
		virtual public float StretchedSpringConst
		{
			get
			{
				return stretchedSpringConst;
			}
			
			set
			{
				this.stretchedSpringConst = value;
			}
			
		}
		/// <summary>The Next ID to be used </summary>
		public static int NEXT_ID = 0;
		
		/// <summary>The first body attached to the joint </summary>
		private Body body1;
		/// <summary>The second body attached to the joint </summary>
		private Body body2;
		
		/// <summary>The local anchor for the first body </summary>
		private Vector2f localAnchor1 = new Vector2f();
		/// <summary>The local anchor for the second body </summary>
		private Vector2f localAnchor2 = new Vector2f();
		
		//	/** Determince the damping caused by compressing or stretching of the spring */ 
		//	private float damping;
		
		/// <summary>The spring constant of Hooke's law, when the spring is streched </summary>
		private float stretchedSpringConst;
		/// <summary>The spring constant of Hooke's law, when the spring is compressed </summary>
		private float compressedSpringConst;
		/// <summary>The spring constant of Hooke's law, when the spring is out of 
		/// the bounds determined by min and maxSpringsize 
		/// </summary>
		private float brokenSpringConst;
		
		/// <summary>Size of the spring </summary>
		private float springSize;
		/// <summary>Maximum Length of a stretched spring, at which point the spring will not stretch any more </summary>
		private float maxSpringSize;
		/// <summary>Minimum Length of a stretched spring, at which point the spring will not compress any more </summary>
		private float minSpringSize;
		
		/// <summary>The ID of this joint </summary>
		private int id;
		
		/// <summary> Create a joint holding two bodies together
		/// 
		/// </summary>
		/// <param name="b1">The first body attached to the joint
		/// </param>
		/// <param name="b2">The second body attached to the joint
		/// </param>
		/// <param name="anchor1">The location of the attachment to the first body, in absolute coordinates.
		/// </param>
		/// <param name="anchor2">The location of the attachment to the second body, in absolute coordinates.
		/// </param>
		public SpringJoint(Body b1, Body b2, ROVector2f anchor1, ROVector2f anchor2)
		{
			id = NEXT_ID++;
			
			stretchedSpringConst = 100;
			compressedSpringConst = 100;
			brokenSpringConst = 100;
			
			Vector2f spring = new Vector2f(anchor1);
			spring.Sub(anchor2);
			springSize = spring.Length();
			minSpringSize = 0;
			maxSpringSize = 2 * springSize;
			
			Reconfigure(b1, b2, anchor1, anchor2);
		}
		
		/// <summary> Reconfigure this joint
		/// 
		/// </summary>
		/// <param name="b1">The first body attached to this joint
		/// </param>
		/// <param name="b2">The second body attached to this joint
		/// </param>
		/// <param name="anchor1">The location of the attachment to the first body, in absolute coordinates.
		/// </param>
		/// <param name="anchor2">The location of the attachment to the second body, in absolute coordinates.
		/// </param>
		public virtual void  Reconfigure(Body b1, Body b2, ROVector2f anchor1, ROVector2f anchor2)
		{
			body1 = b1;
			body2 = b2;
			
			Matrix2f rot1 = new Matrix2f(body1.Rotation);
			Matrix2f rot1T = rot1.Transpose();
			Vector2f a1 = new Vector2f(anchor1);
			a1.Sub(body1.GetPosition());
			localAnchor1 = MathUtil.Mul(rot1T, a1);
			
			Matrix2f rot2 = new Matrix2f(body2.Rotation);
			Matrix2f rot2T = rot2.Transpose();
			Vector2f a2 = new Vector2f(anchor2);
			a2.Sub(body2.GetPosition());
			localAnchor2 = MathUtil.Mul(rot2T, a2);
		}
		
		/// <summary> Precaculate everything and apply initial impulse before the
		/// simulation Step takes place
		/// 
		/// </summary>
		/// <param name="invDT">The amount of time the simulation is being stepped by
		/// </param>
		public virtual void  PreStep(float invDT)
		{
			
			// calculate the spring's vector (pointing from body1 to body2) and Length
			spring = new Vector2f(body2.GetPosition());
			spring.Add(r2);
			spring.Sub(body1.GetPosition());
			spring.Sub(r1);
			springLength = spring.Length();
			
			// the spring vector needs to be normalized for ApplyImpulse as well!
			spring.Normalise();
			
			// calculate the spring's forces
			// note that although theoretically invDT could never be 0
			// but here it can
			float springConst;
			
			if (springLength < minSpringSize || springLength > maxSpringSize)
			{
				// Pre-compute anchors, mass matrix, and bias.
				Matrix2f rot1 = new Matrix2f(body1.Rotation);
				Matrix2f rot2 = new Matrix2f(body2.Rotation);
				
				r1 = MathUtil.Mul(rot1, localAnchor1);
				r2 = MathUtil.Mul(rot2, localAnchor2);
				
				// the mass normal or 'k'
				float rn1 = r1.Dot(spring);
				float rn2 = r2.Dot(spring);
				float kNormal = body1.InvMass + body2.InvMass;
				kNormal += body1.InvI * (r1.Dot(r1) - rn1 * rn1) + body2.InvI * (r2.Dot(r2) - rn2 * rn2);
				massNormal = 1 / kNormal;
				
				
				// The spring is broken so apply force to correct it
				// note that we use biased velocities for this
				float springImpulse = invDT != 0?brokenSpringConst * (springLength - springSize) / invDT:0;
				
				Vector2f impulse = MathUtil.Scale(spring, springImpulse);
				body1.AdjustBiasedVelocity(MathUtil.Scale(impulse, body1.InvMass));
				body1.AdjustBiasedAngularVelocity((body1.InvI * MathUtil.Cross(r1, impulse)));
				
				body2.AdjustBiasedVelocity(MathUtil.Scale(impulse, - body2.InvMass));
				body2.AdjustBiasedAngularVelocity(- (body2.InvI * MathUtil.Cross(r2, impulse)));
				
				isBroken = true;
				return ;
			}
			else if (springLength < springSize)
			{
				springConst = compressedSpringConst;
				isBroken = false;
			}
			else
			{
				// if ( springLength >= springSize )
				springConst = stretchedSpringConst;
				isBroken = false;
			}
			
			float springImpulse2 = invDT != 0?springConst * (springLength - springSize) / invDT:0;
			
			// apply the spring's forces
			Vector2f impulse2 = MathUtil.Scale(spring, springImpulse2);
			body1.AdjustVelocity(MathUtil.Scale(impulse2, body1.InvMass));
			body1.AdjustAngularVelocity((body1.InvI * MathUtil.Cross(r1, impulse2)));
			
			body2.AdjustVelocity(MathUtil.Scale(impulse2, - body2.InvMass));
			body2.AdjustAngularVelocity(- (body2.InvI * MathUtil.Cross(r2, impulse2)));
		}
		
		// The following variables are set by PreStep() to be used in ApplyImpulse()
		
		/// <summary>Current lenght of the spring </summary>
		private float springLength;
		/// <summary>The spring's normalized vector </summary>
		private Vector2f spring;
		/// <summary>The massNormal, normalizes the speed to get the impulse (right?) </summary>
		private float massNormal;
		/// <summary>The rotation of the anchor of the first body </summary>
		private Vector2f r1 = new Vector2f();
		/// <summary>The rotation of the anchor of the second body </summary>
		private Vector2f r2 = new Vector2f();
		/// <summary>True iff the spring is overstretched or overcompressed </summary>
		private bool isBroken;
		
		/// <summary> Apply the impulse caused by the joint to the bodies attached.</summary>
		public virtual void  ApplyImpulse()
		{
			if (isBroken)
			{
				// calculate difference in velocity
				// TODO: share this code with BasicJoint and Arbiter
				Vector2f relativeVelocity = new Vector2f(body2.Velocity);
				relativeVelocity.Add(MathUtil.Cross(body2.AngularVelocity, r2));
				relativeVelocity.Sub(body1.Velocity);
				relativeVelocity.Sub(MathUtil.Cross(body1.AngularVelocity, r1));
				
				// project the relative velocity onto the spring vector and apply the mass normal
				float normalImpulse = massNormal * relativeVelocity.Dot(spring);
				
				//			// TODO: Clamp the accumulated impulse?
				//			float oldNormalImpulse = accumulatedNormalImpulse;
				//			accumulatedNormalImpulse = Math.max(oldNormalImpulse + normalImpulse, 0.0f);
				//			normalImpulse = accumulatedNormalImpulse - oldNormalImpulse;
				
				// only apply the impulse if we are pushing or pulling in the right way
				// i.e. pulling if the string is overstretched and pushing if it is too compressed
				if (springLength < minSpringSize && normalImpulse < 0 || springLength > maxSpringSize && normalImpulse > 0)
				{
					// now apply the impulses to the bodies
					Vector2f impulse = MathUtil.Scale(spring, normalImpulse);
					body1.AdjustVelocity(MathUtil.Scale(impulse, body1.InvMass));
					body1.AdjustAngularVelocity((body1.InvI * MathUtil.Cross(r1, impulse)));
					
					body2.AdjustVelocity(MathUtil.Scale(impulse, - body2.InvMass));
					body2.AdjustAngularVelocity(- (body2.InvI * MathUtil.Cross(r2, impulse)));
				}
			}
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
				return ((SpringJoint) other).id == id;
			}
			
			return false;
		}
		
		//	public float getDamping() {
		//		return damping;
		//	}
		//
		//	public void setDamping(float damping) {
		//		this.damping = damping;
		//	}
	}
}