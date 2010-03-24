# region License
/*
* Silver.Weight - a 2D Physics Engine for Microsoft(R) Silverlight (tm)
* Copyright (c) 2008, Adam Webber
*
* For updated news and information please visit http://shinypath.com/
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
	
	/// <summary> <p>This is a very important class for collision detection between polygons
	/// for several reasons:
	/// <ul>
	/// <li>First of all, it checks two edges for intersections. What edges will be
	/// checked for intersections is decided elsewhere.</li>
	/// <li>All intersecting edges are stored with some additinal information.</li>
	/// <li>Finally we return a very specific (Sub) set of the intersections that
	/// is used for the physics simulation.</li>
	/// </ul>
	/// </p>
	/// 
	/// <p>While intersection tests are rather straight forward and well known problems,
	/// the selection of the intersections to return is quite complicated.
	/// It suprised me that I was completely unable to find applicable information on
	/// this particular problem on the net.</p>
	/// 
	/// <p>Suppose we have two colliding polygons A and B.
	/// Observe that if we trace around the polygons in counterclockwise order starting
	/// on an edge of the polygon that is outside the other, we will encounter first
	/// an intersection that goes into the other polygon and then one that comes out
	/// again.</p>
	/// 
	/// <pre>
	/// ----
	/// out| B  |in
	/// ---+----+---
	/// |A  |    |   |
	/// |    ->--    |
	/// ->----------
	/// </pre>
	/// 
	/// <p>These in- and outgoing intersections can be used to determine two very 
	/// important things:
	/// <ol>
	/// <li>A collision normal which simply is a line perpendicular to the line from 
	/// the ingoing to the outgoing intersection. </li>
	/// <li>The penetration depth, which is determined by tracing along the in-out 
	/// line for the longest line parallel to the normal in the overlapping Area 
	/// (see {@link PenetrationSweep}).</li>
	/// </ol>
	/// Retrieving these pairs is relatively easy. We simply sort the gathered 
	/// intersections in the order they would occur when tracing the contour of A.</p>
	/// 
	/// <p> Things get complicated when polygons don't just Intersect but totally 
	/// penetrate eachother. First of all, the collision normals would (depending on the
	/// specific polygons and their intersection) oppose eachother, this has 
	/// unpredictable but possibly still acceptible results.</p>
	/// 
	/// <p>The biggest problem however is the penetration depth that will be completely
	/// off. Take for example the intersection pair 1 and 2 in the example.
	/// The penetration depth will be determined by the maximum vertical Distance
	/// between the edges of A between in and out and the edges of B between out and in
	/// (both counterclockwise). 
	/// Clearly this gives very large penetration depths which often are completely 
	/// wrong.</p>
	/// 
	/// <p>In the example the final result will not be very problematic: the forces
	/// are most likely to cancel eachother out. But even here it's simple to see that
	/// the penetration depth for the intersection pair (3,4) will be much bigger than
	/// the other. There are many examples that occur frequently in practice which will
	/// cause bodies to be shot in orbit (if the physics would've allowed it ;) ).</p>
	/// 
	/// <pre>
	/// ----
	/// in|A   |out
	/// ---3----2--------
	/// |B  |    |        |
	/// |   |    |        |
	/// ->-4----1--------
	/// out|    |in 
	/// ->--
	/// </pre>
	/// 
	/// <p>The/A solution actually is quite straightforward: if we throw out one of the
	/// two intersection pairs, the penetration depth will be correct and there won't be
	/// any normals that cancel eachother out while they shouldn't. Aditionally this
	/// technique will decrease the number of feature pairs, which is good for speed.
	/// </p>
	/// 
	/// <p>Throwing out the right pairs still is quite a challange, but we can apply a
	/// technique similar to the one we used to get the intersection pairs. Then we
	/// traced the contour of A, now we trace the contour of B starting with an
	/// outgoing intersection. If we encounter an ingoing intersection of the same pair
	/// we accept the pair (i.e. not throw it out) and Move on to the Next intersection.
	/// If we encounter an ingoing intersection belonging to a different pair or an
	/// outgoing intersection we throw it out.</p>
	/// 
	/// <p>We still are not done just yet. The following example shows that it matters
	/// a great deal which intersection we start with. If we start at intersection 2,
	/// all the other intersection pairs will be thrown out and B will be pushed down.
	/// If we had chosen to start with 4 or 6, the result would be more acceptable;
	/// only (1,2) would be thrown out and B would be pushed up.</p>
	/// 
	/// <pre>
	/// --------------
	/// |B             |
	/// |     ----     |
	/// out|  in|    |out |in
	/// ---6----5----4----3--
	/// |A  |    |    |    |  |
	/// |   |    |     ----   |
	/// ->-1----2------------
	/// in|    |out
	/// ->--
	/// </pre>
	/// 
	/// <p>This problem is not really solved currently. There is a heuristic in place
	/// that will select the outgoing edge that has the largest number of vertices 
	/// outside A. In other words, the largest number of vertices between the chosen
	/// outgoing intersection and the last ingoing intersection (last as in clockwise
	/// as opposed to counterclockwise).</p>
	/// 
	/// <p>The heuristic is based on the idea that we are more likely to be pushed
	/// from A into the direction where the largest part of B is. For the example
	/// this would not work, but for many examples in practice it will.</p>
	/// 
	/// TODO: When we're stacking a lot of triangles the heuristic fails occationaly.
	/// The triangles will sometimes jump in directions they shouldn't.
	/// 
	/// <h5>Problems</h5>
	/// 
	/// <p>If we manage to get all intersections between polygons, we should always 
	/// encounter alternating ingoing and outgoing intersections when tracing either
	/// A or B. In practice this isn't really the case for two reasons:</p>
	/// <ol>
	/// <li>We only test intersections for specific edges. These are generated in other
	/// classes which could have bugs. This would be a cause of missing intersections.</li>
	/// <li>The intersection test in this class has given me trouble over and over again
	/// for special cases when endpoints of two edges are involved in the intersections.
	/// For example the case where an edge is only touched by a vertex, only an ingoing
	/// intersection was created. I've had some much trouble and uncertainty here that
	/// I don't know if this is still a problem.</li>
	/// <li>Finally an issue that will be hard to solve: the orderings defined by
	/// {@link IntersectionComparator} and {@link PointerTableComparator} suffer of
	/// floating point rounding errors when in- and outgoing edges are close together.</li>
	/// </ol>
	/// 
	/// <p>In stead of trying to fix this issue or throw out the faulty intersections,
	/// I've made some adjustments that will simply skip these intersections. I am
	/// afraid that these issues might cause trouble somewhere sometime, so if possible
	/// this should be fixed some time.</p>
	/// 
	/// 
	/// TODO: This class could use some specialized data structures in stead of the
	/// countless arrays which clutter the code with modulo indices.
	/// 
	/// </summary>
	public class IntersectionGatherer
	{
		/// <summary> Get the list of intersections, sorted by the order defined by
		/// {@link IntersectionComparator}.
		/// 
		/// </summary>
		/// <returns> A sorted list of intersections
		/// </returns>
		virtual public Intersection[] Intersections
		{
			get
			{
				Intersection[] toReturn = new Intersection[noIntersections];
				
				for (int i = 0; i < noIntersections; i++)
					toReturn[i] = intersections[i];
				
				System.Array.Sort(toReturn, new IntersectionComparator());
				
				return toReturn;
			}
			
		}
		/// <summary> Get the pairs of ingoing and outgoing intersections encountered when tracing
		/// the contour of polygon A. Some pairs will be filtered out as described 
		/// in detail in this class's documentation.
		/// 
		/// </summary>
		/// <returns> An array with the intersection pairs which has the dimensions
		/// [n][2] or [n][1], where n is the number of intersections. 
		/// For a pair i GetIntersectionPairs()[i][0] will contain the ingoing intersection
		/// and GetIntersectionPairs()[i][1] the outgoing intersection.
		/// </returns>
		virtual public Intersection[][] IntersectionPairs
		{
			get
			{
				if (noIntersections < 2)
				{
					Intersection[][] tmpArray = new Intersection[0][];
					for (int i = 0; i < 0; i++)
					{
						tmpArray[i] = new Intersection[2];
					}
					return tmpArray;
				}
				
				// sort the array for a trace 
				System.Array.Sort(intersections, 0, noIntersections - 0, new IntersectionComparator());
				
				// sort a pointer table which uses the indices in the intersections array
				System.Int32[] pointers = new System.Int32[noIntersections];
				for (int i = 0; i < noIntersections; i++)
					pointers[i] = (System.Int32) i;
				System.Array.Sort(pointers, new PointerTableComparator(this));
				
				int referenceVertB = GetReferencePointer(pointers);
				FilterIntersections(referenceVertB, pointers);
				
				// make sure we're starting with an ingoing edge
				int first = intersections[0].isIngoing?0:1;
				
				// now copy our results to a new array

				//System.Collections.ArrayList outIntersections = new System.Collections.ArrayList();
				System.Collections.Generic.List<Intersection[]> outIntersections = new System.Collections.Generic.List<Intersection[]>();
				for (int i = first; i < noIntersections + first; )
				{
					SortableIntersection inComing = intersections[i % noIntersections];
					SortableIntersection outGoing = intersections[(i + 1) % noIntersections];
					
					if (inComing == null)
					{
						i += 1;
						continue;
					}
					
					if (outGoing != null && inComing.isIngoing && !outGoing.isIngoing)
					{
						// pairs that are too close to eachother will
						// often cause problems, so don't create them
						if (!inComing.position.EqualsDelta(outGoing.position, MIN_PAIR_DIST))
						{
							Intersection[] pair = new Intersection[]{inComing, outGoing};
							outIntersections.Add(pair);
							i += 2;
							continue;
						}
					}
					
					Intersection[] inArr = new Intersection[]{inComing};
					outIntersections.Add(inArr);
					i += 1;
				}
				// old code:
				//return (Intersection[][])outIntersections.ToArray(typeof(Intersection));

				// new code - prob slow as and not tested yet...
				// used when doing ply collisions, so prob best to stay away from polys atm
				Intersection[][] returnInters = new Intersection[outIntersections.Count][];
				for (int i = 0; i < outIntersections.Count; i++)
				{
					returnInters[i] = (Intersection[])outIntersections[i];
				}
				return returnInters;
			}
			
		}
		
		/// <summary>The minimum Distance two intersections have to be apart to be considered a pair </summary>
		public static float MIN_PAIR_DIST = 0.5f;
		/// <summary>The Size of the intersections array, thus determening the maximum number
		/// of intersections that the IntersectionGatherer can accept. 
		/// </summary>
		public static int MAX_INTERSECTIONS = 50;
		/// <summary>The array of intersecting edges which is unsorted while gathering the
		/// intersections, but will be sorted in order of occurence along the contour
		/// of polygon A. 
		/// </summary>
		private SortableIntersection[] intersections;
		/// <summary>The amount of intersections gathered </summary>
		private int noIntersections = 0;
		
		/// <summary>The vertices of polygon A </summary>
		private Vector2f[] vertsA;
		/// <summary>The vertices of polygon B </summary>
		private Vector2f[] vertsB;
		
		/// <summary> Construct an IntersectionGatherer for a specific pair of polygons.
		/// 
		/// </summary>
		/// <param name="vertsA">The 'first' polygon involved in this collision check
		/// </param>
		/// <param name="vertsB">The 'second' polygon involved in this collision check
		/// </param>
		public IntersectionGatherer(Vector2f[] vertsA, Vector2f[] vertsB)
		{
			this.intersections = new SortableIntersection[MAX_INTERSECTIONS];
			this.noIntersections = 0;
			this.vertsA = vertsA;
			this.vertsB = vertsB;
		}
		
		/// <summary> Intersect two edges of the two polygons and stores the intersecting lines
		/// along with their position.
		/// 
		/// </summary>
		/// <param name="a">The edge of polygon A to check for intersection with B
		/// </param>
		/// <param name="b">The edge of polygon B to check for intersection with A
		/// </param>
		public virtual void  Intersect(int a, int b)
		{
			if (noIntersections >= MAX_INTERSECTIONS)
				return ;
			
			Vector2f startA = vertsA[a];
			Vector2f endA = vertsA[(a + 1) % vertsA.Length];
			Vector2f startB = vertsB[b];
			Vector2f endB = vertsB[(b + 1) % vertsB.Length];
			//TODO: reuse mathutil.Intersect
			float d = (endB.y - startB.y) * (endA.x - startA.x) - (endB.x - startB.x) * (endA.y - startA.y);
			
			if (d == 0)
			// parallel lines
				return ;
			
			float uA = (endB.x - startB.x) * (startA.y - startB.y) - (endB.y - startB.y) * (startA.x - startB.x);
			uA /= d;
			float uB = (endA.x - startA.x) * (startA.y - startB.y) - (endA.y - startA.y) * (startA.x - startB.x);
			uB /= d;
			
			if (uA < 0 || uA > 1 || uB < 0 || uB > 1)
				return ; // intersection point isn't between the start and endpoints
			
			Vector2f position = new Vector2f(startA.x + uA * (endA.x - startA.x), startA.y + uA * (endA.y - startA.y));
			
			Vector2f dist = new Vector2f(position);
			dist.Sub(startA);
			float distFromVertA = dist.LengthSquared();
			dist = new Vector2f(position);
			dist.Sub(startB);
			float distFromVertB = dist.LengthSquared();
			
			// z axis of 3d Cross product
			float sA = (startA.x - startB.x) * (endB.y - startB.y) - (endB.x - startB.x) * (startA.y - startB.y);
			
			if (sA > 0)
			{
				intersections[noIntersections] = new SortableIntersection( a, b, position, true, distFromVertA, distFromVertB);
			}
			else
			{
				intersections[noIntersections] = new SortableIntersection(a, b, position, false, distFromVertA, distFromVertB);
			}
			
			noIntersections++;
		}
		
		/// <summary> Gets an outgoing intersection that seems appropriate to start the
		/// intersection filter with. This implements the heuristic described in the
		/// class documentation.
		/// 
		/// </summary>
		/// <param name="pointers">An array of pointers which are the indices of the
		/// intersections array. The list should be sorted with the order defined by
		/// {@link PointerTableComparator}.
		/// </param>
		/// <returns> The reference pointer which is an outgoing intersection. 
		/// </returns>
		private int GetReferencePointer(System.Int32[] pointers)
		{
			// we want to find an ingoing edge with the largest number of edges outside of A
			// these edges should be between the in and the nex out, when tracing the contour of B
			int first = intersections[pointers[0]].isIngoing?0:1;
			int maxInOutDist = 0;
			int maxInIndex = first + 1 % noIntersections; //intersections[pointers[first].intValue()].edgeB;
			int lastInEdgeB = - 1;
			for (int i = first; i < noIntersections + first; i++)
			{
				int k = pointers[i % noIntersections];
				SortableIntersection intersection = intersections[k];
				
				if (intersection.isIngoing)
				{
					lastInEdgeB = intersection.edgeB;
				}
				else if (lastInEdgeB >= 0)
				{
					int inOutDist = (intersection.edgeB - lastInEdgeB + vertsB.Length) % vertsB.Length;
					
					// did we find a new max dist?
					if (inOutDist > maxInOutDist)
					{
						maxInOutDist = inOutDist;
						maxInIndex = i % noIntersections;
					}
					lastInEdgeB = - 1;
				}
			}
			
			return maxInIndex;
		}
		
		/// <summary> This function filters out intersection pairs to remedy the situation where
		/// polygons totally penetrate eachother. See this class's documentation for
		/// a more thorough description.
		/// 
		/// </summary>
		/// <param name="referencePointer">The vertex of B that lies outside of A, we will start
		/// at the first outgoing intersection after the reference vertex.
		/// </param>
		/// <param name="pointers">An array of pointers which are the indices of the
		/// intersections array. The list should be sorted with the order defined by
		/// {@link PointerTableComparator}.
		/// </param>
		private void  FilterIntersections(int referencePointer, System.Int32[] pointers)
		{
			// make sure the reference vertex is real
			if (referencePointer >= noIntersections && referencePointer < 0)
				throw new System.SystemException("The reference vertex cannot be correct since B does not have that many vertices.");
			
			// now throw out the total penetrating intersections
			int topOut = - 2; // -2 + 1 will never give an edge number
			for (int i = referencePointer; i < noIntersections + referencePointer; i++)
			{
				int j = i % noIntersections;
				int k = pointers[j];
				SortableIntersection intersection = intersections[k];
				
				// note that we go backwards with respect to A so we expect an outgoing edge first
				if (intersection.isIngoing)
				{
					if ((topOut - 1 + noIntersections) % noIntersections == k)
					{
						// the closing 'in' intersection
						topOut = - 2; // reset our top 'out' intersection
					}
					else
					{
						intersections[k] = null; // Remove the 'in'
					}
				}
				else
				{
					if (topOut < 0)
					{
						topOut = k; // we encountered our new 'out' intersection
					}
					else
					{
						intersections[k] = null; // Remove the out
					}
				}
			}
			
			// now get rid of the nullified intersections
			int noRemoved = 0;
			for (int i = 0; i < noIntersections; i++)
			{
				if (intersections[i] == null)
				{
					noRemoved++;
				}
				else
				{
					intersections[i - noRemoved] = intersections[i];
				}
			}
			noIntersections -= noRemoved;
		}
		
		/// <summary>Class representing a single intersection. It also Contains some information used
		/// to sort the edges along the counters of the polygons A and B. 
		/// </summary>
		internal class SortableIntersection:Intersection
		{
			/// <summary>The squared Distance from the vertice that starts edgeA </summary>
			public float distFromVertA;
			/// <summary>The squared Distance from the vertice that starts edgeB </summary>
			public float distFromVertB;
			
			/// <summary> Construct an Intersection object, immediately setting all the attributes.
			/// 
			/// </summary>
			/// <param name="edgeA">The edge of polygon A that intersects
			/// </param>
			/// <param name="edgeB">The edge of polygon B that intersects
			/// </param>
			/// <param name="position">The position of the intersection in world (absolute) coordinates
			/// </param>
			/// <param name="isIngoing">True iff this is an intersection where polygon A enters B
			/// </param>
			/// <param name="distFromVertA">The squared Distance from the vertice that starts edgeA
			/// </param>
			/// <param name="distFromVertB">The squared Distance from the vertice that starts edgeB
			/// </param>
			public SortableIntersection( int edgeA, int edgeB, Vector2f position, bool isIngoing, float distFromVertA, float distFromVertB):base(edgeA, edgeB, position, isIngoing)
			{
				this.distFromVertA = distFromVertA;
				this.distFromVertB = distFromVertB;
			}
		}
		
		/// <summary>Comparator used to sort intersections by their Distance from A's first
		/// vertex. 
		/// </summary>
		internal class IntersectionComparator : System.Collections.Generic.IComparer<Intersection>
		{
		
			/// <summary>Compares two intersections. Note that this function will/should never
			/// return 0 because no two intersections can have the same Distance from
			/// vertex 0. However, due to the finite precision of floating points this
			/// situation does occur. In those cases we try to put ingoing edges first.
			/// 
			/// </summary>
			/// <seealso cref="Comparator.compare(Object, Object)">
			/// </seealso>
			public virtual int Compare(Intersection first, Intersection second)
			{
				if(first == second)
				{
					return 0;
				}
				SortableIntersection one = (SortableIntersection) first;
				SortableIntersection other = (SortableIntersection) second;
				
				if(one.Equals(other) || one == other )
				{
					return 0;
				}

				if (one.edgeA < other.edgeA)
				{
					return - 1;
				}
				else if (one.edgeA == other.edgeA)
				{
					if (one.distFromVertA < other.distFromVertA)
						return - 1;
					else if (one.distFromVertA == other.distFromVertA && one.isIngoing)
						return - 1;
				}
				
				return 1;
			}
		}
		
		/// <summary>Comparator used to sort intersections by their Distance from B's first
		/// vertex. This sorts an array of pointers which are the indices of the
		/// intersections array. So the actual intersections are retrieved via an
		/// indirection. 
		/// </summary>
		internal class PointerTableComparator : System.Collections.IComparer
		{
			private IntersectionGatherer gatherer;
			public PointerTableComparator(IntersectionGatherer gatherer)
			{
				this.gatherer = gatherer;
			}
			/// <summary>Compares two intersections. Note that this function will/should never
			/// return 0 because no two intersections can have the same Distance from
			/// vertex 0. However, due to the finite precision of floating points this
			/// situation does occur. In those cases we try to put outgoing edges first.
			/// 
			/// </summary>
			/// <seealso cref="Comparator.compare(Object, Object)">
			/// </seealso>
			public virtual int Compare(System.Object first, System.Object second)
			{
				if(first==second)
				{
					return 0;
				}
				SortableIntersection one = (SortableIntersection)gatherer.Intersections[((System.Int32) first)];
				SortableIntersection other = (SortableIntersection)gatherer.Intersections[((System.Int32)second)];

				if (one.Equals(other) || one == other)
				{
					return 0;
				}

				if (one.edgeB < other.edgeB)
				{
					return - 1;
				}
				else if (one.edgeB == other.edgeB)
				{
					if (one.distFromVertB < other.distFromVertB)
						return - 1;
					else if (one.distFromVertB == other.distFromVertB && !one.isIngoing)
						return - 1;
				}
				
				return 1;
			}
		}
	}
}