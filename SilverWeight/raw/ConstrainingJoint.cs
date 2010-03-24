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
	
	/// <summary> A joint which only applys forces when the bodie's attempt to get too far apart, as 
	/// suggested by Jesse on:
	/// 
	/// http://slick.javaunlimited.net/viewtopic.php?t=333
	/// 
	/// </summary>
	public class ConstrainingJoint : Joint
	{
		/// <summary> Check if this contraining joint is currently pulling the bodies back together
		/// 
		/// </summary>
		/// <returns> True if the joint is holding the bodies together
		/// </returns>
		virtual public bool Active
		{
			get
			{
				if (body1.GetPosition().DistanceSquared(body2.GetPosition()) < distance)
				{
					Vector2f to2 = new Vector2f(body2.GetPosition());
					to2.Sub(body1.GetPosition());
					to2.Normalise();
					Vector2f vel = new Vector2f(body1.Velocity);
					vel.Normalise();
					if (body1.Velocity.Dot(to2) < 0)
					{
						return true;
					}
				}
				
				return false;
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

		/// <seealso cref="Silver.Weight.Raw.Joint.getBody1()">
		/// </seealso>
		virtual public Body Body1
		{
			get
			{
				return body1;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Joint.getBody2()">
		/// </seealso>
		virtual public Body Body2
		{
			get
			{
				return body2;
			}
			
		}
		/// <seealso cref="Silver.Weight.Raw.Joint.setRelaxation(float)">
		/// </seealso>
		virtual public float Relaxation
		{
			set
			{
			}
			
		}
		/// <summary>The joint that is used to keep the bodies together if they are seperating </summary>
		private BasicJoint realJoint;
		/// <summary>The Distance the bodies must be apart before force is applied </summary>
		private float distance;
		/// <summary>The first body jointed </summary>
		private Body body1;
		/// <summary>The second body jointed </summary>
		private Body body2;
		/// <summary>True if the joint is active this Update </summary>
		private bool active;
		
		/// <summary> Create a new joint
		/// 
		/// </summary>
		/// <param name="body1">The first body jointed
		/// </param>
		/// <param name="body2">The second body jointed
		/// </param>
		/// <param name="anchor">The anchor point for the underlying basic joint
		/// </param>
		/// <param name="Distance">The Distance the bodies must be apart before force is applied
		/// </param>
		public ConstrainingJoint(Body body1, Body body2, Vector2f anchor, float distance)
		{
			this.distance = distance;
			this.body1 = body1;
			this.body2 = body2;
			realJoint = new BasicJoint(body1, body2, anchor);
		}
		
		/// <seealso cref="Silver.Weight.Raw.Joint.ApplyImpulse()">
		/// </seealso>
		public virtual void  ApplyImpulse()
		{
			if (active)
			{
				realJoint.ApplyImpulse();
			}
		}
		
		/// <seealso cref="Silver.Weight.Raw.Joint.PreStep(float)">
		/// </seealso>
		public virtual void  PreStep(float invDT)
		{
			if (Active)
			{
				active = true;
				realJoint.PreStep(invDT);
			}
			else
			{
				active = false;
			}
		}
	}
}