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
using Body = Silver.Weight.Raw.Body;
using Silver.Weight.Raw.Shapes;
namespace Silver.Weight.Raw.Collide
{
	
	/// <summary> A collider factory to create colliders for arbitrary bodies, 
	/// or actually their shapes.
	/// This was implemented to replace a visitor-pattern based implementation,
	/// that required many files to be edited to Add a new shape.
	/// Furthermore this factory can handle singleton colliders if needed.
	/// 
	/// </summary>
	/// <author>  Gideon Smeding
	/// 
	/// </author>
	public class ColliderFactory
	{
		
		/// <summary> Create a collider for two bodies. The decision depends on
		/// the body's shapes.
		/// 
		/// </summary>
		/// <param name="bodyA">First body in the collision test
		/// </param>
		/// <param name="bodyB">Second body in the collision test
		/// </param>
		/// <returns> A collider that can test wether the two bodies actually Collide
		/// </returns>
		/// <throws>  ColliderUnavailableException  </throws>
		/// <summary>         This exception will be thrown if no suitable collider can be found. 
		/// </summary>
		public virtual Collider CreateCollider(Body bodyA, Body bodyB)
		{
			Shape shapeA = bodyA.Shape;
			Shape shapeB = bodyB.Shape;
			
			if (shapeA is Circle)
			{
				return CreateColliderFor((Circle) shapeA, shapeB);
			}
			else if (shapeA is Box)
			{
				return CreateColliderFor((Box) shapeA, shapeB);
			}
			else if (shapeA is Line)
			{
				return CreateColliderFor((Line) shapeA, shapeB);
			}
			else if (shapeA is Polygon)
			{
				return CreateColliderFor((Polygon) shapeA, shapeB);
			}
			
			throw new ColliderUnavailableException(shapeA, shapeB);
		}
		
		/// <summary> Creates a collider for a Circle and a Shape.
		/// The choice is based on the kind of Shape that is provided
		/// 
		/// </summary>
		/// <param name="shapeA">The circle to provide a collider for
		/// </param>
		/// <param name="shapeB">The shape to provide a collider for
		/// </param>
		/// <returns> a suitable collider
		/// </returns>
		/// <throws>  ColliderUnavailableException </throws>
		/// <summary> 	       This exception will be thrown if no suitable collider can be found.
		/// </summary>
		public virtual Collider CreateColliderFor(Circle shapeA, Shape shapeB)
		{
			
			if (shapeB is Circle)
			{
				return new CircleCircleCollider();
			}
			else if (shapeB is Box)
			{
				return new SwapCollider(new BoxCircleCollider());
			}
			else if (shapeB is Line)
			{
				return new SwapCollider(new LineCircleCollider());
			}
			else if (shapeB is Polygon)
			{
				return new SwapCollider(new PolygonCircleCollider());
			}
			
			throw new ColliderUnavailableException(shapeA, shapeB);
		}
		
		/// <summary> Creates a collider for a Box and a Shape.
		/// The choice is based on the kind of Shape that is provided
		/// 
		/// </summary>
		/// <param name="shapeA">The box to provide a collider for
		/// </param>
		/// <param name="shapeB">The shape to provide a collider for
		/// </param>
		/// <returns> a suitable collider
		/// </returns>
		/// <throws>  ColliderUnavailableException </throws>
		/// <summary> 	       This exception will be thrown if no suitable collider can be found.
		/// </summary>
		public virtual Collider CreateColliderFor(Box shapeA, Shape shapeB)
		{
			
			if (shapeB is Circle)
			{
				return new BoxCircleCollider();
			}
			else if (shapeB is Box)
			{
				return new BoxBoxCollider();
			}
			else if (shapeB is Line)
			{
				return new SwapCollider(new LineBoxCollider());
			}
			else if (shapeB is Polygon)
			{
				return new SwapCollider(new PolygonBoxCollider());
			}
			
			throw new ColliderUnavailableException(shapeA, shapeB);
		}
		
		/// <summary> Creates a collider for a Line and a Shape.
		/// The choice is based on the kind of Shape that is provided
		/// 
		/// </summary>
		/// <param name="shapeA">The line to provide a collider for
		/// </param>
		/// <param name="shapeB">The shape to provide a collider for
		/// </param>
		/// <returns> a suitable collider
		/// </returns>
		/// <throws>  ColliderUnavailableException </throws>
		/// <summary> 	       This exception will be thrown if no suitable collider can be found.
		/// </summary>
		public virtual Collider CreateColliderFor(Line shapeA, Shape shapeB)
		{
			
			if (shapeB is Circle)
			{
				return new LineCircleCollider();
			}
			else if (shapeB is Box)
			{
				return new LineBoxCollider();
			}
			else if (shapeB is Line)
			{
				return new LineLineCollider();
			}
			else if (shapeB is Polygon)
			{
				return new LinePolygonCollider();
			}
			
			throw new ColliderUnavailableException(shapeA, shapeB);
		}
		
		/// <summary> Creates a collider for a ConvexPolygon and a Shape.
		/// The choice is based on the kind of Shape that is provided
		/// 
		/// </summary>
		/// <param name="shapeA">The convex polygon to provide a collider for
		/// </param>
		/// <param name="shapeB">The shape to provide a collider for
		/// </param>
		/// <returns> a suitable collider
		/// </returns>
		/// <throws>  ColliderUnavailableException </throws>
		/// <summary> 	       This exception will be thrown if no suitable collider can be found.
		/// </summary>
		public virtual Collider CreateColliderFor(Polygon shapeA, Shape shapeB)
		{
			
			if (shapeB is Circle)
			{
				return new PolygonCircleCollider();
			}
			else if (shapeB is Box)
			{
				return new PolygonBoxCollider();
			}
			else if (shapeB is Line)
			{
				return new SwapCollider(new LinePolygonCollider());
			}
			else if (shapeB is Polygon)
			{
				return new PolygonPolygonCollider();
			}
			
			throw new ColliderUnavailableException(shapeA, shapeB);
		}
	}
}