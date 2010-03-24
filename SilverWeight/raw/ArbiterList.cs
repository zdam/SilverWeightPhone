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
	
	/// <summary> A typed list of <code>Arbiter</code>
	/// 
	/// </summary>
	public class ArbiterList
	{
		/// <summary>The elements in the list </summary>
		private IList<Arbiter> elements = new List<Arbiter>();
		
		/// <summary> Create an empty list </summary>
		internal ArbiterList()
		{
		}
		
		/// <summary> Add an arbiter to the list
		/// 
		/// </summary>
		/// <param name="arbiter">The arbiter to Add
		/// </param>
		internal virtual void  add(Arbiter arbiter)
		{
			elements.Add(arbiter);
		}
		
		/// <summary> Get the Size of the list
		/// 
		/// </summary>
		/// <returns> The number of elements in the list
		/// </returns>
		public virtual int size()
		{
			return elements.Count;
		}
		
		/// <summary> Return the index of a particular arbiter in the list
		/// 
		/// </summary>
		/// <param name="arbiter">The arbiter to search for
		/// </param>
		/// <returns> The index of -1 if not found
		/// </returns>
		public virtual int indexOf(Arbiter arbiter)
		{
			return elements.IndexOf(arbiter);
		}
		
		/// <summary> Remove an abiter from the list
		/// 
		/// </summary>
		/// <param name="arbiter">The arbiter ot Remove from the list
		/// </param>
		internal virtual void  remove(Arbiter arbiter)
		{
			if (!elements.Contains(arbiter))
			{
				return ;
			}
			elements[elements.IndexOf(arbiter)] = elements[elements.Count - 1];
			elements.RemoveAt(elements.Count - 1);
		}
		
		/// <summary> Get an arbiter at a specified index
		/// 
		/// </summary>
		/// <param name="i">The index of arbiter to retrieve
		/// </param>
		/// <returns> The arbiter at the specified index
		/// </returns>
		public virtual Arbiter Item(int i)
		{
			return (Arbiter) elements[i];
		}
		
		/// <summary> Remove all the elements from the list</summary>
		public virtual void  Clear()
		{
			elements.Clear();
		}
		
		/// <summary> Check if an arbiter is contained within a list
		/// 
		/// </summary>
		/// <param name="arb">The arbiter to check for
		/// </param>
		/// <returns> True if the arbiter is in the list
		/// </returns>
		public virtual bool Contains(Arbiter arb)
		{
			return elements.Contains(arb);
		}
	}
}