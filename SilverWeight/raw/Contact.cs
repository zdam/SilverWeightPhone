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
using FeaturePair = Silver.Weight.Raw.Collide.FeaturePair;
namespace Silver.Weight.Raw
{
	
	/// <summary> A description of a single contact point between two bodies
	/// 
	/// </summary>
	public class Contact
	{
		/// <summary> The position of this contact 
		/// 
		/// </summary>
		virtual public ROVector2f Position
		{
			get
			{
				return position;
			}
			
			set
			{
				this.position.Reconfigure(value);
			}
			
		}
		/// <summary> The seperation between bodies
		/// </summary>
		virtual public float Separation
		{
			get
			{
				return separation;
			}
			
			set
			{
				this.separation = value;
			}
			
		}
		/// <summary> The normal at the point of contact
		/// </summary>
		virtual public ROVector2f Normal
		{
			get
			{
				return normal;
			}
			
			set
			{
				this.normal.Reconfigure(value);
			}
			
		}
		/// <summary> Get the pairing identifing the location of the contact
		/// </summary>
		virtual public FeaturePair Feature
		{
			get
			{
				return feature;
			}
			
			set
			{
				this.feature = value;
			}
			
		}
		// TODO: the positions are absolute, right? if not make them so
		/// <summary>The position of the contact </summary>
		internal Vector2f position = new Vector2f();
		/// <summary>The normal at the contact point which, for convex bodies,
		/// points away from the first body. 
		/// </summary>
		internal Vector2f normal = new Vector2f();
		/// <summary>? </summary>
		internal float separation;
		/// <summary>The impulse accumlated in the direction of the normal </summary>
		internal float accumulatedNormalImpulse;
		/// <summary>The impulse accumlated in the direction of the tangent </summary>
		internal float accumulatedTangentImpulse;
		/// <summary>The mass applied throught the normal at this contact point </summary>
		internal float massNormal;
		/// <summary>The mass applied through the tangent at this contact point </summary>
		internal float massTangent;
		/// <summary>The correction factor penetration </summary>
		internal float bias;
		/// <summary>The pair of edges this contact is between </summary>
		internal FeaturePair feature = new FeaturePair();
		/// <summary>The restitution at this point of contact </summary>
		internal float restitution;
		/// <summary>The bias impulse accumulated </summary>
		internal float biasImpulse;
		
		/// <summary> Create a new contact point</summary>
		public Contact()
		{
			accumulatedNormalImpulse = 0.0f;
			accumulatedTangentImpulse = 0.0f;
		}
		
		/// <summary> Set the contact information based on another contact
		/// 
		/// </summary>
		/// <param name="contact">The contact information
		/// </param>
		internal virtual void  Reconfigure(Contact contact)
		{
			position.Reconfigure(contact.position);
			normal.Reconfigure(contact.normal);
			separation = contact.separation;
			accumulatedNormalImpulse = contact.accumulatedNormalImpulse;
			accumulatedTangentImpulse = contact.accumulatedTangentImpulse;
			massNormal = contact.massNormal;
			massTangent = contact.massTangent;
			bias = contact.bias;
			restitution = contact.restitution;
			feature.SetFromOther(contact.feature);
		}
		
		public override int GetHashCode()
		{
			return feature.GetHashCode();
		}
		
		public  override bool Equals(System.Object other)
		{
			if (other.GetType() == GetType())
			{
				return ((Contact) other).feature.Equals(feature);
			}
			
			return false;
		}
		
		public override System.String ToString()
		{
			return "[Contact " + position + " n: " + normal + " sep: " + separation + "]";
		}
	}
}