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
	
	/// <summary> A joint between two bodies. The joint affects the impulses applied to 
	/// each body each Step constraining the movement. Behaves a bit like elastic
	/// 
	/// Behaviour is undefined - it does something and it's definitely odd. Might
	/// be useful for something
	/// 
	/// </summary>
	public class ElasticJoint : Joint
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
				this.relaxation = value;
			}
			
		}
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
		/// <summary>The Next ID to be used </summary>
		public static int NEXT_ID = 0;
		
		/// <summary>The first body attached to the joint </summary>
		private Body body1;
		/// <summary>The second body attached to the joint </summary>
		private Body body2;
		
		/// <summary>The matrix describing the connection between two bodies </summary>
		private Matrix2f M = new Matrix2f();
		/// <summary>The local anchor for the first body </summary>
		private Vector2f localAnchor1 = new Vector2f();
		/// <summary>The local anchor for the second body </summary>
		private Vector2f localAnchor2 = new Vector2f();
		/// <summary>The rotation of the anchor of the first body </summary>
		private Vector2f r1 = new Vector2f();
		/// <summary>The rotation of the anchor of the second body </summary>
		private Vector2f r2 = new Vector2f();
		/// <summary>? </summary>
		private Vector2f bias = new Vector2f();
		/// <summary>The impulse to be applied throught the joint </summary>
		private Vector2f accumulatedImpulse = new Vector2f();
		/// <summary>How much slip there is in the joint </summary>
		private float relaxation;
		
		/// <summary>The ID of this joint </summary>
		private int id;
		
		/// <summary> Create a joint holding two bodies together
		/// 
		/// </summary>
		/// <param name="b1">The first body attached to the joint
		/// </param>
		/// <param name="b2">The second body attached to the joint
		/// </param>
		public ElasticJoint(Body b1, Body b2)
		{
			id = NEXT_ID++;
			accumulatedImpulse.Reconfigure(0.0f, 0.0f);
			relaxation = 1.0f;
			
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
			
			Matrix2f rot1 = new Matrix2f(body1.Rotation);
			Matrix2f rot2 = new Matrix2f(body2.Rotation);
			Matrix2f rot1T = rot1.Transpose();
			Matrix2f rot2T = rot2.Transpose();
			
			Vector2f a1 = new Vector2f(body2.GetPosition());
			a1.Sub(body1.GetPosition());
			localAnchor1 = MathUtil.Mul(rot1T, a1);
			Vector2f a2 = new Vector2f(body1.GetPosition());
			a2.Sub(body2.GetPosition());
			localAnchor2 = MathUtil.Mul(rot2T, a2);
			
			accumulatedImpulse.Reconfigure(0.0f, 0.0f);
			relaxation = 1.0f;
		}
		
		/// <summary> Precaculate everything and apply initial impulse before the
		/// simulation Step takes place
		/// 
		/// </summary>
		/// <param name="invDT">The amount of time the simulation is being stepped by
		/// </param>
		public virtual void  PreStep(float invDT)
		{
			// Pre-compute anchors, mass matrix, and bias.
			Matrix2f rot1 = new Matrix2f(body1.Rotation);
			Matrix2f rot2 = new Matrix2f(body2.Rotation);
			
			r1 = MathUtil.Mul(rot1, localAnchor1);
			r2 = MathUtil.Mul(rot2, localAnchor2);
			
			// deltaV = deltaV0 + K * impulse
			// invM = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
			//      = [1/m1+1/m2     0    ] + invI1 * [r1.y*r1.y -r1.x*r1.y] + invI2 * [r1.y*r1.y -r1.x*r1.y]
			//        [    0     1/m1+1/m2]           [-r1.x*r1.y r1.x*r1.x]           [-r1.x*r1.y r1.x*r1.x]
			Matrix2f K1 = new Matrix2f();
			K1.col1.x = body1.InvMass + body2.InvMass; K1.col2.x = 0.0f;
			K1.col1.y = 0.0f; K1.col2.y = body1.InvMass + body2.InvMass;
			
			Matrix2f K2 = new Matrix2f();
			K2.col1.x = body1.InvI * r1.y * r1.y; K2.col2.x = (- body1.InvI) * r1.x * r1.y;
			K2.col1.y = (- body1.InvI) * r1.x * r1.y; K2.col2.y = body1.InvI * r1.x * r1.x;
			
			Matrix2f K3 = new Matrix2f();
			K3.col1.x = body2.InvI * r2.y * r2.y; K3.col2.x = (- body2.InvI) * r2.x * r2.y;
			K3.col1.y = (- body2.InvI) * r2.x * r2.y; K3.col2.y = body2.InvI * r2.x * r2.x;
			
			Matrix2f K = MathUtil.Add(MathUtil.Add(K1, K2), K3);
			M = K.Invert();
			
			Vector2f p1 = new Vector2f(body1.GetPosition());
			p1.Add(r1);
			Vector2f p2 = new Vector2f(body2.GetPosition());
			p2.Add(r2);
			Vector2f dp = new Vector2f(p2);
			dp.Sub(p1);
			
			bias = new Vector2f(dp);
			bias.Scale(- 0.1f);
			bias.Scale(invDT);
			
			// Apply accumulated impulse.
			accumulatedImpulse.Scale(relaxation);
			
			Vector2f accum1 = new Vector2f(accumulatedImpulse);
			accum1.Scale(- body1.InvMass);
			body1.AdjustVelocity(accum1);
			body1.AdjustAngularVelocity(- (body1.InvI * MathUtil.Cross(r1, accumulatedImpulse)));
			
			Vector2f accum2 = new Vector2f(accumulatedImpulse);
			accum2.Scale(body2.InvMass);
			body2.AdjustVelocity(accum2);
			body2.AdjustAngularVelocity(body2.InvI * MathUtil.Cross(r2, accumulatedImpulse));
		}
		
		/// <summary> Apply the impulse caused by the joint to the bodies attached.</summary>
		public virtual void  ApplyImpulse()
		{
			Vector2f dv = new Vector2f(body2.Velocity);
			dv.Add(MathUtil.Cross(body2.AngularVelocity, r2));
			dv.Sub(body1.Velocity);
			dv.Sub(MathUtil.Cross(body1.AngularVelocity, r1));
			dv.Scale(- 1);
			dv.Add(bias);
			
			if (dv.LengthSquared() == 0)
			{
				return ;
			}
			
			Vector2f impulse = MathUtil.Mul(M, dv);
			
			Vector2f delta1 = new Vector2f(impulse);
			delta1.Scale(- body1.InvMass);
			body1.AdjustVelocity(delta1);
			body1.AdjustAngularVelocity((- body1.InvI) * MathUtil.Cross(r1, impulse));
			
			Vector2f delta2 = new Vector2f(impulse);
			delta2.Scale(body2.InvMass);
			body2.AdjustVelocity(delta2);
			body2.AdjustAngularVelocity(body2.InvI * MathUtil.Cross(r2, impulse));
			
			accumulatedImpulse.Add(impulse);
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
				return ((ElasticJoint) other).id == id;
			}
			
			return false;
		}
	}
}