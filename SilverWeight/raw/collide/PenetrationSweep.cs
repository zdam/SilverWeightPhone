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
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> This class will, given an intersection pair (an ingoing and outgoing 
	/// intersection), calculate the penetration depth. This is the minimum Distance
	/// that A and B need to be separated to get rid of any overlap.
	/// 
	/// <p>The penetration depth or separation is calculated by running a sweepline
	/// between the two points of an intersection pair. We keep track of the upper
	/// bound defined by edges of polygon A and the lower bound defined by B.
	/// The maximum Distance between these bounds is the value we are searching for.
	/// </p>
	/// 
	/// <pre>
	/// -<----
	/// |B     |
	/// |      | 
	/// out|      |in
	/// -----+--.---+-------
	/// |A    |  !  /        |
	/// |      \ ! /         |
	/// |       \!/          |
	/// |        .           |
	/// ->------------------
	/// </pre>
	/// 
	/// <p>The sweepline always runs from the ingoing to the outgoing
	/// intersection. Usually the normal is perpendicular to the sweepline.</p>
	/// 
	/// <p>We cannot always use the whole intersection Area. Take a look at the 
	/// following example. If we would allow the vertex marked with to be included
	/// in the sweep, the penetration depth would be far too big. Therefore we
	/// 'cut off' the intersection Area with two lines through the intersection
	/// points perpendicular to the sweep direction. Unfortunately this will break
	/// the algorithm for collision normals other than those perpendicular to the
	/// sweepline (it's should still be usable). The lines are called the borders
	/// of the intersection Area.
	/// </p>
	/// 
	/// <pre>
	/// +--/---/---------------------*                                                             
	/// +-|-/-+ /                    A |                                            
	/// \|/ B|/                       |                                          
	/// x   /                        |                                            
	/// /|\ /|                        |                                            
	/// +-x--------------------------+                                         
	/// / \|  
	/// </p>
	/// 
	/// <h3>Convexity</h3>
	/// <p>This algorithm should always work well for convex intersection areas.
	/// When the intersection Area is not convex, the resulting separation might
	/// be erroneous.</p>
	/// <p>Since colliding two convex polygons will always result in convex
	/// intersection areas, they should be used if possible. Colliding non-convex
	/// polygons seems to work pretty well in practice too, but more testing is
	/// needed.</p>
	/// 
	/// </summary>
	public class PenetrationSweep
	{
		/// <summary>The collision normal onto which the penetration depth is projected </summary>
		private Vector2f normal;
		/// <summary>The direction of our sweep, pointing from the ingoing intersection to
		/// the outgoing intersection 
		/// </summary>
		private Vector2f sweepDir;
		/// <summary>The projection of the ingoing intersection onto the sweepDir, defines
		/// a border of the intersecting Area. 
		/// </summary>
		private float startDist;
		/// <summary>The projection of the outgoing intersection onto the sweepDir, defines
		/// a border of the intersecting Area. 
		/// </summary>
		private float endDist;
		
		/// <summary> Constructs a Penetration Sweep object, with all its attributes set.
		/// This constructor is public only for testing purposes. The static method
		/// {@link PenetrationSweep#GetPenetrationDepth(Intersection, Intersection, Vector2f, Vector2f[], Vector2f[])}
		/// should be called to get the penetration depth. 
		/// 
		/// </summary>
		/// <param name="normal">The collision normal
		/// </param>
		/// <param name="sweepDir">The sweep direction
		/// </param>
		/// <param name="intersectionStart">The start bound of the intersection Area
		/// </param>
		/// <param name="intersectionEnd">The end bound of the intersection Area.
		/// </param>
		public PenetrationSweep(Vector2f normal, Vector2f sweepDir, Vector2f intersectionStart, Vector2f intersectionEnd):base()
		{
			this.normal = normal;
			this.sweepDir = sweepDir;
			this.startDist = intersectionStart.Dot(sweepDir);
			this.endDist = intersectionEnd.Dot(sweepDir);
		}
		
		
		/// <summary> Given two intersecting polygons, the intersection points and a collision
		/// normal, get the maximum penetration Distance along the normal.
		/// 
		/// </summary>
		/// <param name="in">The ingoing intersection
		/// </param>
		/// <param name="out">The outgoing intersection
		/// </param>
		/// <param name="normal">The collision normal
		/// </param>
		/// <param name="vertsA">The vertices of polygon A
		/// </param>
		/// <param name="vertsB">The vertices of polygon B
		/// </param>
		/// <returns> the maximum penetration depth along the given normal
		/// </returns>
		public static float GetPenetrationDepth(Intersection ingoing, Intersection outgoing, Vector2f normal, Vector2f[] vertsA, Vector2f[] vertsB)
		{
			Vector2f sweepdir = new Vector2f(outgoing.position);
			sweepdir.Sub(ingoing.position);
			
			PenetrationSweep ps = new PenetrationSweep(normal, sweepdir, ingoing.position, outgoing.position);
			
			//TODO: most penetrations are very simple, similar to:
			// \               +       |        
			//  \             / \      |          
			//   +-----------x---x-----+          
			//              /     \                    
			// these should be handled separately
			
			
			ContourWalker walkerA = new Silver.Weight.Raw.Collide.PenetrationSweep.ContourWalker(ps, vertsA, ingoing.edgeA, outgoing.edgeA, false);
			ContourWalker walkerB = new Silver.Weight.Raw.Collide.PenetrationSweep.ContourWalker(ps, vertsB, (outgoing.edgeB + 1) % vertsB.Length, (ingoing.edgeB + 1) % vertsB.Length, true);
			
			float penetration = 0;
			float lowerBound = ingoing.position.Dot(normal);
			float upperBound = lowerBound;
			
			while (walkerA.HasNext() || walkerB.HasNext())
			{
				// if walker a has more and the Next vertex comes before B's
				// or if walker a has more but walker b hasn't, go and take a Step
				if (walkerA.HasNext() && (walkerA.NextDistance < walkerB.NextDistance || !walkerB.HasNext()))
				{
					walkerA.Next();
					if (walkerA.Distance < ps.startDist || walkerA.Distance > ps.endDist)
						continue; // we don't care for vertices outside of the intersecting borders
					
					upperBound = walkerA.GetPenetration();
					lowerBound = walkerB.GetPenetration(walkerA.Distance);
				}
				else
				{
					walkerB.Next();
					if (walkerB.Distance < ps.startDist || walkerB.Distance > ps.endDist)
						continue;
					
					upperBound = walkerA.GetPenetration(walkerB.Distance);
					lowerBound = walkerB.GetPenetration();
				}
				
				penetration = System.Math.Max(penetration, upperBound - lowerBound);
			}
			
			return penetration;
		}
		
		/// <summary> The contour walker walks over the edges or vertices of a polygon.
		/// The class keeps track of two values: 
		/// <ul>
		/// <li>The penetration, which is the projection of the current vertex 
		/// onto the collision normal</li>
		/// <li>The Distance, which is the projection of the current vertex onto
		/// the sweep direction</li>
		/// </ul>
		/// 
		/// TODO: yes this use of nested classes is strange and possibly undersirable
		/// and no, it is not a misguided attempt to save memory, it simply evolved this way
		/// </summary>
		public class ContourWalker
		{
			private void  InitBlock(PenetrationSweep parent)
			{
				this.parent = parent;
			}
			private PenetrationSweep parent;
			/// <summary> Get the Distance of the current vertex</summary>
			/// <returns> the Distance of the current vertex
			/// </returns>
			virtual public float Distance
			{
				get
				{
					return distance;
				}
				
			}
			/// <summary> Get the Distance of the Next vertex which can be a point on one of
			/// the borders or a vertex on the polygon's contour.
			/// 
			/// </summary>
			/// <returns> The Next vertex's Distance
			/// </returns>
			virtual public float NextDistance
			{
				get
				{
					if (distance < Parent.startDist)
						return System.Math.Min(nextDistance, Parent.startDist);
					if (distance < Parent.endDist)
						return System.Math.Min(nextDistance, Parent.endDist);
					
					return nextDistance;
				}
				
			}
			public PenetrationSweep Parent
			{
				get
				{
					return parent;
				}
				
			}
			
			/// <summary>The vertices of the polygon which's contour is being followed </summary>
			private Vector2f[] verts;
			/// <summary>The index of the vertex we are currently at </summary>
			private int currentVert;
			/// <summary>The index of the vertex where the contour's subsection which we 
			/// walk on starts 
			/// </summary>
			private int firstVert;
			/// <summary>The index of the vertex where the contour's subsection which we 
			/// walk on ends 
			/// </summary>
			private int lastVert;
			/// <summary>True if we are walking backwards, from lastVert to firstVert.
			/// False if we are walking forwards, from firstVert to lastVert. 
			/// </summary>
			private bool isBackwards;
			
			/// <summary>The Distance of the current vertex, which is the projection of the current vertex 
			/// onto the collision normal 
			/// </summary>
			private float distance;
			/// <summary>The Distance of the Next vertex </summary>
			private float nextDistance;
			/// <summary>The penetration of the current vertex, which is the projection of the current vertex onto
			/// the sweep direction 
			/// </summary>
			private float penetration;
			/// <summary>The slope of the current edge with wich the penetration increases.
			/// The current edge is defined by the line from the current vertex to the Next. 
			/// </summary>
			private float penetrationDelta;
			
			/// <summary> Construct a contourwalker.
			/// 
			/// </summary>
			/// <param name="verts">The vertices of the polygon which's contour is being followed
			/// </param>
			/// <param name="firstVert">The index of the vertex where the contour's subsection which we 
			/// walk on starts
			/// </param>
			/// <param name="lastVert">The index of the vertex where the contour's subsection which we 
			/// walk on ends
			/// </param>
			/// <param name="isBackwards">True iff we're walking backwards over the contour
			/// </param>
			public ContourWalker(PenetrationSweep sweep, Vector2f[] verts, int firstVert, int lastVert, bool isBackwards)
			{
				InitBlock(sweep);
				if (firstVert < 0 || lastVert < 0)
					throw new System.ArgumentException("Vertex numbers cannot be negative.");
				
				if (firstVert > verts.Length || lastVert > verts.Length)
					throw new System.ArgumentException("The given vertex array doesn't include the first or the last vertex.");
				
				this.isBackwards = isBackwards;
				this.verts = verts;
				this.firstVert = firstVert;
				this.lastVert = lastVert;
				this.currentVert = isBackwards?lastVert:firstVert;
				
				this.distance = verts[currentVert].Dot(Parent.sweepDir);
				this.penetration = verts[currentVert].Dot(Parent.normal);
				CalculateNextValues();
			}
			
			/// <summary> Get the penetration of the current vertex.</summary>
			/// <returns> the penetration of the current vertex
			/// </returns>
			public virtual float GetPenetration()
			{
				return penetration;
			}
			
			/// <summary> Get the penetration of a point on the current edge at the supplied
			/// Distance.
			/// 
			/// </summary>
			/// <param name="Distance">The Distance at which we want the penetration on the current edge
			/// </param>
			/// <returns> The penetration at the supplied Distance
			/// </returns>
			public virtual float GetPenetration(float distance)
			{
				return penetration + penetrationDelta * (distance - this.distance);
			}
			
			/// <summary> Let this walker take a Step to the Next vertex, which is either the
			/// Next vertex in the contour or a point on the current edge that crosses
			/// one of the sweep Area borders. 
			/// </summary>
			public virtual void  Next()
			{
				if (!HasNext())
					return ;
				
				// if the edge crosses the sweep Area border, set our position on the border
				if (distance < Parent.startDist && nextDistance > Parent.startDist)
				{
					this.penetration = GetPenetration(Parent.startDist);
					this.distance = Parent.startDist;
					return ;
				}
				
				if (distance < Parent.endDist && nextDistance > Parent.endDist)
				{
					this.penetration = GetPenetration(Parent.endDist);
					this.distance = Parent.endDist;
					return ;
				}
				
				if (isBackwards)
				{
					currentVert = (currentVert - 1 + verts.Length) % verts.Length;
				}
				else
				{
					currentVert = (currentVert + 1) % verts.Length;
				}
				
				distance = verts[currentVert].Dot(Parent.sweepDir);
				penetration = verts[currentVert].Dot(Parent.normal);
				CalculateNextValues();
			}
			
			/// <summary> Take a look at the Next vertex and sets nextDistance and 
			/// penetrationDelta.
			/// </summary>
			private void  CalculateNextValues()
			{
				int nextVert = isBackwards?currentVert - 1:currentVert + 1;
				nextVert = (nextVert + verts.Length) % verts.Length;
				
				nextDistance = verts[nextVert].Dot(Parent.sweepDir);
				
				penetrationDelta = verts[nextVert].Dot(Parent.normal) - penetration;
				if (nextDistance == distance)
				{
					// the Next vertex is straight up, since we're searching
					// for the maximum anyway, it's safe to set our penetration
					// to the Next vertex's penetration.
					penetration += penetrationDelta;
					penetrationDelta = 0;
				}
				else
				{
					penetrationDelta /= (nextDistance - distance);
				}
			}
			
			/// <summary> Check if there are still vertices to walk to.</summary>
			/// <returns> True iff a call to HasNext would succeed.
			/// </returns>
			public virtual bool HasNext()
			{
				// if the current edge crosses the boundaries defined by
				// start and end edge, we will definately have a Next
				// vertex to report.
				if (distance < Parent.startDist && nextDistance > Parent.startDist)
					return true;
				
				if (distance < Parent.endDist && nextDistance > Parent.endDist)
					return true;
				
				// first make x the Distance from the 'start'
				// where the start depends on whether we're going backwards
				int x = isBackwards?lastVert - currentVert:currentVert - firstVert;
				x = (x + verts.Length) % verts.Length;
				
				// now subtract the number of verts between the first and last vertex
				x = (lastVert - firstVert + verts.Length) % verts.Length - x;
				
				return x > 0;
			}
			
			/// <summary> Reverse the direction of this walker.</summary>
			public virtual void  Reverse()
			{
				isBackwards = !isBackwards;
				
				CalculateNextValues();
			}
		}
	}
}