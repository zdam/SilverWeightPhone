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
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> <p>Implements a sweepline algorithm that facilitates collision detection between
	/// two polygons. For two polygons A and B it determines a set of collision candidates,
	/// i.e. the edges of A and B that can Collide. </p>
	/// 
	/// <p>Getting a good approximation of the set of colliding edges is important
	/// because given two polygons with n and m vertices each, checking every 
	/// combination would take n*m operations.</p>
	/// 
	/// <p>To limit the number of candidates we project all edges of both polygons onto
	/// a line. This line is the direction of the sweepline, the sweepline itself would
	/// be perpendicular to it. The start and endpoints of the edges are sorted by their
	/// projection onto the sweep direction.</p>
	/// 
	/// <p>The collision candidates can now be determined by walking through the list
	/// of start- and endpoints of the edges and check which edges of A and B overlap.
	/// If two edges do not overlap in the projection, they will not Intersect,
	/// therefore it is safe to discard the combination as a collision candidate.</p>
	/// 
	/// <p><i>Note that this approach is very similar to and indeed inspired by the
	/// separating axes theorem.</i></p>
	/// 
	/// <p>The effectiveness of this algorithm depends on the choice of sweep direction.
	/// For example using the line from one polygon's center to the other's center as
	/// our sweep direction will give great results if both polygons are more or less 
	/// round. However, when one of the polygons is very long, this will be (depending
	/// on the polygon's positions) a very bad idea.<br>
	/// The choice for the sweep direction is left to the user of this class.</p>
	/// 
	/// <h3>Insertion Sort</h3>
	/// <p>For this sweepline algorithm there is a major assumption
	/// regarding the complexity of the sorting. The used sorting algorithm uses the
	/// fact that the edges will mostly be inserted in order of their position on the
	/// sweepline (already sorted).</p>
	/// 
	/// <p>This seems justified by the observation that most polygons will be more or less
	/// monotone in any direction, including the sweep direction. For convex polygons
	/// this clearly holds, giving us a worst case complexity of O(n). Non-convex
	/// polygons can however cause trouble with a worst case complexity of O(n*n).</p>   
	/// 
	/// 
	/// </summary>
	public class EdgeSweep
	{
		/// <summary> Get all edges whose projection onto the sweep direction overlap.
		/// 
		/// </summary>
		/// <returns> The numbers of the overlapping edges. The array will always have
		/// dimension [n][2], where [i][0] is the edge of polygon A and [i][1] of B.
		/// </returns>
		virtual public int[][] OverlappingEdges
		{
			get
			{
				if (current == null)
				{
					int[][] tmpArray = new int[0][];
					for (int i = 0; i < 0; i++)
					{
						tmpArray[i] = new int[2];
					}
					return tmpArray;
				}
				
				GoToStart();
				
				CurrentEdges edgesA = new CurrentEdges();
				CurrentEdges edgesB = new CurrentEdges();
				EdgePairs collidingEdges = new EdgePairs();
				
				float lastDist = - System.Single.MaxValue;
				
				while (current != null)
				{
					if (current.distance > lastDist)
					{
						lastDist = current.distance;
						edgesA.RemoveScheduled();
						edgesB.RemoveScheduled();
					}
					
					if (current.isA)
					{
						if (!edgesA.Contains(current.vertex))
						{
							edgesA.AddEdge(current.vertex);
							
							int[] edgeListB = edgesB.Edges;
							for (int i = 0; i < edgeListB.Length; i++)
								collidingEdges.Add(current.vertex, edgeListB[i]);
						}
						else
						{
							edgesA.ScheduleRemoval(current.vertex);
						}
					}
					else
					{
						if (!edgesB.Contains(current.vertex))
						{
							edgesB.AddEdge(current.vertex);
							
							int[] edgeListA = edgesA.Edges;
							for (int i = 0; i < edgeListA.Length; i++)
								collidingEdges.Add(edgeListA[i], current.vertex);
						}
						else
						{
							edgesB.ScheduleRemoval(current.vertex);
						}
					}
					
					current = current.next;
				}
				
				return collidingEdges.ToList();
			}
			
		}
		/// <summary> Get the direction of this edgesweep
		/// 
		/// </summary>
		/// <returns> the direction of this edgesweep
		/// </returns>
		virtual public ROVector2f SweepDir
		{
			get
			{
				return sweepDir;
			}
			
		}
		
		/// <summary>The doubly linked list list of inserted vertices </summary>
		internal class ProjectedVertex
		{

			/// <summary>Vertex number, usually the index of the vertex in a polygon's array </summary>
			public int vertex;
			/// <summary>True if this is a vertex belonging to polygon A, false if B </summary>
			public bool isA;
			/// <summary>Distance of the projection onto the sweep direction from the origin </summary>
			public float distance;
			
			/// <summary>Next vertex in the list </summary>
			public ProjectedVertex next;
			/// <summary>Next previous in the list </summary>
			public ProjectedVertex previous;
			
			/// <summary> Construct a list element with all it's values set except the Next and
			/// previous elements of the list.
			/// 
			/// </summary>
			/// <param name="vertex">Vertex number, usually the index of the vertex in a polygon's array
			/// </param>
			/// <param name="isA">True if this is a vertex belonging to polygon A, false if B
			/// </param>
			/// <param name="Distance">Distance of the projection onto the sweep direction from the origin
			/// </param>
			public ProjectedVertex(int vertex, bool isA, float distance)
			{
				this.vertex = vertex;
				this.isA = isA;
				this.distance = distance;
			}
		}
		
		/// <summary>The last inserted element in the projected vertex list </summary>
		private ProjectedVertex current;
		
		/// <summary>The direction in which to sweep </summary>
		private Vector2f sweepDir;
		
		/// <summary>Constructs an EdgeSweep object with the given sweep direction.
		/// 
		/// </summary>
		/// <param name="sweepDir">The direction in which to sweep
		/// </param>
		public EdgeSweep(ROVector2f sweepDir)
		{
			this.sweepDir = new Vector2f(sweepDir);
		}
		
		/// <summary> Insert a new element into our list that is known to be somewhere before 
		/// the current element. It walks backwards over the vertex list untill a vertex
		/// with a smaller Distance or the start of the list is reached and inserts
		/// the element there.
		/// 
		/// </summary>
		/// <param name="vertex">Vertex number, usually the index of the vertex in a polygon's array
		/// </param>
		/// <param name="isA">True if this is a vertex belonging to polygon A, false if B
		/// </param>
		/// <param name="Distance">Distance of the projection onto the sweep direction from the origin
		/// </param>
		private void  InsertBackwards(int vertex, bool isA, float distance)
		{
			ProjectedVertex svl = new ProjectedVertex(vertex, isA, distance);
			
			if (current == null)
			{
				current = svl;
				return ;
			}
			
			while (current.distance > svl.distance)
			{
				if (current.previous == null)
				{
					// Insert before current
					current.previous = svl;
					svl.next = current;
					current = svl;
					return ;
				}
				
				current = current.previous;
			}
			
			// Insert after current
			svl.next = current.next;
			svl.previous = current;
			current.next = svl;
			
			if (svl.next != null)
				svl.next.previous = svl;
			
			current = svl;
		}
		
		/// <summary> Insert a vertex into the sorted list.
		/// 
		/// </summary>
		/// <param name="vertex">Vertex number, usually the index of the vertex in a polygon's array
		/// </param>
		/// <param name="isA">True if this is a vertex belonging to polygon A, false if B
		/// </param>
		/// <param name="Distance">Distance of the projection onto the sweep direction from the origin
		/// </param>
		public virtual void  Insert(int vertex, bool isA, float distance)
		{
			if (current == null || current.distance <= distance)
				InsertForwards(vertex, isA, distance);
			else
				InsertBackwards(vertex, isA, distance);
		}
		
		/// <summary> Insert a new element into our list that is known to be somewhere after 
		/// the current element. It walks forwards over the vertex list untill a vertex
		/// with a smaller Distance or the end of the list is reached and inserts
		/// the element there.
		/// 
		/// </summary>
		/// <param name="vertex">Vertex number, usually the index of the vertex in a polygon's array
		/// </param>
		/// <param name="isA">True if this is a vertex belonging to polygon A, false if B
		/// </param>
		/// <param name="Distance">Distance of the projection onto the sweep direction from the origin
		/// </param>
		private void  InsertForwards(int vertex, bool isA, float distance)
		{
			ProjectedVertex svl = new ProjectedVertex(vertex, isA, distance);
			
			if (current == null)
			{
				current = svl;
				return ;
			}
			
			while (current.distance <= svl.distance)
			{
				if (current.next == null)
				{
					// Insert after current
					current.next = svl;
					svl.previous = current;
					current = svl;
					return ;
				}
				
				current = current.next;
			}
			
			// Insert before current
			svl.next = current;
			svl.previous = current.previous;
			current.previous = svl;
			
			if (svl.previous != null)
				svl.previous.next = svl;
			
			current = svl;
		}
		
		/// <summary>Set current to the first element of the list 
		/// TODO: make this return something, touching the global current is ugly
		/// </summary>
		private void  GoToStart()
		{
			// get the first vertex
			while (current.previous != null)
				current = current.previous;
		}
		
		/// <summary>The list of edges that are touched by the sweepline at a given time. 
		/// 
		/// Note that this implementation proved faster than one with a HashSet,
		/// a specialized IntegerSet library and BitSet. This is mostly because
		/// this list will rarely contain more than 10 edges at a time.
		/// The IntegerSet and BitSet implementation both had poor performance
		/// in the getEdges function.
		/// 
		/// TODO: implement this with a tree to improve the performance of 'Contains'?
		/// If that is done, one should take care that it handles edges that are mostly
		/// sorted (because that will definately be the case).
		/// </summary>
		private class CurrentEdges
		{

			/// <summary> Get the total number of edges, this includes the edges that are
			/// scheduled for removal.
			/// 
			/// </summary>
			/// <returns> The total number of edges
			/// </returns>
			virtual public int NoEdges
			{
				get
				{
					int count = 0;
					LinkedEdgeList current = currentEdges;
					while (current != null)
					{
						count++;
						current = current.next;
					}
					
					current = scheduledForRemoval;
					while (current != null)
					{
						count++;
						current = current.next;
					}
					
					return count;
				}
				
			}
			/// <summary> Get the list of edges in this list.
			/// It should not contain any duplicates, but that depends on the insertion
			/// of elements.
			/// 
			/// </summary>
			/// <returns> the list of edges
			/// </returns>
			virtual public int[] Edges
			{
				get
				{
					int[] returnEdges = new int[NoEdges];
					
					int i = 0;
					LinkedEdgeList current = currentEdges;
					while (current != null)
					{
						returnEdges[i] = current.edge;
						i++;
						current = current.next;
					}
					current = scheduledForRemoval;
					while (current != null)
					{
						returnEdges[i] = current.edge;
						i++;
						current = current.next;
					}
					
					return returnEdges;
				}
				
			}

			/// <summary>The first element of the list of edges that have been inserted </summary>
			private LinkedEdgeList currentEdges;
			/// <summary>The edges that have been scheduled for removal but have not yet been removed </summary>
			private LinkedEdgeList scheduledForRemoval;
			
			/// <summary> Add an edge to the top of the list.
			/// We do not check wether it is already in the list, but maybe this should
			/// be done to be on the safe side.
			/// TODO: do that
			/// 
			/// </summary>
			/// <param name="e">The edge to be added
			/// </param>
			public virtual void  AddEdge(int e)
			{
				currentEdges = new LinkedEdgeList(e, currentEdges);
			}
			
			/// <summary> Schedule an edge for removal, it will be removed as soon as 
			/// {@link CurrentEdges#RemoveScheduled()} is called.
			/// 
			/// </summary>
			/// <param name="e">The edge to be scheduled for removal
			/// </param>
			public virtual void  ScheduleRemoval(int e)
			{
				if (currentEdges == null)
					return ; // this shouldn't happen, but to be sure..
				
				if (currentEdges.edge == e)
				{
					currentEdges = currentEdges.next;
				}
				else
				{
					LinkedEdgeList current = currentEdges.next;
					LinkedEdgeList last = currentEdges;
					
					while (current != null)
					{
						if (current.edge == e)
						{
							last.next = current.next;
							scheduledForRemoval = new LinkedEdgeList(e, scheduledForRemoval);
							return ;
						}
						last = current;
						current = current.next;
					}
				}
			}
			
			/// <summary>Remove the edges that have been scheduled for removal by
			/// {@link CurrentEdges#ScheduleRemoval(int)}. 
			/// </summary>
			public virtual void  RemoveScheduled()
			{
				scheduledForRemoval = null;
			}
			
			/// <summary> Check if this edge list Contains a specific edge.
			/// 
			/// </summary>
			/// <param name="e">The edge to look for
			/// </param>
			/// <returns> True iff the edgelist Contains the edge
			/// </returns>
			public virtual bool Contains(int e)
			{
				LinkedEdgeList current = currentEdges;
				while (current != null)
				{
					if (current.edge == e)
						return true;
					current = current.next;
				}
				
				current = scheduledForRemoval;
				while (current != null)
				{
					if (current.edge == e)
						return true;
					current = current.next;
				}
				
				return false;
			}
			
			/// <summary>A singly linked list for edges </summary>
			internal class LinkedEdgeList
			{
				/// <summary>The edge number </summary>
				public int edge;
				/// <summary>The Next list element </summary>
				public LinkedEdgeList next;
				
				/// <summary> Construct a new list element with its attributes set to the
				/// supplied values.
				/// 
				/// </summary>
				/// <param name="edge">The edge number
				/// </param>
				/// <param name="Next">The Next list element
				/// </param>
				public LinkedEdgeList(int edge, LinkedEdgeList next)
				{
					this.edge = edge;
					this.next = next;
				}
			}
		}
		
		/// <summary>The list of collision candidates in a linked list </summary>
		private class EdgePairs
		{
			/// <summary>The first element of the list of collision candidates </summary>
			private EdgePair first;
			/// <summary>The total number of collision candidates </summary>
			private int size = 0;
			
			/// <summary> Add a pair of edges to this list
			/// 
			/// </summary>
			/// <param name="idA">An edge of polygon A
			/// </param>
			/// <param name="idB">An edge of polygon B 
			/// </param>
			public virtual void  Add(int idA, int idB)
			{
				first = new EdgePair(idA, idB, first);
				size++;
			}
			
			/// <summary> Convert this linked list into a two dimensional array
			/// 
			/// </summary>
			/// <returns> The numbers of the overlapping edges. The array will always have
			/// dimension [n][2], where [i][0] is the edge of polygon A and [i][1] of B.
			/// </returns>
			public virtual int[][] ToList()
			{
				int[][] list = new int[size][];
				for (int i = 0; i < size; i++)
				{
					list[i] = new int[2];
				}
				
				EdgePair current = first;
				for (int i = 0; i < size; i++)
				{
					list[i][0] = current.a;
					list[i][1] = current.b;
					
					current = current.next;
				}
				
				return list;
			}
			
			/// <summary>The singly linked list representing one pair of edges </summary>
			internal class EdgePair
			{

				/// <summary>An edge of polygon A </summary>
				public int a;
				/// <summary>An edge of polygon B </summary>
				public int b;
				/// <summary>The Next element in the list </summary>
				public EdgePair next;
				
				/// <summary>Construct a new list element with all the attributes set to the
				/// provided values.
				/// 
				/// </summary>
				/// <param name="a">An edge of polygon A
				/// </param>
				/// <param name="b">An edge of polygon B
				/// </param>
				/// <param name="Next">The Next element in the list
				/// </param>
				public EdgePair( int a, int b, EdgePair next)
				{
					this.a = a;
					this.b = b;
					this.next = next;
				}
			}
		}
		
		/// <summary> Insert a list of edges
		/// 
		/// </summary>
		/// <param name="isA">True iff the inserted vertices are of the first object
		/// </param>
		/// <param name="verts">The list of vertices to be inserted in counter clockwise order
		/// </param>
		public virtual void  AddVerticesToSweep(bool isA, Vector2f[] verts)
		{
			for (int i = 0, j = verts.Length - 1; i < verts.Length; j = i, i++)
			{
				float dist = sweepDir.Dot(verts[i]);
				
				Insert(i, isA, dist);
				Insert(j, isA, dist);
			}
		}
	}
}