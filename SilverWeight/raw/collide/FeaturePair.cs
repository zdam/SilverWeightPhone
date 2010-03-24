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
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> An identifier for a pair of edges between two bodies. This identifies which
	/// contact points we're using in any given situation
	/// 
	/// For polygons defined by counter clockwise lists of vertices, the following
	/// will hold (vertsA is the first, and vertsB the second polygon):
	/// vertsA[inEdge1] is outside the overlap Area
	/// vertsA[inEdge1+1] is inside the overlap Area
	/// vertsB[inEdge2] is inside the overlap Area
	/// vertsB[inEdge2+1] is outside the overlap Area
	/// vertsA[outEdge1] is inside the overlap Area
	/// vertsA[outEdge1+1] is outside the overlap Area
	/// vertsB[outEdge1] is outside the overlap Area
	/// vertsB[outEdge1+1] is inside the overlap Area
	/// 
	/// Keep in mind that the edges outside of the overlap Area could be inside
	/// another overlapping Area that has another feature set.
	/// 
	/// TODO: check if this also holds for boxes
	/// 
	/// </summary>
	public class FeaturePair
	{
		/// <summary> Get this feature pair as a key value used for hashing
		/// 
		/// </summary>
		/// <returns> The key value
		/// </returns>
		virtual internal int Key
		{
			get
			{
				return inEdge1 + (outEdge1 << 8) + (inEdge2 << 16) + (outEdge2 << 24);
			}
			
		}
		
		
		/// <summary>The edge of the first polygon entering the second polygon </summary>
		internal int inEdge1;
		/// <summary>The first edge in the collision </summary>
		internal int outEdge1;
		/// <summary>The second edge in the collision </summary>
		internal int inEdge2;
		/// <summary>The second edge in the collision </summary>
		internal int outEdge2;
		
		/// <summary> Public constructor since something in the raw port want to access
		/// it. Should not be constructed by a user.
		/// </summary>
		public FeaturePair()
		{
		}
		
		/// <summary> Construct a feature pair and set edges.
		/// 
		/// </summary>
		/// <param name="inEdge1">
		/// </param>
		/// <param name="outEdge1">
		/// </param>
		/// <param name="inEdge2">
		/// </param>
		/// <param name="outEdge2">
		/// </param>
		public FeaturePair(int inEdge1, int inEdge2, int outEdge1, int outEdge2)
		{
			this.inEdge1 = inEdge1;
			this.inEdge2 = inEdge2;
			this.outEdge1 = outEdge1;
			this.outEdge2 = outEdge2;
		}
		
		/// <summary> Create a new feature pair
		/// 
		/// </summary>
		/// <param name="index">The index identifing the feature pair that Collided
		/// </param>
		internal FeaturePair(int index)
		{
			inEdge1 = index;
		}
		
		/// <seealso cref="java.lang.Object.hashCode()">
		/// </seealso>
		public override int GetHashCode()
		{
			return Key;
		}
		
		/// <seealso cref="java.lang.Object.equals(java.lang.Object)">
		/// </seealso>
		public  override bool Equals(System.Object other)
		{
			if (other is FeaturePair)
			{
				return ((FeaturePair) other).Key == Key;
			}
			
			return false;
		}
		
		/// <summary> Set the contents of this pair from another
		/// 
		/// </summary>
		/// <param name="other">The other pair to populate this pair from
		/// </param>
		public virtual void  SetFromOther(FeaturePair other)
		{
			inEdge1 = other.inEdge1;
			inEdge2 = other.inEdge2;
			outEdge1 = other.outEdge1;
			outEdge2 = other.outEdge2;
		}
		
		/// <seealso cref="Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "((" + inEdge1 + "," + inEdge2 + "),(" + outEdge1 + "," + outEdge2 + "))";
		}
	}
}