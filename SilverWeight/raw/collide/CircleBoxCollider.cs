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
using MathUtil = Silver.Weight.Math.MathUtil;
using Vector2f = Silver.Weight.Math.Vector2f;
using Body = Silver.Weight.Raw.Body;
using Contact = Silver.Weight.Raw.Contact;
using Circle = Silver.Weight.Raw.Shapes.Circle;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> A collider for circles hitting boxes, Circle = BodyA, Box = BodyB
	/// 
	/// The create() method is used as a factory incase this class should
	/// ever become stateful.
	/// 
	/// </summary>
	public class CircleBoxCollider:BoxCircleCollider
	{
		/// <summary>The single instance of this collider to exist </summary>
		private static CircleBoxCollider single = new CircleBoxCollider();
		
		/// <summary> Get an instance of this collider
		/// 
		/// </summary>
		/// <returns> The instance of this collider
		/// </returns>
		public static CircleBoxCollider createCircleBoxCollider()
		{
			return single;
		}
		
		/// <summary> Prevent construction</summary>
		private CircleBoxCollider()
		{
		}
		
		/// <seealso cref="Silver.Weight.Raw.Collide.Collider.Collide(Silver.Weight.Raw.Contact[], Silver.Weight.Raw.Body, Silver.Weight.Raw.Body)">
		/// </seealso>
		public override int Collide(Contact[] contacts, Body circleBody, Body boxBody)
		{
			int count = base.Collide(contacts, boxBody, circleBody);
			
			// Reverse the collision results by inverting normals
			// and projecting the results onto the circle
			for (int i = 0; i < count; i++)
			{
				Vector2f vec = MathUtil.Scale(contacts[i].Normal, - 1);
				contacts[i].Normal = vec;
				
				Vector2f pt = MathUtil.Sub(contacts[i].Position, circleBody.GetPosition());
				pt.Normalise();
				pt.Scale(((Circle) circleBody.Shape).Radius);
				pt.Add(circleBody.GetPosition());
				contacts[i].Position = pt;
			}
			
			return count;
		}
	}
}