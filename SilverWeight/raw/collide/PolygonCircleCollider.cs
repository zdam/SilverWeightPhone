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
using Polygon = Silver.Weight.Raw.Shapes.Polygon;
using Line = Silver.Weight.Raw.Shapes.Line;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> Collide a circle with a convex polygon
	/// 
	/// </summary>
	public class PolygonCircleCollider:PolygonPolygonCollider
	{
		
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public override int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			Polygon polyA = (Polygon) bodyA.Shape;
			Circle circle = (Circle) bodyB.Shape;
			
			// TODO: this can be optimized using matrix multiplications and moving only the circle
			Vector2f[] vertsA = polyA.GetVertices(bodyA.GetPosition(), bodyA.Rotation);
			
			Vector2f centroidA = new Vector2f(polyA.GetCentroid());
			centroidA.Add(bodyA.GetPosition());
			
			
			int[][] collPairs = GetCollisionCandidates(vertsA, centroidA, circle.Radius, bodyB.GetPosition());
			
			int noContacts = 0;
			for (int i = 0; i < collPairs.Length; i++)
			{
				if (noContacts >= contacts.Length)
					return contacts.Length;
				
				Vector2f lineStartA = vertsA[collPairs[i][0]];
				Vector2f lineEndA = vertsA[(collPairs[i][0] + 1) % vertsA.Length];
				Line line = new Line(lineStartA, lineEndA);
				
				float dis2 = line.distanceSquared(bodyB.GetPosition());
				float r2 = circle.Radius * circle.Radius;
				
				if (dis2 < r2)
				{
					Vector2f pt = new Vector2f();
					
					line.getClosestPoint(bodyB.GetPosition(), pt);
					Vector2f normal = new Vector2f(bodyB.GetPosition());
					normal.Sub(pt);
					float sep = circle.Radius - normal.Length();
					normal.Normalise();
					
					contacts[noContacts].Separation = - sep;
					contacts[noContacts].Position = pt;
					contacts[noContacts].Normal = normal;
					contacts[noContacts].Feature = new FeaturePair();
					noContacts++;
				}
			}
			
			return noContacts;
		}
		
		/// <summary> Get the edges from a list of vertices that can Collide with the given circle.
		/// This uses a sweepline algorithm which is only efficient if some assumptions
		/// are indeed true. See CPolygonCPolygonCollider for more information.
		/// 
		/// </summary>
		/// <param name="vertsA">The vertices of a polygon that is Collided with a circle
		/// </param>
		/// <param name="centroid">The center of the polygon
		/// </param>
		/// <param name="radius">The radius of the circle
		/// </param>
		/// <param name="circlePos">The position (center) of the circle
		/// </param>
		/// <returns> The list of edges that can Collide with the circle
		/// </returns>
		protected internal virtual int[][] GetCollisionCandidates(Vector2f[] vertsA, ROVector2f centroid, float radius, ROVector2f circlePos)
		{
			Vector2f sweepDir = new Vector2f(centroid);
			sweepDir.Sub(circlePos);
			sweepDir.Normalise(); //TODO: this normalization might not be necessary
			
			EdgeSweep sweep = new EdgeSweep(sweepDir); //vertsA[0], true, true, dist);
			
			sweep.AddVerticesToSweep(true, vertsA);
			
			float circProj = circlePos.Dot(sweepDir);
			
			sweep.Insert(0, false, - radius + circProj);
			sweep.Insert(0, false, radius + circProj);
			
			return sweep.OverlappingEdges;
		}
	}
}