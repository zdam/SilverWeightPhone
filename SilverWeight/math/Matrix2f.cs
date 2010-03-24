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
	public class Matrix2f
	{
		/// <summary>The first column of the matrix </summary>
		public Vector2f col1 = new Vector2f();
		/// <summary>The second column of the matrix </summary>
		public Vector2f col2 = new Vector2f();
		
		/// <summary> Create an empty matrix</summary>
		public Matrix2f()
		{
		}
		
		/// <summary> Create a matrix with a rotation
		/// 
		/// </summary>
		/// <param name="angle">The angle of the rotation decribed by the matrix
		/// </param>
		public Matrix2f(float angle)
		{
			float c = (float) System.Math.Cos(angle);
			float s = (float) System.Math.Sin(angle);
			col1.x = c; col2.x = - s;
			col1.y = s; col2.y = c;
		}
		
		/// <summary> Create a matrix
		/// 
		/// </summary>
		/// <param name="col1">The first column
		/// </param>
		/// <param name="col2">The second column
		/// </param>
		public Matrix2f(Vector2f col1, Vector2f col2)
		{
			this.col1.Reconfigure(col1);
			this.col2.Reconfigure(col2);
		}
		
		/// <summary> Transpose the matrix
		/// 
		/// </summary>
		/// <returns> A newly created matrix containing the Transpose of this matrix
		/// </returns>
		public virtual Matrix2f Transpose()
		{
			return new Matrix2f(new Vector2f(col1.x, col2.x), new Vector2f(col1.y, col2.y));
		}
		
		/// <summary> Transpose the Invert
		/// 
		/// </summary>
		/// <returns> A newly created matrix containing the Invert of this matrix
		/// </returns>
		public virtual Matrix2f Invert()
		{
			float a = col1.x, b = col2.x, c = col1.y, d = col2.y;
			Matrix2f B = new Matrix2f();
			
			float det = a * d - b * c;
			if (det == 0.0f)
			{
				throw new System.SystemException("Matrix2f: Invert() - determinate is zero!");
			}
			
			det = 1.0f / det;
			B.col1.x = det * d; B.col2.x = (- det) * b;
			B.col1.y = (- det) * c; B.col2.y = det * a;
			return B;
		}
	}
}