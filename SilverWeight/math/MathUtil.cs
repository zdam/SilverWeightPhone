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
	
	/// <summary> Simple utility wrapping up a bunch of math operations so that 
	/// the rest of the code doesn't have to look so cluttered.
	/// 
	/// </summary>
	public sealed class MathUtil
	{
		/// <summary>Prevent construction </summary>
		private MathUtil()
		{
		}
		
		/// <summary> Scale a vector by a given value
		/// 
		/// </summary>
		/// <param name="a">The vector to be scaled
		/// </param>
		/// <param name="Scale">The amount to Scale the vector by
		/// </param>
		/// <returns> A newly created vector - a scaled version of the new vector
		/// </returns>
		public static Vector2f Scale(ROVector2f a, float scale)
		{
			Vector2f temp = new Vector2f(a);
			temp.Scale(scale);
			
			return temp;
		}
		
		/// <summary> Subtract one vector from another
		/// 
		/// </summary>
		/// <param name="a">The vector to be subtracted from
		/// </param>
		/// <param name="b">The vector to subtract
		/// </param>
		/// <returns> A newly created containing the result
		/// </returns>
		public static Vector2f Sub(ROVector2f a, ROVector2f b)
		{
			Vector2f temp = new Vector2f(a);
			temp.Sub(b);
			
			return temp;
		}
		
		/// <summary> Check the Sign of a value 
		/// 
		/// </summary>
		/// <param name="x">The value to check
		/// </param>
		/// <returns> -1.0f if negative, 1.0 if positive
		/// </returns>
		public static float Sign(float x)
		{
			return x < 0.0f?- 1.0f:1.0f;
		}
		
		/// <summary> Multiply a matrix by a vector
		/// 
		/// </summary>
		/// <param name="A">The matrix to be multiplied
		/// </param>
		/// <param name="v">The vector to multiple by
		/// </param>
		/// <returns> A newly created vector containing the resultant vector
		/// </returns>
		public static Vector2f Mul(Matrix2f A, ROVector2f v)
		{
			return new Vector2f(A.col1.x * v.X + A.col2.x * v.Y, A.col1.y * v.X + A.col2.y * v.Y);
		}
		
		/// <summary> Multiple two matricies
		/// 
		/// </summary>
		/// <param name="A">The first matrix
		/// </param>
		/// <param name="B">The second matrix
		/// </param>
		/// <returns> A newly created matrix containing the result
		/// </returns>
		public static Matrix2f Mul(Matrix2f A, Matrix2f B)
		{
			return new Matrix2f(Mul(A, B.col1), Mul(A, B.col2));
		}
		
		/// <summary> Create the absolute version of a matrix
		/// 
		/// </summary>
		/// <param name="A">The matrix to make absolute
		/// </param>
		/// <returns> A newly created absolute matrix
		/// </returns>
		public static Matrix2f Abs(Matrix2f A)
		{
			return new Matrix2f(Abs(A.col1), Abs(A.col2));
		}
		
		/// <summary> Make a vector absolute
		/// 
		/// </summary>
		/// <param name="a">The vector to make absolute
		/// </param>
		/// <returns> A newly created result vector
		/// </returns>
		public static Vector2f Abs(Vector2f a)
		{
			return new Vector2f(System.Math.Abs(a.x), System.Math.Abs(a.y));
		}
		
		/// <summary> Add two matricies
		/// 
		/// </summary>
		/// <param name="A">The first matrix
		/// </param>
		/// <param name="B">The second matrix
		/// </param>
		/// <returns> A newly created matrix containing the result
		/// </returns>
		public static Matrix2f Add(Matrix2f A, Matrix2f B)
		{
			Vector2f temp1 = new Vector2f(A.col1);
			temp1.Add(B.col1);
			Vector2f temp2 = new Vector2f(A.col2);
			temp2.Add(B.col2);
			
			return new Matrix2f(temp1, temp2);
		}
		
		/// <summary> Find the Cross product of two vectors
		/// 
		/// </summary>
		/// <param name="a">The first vector
		/// </param>
		/// <param name="b">The second vector
		/// </param>
		/// <returns> The Cross product of the two vectors
		/// </returns>
		public static float Cross(Vector2f a, Vector2f b)
		{
			return a.x * b.y - a.y * b.x;
		}
		
		/// <summary> Find the Cross product of a vector and a float
		/// 
		/// </summary>
		/// <param name="s">The scalar float
		/// </param>
		/// <param name="a">The vector to fidn the Cross of
		/// </param>
		/// <returns> A newly created resultant vector
		/// </returns>
		public static Vector2f Cross(float s, Vector2f a)
		{
			return new Vector2f((- s) * a.y, s * a.x);
		}
		
		/// <summary> Find the Cross product of a vector and a float
		/// 
		/// </summary>
		/// <param name="s">The scalar float
		/// </param>
		/// <param name="a">The vector to fidn the Cross of
		/// </param>
		/// <returns> A newly created resultant vector
		/// </returns>
		public static Vector2f Cross(Vector2f a, float s)
		{
			return new Vector2f(s * a.y, (- s) * a.x);
		}
		
		/// <summary> Clamp a value 
		/// 
		/// </summary>
		/// <param name="a">The original value
		/// </param>
		/// <param name="low">The lower bound
		/// </param>
		/// <param name="high">The upper bound
		/// </param>
		/// <returns> The clamped value
		/// </returns>
		public static float Clamp(float a, float low, float high)
		{
			return System.Math.Max(low, System.Math.Min(a, high));
		}
		
		
		/// <summary> Get the normal of a line x y (or edge). 
		/// When standing on x facing y, the normal will point
		/// to the left.
		/// 
		/// TODO: Move this function somewhere else?
		/// 
		/// </summary>
		/// <param name="x">startingpoint of the line
		/// </param>
		/// <param name="y">endpoint of the line
		/// </param>
		/// <returns> a (normalised) normal
		/// </returns>
		public static Vector2f GetNormal(ROVector2f x, ROVector2f y)
		{
			Vector2f normal = new Vector2f(y);
			normal.Sub(x);
			
			normal = new Vector2f(normal.y, - normal.x);
			normal.Normalise();
			
			return normal;
		}

		private const float Pi = 3.14159265358979323846264338327950288419716939937510F;
		/// <summary>
		/// Convert Radians to Degrees
		/// </summary>
		/// <param name="radians"></param>
		/// <returns></returns>
		public static float RadToDeg(float radians)
		{
			return radians * (180 / Pi);
		}

		public static float DegToRad(float degrees)
		{
			return  degrees * (Pi / 180  );
		}
	
		//	public static Vector2f Intersect(Vector2f startA, Vector2f endA, Vector2f startB, Vector2f endB) {				
		//		float d = (endB.y - startB.y) * (endA.x - startA.x) - (endB.x - startB.x) * (endA.y - startA.y);
		//		
		//		if ( d == 0 ) // parallel lines
		//			return null;
		//		
		//		float uA = (endB.x - startB.x) * (startA.y - startB.y) - (endB.y - startB.y) * (startA.x - startB.x);
		//		uA /= d;
		//		float uB = (endA.x - startA.x) * (startA.y - startB.y) - (endA.y - startA.y) * (startA.x - startB.x);
		//		uB /= d;
		//		
		//		if ( uA < 0 || uA > 1 || uB < 0 || uB > 1 ) 
		//			return null; // intersection point isn't between the start and endpoints
		//		
		//		return new Vector2f(
		//				startA.x + uA * (endA.x - startA.x),
		//				startA.y + uA * (endA.y - startA.y));
		//	}
	}
}