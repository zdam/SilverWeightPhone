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
	
	/// <summary> A typed list of <code>Body</code>
	/// 
	/// </summary>
	public class BodyList
	{
		/// <summary>The elements in the list </summary>
		private List<Body> elements = new List<Body>();
		
		/// <summary> Create an empty list</summary>
		public BodyList()
		{
		}
		
		/// <summary> Create a new list containing the elements specified
		/// 
		/// </summary>
		/// <param name="list">The list of elements to Add to the new list
		/// </param>
		internal BodyList(BodyList list)
		{			
			elements.AddRange(list.elements);
		}
		
		/// <summary> Add a body to the list
		/// 
		/// </summary>
		/// <param name="body">The body to Add
		/// </param>
		public virtual void  Add(Body body)
		{
			elements.Add(body);
		}
		
		/// <summary> Get the number of elements in the list
		/// 
		/// </summary>
		/// <returns> The number of the element in the list
		/// </returns>
		public virtual int Size()
		{
			return elements.Count;
		}
		
		/// <summary> Remove a body from the list
		/// 
		/// </summary>
		/// <param name="body">The body to Remove from the list 
		/// </param>
		public virtual void  Remove(Body body)
		{
			//SupportClass.ICollectionSupport.Remove(elements, body);
			elements.Remove(body);
		}
		
		/// <summary> Get a body at a specific index
		/// 
		/// </summary>
		/// <param name="i">The index of the body to retrieve
		/// </param>
		/// <returns> The body retrieved
		/// </returns>
		public virtual Body Item(int i)
		{
			return (Body) elements[i];
		}
		
		/// <summary> Clear all the elements out of the list</summary>
		public virtual void  Clear()
		{
			elements.Clear();
		}
		
		/// <summary> Check if this list Contains the specified body
		/// 
		/// </summary>
		/// <param name="body">The body to look for
		/// </param>
		/// <returns> True if this list Contains the specified body
		/// </returns>
		public virtual bool Contains(Body body)
		{
			return elements.Contains(body);
		}
		
		/// <summary> Get a list of bodies containing all of the bodies in this
		/// list except those specified
		/// 
		/// </summary>
		/// <param name="others">The bodies that should be removed from the contents
		/// </param>
		/// <returns> The list of bodies excluding those specified
		/// </returns>
		public virtual BodyList GetContentsExcluding(BodyList others)
		{
			BodyList list = new BodyList(this);
			//SupportClass.ICollectionSupport.RemoveAll(list.elements, others.elements);
			
			foreach (Body b in others.elements)
			{
				list.Remove(b);
			}
			return list;
		}
		
		/// <seealso cref="java.lang.Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			System.String str = "[BodyList ";
			for (int i = 0; i < elements.Count; i++)
			{
				str += (Item(i) + ",");
			}
			str += "]";
			
			return str;
		}
	}
}