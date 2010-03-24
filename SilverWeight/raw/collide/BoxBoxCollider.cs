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
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
using Box = Silver.Weight.Raw.Shapes.Box;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> The implementation of box to box collision. The create() method is
	/// used as a factory to produce the collider instance.
	/// 
	/// Currently the collider is stateless so a single instance is 
	/// returned each time.
	/// 
	/// </summary>
	public class BoxBoxCollider : Collider
	{
		
		//	 Box vertex and edge numbering:
		//
		//	        ^ y
		//	        |
		//	        e1
		//	   v2 ------ v1
		//	    |        |
		//	 e2 |        | e4  --> x
		//	    |        |
		//	   v3 ------ v4
		//	        e3
		
		/// <summary>The identifier for the x coordinate of the first face </summary>
		public const int FACE_A_X = 1;
		/// <summary>The identifier for the y coordinate of the first face </summary>
		public const int FACE_A_Y = 2;
		/// <summary>The identifier for the x coordinate of the second face </summary>
		public const int FACE_B_X = 3;
		/// <summary>The identifier for the y coordinate of the second face </summary>
		public const int FACE_B_Y = 4;
		
		/// <summary>The identifier indicating no edges Collided </summary>
		public const int NO_EDGE = 0;
		/// <summary>The identifier indicating the first edge collides </summary>
		public const int EDGE1 = 1;
		/// <summary>The identifier indicating the second edge collides </summary>
		public const int EDGE2 = 2;
		/// <summary>The identifier indicating the third edge collides </summary>
		public const int EDGE3 = 3;
		/// <summary>The identifier indicating the forth edge collides </summary>
		public const int EDGE4 = 4;
		
		/// <summary>Temp vector </summary>
		private static Vector2f hA = new Vector2f();
		/// <summary>Temp vector </summary>
		private static Vector2f hB = new Vector2f();
		
		/// <summary> A simple structure describe a vertex against which the
		/// shape should be clipped
		/// 
		/// </summary>
		private class ClipVertex
		{
			/// <summary>The vertex </summary>
			internal Vector2f v = new Vector2f();
			/// <summary>The pair this clipping applied to </summary>
			internal FeaturePair fp = new FeaturePair();
		}
		
		
		/// <summary> Swap the two body edges within a feature pair over
		/// 
		/// </summary>
		/// <param name="fp">The feature pair to Flip
		/// </param>
		private void  Flip(FeaturePair fp)
		{
			int temp = fp.inEdge1;
			fp.inEdge1 = fp.inEdge2;
			fp.inEdge2 = temp;
			
			temp = fp.outEdge1;
			fp.outEdge1 = fp.outEdge2;
			fp.outEdge2 = temp;
		}
		
		/// <summary> Clip a line segment against a line
		/// 
		/// </summary>
		/// <param name="vOut">The segment to be clipped
		/// </param>
		/// <param name="vIn">The line to be clipped against
		/// </param>
		/// <param name="normal">The normal of the line
		/// </param>
		/// <param name="offset">The offset from segment to line 
		/// </param>
		/// <param name="clipEdge">The edge against which we're clipping
		/// </param>
		/// <returns> The number of points we've clipped
		/// </returns>
		private int ClipSegmentToLine(ClipVertex[] vOut, ClipVertex[] vIn, Vector2f normal, float offset, char clipEdge)
		{
			// Start with no output points
			int numOut = 0;
			
			// Calculate the Distance of end points to the line
			float distance0 = normal.Dot(vIn[0].v) - offset;
			float distance1 = normal.Dot(vIn[1].v) - offset;
			
			// If the points are behind the plane
			if (distance0 <= 0.0f)
				vOut[numOut++] = vIn[0];
			if (distance1 <= 0.0f)
				vOut[numOut++] = vIn[1];
			
			// If the points are on different sides of the plane
			if (distance0 * distance1 < 0.0f)
			{
				// Find intersection point of edge and plane
				float interp = distance0 / (distance0 - distance1);
				vOut[numOut].v = MathUtil.Scale(MathUtil.Sub(vIn[1].v, vIn[0].v), interp);
				vOut[numOut].v.Add(vIn[0].v);
				
				if (distance0 > 0.0f)
				{
					vOut[numOut].fp = vIn[0].fp;
					vOut[numOut].fp.inEdge1 = clipEdge;
					vOut[numOut].fp.inEdge2 = NO_EDGE;
				}
				else
				{
					vOut[numOut].fp = vIn[1].fp;
					vOut[numOut].fp.outEdge1 = clipEdge;
					vOut[numOut].fp.outEdge2 = NO_EDGE;
				}
				++numOut;
			}
			
			return numOut;
		}
		
		/// <summary> ??
		/// 
		/// </summary>
		/// <param name="c">
		/// </param>
		/// <param name="h">
		/// </param>
		/// <param name="pos">
		/// </param>
		/// <param name="rot">
		/// </param>
		/// <param name="normal">
		/// </param>
		private void  ComputeIncidentEdge(ClipVertex[] c, ROVector2f h, ROVector2f pos, Matrix2f rot, Vector2f normal)
		{
			// The normal is from the reference box. Convert it
			// to the incident boxe's frame and Flip Sign.
			Matrix2f rotT = rot.Transpose();
			Vector2f n = MathUtil.Scale(MathUtil.Mul(rotT, normal), - 1);
			Vector2f nAbs = MathUtil.Abs(n);
			
			if (nAbs.x > nAbs.y)
			{
				if (MathUtil.Sign(n.x) > 0.0f)
				{
					c[0].v.Reconfigure(h.X, - h.Y);
					c[0].fp.inEdge2 = EDGE3;
					c[0].fp.outEdge2 = EDGE4;
					
					c[1].v.Reconfigure(h.X, h.Y);
					c[1].fp.inEdge2 = EDGE4;
					c[1].fp.outEdge2 = EDGE1;
				}
				else
				{
					c[0].v.Reconfigure(- h.X, h.Y);
					c[0].fp.inEdge2 = EDGE1;
					c[0].fp.outEdge2 = EDGE2;
					
					c[1].v.Reconfigure(- h.X, - h.Y);
					c[1].fp.inEdge2 = EDGE2;
					c[1].fp.outEdge2 = EDGE3;
				}
			}
			else
			{
				if (MathUtil.Sign(n.y) > 0.0f)
				{
					c[0].v.Reconfigure(h.X, h.Y);
					c[0].fp.inEdge2 = EDGE4;
					c[0].fp.outEdge2 = EDGE1;
					
					c[1].v.Reconfigure(- h.X, h.Y);
					c[1].fp.inEdge2 = EDGE1;
					c[1].fp.outEdge2 = EDGE2;
				}
				else
				{
					c[0].v.Reconfigure(- h.X, - h.Y);
					c[0].fp.inEdge2 = EDGE2;
					c[0].fp.outEdge2 = EDGE3;
					
					c[1].v.Reconfigure(h.X, - h.Y);
					c[1].fp.inEdge2 = EDGE3;
					c[1].fp.outEdge2 = EDGE4;
				}
			}
			
			c[0].v = MathUtil.Mul(rot, c[0].v);
			c[0].v.Add(pos);
			
			c[1].v = MathUtil.Mul(rot, c[1].v);
			c[1].v.Add(pos);
		}
		
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
			
			// Setup
			hA.Reconfigure(((Box) bodyA.Shape).Size);
			hA.Scale(0.5f);
			//Vector2f hA = MathUtil.Scale(((Box) bodyA.getShape()).getSize(), 0.5f);
			hB.Reconfigure(((Box) bodyB.Shape).Size);
			hB.Scale(0.5f);
			//Vector2f hB = MathUtil.Scale(((Box) bodyB.getShape()).getSize(), 0.5f);
			//Vector2f hA = MathUtil.Scale(bodyA.getSize(), 0.5f);
			//Vector2f hB = MathUtil.Scale(bodyB.getSize(), 0.5f);
			
			ROVector2f posA = bodyA.GetPosition();
			ROVector2f posB = bodyB.GetPosition();
			
			Matrix2f rotA = new Matrix2f(bodyA.Rotation);
			Matrix2f rotB = new Matrix2f(bodyB.Rotation);
			
			Matrix2f RotAT = rotA.Transpose();
			Matrix2f RotBT = rotB.Transpose();
			
			// unused?
			//		Vector2f a1 = rotA.col1;
			//		Vector2f a2 = rotA.col2;
			//		Vector2f b1 = rotB.col1;
			//		Vector2f b2 = rotB.col2;
			
			Vector2f dp = MathUtil.Sub(posB, posA);
			Vector2f dA = MathUtil.Mul(RotAT, dp);
			Vector2f dB = MathUtil.Mul(RotBT, dp);
			
			Matrix2f C = MathUtil.Mul(RotAT, rotB);
			Matrix2f absC = MathUtil.Abs(C);
			Matrix2f absCT = absC.Transpose();
			
			// Box A faces
			Vector2f faceA = MathUtil.Abs(dA);
			faceA.Sub(hA);
			faceA.Sub(MathUtil.Mul(absC, hB));
			
			if (faceA.x > 0.0f || faceA.y > 0.0f)
			{
				return 0;
			}
			
			// Box B faces
			Vector2f faceB = MathUtil.Abs(dB);
			faceB.Sub(MathUtil.Mul(absCT, hA));
			faceB.Sub(hB);
			//MathUtil.Sub(MathUtil.Sub(MathUtil.Abs(dB),MathUtil.Mul(absCT,hA)),hB);
			if (faceB.x > 0.0f || faceB.y > 0.0f)
			{
				return 0;
			}
			
			// Find best axis
			int axis;
			float separation;
			Vector2f normal;
			
			// Box A faces
			axis = FACE_A_X;
			separation = faceA.x;
			normal = dA.x > 0.0f?rotA.col1:MathUtil.Scale(rotA.col1, - 1);
			
			if (faceA.y > 1.05f * separation + 0.01f * hA.y)
			{
				axis = FACE_A_Y;
				separation = faceA.y;
				normal = dA.y > 0.0f?rotA.col2:MathUtil.Scale(rotA.col2, - 1);
			}
			
			// Box B faces
			if (faceB.x > 1.05f * separation + 0.01f * hB.x)
			{
				axis = FACE_B_X;
				separation = faceB.x;
				normal = dB.x > 0.0f?rotB.col1:MathUtil.Scale(rotB.col1, - 1);
			}
			
			if (faceB.y > 1.05f * separation + 0.01f * hB.y)
			{
				axis = FACE_B_Y;
				separation = faceB.y;
				normal = dB.y > 0.0f?rotB.col2:MathUtil.Scale(rotB.col2, - 1);
			}
			
			// Setup clipping plane data based on the separating axis
			Vector2f frontNormal, sideNormal;
			ClipVertex[] incidentEdge = new ClipVertex[]{new ClipVertex(), new ClipVertex()};
			float front, negSide, posSide;
			char negEdge, posEdge;
			
			// Compute the clipping lines and the line segment to be clipped.
			switch (axis)
			{
				
				case FACE_A_X: 
					{
						frontNormal = normal;
						front = posA.Dot(frontNormal) + hA.x;
						sideNormal = rotA.col2;
						float side = posA.Dot(sideNormal);
						negSide = - side + hA.y;
						posSide = side + hA.y;
						negEdge = (char) (EDGE3);
						posEdge = (char) (EDGE1);
						ComputeIncidentEdge(incidentEdge, hB, posB, rotB, frontNormal);
					}
					break;
				
				
				case FACE_A_Y: 
					{
						frontNormal = normal;
						front = posA.Dot(frontNormal) + hA.y;
						sideNormal = rotA.col1;
						float side = posA.Dot(sideNormal);
						negSide = - side + hA.x;
						posSide = side + hA.x;
						negEdge = (char) (EDGE2);
						posEdge = (char) (EDGE4);
						ComputeIncidentEdge(incidentEdge, hB, posB, rotB, frontNormal);
					}
					break;
				
				
				case FACE_B_X: 
					{
						frontNormal = MathUtil.Scale(normal, - 1);
						front = posB.Dot(frontNormal) + hB.x;
						sideNormal = rotB.col2;
						float side = posB.Dot(sideNormal);
						negSide = - side + hB.y;
						posSide = side + hB.y;
						negEdge = (char) (EDGE3);
						posEdge = (char) (EDGE1);
						ComputeIncidentEdge(incidentEdge, hA, posA, rotA, frontNormal);
					}
					break;
				
				
				case FACE_B_Y: 
					{
						frontNormal = MathUtil.Scale(normal, - 1);
						front = posB.Dot(frontNormal) + hB.y;
						sideNormal = rotB.col1;
						float side = posB.Dot(sideNormal);
						negSide = - side + hB.x;
						posSide = side + hB.x;
						negEdge = (char) (EDGE2);
						posEdge = (char) (EDGE4);
						ComputeIncidentEdge(incidentEdge, hA, posA, rotA, frontNormal);
					}
					break;
				
				default: 
					throw new System.SystemException("Unknown face!");
				
			}
			
			// clip other face with 5 box planes (1 face plane, 4 edge planes)
			
			ClipVertex[] clipPoints1 = new ClipVertex[]{new ClipVertex(), new ClipVertex()};
			ClipVertex[] clipPoints2 = new ClipVertex[]{new ClipVertex(), new ClipVertex()};
			int np;
			
			// Clip to box side 1
			np = ClipSegmentToLine(clipPoints1, incidentEdge, MathUtil.Scale(sideNormal, - 1), negSide, negEdge);
			
			if (np < 2)
				return 0;
			
			// Clip to negative box side 1
			np = ClipSegmentToLine(clipPoints2, clipPoints1, sideNormal, posSide, posEdge);
			
			if (np < 2)
				return 0;
			
			// Now clipPoints2 Contains the clipping points.
			// Due to roundoff, it is possible that clipping removes all points.
			
			int numContacts = 0;
			for (int i = 0; i < 2; ++i)
			{
				float separation2 = frontNormal.Dot(clipPoints2[i].v) - front;
				
				if (separation2 <= 0)
				{
					contacts[numContacts].Separation = separation2;
					contacts[numContacts].Normal = normal;
					// slide contact point onto reference face (easy to cull)
					contacts[numContacts].Position = MathUtil.Sub(clipPoints2[i].v, MathUtil.Scale(frontNormal, separation2));
					contacts[numContacts].Feature = clipPoints2[i].fp;
					if (axis == FACE_B_X || axis == FACE_B_Y)
						Flip(contacts[numContacts].Feature);
					++numContacts;
				}
			}
			
			return numContacts;
		}
	}
}