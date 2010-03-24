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
using Line = Silver.Weight.Raw.Shapes.Line;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> The logic for checking lines against boxes
	/// 
	/// </summary>
	public class LineBoxCollider : Collider
	{
		/// <summary>The single instance of this class </summary>
		private static LineBoxCollider single = new LineBoxCollider();
		
		/// <summary> Create a new collider - place holder in case the collider
		/// becomes stateful later.
		/// 
		/// </summary>
		/// <returns> The new collider
		/// </returns>
		public static LineBoxCollider create()
		{
			return single;
		}
		
		/// <summary> Get the proportion that the src vector is of 
		/// the DENominator vector
		/// 
		/// </summary>
		/// <param name="src">The source vector
		/// </param>
		/// <param name="den">The denominator vector
		/// </param>
		/// <returns> The proportion of the den that src is
		/// </returns>
		private float GetProp(Vector2f src, Vector2f den)
		{
			if ((den.X == 0) && (den.Y == 0))
			{
				return 0;
			}
			
			if (den.X != 0)
			{
				return src.X / den.X;
			}
			
			return src.Y / den.Y;
		}
		
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			int numContacts = 0;
			
			Line line = (Line) bodyA.Shape;
			Box box = (Box) bodyB.Shape;
			
			Vector2f lineVec = new Vector2f(line.DX, line.DY);
			lineVec.Normalise();
			Vector2f axis = new Vector2f(- line.DY, line.DX);
			axis.Normalise();
			
			Vector2f res = new Vector2f();
			line.Start.ProjectOntoUnit(axis, res);
			float linePos = GetProp(res, axis);
			
			Vector2f c = MathUtil.Sub(bodyB.GetPosition(), bodyA.GetPosition());
			c.ProjectOntoUnit(axis, res);
			float centre = GetProp(res, axis);
			
			Vector2f[] pts = box.GetPoints(bodyB.GetPosition(), bodyB.Rotation);
			float[] tangent = new float[4];
			float[] proj = new float[4];
			
			int outOfRange = 0;
			
			for (int i = 0; i < 4; i++)
			{
				pts[i].Sub(bodyA.GetPosition());
				pts[i].ProjectOntoUnit(axis, res);
				tangent[i] = GetProp(res, axis);
				pts[i].ProjectOntoUnit(lineVec, res);
				proj[i] = GetProp(res, new Vector2f(line.DX, line.DY));
				
				if ((proj[i] >= 1) || (proj[i] <= 0))
				{
					outOfRange++;
				}
			}
			if (outOfRange == 4)
			{
				return 0;
			}
			
			Vector2f normal = new Vector2f(axis);
			
			if (centre < linePos)
			{
				if (!line.BlocksInnerEdge)
				{
					return 0;
				}
				
				normal.Scale(- 1);
				for (int i = 0; i < 4; i++)
				{
					if (tangent[i] > linePos)
					{
						if (proj[i] < 0)
						{
							Vector2f onAxis = new Vector2f();
							Line leftLine = new Line(GetPt(pts, i - 1), pts[i]);
							Line rightLine = new Line(GetPt(pts, i + 1), pts[i]);
							leftLine.getClosestPoint(line.Start, res);
							res.ProjectOntoUnit(axis, onAxis);
							float left = GetProp(onAxis, axis);
							rightLine.getClosestPoint(line.Start, res);
							res.ProjectOntoUnit(axis, onAxis);
							float right = GetProp(onAxis, axis);
							
							if ((left > 0) && (right > 0))
							{
								Vector2f pos = new Vector2f(bodyA.GetPosition());
								pos.Add(line.Start);
								
								ResolveEndPointCollision(pos, bodyA, bodyB, normal, leftLine, rightLine, contacts[numContacts], i);
								numContacts++;
							}
						}
						else if (proj[i] > 1)
						{
							Vector2f onAxis = new Vector2f();
							Line leftLine = new Line(GetPt(pts, i - 1), pts[i]);
							Line rightLine = new Line(GetPt(pts, i + 1), pts[i]);
							leftLine.getClosestPoint(line.End, res);
							res.ProjectOntoUnit(axis, onAxis);
							float left = GetProp(onAxis, axis);
							rightLine.getClosestPoint(line.End, res);
							res.ProjectOntoUnit(axis, onAxis);
							float right = GetProp(onAxis, axis);
							
							if ((left > 0) && (right > 0))
							{
								Vector2f pos = new Vector2f(bodyA.GetPosition());
								pos.Add(line.End);
								
								ResolveEndPointCollision(pos, bodyA, bodyB, normal, leftLine, rightLine, contacts[numContacts], i);
								numContacts++;
							}
						}
						else
						{
							pts[i].ProjectOntoUnit(lineVec, res);
							res.Add(bodyA.GetPosition());
							contacts[numContacts].Separation = - (tangent[i] - linePos);
							contacts[numContacts].Position = new Vector2f(res);
							contacts[numContacts].Normal = normal;
							contacts[numContacts].Feature = new FeaturePair(i);
							numContacts++;
						}
					}
				}
			}
			else
			{
				if (!line.BlocksOuterEdge)
				{
					return 0;
				}
				
				for (int i = 0; i < 4; i++)
				{
					if (tangent[i] < linePos)
					{
						if (proj[i] < 0)
						{
							Vector2f onAxis = new Vector2f();
							Line leftLine = new Line(GetPt(pts, i - 1), pts[i]);
							Line rightLine = new Line(GetPt(pts, i + 1), pts[i]);
							leftLine.getClosestPoint(line.Start, res);
							res.ProjectOntoUnit(axis, onAxis);
							float left = GetProp(onAxis, axis);
							rightLine.getClosestPoint(line.Start, res);
							res.ProjectOntoUnit(axis, onAxis);
							float right = GetProp(onAxis, axis);
							
							if ((left < 0) && (right < 0))
							{
								Vector2f pos = new Vector2f(bodyA.GetPosition());
								pos.Add(line.Start);
								
								ResolveEndPointCollision(pos, bodyA, bodyB, normal, leftLine, rightLine, contacts[numContacts], i);
								numContacts++;
							}
						}
						else if (proj[i] > 1)
						{
							Vector2f onAxis = new Vector2f();
							Line leftLine = new Line(GetPt(pts, i - 1), pts[i]);
							Line rightLine = new Line(GetPt(pts, i + 1), pts[i]);
							leftLine.getClosestPoint(line.End, res);
							res.ProjectOntoUnit(axis, onAxis);
							float left = GetProp(onAxis, axis);
							rightLine.getClosestPoint(line.End, res);
							res.ProjectOntoUnit(axis, onAxis);
							float right = GetProp(onAxis, axis);
							
							if ((left < 0) && (right < 0))
							{
								Vector2f pos = new Vector2f(bodyA.GetPosition());
								pos.Add(line.End);
								
								ResolveEndPointCollision(pos, bodyA, bodyB, normal, leftLine, rightLine, contacts[numContacts], i);
								numContacts++;
							}
						}
						else
						{
							pts[i].ProjectOntoUnit(lineVec, res);
							res.Add(bodyA.GetPosition());
							contacts[numContacts].Separation = - (linePos - tangent[i]);
							contacts[numContacts].Position = new Vector2f(res);
							contacts[numContacts].Normal = normal;
							contacts[numContacts].Feature = new FeaturePair();
							numContacts++;
						}
					}
				}
			}
			
			if (numContacts > 2)
			{
				throw new System.SystemException("LineBoxCollision: > 2 contacts");
			}
			
			return numContacts;
		}
		
		/// <summary> Resolve the collision math around an end point
		/// 
		/// </summary>
		/// <param name="pos">The position of the contact
		/// </param>
		/// <param name="bodyA">The first body in the collision
		/// </param>
		/// <param name="bodyB">The second body in the collision
		/// </param>
		/// <param name="leftLine">The line to the left of the vertex of collision
		/// </param>
		/// <param name="rightLine">The line to the right of the vertex of collision
		/// </param>
		/// <param name="contact">The contact to populate
		/// </param>
		/// <param name="norm">The normal determined for the line
		/// </param>
		/// <param name="i">The index of teh face we're resolving for feature ID
		/// </param>
		private void  ResolveEndPointCollision(Vector2f pos, Body bodyA, Body bodyB, Vector2f norm, Line leftLine, Line rightLine, Contact contact, int i)
		{
			Vector2f start = new Vector2f(pos);
			Vector2f end = new Vector2f(start);
			end.Add(norm);
			
			rightLine.move(bodyA.GetPosition());
			leftLine.move(bodyA.GetPosition());
			Line normLine = new Line(start, end);
			Vector2f rightPoint = normLine.intersect(rightLine);
			Vector2f leftPoint = normLine.intersect(leftLine);
			
			float dis1 = System.Single.MaxValue;
			if (rightPoint != null)
			{
				dis1 = rightPoint.Distance(start) - norm.Length();
			}
			float dis2 = System.Single.MaxValue;
			if (leftPoint != null)
			{
				dis2 = leftPoint.Distance(start) - norm.Length();
			}
			
			norm.Normalise();
			float dis = System.Math.Min(dis1, dis2);
			
			contact.Separation = - dis;
			contact.Position = pos;
			contact.Normal = norm;
			contact.Feature = new FeaturePair(i);
		}
		/// <summary> Get a specified point in the array using wrap round
		/// 
		/// </summary>
		/// <param name="pts">The points array to access
		/// </param>
		/// <param name="index">The index into the array to retrieve (negative and > Length
		/// will be resolved)
		/// </param>
		/// <returns> The vector at the index requested
		/// </returns>
		private Vector2f GetPt(Vector2f[] pts, int index)
		{
			if (index < 0)
			{
				index += pts.Length;
			}
			if (index >= pts.Length)
			{
				index -= pts.Length;
			}
			
			return pts[index];
		}
	}
}