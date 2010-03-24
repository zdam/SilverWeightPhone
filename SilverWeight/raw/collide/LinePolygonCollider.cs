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
using Line = Silver.Weight.Raw.Shapes.Line;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> Collider for a Line and a Convex Polygon.
	/// 
	/// </summary>
	public class LinePolygonCollider:PolygonPolygonCollider
	{
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public override int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			Line line = (Line) bodyA.Shape;
			Polygon poly = (Polygon) bodyB.Shape;
			
			// TODO: this can be optimized using matrix multiplications and moving only one shape
			// specifically the line, because it has only two vertices
			Vector2f[] vertsA = line.getVertices(bodyA.GetPosition(), bodyA.Rotation);
			Vector2f[] vertsB = poly.GetVertices(bodyB.GetPosition(), bodyB.Rotation);
			
			Vector2f pos = poly.GetCentroid(bodyB.GetPosition(), bodyB.Rotation);
			
			// using the z axis of a 3d Cross product we determine on what side B is
			bool isLeftOf = 0 > (pos.x - vertsA[0].x) * (vertsA[1].y - vertsA[0].y) - (vertsA[1].x - vertsA[0].x) * (pos.y - vertsA[0].y);
			
			// to get the proper intersection pairs we make sure 
			// the line's normal is pointing towards the polygon
			// TODO: verify that it's not actually pointing in the opposite direction
			if (isLeftOf)
			{
				Vector2f tmp = vertsA[0];
				vertsA[0] = vertsA[1];
				vertsA[1] = tmp;
			}
			
			// we use the line's normal for our sweepline projection
			Vector2f normal = new Vector2f(vertsA[1]);
			normal.Sub(vertsA[0]);
			normal.Reconfigure(normal.y, - normal.x);
			EdgeSweep sweep = new EdgeSweep(normal);
			sweep.Insert(0, true, vertsA[0].Dot(normal));
			sweep.Insert(0, true, vertsA[1].Dot(normal));
			sweep.AddVerticesToSweep(false, vertsB);
			int[][] collEdgeCands = sweep.OverlappingEdges;
			
			IntersectionGatherer intGath = new IntersectionGatherer(vertsA, vertsB);
			for (int i = 0; i < collEdgeCands.Length; i++)
				intGath.Intersect(collEdgeCands[i][0], collEdgeCands[i][1]);
			
			Intersection[] intersections = intGath.Intersections;
			
			return PopulateContacts(contacts, vertsA, vertsB, intersections);
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
		/// <param name="intersections">The array of intersection as returned by 
		/// {@link IntersectionGatherer#getIntersections()}
		/// </param>
		/// <returns> The number of contacts that have been set in the contact array
		/// </returns>
		public virtual int PopulateContacts(Contact[] contacts, Vector2f[] vertsA, Vector2f[] vertsB, Intersection[] intersections)
		{
			if (intersections.Length == 0)
				return 0;
			
			int noContacts = 0;
			
			// is the first intersection outgoing?
			if (!intersections[0].isIngoing)
			{
				SetLineEndContact(contacts[noContacts], intersections[intersections.Length - 1], vertsA, vertsB);
				
				if (contacts[noContacts].Separation < - 10)
					System.Console.Out.WriteLine("first " + contacts[noContacts].Separation);
				
				noContacts++;
			}
			
			
			int i = noContacts;
			while (i < intersections.Length - 1)
			{
				if (noContacts > contacts.Length - 2)
					return noContacts;
				
				// check if we have an intersection pair
				if (!intersections[i].isIngoing || intersections[i + 1].isIngoing)
				{
					SetContact(contacts[noContacts], intersections[i], vertsA, vertsB);
					i++;
					noContacts++;
					continue;
				}
				
				SetContactPair(contacts[noContacts], contacts[noContacts + 1], intersections[i], intersections[i + 1], vertsA, vertsB);
				
				if (contacts[noContacts].Separation < - 10)
					System.Console.Out.WriteLine("m " + contacts[noContacts].Separation);
				
				noContacts += 2;
				i += 2;
			}
			
			// is there still an ingoing intersection left?
			if (i < intersections.Length && intersections[intersections.Length - 1].isIngoing && noContacts < contacts.Length)
			{
				SetLineEndContact(contacts[noContacts], intersections[intersections.Length - 1], vertsA, vertsB);
				
				if (contacts[noContacts].Separation < - 10)
					System.Console.Out.WriteLine(" last " + contacts[noContacts].Separation);
				noContacts++;
			}
			
			
			return noContacts;
		}
		
		/// <summary> Set a contact for an intersection where the colliding line's start- or endpoint
		/// is contained in the colliding polygon.
		/// 
		/// TODO: The current implementation doesn't work properly: because lines are very
		/// thin, they can slide into a polygon sideways which gives a very deep penetration
		/// |
		/// |->
		/// |      +-----+
		/// |->    |     |
		/// |      |     |
		/// |     |
		/// +-----+
		/// 
		/// A possible solution would be to use the velocity of the line relative to the 
		/// polygon to construct a collision normal and penetration depth.
		/// Another possibility is to use the line's normals (both directions) and calculate
		/// proper intersection distances for them.
		/// If one has multiple normals/penetration depths to choose from, the one with the
		/// minimum penetration depth will probably be the best bet.
		/// 
		/// </summary>
		/// <param name="contact">The contact to set
		/// </param>
		/// <param name="intersection">The intersection where the line enters or exits the polygon
		/// </param>
		/// <param name="vertsA">The line's vertices
		/// </param>
		/// <param name="vertsB">The polygon's vertices
		/// </param>
		public virtual void  SetLineEndContact(Contact contact, Intersection intersection, Vector2f[] vertsA, Vector2f[] vertsB)
		{
			Vector2f separation = new Vector2f(intersection.position);
			if (intersection.isIngoing)
				separation.Sub(vertsA[1]);
			else
				separation.Sub(vertsA[0]);
			
			float depthA = 0; //separation.Length();
			
			contact.Separation = - depthA;
			contact.Normal = MathUtil.GetNormal(vertsB[(intersection.edgeB + 1) % vertsB.Length], vertsB[intersection.edgeB]);
			contact.Position = intersection.position;
			contact.Feature = new FeaturePair(0, 0, intersection.edgeA, intersection.edgeB);
		}
	}
}