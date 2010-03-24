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
using Polygon = Silver.Weight.Raw.Shapes.Polygon;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> Collision detection functions for colliding two polygons.
	/// 
	/// </summary>
	public class PolygonPolygonCollider : Collider
	{
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			Polygon polyA = (Polygon) bodyA.Shape;
			Polygon polyB = (Polygon) bodyB.Shape;
			
			Vector2f[] vertsA = polyA.GetVertices(bodyA.GetPosition(), bodyA.Rotation);
			Vector2f[] vertsB = polyB.GetVertices(bodyB.GetPosition(), bodyB.Rotation);
			
			Vector2f centroidA = new Vector2f(polyA.GetCentroid());
			centroidA.Add(bodyA.GetPosition());
			Vector2f centroidB = new Vector2f(polyB.GetCentroid());
			centroidB.Add(bodyB.GetPosition());
			
			int[][] collEdgeCands = GetCollisionCandidates(vertsA, vertsB, centroidA, centroidB);
			Intersection[][] intersections = GetIntersectionPairs(vertsA, vertsB, collEdgeCands);
			return PopulateContacts(contacts, vertsA, vertsB, intersections);
		}
		
		/// <summary> Collides two polygons represented by two (already translated and rotated)
		/// lists of vertices for both vertices.
		/// This function will check for collisions between the supplied list of edge
		/// pairs and find the edges where the two polygons Intersect.
		/// 
		/// </summary>
		/// <param name="vertsA">The rotated and translated vertices of the first polygon
		/// </param>
		/// <param name="vertsB">The rotated and translated vertices of the second polygon
		/// </param>
		/// <param name="collEdgeCands">The edges of the two vertices that can Collide. Expects the
		/// same layout as returned by 
		/// {@link PolygonPolygonCollider#GetCollisionCandidates(EdgeSweep, Vector2f[], Vector2f[])}
		/// </param>
		/// <returns> The points where the two polygons overlap, with for each overlapping
		/// Area the ingoing and outgoing edges in feature pairs.
		/// </returns>
		public virtual Intersection[][] GetIntersectionPairs(Vector2f[] vertsA, Vector2f[] vertsB, int[][] collEdgeCands)
		{
			if (collEdgeCands.Length == 0)
			{
				Intersection[][] tmpArray = new Intersection[0][];
				for (int i = 0; i < 0; i++)
				{
					tmpArray[i] = new Intersection[2];
				}
				return tmpArray;
			}
			
			IntersectionGatherer fpl = new IntersectionGatherer(vertsA, vertsB);
			
			for (int i = 0; i < collEdgeCands.Length; i++)
			{
				fpl.Intersect(collEdgeCands[i][0], collEdgeCands[i][1]);
			}
			
			return fpl.IntersectionPairs;
		}
		
		/// <summary> Given a list of intersections, calculate the collision information and
		/// set the contacts with that information.
		/// 
		/// </summary>
		/// <param name="contacts">The array of contacts to fill
		/// </param>
		/// <param name="vertsA">The vertices of polygon A
		/// </param>
		/// <param name="vertsB">The vertices of polygon B
		/// </param>
		/// <param name="intersections">The array of intersections as returned by
		/// {@link PolygonPolygonCollider#GetIntersectionPairs(Vector2f[], Vector2f[], int[][])}
		/// </param>
		/// <returns> The number of contacts that have been determined and hence
		/// populated in the array.
		/// </returns>
		public virtual int PopulateContacts(Contact[] contacts, Vector2f[] vertsA, Vector2f[] vertsB, Intersection[][] intersections)
		{
			if (intersections.Length == 0)
				return 0;
			
			int noContacts = 0;
			
			for (int i = 0; i < intersections.Length; i++)
			{
				if (noContacts >= contacts.Length)
					return contacts.Length;
				
				if (intersections[i].Length == 2 && noContacts < contacts.Length - 1)
				{
					SetContactPair(contacts[noContacts], contacts[noContacts + 1], intersections[i][0], intersections[i][1], vertsA, vertsB);
					
					noContacts += 2;
				}
				else if (intersections[i].Length == 1)
				{
					SetContact(contacts[noContacts], intersections[i][0], vertsA, vertsB);
					noContacts += 1;
				}
			}
			
			return noContacts;
		}
		
		/// <summary> Set a single contact for an intersection. This is used when a contact wasn't
		/// be paired up with another. Because we know that this only happens to very 
		/// shallow penetrations, it is safe to use a separation of 0.
		/// 
		/// </summary>
		/// <param name="contact">The contact to be set
		/// </param>
		/// <param name="intersection">The intection to set the contact information for
		/// </param>
		/// <param name="vertsA">The vertices of polygon A
		/// </param>
		/// <param name="vertsB">The vertices of polygon B
		/// </param>
		public virtual void  SetContact(Contact contact, Intersection intersection, Vector2f[] vertsA, Vector2f[] vertsB)
		{
			Vector2f startA = vertsA[intersection.edgeA];
			Vector2f endA = vertsA[(intersection.edgeA + 1) % vertsA.Length];
			Vector2f startB = vertsB[intersection.edgeB];
			Vector2f endB = vertsB[(intersection.edgeB + 1) % vertsB.Length];
			
			Vector2f normal = MathUtil.GetNormal(startA, endA);
			normal.Sub(MathUtil.GetNormal(startB, endB));
			normal.Normalise();
			
			contact.Normal = normal;
			contact.Separation = 0;
			contact.Feature = new FeaturePair(intersection.edgeA, intersection.edgeB, 0, 0);
			contact.Position = intersection.position;
		}
		
		/// <summary> Calculate the collision normal and penetration depth of one intersection pair
		/// and set two contacts.
		/// 
		/// </summary>
		/// <param name="contact1">The first contact to be set
		/// </param>
		/// <param name="contact2">The first contact to be set
		/// </param>
		/// <param name="in">The ingoing intersection of the pair
		/// </param>
		/// <param name="out">The outgoing intersection of the pair
		/// </param>
		/// <param name="vertsA">The vertices of polygon A
		/// </param>
		/// <param name="vertsB">The vertices of polygon B
		/// </param>
		public virtual void  SetContactPair(Contact contact1, Contact contact2, Intersection ingoing, Intersection outgoing, Vector2f[] vertsA, Vector2f[] vertsB)
		{
			Vector2f entryPoint = ingoing.position;
			Vector2f exitPoint = outgoing.position;
			
			Vector2f normal = MathUtil.GetNormal(entryPoint, exitPoint);
			
			FeaturePair feature = new FeaturePair(ingoing.edgeA, ingoing.edgeB, outgoing.edgeA, outgoing.edgeB);
			
			float separation = - PenetrationSweep.GetPenetrationDepth(ingoing, outgoing, normal, vertsA, vertsB);
			// divided by 2 because there are two contact points
			// divided by 2 (again) because both objects Move (I think)
			separation /= 4;
			
			contact1.Separation = separation;
			contact1.Normal = normal;
			contact1.Position = entryPoint;
			contact1.Feature = feature;
			
			contact2.Separation = separation;
			contact2.Normal = normal;
			contact2.Position = exitPoint;
			contact2.Feature = feature;
		}
		
		
		
		/// <summary> This function finds pairs of edges of two polygons by projecting all the
		/// edges on a line. Essentially this is just an optimization to minimize the
		/// number of line-line intersections that is tested for collisions.
		/// 
		/// </summary>
		/// <param name="sweep">The sweepline object to use, this allows a user of this function to Add other vertices
		/// </param>
		/// <param name="vertsA">The vertices of the first polygon ordered counterclockwise (TODO: verify this/order matters?)
		/// </param>
		/// <param name="vertsB">The vertices of the second polygon ordered counterclockwise
		/// </param>
		/// <returns> 
		/// The pairs of vertices that overlap in the sweepline and are therefore collision candidates.
		/// The returned array is of a shape int[n][2] where n is the number of overlapping edges.
		/// For a returned array r
		/// the edge between vertsA[r[x][0]] and vertsA[r[x][0] + 1]
		/// overlaps with vertsB[r[x][1]] and vertsB[r[x][1] + 1].
		/// </returns>
		public virtual int[][] GetCollisionCandidates(EdgeSweep sweep, Vector2f[] vertsA, Vector2f[] vertsB)
		{
			sweep.AddVerticesToSweep(true, vertsA);
			sweep.AddVerticesToSweep(false, vertsB);
			
			return sweep.OverlappingEdges;
		}
		
		/// <summary> This function finds pairs of edges of two polygons by projecting all the
		/// edges on a line. Essentially this is just an optimization to minimize the
		/// number of line-line intersections that is tested for collisions.
		/// 
		/// This version simply calls 
		/// {@link PolygonPolygonCollider#GetCollisionCandidates(EdgeSweep, Vector2f[], Vector2f[]) }
		/// with a new empty EdgeSweep.
		/// 
		/// </summary>
		/// <param name="vertsA">The vertices of the first polygon ordered counterclockwise (TODO: verify this/order matters?)
		/// </param>
		/// <param name="sweepDirStart">The 'real' center of the first polygon
		/// </param>
		/// <param name="vertsB">The vertices of the second polygon ordered counterclockwise
		/// </param>
		/// <param name="sweepDirEnd">The 'real' center of the second polygon
		/// </param>
		/// <returns> 
		/// The pairs of vertices that overlap in the sweepline and are therefore collision candidates.
		/// The returned array is of a shape int[n][2] where n is the number of overlapping edges.
		/// For a returned array r
		/// the edge between vertsA[r[x][0]] and vertsA[r[x][0] + 1]
		/// overlaps with vertsB[r[x][1]] and vertsB[r[x][1] + 1].
		/// </returns>
		public virtual int[][] GetCollisionCandidates(Vector2f[] vertsA, Vector2f[] vertsB, Vector2f sweepDirStart, Vector2f sweepDirEnd)
		{
			Vector2f sweepDir = new Vector2f(sweepDirEnd);
			sweepDir.Sub(sweepDirStart);
			
			return GetCollisionCandidates(new EdgeSweep(sweepDir), vertsA, vertsB);
		}
	}
}