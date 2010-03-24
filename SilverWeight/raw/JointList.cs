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
using System.Collections.Generic;
namespace Silver.Weight.Raw
{
	
	/// <summary> A typed list of type <code>Joint</code>
	/// 
	/// </summary>
	public class JointList
	{
		/// <summary>The elements in the list </summary>
		private IList<Joint> elements = new List<Joint>();
		
		/// <summary> Create an empty list</summary>
		public JointList()
		{
		}
		
		/// <summary> Check if a given joint in container within this list
		/// 
		/// </summary>
		/// <param name="joint">The joint to check for
		/// </param>
		/// <returns> True if the joint is contained in this list
		/// </returns>
		public virtual bool Contains(Joint joint)
		{
			return elements.Contains(joint);
		}
		
		/// <summary> Add a joint to the list
		/// 
		/// </summary>
		/// <param name="joint">The joint to Add
		/// </param>
		public virtual void  Add(Joint joint)
		{
			elements.Add(joint);
		}
		
		/// <summary> Get the Size of the list
		/// 
		/// </summary>
		/// <returns> The Size of the list
		/// </returns>
		public virtual int Size()
		{
			return elements.Count;
		}
		
		/// <summary> Remove a joint from the list
		/// 
		/// </summary>
		/// <param name="joint">The joint to Remove
		/// </param>
		public virtual void  Remove(Joint joint)
		{
			//SupportClass.ICollectionSupport.Remove(elements, joint);
			elements.Remove(joint);
		}
		
		/// <summary> Get a joint from the list
		/// 
		/// </summary>
		/// <param name="i">The index of the joint to retrieve
		/// </param>
		/// <returns> The joint requested
		/// </returns>
		public virtual Joint Item(int i)
		{
			return (Joint) elements[i];
		}
		
		/// <summary> Empty the list</summary>
		public virtual void  Clear()
		{
			elements.Clear();
		}
	}
}