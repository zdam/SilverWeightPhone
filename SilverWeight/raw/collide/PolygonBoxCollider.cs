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
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
using Box = Silver.Weight.Raw.Shapes.Box;
using Polygon = Silver.Weight.Raw.Shapes.Polygon;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> Collide a Convex Polygon with a Box.
	/// 
	/// </summary>
	public class PolygonBoxCollider:PolygonPolygonCollider
	{
		
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public override int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			Polygon poly = (Polygon) bodyA.Shape;
			Box box = (Box) bodyB.Shape;
			
			// TODO: this can be optimized using matrix multiplications and moving only one shape
			// specifically the box, because it has fewer vertices.
			Vector2f[] vertsA = poly.GetVertices(bodyA.GetPosition(), bodyA.Rotation);
			Vector2f[] vertsB = box.GetPoints(bodyB.GetPosition(), bodyB.Rotation);
			
			// TODO: use a sweepline that has the smallest projection of the box
			// now we use just an arbitrary one
			Vector2f sweepline = new Vector2f(vertsB[1]);
			sweepline.Sub(vertsB[2]);
			
			EdgeSweep sweep = new EdgeSweep(sweepline);
			
			sweep.AddVerticesToSweep(true, vertsA);
			sweep.AddVerticesToSweep(false, vertsB);
			
			int[][] collEdgeCands = sweep.OverlappingEdges;
			//		FeaturePair[] featurePairs = getFeaturePairs(contacts.Length, vertsA, vertsB, collEdgeCands);
			//		return PopulateContacts(contacts, vertsA, vertsB, featurePairs);
			
			Intersection[][] intersections = GetIntersectionPairs(vertsA, vertsB, collEdgeCands);
			return PopulateContacts(contacts, vertsA, vertsB, intersections);
		}
	}
}