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
namespace Silver.Weight.Raw
{
	
	/// <summary> A joint connecting two bodies
	/// 
	/// </summary>
	public interface Joint
	{
		/// <summary> Set the relaxtion value on this joint. This value determines
		/// how loose the joint will be
		/// 
		/// </summary>
		/// <param name="relaxation">The relaxation value
		/// </param>
		float Relaxation
		{
			set;
			
		}
		/// <summary> Get the first body attached to this joint
		/// 
		/// </summary>
		/// <returns> The first body attached to this joint
		/// </returns>
		Body Body1
		{
			get;
			
		}
		/// <summary> Get the second body attached to this joint
		/// 
		/// </summary>
		/// <returns> The second body attached to this joint
		/// </returns>
		Body Body2
		{
			get;
			
		}
		
		/// <summary> Apply the impulse caused by the joint to the bodies attached.</summary>
		void  ApplyImpulse();
		
		/// <summary> Precaculate everything and apply initial impulse before the
		/// simulation Step takes place
		/// 
		/// </summary>
		/// <param name="invDT">The amount of time the simulation is being stepped by
		/// </param>
		void  PreStep(float invDT);

		/// <summary>
		/// 
		/// </summary>
		object UserData { get; set; }
	}
}