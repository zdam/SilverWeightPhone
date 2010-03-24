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
namespace Silver.Weight.Math
{
	
	/// <summary> A two dimensional vector
	/// 
	/// </summary>
	public class Vector2f : ROVector2f
	{
		/// <seealso cref="Silver.Weight.Math.ROVector2f.getX()">
		/// </seealso>
		virtual public float X
		{
			get
			{
				return x;
			}
			
		}
		/// <seealso cref="Silver.Weight.Math.ROVector2f.getY()">
		/// </seealso>
		virtual public float Y
		{
			get
			{
				return y;
			}
			
		}
		/// <summary>The x component of this vector </summary>
		public float x;
		/// <summary>The y component of this vector </summary>
		public float y;
		
		/// <summary> Create an empty vector</summary>
		public Vector2f()
		{
		}
		
		/// <summary> Create a new vector based on another
		/// 
		/// </summary>
		/// <param name="other">The other vector to copy into this one
		/// </param>
		public Vector2f(ROVector2f other):this(other.X, other.Y)
		{
		}
		
		/// <summary> Create a new vector 
		/// 
		/// </summary>
		/// <param name="x">The x component to assign
		/// </param>
		/// <param name="y">The y component to assign
		/// </param>
		public Vector2f(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		/// <summary> Set the value of this vector
		/// 
		/// </summary>
		/// <param name="other">The values to set into the vector
		/// </param>
		public virtual void  Reconfigure(ROVector2f other)
		{
			Reconfigure(other.X, other.Y);
		}
		
		/// <seealso cref="Silver.Weight.Math.ROVector2f.Dot(Silver.Weight.Math.ROVector2f)">
		/// </seealso>
		public virtual float Dot(ROVector2f other)
		{
			return (x * other.X) + (y * other.Y);
		}
		
		/// <summary> Set the values in this vector
		/// 
		/// </summary>
		/// <param name="x">The x component to set
		/// </param>
		/// <param name="y">The y component to set
		/// </param>
		public virtual void  Reconfigure(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		/// <summary> Negate this vector 
		/// 
		/// </summary>
		/// <returns> A copy of this vector negated
		/// </returns>
		public virtual Vector2f Negate()
		{
			return new Vector2f(- x, - y);
		}
		
		/// <summary> Add a vector to this vector
		/// 
		/// </summary>
		/// <param name="v">The vector to Add
		/// </param>
		public virtual void  Add(ROVector2f v)
		{
			x += v.X;
			y += v.Y;
		}
		
		/// <summary> Subtract a vector from this vector
		/// 
		/// </summary>
		/// <param name="v">The vector subtract
		/// </param>
		public virtual void  Sub(ROVector2f v)
		{
			x -= v.X;
			y -= v.Y;
		}
		
		/// <summary> Scale this vector by a value
		/// 
		/// </summary>
		/// <param name="a">The value to Scale this vector by
		/// </param>
		public virtual void  Scale(float a)
		{
			x *= a;
			y *= a;
		}
		
		/// <summary> Normalise the vector
		/// 
		/// </summary>
		public virtual void  Normalise()
		{
			float l = Length();
			
			if (l == 0)
				return ;
			
			x /= l;
			y /= l;
		}
		
		/// <summary> The Length of the vector squared
		/// 
		/// </summary>
		/// <returns> The Length of the vector squared
		/// </returns>
		public virtual float LengthSquared()
		{
			return (x * x) + (y * y);
		}
		
		/// <seealso cref="Silver.Weight.Math.ROVector2f.Length()">
		/// </seealso>
		public virtual float Length()
		{
			return (float) System.Math.Sqrt(LengthSquared());
		}
		
		/// <summary> Project this vector onto another
		/// 
		/// </summary>
		/// <param name="b">The vector to project onto
		/// </param>
		/// <param name="result">The projected vector
		/// </param>
		public virtual void  ProjectOntoUnit(ROVector2f b, Vector2f result)
		{
			float dp = b.Dot(this);
			
			result.x = dp * b.X;
			result.y = dp * b.Y;
		}
		
		/// <seealso cref="java.lang.Object.toString()">
		/// </seealso>
		public override System.String ToString()
		{
			return "[Vec " + x + "," + y + " (" + Length() + ")]";
		}
		
		/// <summary> Get the Distance from this point to another
		/// 
		/// </summary>
		/// <param name="other">The other point we're measuring to
		/// </param>
		/// <returns> The Distance to the other point
		/// </returns>
		public virtual float Distance(ROVector2f other)
		{
			return (float) System.Math.Sqrt(DistanceSquared(other));
		}
		
		/// <summary> Get the Distance squared from this point to another
		/// 
		/// </summary>
		/// <param name="other">The other point we're measuring to
		/// </param>
		/// <returns> The Distance to the other point
		/// </returns>
		public virtual float DistanceSquared(ROVector2f other)
		{
			float dx = other.X - X;
			float dy = other.Y - Y;
			
			return (dx * dx) + (dy * dy);
		}
		
		/// <summary> Compare two vectors allowing for a (small) error as indicated by the delta.
		/// Note that the delta is used for the vector's components separately, i.e.
		/// any other vector that is contained in the square box with sides 2*delta and this
		/// vector at the center is considered equal. 
		/// 
		/// </summary>
		/// <param name="other">The other vector to compare this one to
		/// </param>
		/// <param name="delta">The allowed error
		/// </param>
		/// <returns> True iff this vector is equal to other, with a tolerance defined by delta
		/// </returns>
		public virtual bool EqualsDelta(ROVector2f other, float delta)
		{
			return (other.X - delta < x && other.X + delta > x && other.Y - delta < y && other.Y + delta > y);
		}
	}
}