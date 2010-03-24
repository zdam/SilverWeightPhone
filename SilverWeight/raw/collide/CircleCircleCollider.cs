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
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
using Circle = Silver.Weight.Raw.Shapes.Circle;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> A collider for circle 2 circle collisions
	/// 
	/// The create() method is used as a factory just in case this 
	/// class becomes stateful eventually.
	/// 
	/// </summary>
	public class CircleCircleCollider : Collider
	{
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			float x1 = bodyA.GetPosition().X;
			float y1 = bodyA.GetPosition().Y;
			float x2 = bodyB.GetPosition().X;
			float y2 = bodyB.GetPosition().Y;
			
			bool touches = bodyA.Shape.Bounds.Touches(x1, y1, bodyB.Shape.Bounds, x2, y2);
			if (!touches)
			{
				return 0;
			}
			
			Circle circleA = (Circle) bodyA.Shape;
			Circle circleB = (Circle) bodyB.Shape;
			
			touches = circleA.Touches(x1, y1, circleB, x2, y2);
			if (!touches)
			{
				return 0;
			}
			
			Vector2f normal = MathUtil.Sub(bodyB.GetPosition(), bodyA.GetPosition());
			float sep = (circleA.Radius + circleB.Radius) - normal.Length();
			
			normal.Normalise();
			Vector2f pt = MathUtil.Scale(normal, circleA.Radius);
			pt.Add(bodyA.GetPosition());
			
			contacts[0].Separation = - sep;
			contacts[0].Position = pt;
			contacts[0].Normal = normal;
			
			FeaturePair fp = new FeaturePair();
			contacts[0].Feature = fp;
			
			return 1;
		}
	}
}