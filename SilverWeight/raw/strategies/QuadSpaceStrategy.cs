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
using BodyList = Silver.Weight.Raw.BodyList;
using BroadCollisionStrategy = Silver.Weight.Raw.BroadCollisionStrategy;
using CollisionContext = Silver.Weight.Raw.CollisionContext;
using AABox = Silver.Weight.Raw.Shapes.AABox;
using System.Collections;
using System.Collections.Generic;
namespace Silver.Weight.Raw.Strategies
{
	
	/// <summary> A strategy that divides the space into 4 repeatedly until either
	/// the target number of bodies is reached or the a given level of
	/// subdivisions is reached.
	/// 
	/// </summary>
	public class QuadSpaceStrategy : BroadCollisionStrategy
	{
		/// <summary> Get the spaces dervied in the quad Process
		/// 
		/// </summary>
		/// <returns> The list of spaces dervied (QuadSpaceStrategy.Space)
		/// </returns>
		virtual public IList<Space> Spaces
		{
			get {	return spaces; }			
		}
		/// <summary>The spaces dervied </summary>
		private IList<Space> spaces = new List<Space>();
		/// <summary>The number of Sub divisions allows </summary>
		private int maxLevels;
		/// <summary>The maximum number of bodies in a given space acceptable </summary>
		private int maxInSpace;
		
		/// <summary> Create a new strategy
		/// 
		/// </summary>
		/// <param name="maxInSpace">The maximum number of bodies in a given space acceptable
		/// </param>
		/// <param name="maxLevels">The number of Sub divisions allows
		/// </param>
		public QuadSpaceStrategy(int maxInSpace, int maxLevels)
		{
			this.maxInSpace = maxInSpace;
			this.maxLevels = maxLevels;
		}
		
		/// <seealso cref="Silver.Weight.Raw.BroadCollisionStrategy.CollideBodies(Silver.Weight.Raw.CollisionContext, Silver.Weight.Raw.BodyList, float)">
		/// </seealso>
		public virtual void  CollideBodies(CollisionContext context, BodyList bodies, float dt)
		{
			spaces.Clear();
			Space space = new Space( 0, 0, 0, 0);
			
			for (int i = 0; i < bodies.Size(); i++)
			{
				Body body = bodies.Item(i);
				
				space.AddAABox(body.Shape.Bounds, body.GetPosition().X, body.GetPosition().Y);
				space.AddBody(body);
			}
			
			SplitSpace(space, 0, maxInSpace, spaces);
			
			for (int i = 0; i < spaces.Count; i++)
			{
				context.Resolve((Space) spaces[i], dt);
			}
		}
		
		/// <summary> Considering splitting a space into 4 Sub-spaces
		/// 
		/// </summary>
		/// <param name="space">The space to subdivide
		/// </param>
		/// <param name="level">The number of levels of subdivision allowed 
		/// </param>
		/// <param name="target">The target number of bodies per space
		/// </param>
		/// <param name="spaceList">The list of spaces to populate
		/// </param>
		/// <returns> True if the target has found
		/// </returns>
		private bool SplitSpace(Space space, int level, int target, IList<Space> spaceList)
		{
			if (space.Size() <= target)
			{
				spaceList.Add(space);
				return true;
			}
			if (level > maxLevels)
			{
				spaceList.Add(space);
				return true;
			}
			
			Space[] spaces = space.QuadSpaces;
			for (int j = 0; j < 4; j++)
			{
				SplitSpace(spaces[j], level + 1, target, spaceList);
			}
			
			return false;
		}
		
		/// <summary> A single space within the quad-tree
		/// 
		/// </summary>
		public class Space:BodyList
		{
			/// <summary> Sub-divide this space into four seperate Sub-spaces dolling
			/// out the bodies into each space
			/// 
			/// </summary>
			/// <returns> The spaces created by the subdivision (always Length 4)
			/// </returns>
			virtual public Space[] QuadSpaces
			{
				get
				{
					Space[] spaces = new Space[4];
					float width = (this.x2 - this.x1) / 2;
					float height = (this.y2 - this.y1) / 2;
					
					spaces[0] = new Space( x1, y1, width, height);
					spaces[1] = new Space(x1, y1 + height, width, height);
					spaces[2] = new Space(x1 + width, y1, width, height);
					spaces[3] = new Space(x1 + width, y1 + height, width, height);
					
					for (int i = 0; i < Size(); i++)
					{
						Body body = Item(i);
						for (int j = 0; j < 4; j++)
						{
							if (spaces[j].Touches(body.Shape.Bounds, body.GetPosition().X, body.GetPosition().Y))
							{
								spaces[j].Add(body);
							}
						}
					}
					
					return spaces;
				}
				
			}
			/// <summary> Get the top left x coordinate of this space
			/// 
			/// </summary>
			/// <returns> The top left x coordinate of this space
			/// </returns>
			virtual public float X1
			{
				get
				{
					return x1;
				}
				
			}
			/// <summary> Get the bottom right x coordinate of this space
			/// 
			/// </summary>
			/// <returns> The bottom right x coordinate of this space
			/// </returns>
			virtual public float X2
			{
				get
				{
					return x2;
				}
				
			}
			/// <summary> Get the top left y coordinate of this space
			/// 
			/// </summary>
			/// <returns> The top left y coordinate of this space
			/// </returns>
			virtual public float Y1
			{
				get
				{
					return y1;
				}
				
			}
			/// <summary> Get the bottom right y coordinate of this space
			/// 
			/// </summary>
			/// <returns> The bottom right y coordinate of this space
			/// </returns>
			virtual public float Y2
			{
				get
				{
					return y2;
				}
				
			}
				
			//}
			/// <summary>The top left x coordinate </summary>
			public float x1;
			/// <summary>The top left y coordinate </summary>
			public float y1;
			/// <summary>The bottom right x coordinate </summary>
			public float x2;
			/// <summary>The bottom right y coordinate </summary>
			public float y2;
			
			/// <summary> Create a space within the quad tree
			/// 
			/// </summary>
			/// <param name="x">The x position of the space
			/// </param>
			/// <param name="y">The y position of the space
			/// </param>
			/// <param name="width">The width of the space
			/// </param>
			/// <param name="height">The height of the space
			/// </param>
			public Space(float x, float y, float width, float height)
			{
				this.x1 = x;
				this.y1 = y;
				this.x2 = x + width;
				this.y2 = y + height;
			}
			
			/// <summary> Add a pody to the space
			/// 
			/// </summary>
			/// <param name="body">The body to Add to the space
			/// </param>
			public virtual void  AddBody(Body body)
			{
				Add(body);
			}
			
			/// <summary> Combine this space with another box
			/// 
			/// </summary>
			/// <param name="box">The box to include in this space
			/// </param>
			/// <param name="xp">The x position of the box
			/// </param>
			/// <param name="yp">The y position of the box
			/// </param>
			public virtual void  AddAABox(AABox box, float xp, float yp)
			{
				float x1 = xp - box.Width;
				float x2 = xp + box.Width;
				float y1 = yp - box.Height;
				float y2 = yp + box.Height;
				
				this.x1 = System.Math.Min(x1, this.x1);
				this.y1 = System.Math.Min(y1, this.y1);
				this.x2 = System.Math.Max(x2, this.x2);
				this.y2 = System.Math.Max(y2, this.y2);
			}
			
			/// <summary> Check if this space Touches a box
			/// 
			/// </summary>
			/// <param name="box">The box to check against
			/// </param>
			/// <param name="xp">The x position of the box to check
			/// </param>
			/// <param name="yp">The y position of the box to check
			/// </param>
			/// <returns> True if the box Touches these space
			/// </returns>
			public virtual bool Touches(AABox box, float xp, float yp)
			{
				float thisWidth = (this.x2 - this.x1) / 2;
				float thisHeight = (this.y2 - this.y1) / 2;
				float thisCx = this.x1 + thisWidth;
				float thisCy = this.y1 + thisHeight;
				
				float x1 = xp - (box.Width / 2);
				float x2 = xp + (box.Width / 2);
				float y1 = yp - (box.Height / 2);
				float y2 = yp + (box.Height / 2);
				
				float otherWidth = (x2 - x1) / 2;
				float otherHeight = (y2 - y1) / 2;
				float otherCx = xp;
				float otherCy = yp;
				
				float dx = System.Math.Abs(thisCx - otherCx);
				float dy = System.Math.Abs(thisCy - otherCy);
				float totalWidth = thisWidth + otherWidth;
				float totalHeight = thisHeight + otherHeight;
				
				return (totalWidth > dx) && (totalHeight > dy);
			}
			
			/// <seealso cref="java.lang.Object.toString()">
			/// </seealso>
			public override System.String ToString()
			{
				return "[Space " + x1 + "," + y1 + " " + x2 + "," + y2 + " " + Size() + " bodies]";
			}
		}
	}
}