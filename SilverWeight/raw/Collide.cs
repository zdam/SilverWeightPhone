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
//using System;
//using Collider = Silver.Weight.Raw.Collide.Collider;
//using ColliderFactory = Silver.Weight.Raw.Collide.ColliderFactory;
//using ColliderUnavailableException = Silver.Weight.Raw.Collide.ColliderUnavailableException;
//namespace Silver.Weight.Raw
//{
	
//    /// <summary> A static utility for Resolve the collision between shapes
//    /// 
//    /// TODO: make this nonstatic to allow a user to provide his/her own factory
//    /// 
//    /// </summary>
//    public static class Collidex
//    {
		
//        /// <summary>The factory that provides us with colliders </summary>
//        private static ColliderFactory collFactory = new ColliderFactory();
		
//        /// <summary> Perform the collision between two bodies
//        /// 
//        /// </summary>
//        /// <param name="contacts">The points of contact that should be populated
//        /// </param>
//        /// <param name="bodyA">The first body
//        /// </param>
//        /// <param name="bodyB">The second body
//        /// </param>
//        /// <param name="dt">The amount of time that's passed since we last checked collision
//        /// </param>
//        /// <returns> The number of points at which the two bodies contact
//        /// </returns>
//        public static int Collide(Contact[] contacts, Body bodyA, Body bodyB, float dt)
//        {
//            Collider collider;
//            try
//            {
//                collider = collFactory.CreateCollider(bodyA, bodyB);
//            }
//            catch (ColliderUnavailableException e)
//            {
//                System.Console.Out.WriteLine(e.Message + "\n Ignoring any possible collision between the bodies in question");
//                return 0;
//            }
			
//            return collider.Collide(contacts, bodyA, bodyB);
//        }

//    }
//}