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
using Vector2f = Silver.Weight.Math.Vector2f;
using Silver.Weight.Raw.Collide;
namespace Silver.Weight.Raw
{
	
	/// <summary> A arbiter resolving contacts between a pair of bodies
	/// 
	/// </summary>
	public class Arbiter
	{
		/// <summary> Retrieve the contacts being resolved by this arbiter
		/// 
		/// </summary>
		/// <returns> The contacts being resolved by this arbiter
		/// </returns>
		virtual public Contact[] Contacts
		{
			get
			{
				return contacts;
			}
			
		}
		/// <summary> The number of contacts being resolved by this arbiter
		/// 
		/// </summary>
		/// <returns> The number of contacts being Resolve by this arbiter
		/// </returns>
		virtual public int NumContacts
		{
			get
			{
				return numContacts;
			}
			
		}
		/// <summary> Get the first of the two bodies handled by this arbiter
		/// 
		/// </summary>
		/// <returns> The first of the two bodies handled by this arbiter
		/// </returns>
		virtual public Body Body1
		{
			get
			{
				return body1;
			}
			
		}
		/// <summary> Get the second of the two bodies handled by this arbiter
		/// 
		/// </summary>
		/// <returns> The second of the two bodies handled by this arbiter
		/// </returns>
		virtual public Body Body2
		{
			get
			{
				return body2;
			}
			
		}
		/// <summary>The maximum number of points of contact </summary>
		public const int MAX_POINTS = 10;
		
		/// <summary>The contacts being resolved by this arbiter </summary>
		private Contact[] contacts = new Contact[MAX_POINTS];
		/// <summary>The number of contacts made </summary>
		private int numContacts;
		/// <summary>The first body in contact </summary>
		private Body body1;
		/// <summary>The second body in contact </summary>
		private Body body2;
		/// <summary>Combined friction between two bodies </summary>
		private float friction;
		
		/// <summary> Create a new arbiter - this should only be done by the 
		/// engine
		/// 
		/// </summary>
		/// <param name="b1">The first body in contact
		/// </param>
		/// <param name="b2">The second body in contact
		/// </param>
		internal Arbiter(Body b1, Body b2)
		{
			for (int i = 0; i < MAX_POINTS; i++)
			{
				contacts[i] = new Contact();
			}
			
			if (!(b2 is StaticBody) && b1.GetHashCode() < b2.GetHashCode())
			{
				body1 = b1;
				body2 = b2;
			}
			else
			{
				body1 = b2;
				body2 = b1;
			}
		}
		
		/// <summary> Check if this arbiter has two bodies that are resting
		/// 
		/// </summary>
		/// <returns> True if the arbiter has two bodies that are "at rest"
		/// </returns>
		public virtual bool HasRestingPair()
		{
			return body1.Resting && body2.Resting;
		}
		
		/// <summary> Perform the collision analysis between the two bodies 
		/// arbitrated
		/// 
		/// </summary>
		/// <param name="dt">The amount of time passed since last collision check
		/// </param>
		public virtual void  Collide(float dt)
		{
			Collider c = new ColliderFactory().CreateCollider(body1, body2);
			numContacts = c.Collide(contacts, body1, body2);
		}
		
		/// <summary> Get one of the two contacts handled being handled by this
		/// arbiter
		/// 
		/// </summary>
		/// <param name="index">The index of the contact to retrieve
		/// </param>
		/// <returns> The contact or null if no contact has been detected 
		/// </returns>
		internal virtual Contact GetContact(int index)
		{
			return contacts[index];
		}
		
		/// <summary> Initialise state for this arbiter - this is only done 
		/// once per pair of bodies. It's used to caculated static
		/// data between them
		/// 
		/// </summary>
		public virtual void  Init()
		{
			if (numContacts > 0)
			{
				friction = (float) System.Math.Sqrt(body1.Friction * body2.Friction);
			}
		}
		
		/// <summary> Update this arbiter from a second set of data determined
		/// as the simulation continues
		/// 
		/// </summary>
		/// <param name="newContacts">The new contacts that have been found
		/// </param>
		/// <param name="numNewContacts">The number of new contacts discovered
		/// </param>
		internal virtual void  Update(Contact[] newContacts, int numNewContacts)
		{
			Contact[] mergedContacts = new Contact[MAX_POINTS];
			for (int i = 0; i < mergedContacts.Length; i++)
			{
				mergedContacts[i] = new Contact();
			}
			
			for (int i = 0; i < numNewContacts; ++i)
			{
				Contact cNew = newContacts[i];
				int k = - 1;
				for (int j = 0; j < numContacts; ++j)
				{
					Contact cOld = contacts[j];
					if (cNew.feature.Equals(cOld.feature))
					{
						k = j;
						break;
					}
				}
				
				if (k > - 1)
				{
					Contact c = mergedContacts[i];
					Contact cOld = contacts[k];
					c.Reconfigure(cNew);
					c.accumulatedNormalImpulse = cOld.accumulatedNormalImpulse;
					c.accumulatedTangentImpulse = cOld.accumulatedTangentImpulse;
				}
				else
				{
					mergedContacts[i].Reconfigure(newContacts[i]);
				}
			}
			
			for (int i = 0; i < numNewContacts; ++i)
			{
				contacts[i].Reconfigure(mergedContacts[i]);
			}
			
			numContacts = numNewContacts;
		}
		
		/// <summary> Check if this arbiter affects the specified body
		/// 
		/// </summary>
		/// <param name="body">The body to check for
		/// </param>
		/// <returns> True if this arbiter effects the body
		/// </returns>
		public virtual bool Concerns(Body body)
		{
			return (body1 == body) || (body2 == body);
		}
		
		/// <summary> Apply the friction impulse from each contact.
		/// 
		/// </summary>
		/// <param name="dt">The amount of time to Step the simulation by
		/// </param>
		/// <param name="invDT">The inverted time
		/// </param>
		/// <param name="damping">The percentage of energy to retain through out
		/// collision. (1 = no loss, 0 = total loss)
		/// </param>
		internal virtual void  PreStep(float invDT, float dt, float damping)
		{
			float allowedPenetration = 0.01f;
			float biasFactor = 0.8f;
			
			for (int i = 0; i < numContacts; ++i)
			{
				Contact c = contacts[i];
				c.normal.Normalise();
				
				Vector2f r1 = new Vector2f(c.position);
				r1.Sub(body1.GetPosition());
				Vector2f r2 = new Vector2f(c.position);
				r2.Sub(body2.GetPosition());
				
				// Precompute normal mass, tangent mass, and bias.
				float rn1 = r1.Dot(c.normal);
				float rn2 = r2.Dot(c.normal);
				float kNormal = body1.InvMass + body2.InvMass;
				kNormal += body1.InvI * (r1.Dot(r1) - rn1 * rn1) + body2.InvI * (r2.Dot(r2) - rn2 * rn2);
				c.massNormal = damping / kNormal;
				
				Vector2f tangent = MathUtil.Cross(c.normal, 1.0f);
				float rt1 = r1.Dot(tangent);
				float rt2 = r2.Dot(tangent);
				float kTangent = body1.InvMass + body2.InvMass;
				kTangent += body1.InvI * (r1.Dot(r1) - rt1 * rt1) + body2.InvI * (r2.Dot(r2) - rt2 * rt2);
				c.massTangent = damping / kTangent;
				
				// Compute restitution
				// Relative velocity at contact 
				Vector2f relativeVelocity = new Vector2f(body2.Velocity);
				relativeVelocity.Add(MathUtil.Cross(r2, body2.AngularVelocity));
				relativeVelocity.Sub(body1.Velocity);
				relativeVelocity.Sub(MathUtil.Cross(r1, body1.AngularVelocity));
				
				float combinedRestitution = (body1.Restitution * body2.Restitution);
				float relVel = c.normal.Dot(relativeVelocity);
				c.restitution = combinedRestitution * (- relVel);
				c.restitution = System.Math.Max(c.restitution, 0);
				
				float penVel = (- c.separation) / dt;
				if (c.restitution >= penVel)
				{
					c.bias = 0;
				}
				else
				{
					c.bias = (- biasFactor) * invDT * System.Math.Min(0.0f, c.separation + allowedPenetration);
				}
				
				// apply damping
				c.accumulatedNormalImpulse *= damping;
				
				// Apply normal + friction impulse
				Vector2f impulse = MathUtil.Scale(c.normal, c.accumulatedNormalImpulse);
				impulse.Add(MathUtil.Scale(tangent, c.accumulatedTangentImpulse));
				
				body1.AdjustVelocity(MathUtil.Scale(impulse, - body1.InvMass));
				body1.AdjustAngularVelocity((- body1.InvI) * MathUtil.Cross(r1, impulse));
				
				body2.AdjustVelocity(MathUtil.Scale(impulse, body2.InvMass));
				body2.AdjustAngularVelocity(body2.InvI * MathUtil.Cross(r2, impulse));
				
				// rest bias
				c.biasImpulse = 0;
			}
		}
		
		//	private static Vector2f r1 = new Vector2f();
		//	private static Vector2f r2 = new Vector2f();
		//	private static Vector2f relativeVelocity = new Vector2f();
		//	private static Vector2f impulse = new Vector2f();
		//	private static Vector2f Pb = new Vector2f();
		//	private static Vector2f tmp = new Vector2f();
		//	
		//	/**
		//	 * Apply the impulse accumlated at the contact points maintained
		//	 * by this arbiter.
		//	 */
		//	void ApplyImpulse() {
		//		Body b1 = body1;
		//		Body b2 = body2;
		//		
		//		for (int i = 0; i < numContacts; ++i)
		//		{
		//			Contact c = contacts[i];
		//			
		//			r1.set(c.position);
		//			r1.Sub(b1.GetPosition());
		//			r2.set(c.position);
		//			r2.Sub(b2.GetPosition());
		//
		//			// Relative velocity at contact
		//			relativeVelocity.set(b2.getVelocity());
		//			relativeVelocity.Add(MathUtil.Cross(b2.getAngularVelocity(), r2));
		//			relativeVelocity.Sub(b1.getVelocity());
		//			relativeVelocity.Sub(MathUtil.Cross(b1.getAngularVelocity(), r1));
		//			
		//			// Compute normal impulse with bias.
		//			float vn = relativeVelocity.Dot(c.normal);
		//			
		//			// bias caculations are now handled seperately hence we only
		//			// handle the real impulse caculations here
		//			//float normalImpulse = c.massNormal * ((c.restitution - vn) + c.bias);
		//			float normalImpulse = c.massNormal * (c.restitution - vn);
		//			
		//			// Clamp the accumulated impulse
		//			float oldNormalImpulse = c.accumulatedNormalImpulse;
		//			c.accumulatedNormalImpulse = Math.max(oldNormalImpulse + normalImpulse, 0.0f);
		//			normalImpulse = c.accumulatedNormalImpulse - oldNormalImpulse;
		//			
		//			if (normalImpulse != 0) {
		//				// Apply contact impulse
		//				impulse.set(c.normal);
		//				impulse.Scale(normalImpulse);
		//				
		//				tmp.set(impulse);
		//				tmp.Scale(-b1.getInvMass());
		//				b1.AdjustVelocity(tmp);
		//				b1.AdjustAngularVelocity(-(b1.getInvI() * MathUtil.Cross(r1, impulse)));
		//	
		//				tmp.set(impulse);
		//				tmp.Scale(b2.getInvMass());
		//				b2.AdjustVelocity(tmp);
		//				b2.AdjustAngularVelocity(b2.getInvI() * MathUtil.Cross(r2, impulse));
		//			}
		//			
		//			// Compute bias impulse
		//			// NEW STUFF FOR SEPERATING BIAS
		//			relativeVelocity.set(b2.getBiasedVelocity());
		//			relativeVelocity.Add(MathUtil.Cross(b2.getBiasedAngularVelocity(), r2));
		//			relativeVelocity.Sub(b1.getBiasedVelocity());
		//			relativeVelocity.Sub(MathUtil.Cross(b1.getBiasedAngularVelocity(), r1));
		//			float vnb = relativeVelocity.Dot(c.normal);
		//
		//			float biasImpulse = c.massNormal * (-vnb + c.bias);
		//			float oldBiasImpulse = c.biasImpulse;
		//			c.biasImpulse = Math.max(oldBiasImpulse + biasImpulse, 0.0f);
		//			biasImpulse = c.biasImpulse - oldBiasImpulse;
		//
		//			if (biasImpulse != 0) {
		//				Pb.set(c.normal);
		//				Pb.Scale(biasImpulse);
		//				
		//				tmp.set(Pb);
		//				tmp.Scale(-b1.getInvMass());
		//				b1.AdjustBiasedVelocity(tmp);
		//				b1.AdjustBiasedAngularVelocity(-(b1.getInvI() * MathUtil.Cross(r1, Pb)));
		//	
		//				tmp.set(Pb);
		//				tmp.Scale(b2.getInvMass());
		//				b2.AdjustBiasedVelocity(tmp);
		//				b2.AdjustBiasedAngularVelocity((b2.getInvI() * MathUtil.Cross(r2, Pb)));
		//			}
		//			// END NEW STUFF
		//			
		//			//
		//			// Compute friction (tangent) impulse
		//			//
		//			float maxTangentImpulse = friction * c.accumulatedNormalImpulse;
		//
		//			// Relative velocity at contact
		//			relativeVelocity.set(b2.getVelocity());
		//			relativeVelocity.Add(MathUtil.Cross(b2.getAngularVelocity(), r2));
		//			relativeVelocity.Sub(b1.getVelocity());
		//			relativeVelocity.Sub(MathUtil.Cross(b1.getAngularVelocity(), r1));
		//			
		//			Vector2f tangent = MathUtil.Cross(c.normal, 1.0f);
		//			float vt = relativeVelocity.Dot(tangent);
		//			float tangentImpulse = c.massTangent * (-vt);
		//
		//			// Clamp friction
		//			float oldTangentImpulse = c.accumulatedTangentImpulse;
		//			c.accumulatedTangentImpulse = MathUtil.Clamp(oldTangentImpulse + tangentImpulse, -maxTangentImpulse, maxTangentImpulse);
		//			tangentImpulse = c.accumulatedTangentImpulse - oldTangentImpulse;
		//
		//			// Apply contact impulse
		//			if ((tangentImpulse > 0.1f) || (tangentImpulse < -0.1f)) {
		//				impulse = MathUtil.Scale(tangent, tangentImpulse);
		//	
		//				tmp.set(impulse);
		//				tmp.Scale(-b1.getInvMass());
		//				b1.AdjustVelocity(tmp);
		//				b1.AdjustAngularVelocity(-b1.getInvI() * MathUtil.Cross(r1, impulse));
		//	
		//				tmp.set(impulse);
		//				tmp.Scale(b2.getInvMass());
		//				b2.AdjustVelocity(tmp);
		//				b2.AdjustAngularVelocity(b2.getInvI() * MathUtil.Cross(r2, impulse));
		//			}
		//		}
		//	}
		
		/// <summary> Apply the impulse accumlated at the contact points maintained
		/// by this arbiter.
		/// </summary>
		internal virtual void  ApplyImpulse()
		{
			Body b1 = body1;
			Body b2 = body2;
			
			for (int i = 0; i < numContacts; ++i)
			{
				Contact c = contacts[i];
				
				Vector2f r1 = new Vector2f(c.position);
				r1.Sub(b1.GetPosition());
				Vector2f r2 = new Vector2f(c.position);
				r2.Sub(b2.GetPosition());
				
				// Relative velocity at contact
				Vector2f relativeVelocity = new Vector2f(b2.Velocity);
				relativeVelocity.Add(MathUtil.Cross(b2.AngularVelocity, r2));
				relativeVelocity.Sub(b1.Velocity);
				relativeVelocity.Sub(MathUtil.Cross(b1.AngularVelocity, r1));
				
				// Compute normal impulse with bias.
				float vn = relativeVelocity.Dot(c.normal);
				
				// bias caculations are now handled seperately hence we only
				// handle the real impulse caculations here
				//float normalImpulse = c.massNormal * ((c.restitution - vn) + c.bias);
				float normalImpulse = c.massNormal * (c.restitution - vn);
				
				// Clamp the accumulated impulse
				float oldNormalImpulse = c.accumulatedNormalImpulse;
				c.accumulatedNormalImpulse = System.Math.Max(oldNormalImpulse + normalImpulse, 0.0f);
				normalImpulse = c.accumulatedNormalImpulse - oldNormalImpulse;
				
				// Apply contact impulse
				Vector2f impulse = MathUtil.Scale(c.normal, normalImpulse);
				
				b1.AdjustVelocity(MathUtil.Scale(impulse, - b1.InvMass));
				b1.AdjustAngularVelocity(- (b1.InvI * MathUtil.Cross(r1, impulse)));
				
				b2.AdjustVelocity(MathUtil.Scale(impulse, b2.InvMass));
				b2.AdjustAngularVelocity(b2.InvI * MathUtil.Cross(r2, impulse));
				
				// Compute bias impulse
				// NEW STUFF FOR SEPERATING BIAS
				relativeVelocity.Reconfigure(b2.BiasedVelocity);
				relativeVelocity.Add(MathUtil.Cross(b2.BiasedAngularVelocity, r2));
				relativeVelocity.Sub(b1.BiasedVelocity);
				relativeVelocity.Sub(MathUtil.Cross(b1.BiasedAngularVelocity, r1));
				float vnb = relativeVelocity.Dot(c.normal);
				
				float biasImpulse = c.massNormal * (- vnb + c.bias);
				float oldBiasImpulse = c.biasImpulse;
				c.biasImpulse = System.Math.Max(oldBiasImpulse + biasImpulse, 0.0f);
				biasImpulse = c.biasImpulse - oldBiasImpulse;
				
				Vector2f Pb = MathUtil.Scale(c.normal, biasImpulse);
				
				b1.AdjustBiasedVelocity(MathUtil.Scale(Pb, - b1.InvMass));
				b1.AdjustBiasedAngularVelocity(- (b1.InvI * MathUtil.Cross(r1, Pb)));
				
				b2.AdjustBiasedVelocity(MathUtil.Scale(Pb, b2.InvMass));
				b2.AdjustBiasedAngularVelocity((b2.InvI * MathUtil.Cross(r2, Pb)));
				
				// END NEW STUFF
				
				//
				// Compute friction (tangent) impulse
				//
				float maxTangentImpulse = friction * c.accumulatedNormalImpulse;
				
				// Relative velocity at contact
				relativeVelocity.Reconfigure(b2.Velocity);
				relativeVelocity.Add(MathUtil.Cross(b2.AngularVelocity, r2));
				relativeVelocity.Sub(b1.Velocity);
				relativeVelocity.Sub(MathUtil.Cross(b1.AngularVelocity, r1));
				
				Vector2f tangent = MathUtil.Cross(c.normal, 1.0f);
				float vt = relativeVelocity.Dot(tangent);
				float tangentImpulse = c.massTangent * (- vt);
				
				// Clamp friction
				float oldTangentImpulse = c.accumulatedTangentImpulse;
				c.accumulatedTangentImpulse = MathUtil.Clamp(oldTangentImpulse + tangentImpulse, - maxTangentImpulse, maxTangentImpulse);
				tangentImpulse = c.accumulatedTangentImpulse - oldTangentImpulse;
				
				// Apply contact impulse
				impulse = MathUtil.Scale(tangent, tangentImpulse);
				
				b1.AdjustVelocity(MathUtil.Scale(impulse, - b1.InvMass));
				b1.AdjustAngularVelocity((- b1.InvI) * MathUtil.Cross(r1, impulse));
				
				b2.AdjustVelocity(MathUtil.Scale(impulse, b2.InvMass));
				b2.AdjustAngularVelocity(b2.InvI * MathUtil.Cross(r2, impulse));
			}
		}
		
		/// <summary> Get the energy contained within 2 bodies
		/// 
		/// </summary>
		/// <param name="body1">The first body
		/// </param>
		/// <param name="body2">The second body
		/// </param>
		/// <returns> The energy contained
		/// </returns>
		protected internal virtual float GetEnergy(Body body1, Body body2)
		{
			Vector2f combinedVel = MathUtil.Scale(body1.Velocity, body1.Mass);
			combinedVel.Add(MathUtil.Scale(body2.Velocity, body2.Mass));
			
			float combinedInertia = body1.Inertia * body1.AngularVelocity;
			combinedInertia += body2.Inertia * body2.AngularVelocity;
			
			float vel1Energy = body1.Mass * body1.Velocity.Dot(body1.Velocity);
			float vel2Energy = body2.Mass * body2.Velocity.Dot(body2.Velocity);
			float ang1Energy = body1.Inertia * (body1.AngularVelocity * body1.AngularVelocity);
			float ang2Energy = body2.Inertia * (body2.AngularVelocity * body2.AngularVelocity);
			float energy = vel1Energy + vel2Energy + ang1Energy + ang2Energy;
			
			return energy;
		}
		
		/// <seealso cref="java.lang.Object.hashCode()">
		/// </seealso>
		public override int GetHashCode()
		{
			return body1.GetHashCode() + body2.GetHashCode();
		}
		
		/// <seealso cref="java.lang.Object.equals(java.lang.Object)">
		/// </seealso>
		public  override bool Equals(System.Object other)
		{
			if (other.GetType().Equals(GetType()))
			{
				Arbiter o = (Arbiter) other;
				
				return (o.body1.Equals(body1) && o.body2.Equals(body2));
			}
			
			return false;
		}
	}
}