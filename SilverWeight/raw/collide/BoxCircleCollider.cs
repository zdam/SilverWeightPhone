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
using Box = Silver.Weight.Raw.Shapes.Box;
using Circle = Silver.Weight.Raw.Shapes.Circle;
using Line = Silver.Weight.Raw.Shapes.Line;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> A collider for boxes hitting circles. Box = bodyA, Circle = bodyB
	/// 
	/// The create() method is used as a factor although since this collider
	/// is currently stateless a single instance is returned.
	/// 
	/// </summary>
	public class BoxCircleCollider : Collider
	{
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body boxBody, Body circleBody)
		{
			float x1 = boxBody.GetPosition().X;
			float y1 = boxBody.GetPosition().Y;
			float x2 = circleBody.GetPosition().X;
			float y2 = circleBody.GetPosition().Y;
			
			bool touches = boxBody.Shape.Bounds.Touches(x1, y1, circleBody.Shape.Bounds, x2, y2);
			if (!touches)
			{
				return 0;
			}
			
			Box box = (Box) boxBody.Shape;
			Circle circle = (Circle) circleBody.Shape;
			
			Vector2f[] pts = box.GetPoints(boxBody.GetPosition(), boxBody.Rotation);
			Line[] lines = new Line[4];
			lines[0] = new Line(pts[0], pts[1]);
			lines[1] = new Line(pts[1], pts[2]);
			lines[2] = new Line(pts[2], pts[3]);
			lines[3] = new Line(pts[3], pts[0]);
			
			float r2 = circle.Radius * circle.Radius;
			int closest = - 1;
			float closestDistance = System.Single.MaxValue;
			
			for (int i = 0; i < 4; i++)
			{
				float dis = lines[i].distanceSquared(circleBody.GetPosition());
				if (dis < r2)
				{
					if (closestDistance > dis)
					{
						closestDistance = dis;
						closest = i;
					}
				}
			}
			
			if (closest > - 1)
			{
				float dis = (float) System.Math.Sqrt(closestDistance);
				contacts[0].Separation = dis - circle.Radius;
				
				// this should really be where the edge and the line
				// between the two elements Cross?
				Vector2f contactPoint = new Vector2f();
				lines[closest].getClosestPoint(circleBody.GetPosition(), contactPoint);
				
				Vector2f normal = MathUtil.Sub(circleBody.GetPosition(), contactPoint);
				normal.Normalise();
				contacts[0].Normal = normal;
				contacts[0].Position = contactPoint;
				contacts[0].Feature = new FeaturePair();
				
				return 1;
			}
			
			return 0;
		}
	}
}