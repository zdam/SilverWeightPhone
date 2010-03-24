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
namespace Silver.Weight.Raw
{
	
	
	/// <summary> A joint between two bodies. The joint affects the impulses applied to 
	/// each body each Step constraining the movement. This joint is anchored
	/// on the opposing bodies centre
	/// 
	/// </summary>
	public class FixedJoint : Joint
	{
		/// <summary> Set the relaxtion value on this joint. This value determines
		/// how loose the joint will be
		/// 
		/// </summary>
		/// <param name="relaxation">The relaxation value
		/// </param>
		virtual public float Relaxation
		{
			set
			{
				joint1.Relaxation = value;
				joint2.Relaxation = value;
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
		/// <summary>The Next ID to be used </summary>
		public static int NEXT_ID = 0;
		
		/// <summary>The first body attached to the joint </summary>
		private Body body1;
		/// <summary>The second body attached to the joint </summary>
		private Body body2;
		
		/// <summary>The ID of this joint </summary>
		private int id;
		
		/// <summary>The first joint fixing the position in one direction </summary>
		private BasicJoint joint1;
		/// <summary>The second joint fixing the position in one direction </summary>
		private BasicJoint joint2;
		
		/// <summary> Create a joint holding two bodies together
		/// 
		/// </summary>
		/// <param name="b1">The first body attached to the joint
		/// </param>
		/// <param name="b2">The second body attached to the joint
		/// </param>
		public FixedJoint(Body b1, Body b2)
		{
			id = NEXT_ID++;
			
			Reconfigure(b1, b2);
		}
		
		/// <summary> Reconfigure this joint
		/// 
		/// </summary>
		/// <param name="b1">The first body attached to this joint
		/// </param>
		/// <param name="b2">The second body attached to this joint
		/// </param>
		public virtual void  Reconfigure(Body b1, Body b2)
		{
			body1 = b1;
			body2 = b2;
			
			joint1 = new BasicJoint(b1, b2, new Vector2f(b1.GetPosition()));
			joint2 = new BasicJoint(b2, b1, new Vector2f(b2.GetPosition()));
		}
		
		/// <summary> Precaculate everything and apply initial impulse before the
		/// simulation Step takes place
		/// 
		/// </summary>
		/// <param name="invDT">The amount of time the simulation is being stepped by
		/// </param>
		public virtual void  PreStep(float invDT)
		{
			joint1.PreStep(invDT);
			joint2.PreStep(invDT);
		}
		
		/// <summary> Apply the impulse caused by the joint to the bodies attached.</summary>
		public virtual void  ApplyImpulse()
		{
			joint1.ApplyImpulse();
			joint2.ApplyImpulse();
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
				return ((FixedJoint) other).id == id;
			}
			
			return false;
		}
	}
}