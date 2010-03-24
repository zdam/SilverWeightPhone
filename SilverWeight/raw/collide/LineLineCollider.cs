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
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> Collides two lines with oneanother.
	/// 
	/// </summary>
	public class LineLineCollider : Collider
	{
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public virtual int Collide(Contact[] contacts, Body bodyA, Body bodyB)
		{
			// TODO: function disabled until we can remember on what side of A,
			// B used to be, which is crucial to determine a proper collision normal
			return 0;
			//		Line lineA = (Line) bodyA.getShape();
			//		Line lineB = (Line) bodyB.getShape();
			//		
			//		Vector2f[] vertsA = lineA.GetVertices(bodyA.GetPosition(), bodyA.getRotation());
			//		Vector2f[] vertsB = lineB.GetVertices(bodyB.GetPosition(), bodyB.getRotation());
			//		
			//		Vector2f startA = vertsA[0];
			//		Vector2f endA = vertsA[1];
			//		Vector2f startB = vertsB[0];
			//		Vector2f endB = vertsB[1];
			//		
			//		//TODO: reuse mathutil.Intersect?
			//		float d = (endB.y - startB.y) * (endA.x - startA.x) - (endB.x - startB.x) * (endA.y - startA.y);
			//		
			//		if ( d == 0 ) // parallel lines
			//			return 0;
			//		
			//		float uA = (endB.x - startB.x) * (startA.y - startB.y) - (endB.y - startB.y) * (startA.x - startB.x);
			//		uA /= d;
			//		float uB = (endA.x - startA.x) * (startA.y - startB.y) - (endA.y - startA.y) * (startA.x - startB.x);
			//		uB /= d;
			//		
			//		if ( uA < 0 || uA > 1 || uB < 0 || uB > 1 ) 
			//			return 0; // intersection point isn't between the start and endpoints
			//		
			//		// there must be a collision, let's determine our contact information
			//		// we're searching for a contact with the smallest penetration depth
			//		Vector2f[][] closestPoints = {
			//				{startB, GetClosestPoint(startA, endA, startB)},
			//				{endB, GetClosestPoint(startA, endA, endB)},
			//				{startA, GetClosestPoint(startB, endB, startA)},
			//				{endA, GetClosestPoint(startB, endB, endA)}
			//		};
			//		
			//		float distSquared = Float.MAX_VALUE;
			//		Vector2f position = null;
			//		Vector2f normal = new Vector2f();
			//		
			//		for ( int i = 0; i < 4; i++ ) {
			//			Vector2f l;
			//			if ( i < 2 ) {
			//				l = closestPoints[i][1];
			//				l.Sub(closestPoints[i][0]);
			//			} else {
			//				l = closestPoints[i][0];
			//				l.Sub(closestPoints[i][1]);
			//			}
			//			
			//			float newDistSquared = l.LengthSquared();
			//			if ( newDistSquared < distSquared ) {
			//				distSquared = newDistSquared;
			//				position = closestPoints[i][0];
			//				normal.set(l);
			//			}
			//		}
			//		
			//		normal.Normalise();
			//		contacts[0].setNormal(normal);
			//		contacts[0].SetPosition(position);
			//		if ( Math.sqrt(distSquared) > 10f )
			//			System.out.println(Math.sqrt(distSquared));
			//		contacts[0].setSeparation((float) -Math.sqrt(distSquared));
			//		
			//		return 1;
		}
		
		/// <summary> Gets the closest point to a given point on the indefinately extended line.
		/// TODO: Move this somewhere in math package
		/// 
		/// </summary>
		/// <param name="startA">Starting point of the line
		/// </param>
		/// <param name="endA">End point of the line
		/// </param>
		/// <param name="point">The point to get a closes point on the line for
		/// </param>
		/// <returns> the closest point on the line or null if the lines are parallel
		/// </returns>
		public static Vector2f GetClosestPoint(Vector2f startA, Vector2f endA, Vector2f point)
		{
			Vector2f startB = point;
			Vector2f endB = new Vector2f(endA);
			endB.Sub(startA);
			endB.Reconfigure(endB.y, - endB.x);
			
			float d = endB.y * (endA.x - startA.x);
			d -= endB.x * (endA.y - startA.y);
			
			if (d == 0)
				return null;
			
			float uA = endB.x * (startA.y - startB.Y);
			uA -= endB.y * (startA.x - startB.X);
			uA /= d;
			
			return new Vector2f(startA.x + uA * (endA.x - startA.x), startA.y + uA * (endA.y - startA.y));
		}
	}
}