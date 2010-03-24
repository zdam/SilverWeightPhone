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
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
using Circle = Silver.Weight.Raw.Shapes.Circle;
using Line = Silver.Weight.Raw.Shapes.Line;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> Collision routines betwene a circle and a line. The create method is
	/// provided in case this collider becomes stateful at some point.
	/// 
	/// </summary>
	public class LineCircleCollider : Collider
	{
		
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			Line line = (Line) bodyA.Shape;
			Circle circle = (Circle) bodyB.Shape;
			
			Vector2f[] vertsA = line.getVertices(bodyA.GetPosition(), bodyA.Rotation);
			
			// compute intersection of the line A and a line parallel to 
			// the line A's normal passing through the origin of B
			Vector2f startA = vertsA[0];
			Vector2f endA = vertsA[1];
			ROVector2f startB = bodyB.GetPosition();
			Vector2f endB = new Vector2f(endA);
			endB.Sub(startA);
			endB.Reconfigure(endB.y, - endB.x);
			//		endB.Add(startB);// TODO: inline endB into equations below, this last operation will be useless..
			
			//TODO: reuse mathutil.Intersect
			//		float d = (endB.y - startB.getY()) * (endA.x - startA.x);
			//		d -= (endB.x - startB.getX()) * (endA.y - startA.y);
			//		
			//		float uA = (endB.x - startB.getX()) * (startA.y - startB.getY());
			//		uA -= (endB.y - startB.getY()) * (startA.x - startB.getX());
			//		uA /= d;
			float d = endB.y * (endA.x - startA.x);
			d -= endB.x * (endA.y - startA.y);
			
			float uA = endB.x * (startA.y - startB.Y);
			uA -= endB.y * (startA.x - startB.X);
			uA /= d;
			
			Vector2f position = null;
			
			if (uA < 0)
			{
				// the intersection is somewhere before startA
				position = startA;
			}
			else if (uA > 1)
			{
				// the intersection is somewhere after endA
				position = endA;
			}
			else
			{
				position = new Vector2f(startA.x + uA * (endA.x - startA.x), startA.y + uA * (endA.y - startA.y));
			}
			
			Vector2f normal = endB; // reuse of vector object
			normal.Reconfigure(startB);
			normal.Sub(position);
			float distSquared = normal.LengthSquared();
			float radiusSquared = circle.Radius * circle.Radius;
			
			if (distSquared < radiusSquared)
			{
				contacts[0].Position = position;
				contacts[0].Feature = new FeaturePair();
				
				normal.Normalise();
				contacts[0].Normal = normal;
				
				float separation = (float) System.Math.Sqrt(distSquared) - circle.Radius;
				contacts[0].Separation = separation;
				
				return 1;
			}
			
			return 0;
		}
	}
}