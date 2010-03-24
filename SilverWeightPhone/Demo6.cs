using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Silver.Weight.Raw;
using Silver.Weight.Raw.Shapes;
using Silver.Weight.Math;
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverWeightHarness
{
	public class Demo6 : AbstractDemo
	{
        public Demo6(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
		}

		private Body ball;

		protected override void HandleKey(KeyHandler handler)
		{
			base.HandleKey(handler);

			if (handler.IsKeyPressed(Key.Space))
			{
				if (ball.Velocity.Length() == 0)
				{
					ball.AddForce(new Vector2f(2000000, 0));
				}
			}
		}

		protected override void Init(World world)
		{

			Body body;
			BasicJoint j;

			Body ground1 = new StaticBody("Ground1", new Box(500.0f, 20.0f));
			ground1.SetPosition(250.0f, 400);
			world.Add(ground1);

			body = new StaticBody("Ground2", new Box(250.0f, 20.0f));
			body.SetPosition(225.0f, 200);
			body.Friction = 3.0f;
			world.Add(body);

			// pendulum
			body = new StaticBody("Pen1", new Box(20.0f, 20.0f));
			body.SetPosition(70.0f, 100);
			world.Add(body);
			ball = new Body("Ball", new Box(10.0f, 10.0f), 1000);
			ball.SetPosition(70.0f, 170);
			world.Add(ball);

			j = new BasicJoint(body, ball, new Vector2f(70, 110));
			world.Add(j);

			// dominos
			for (int i = 0; i < 8; i++)
			{
				body = new Body("Domino " + i, new Box(10.0f, 40.0f), 10 - i);
				body.SetPosition(120.0f + (i * 30), 170);
				world.Add(body);
			}

			// ramp
			body = new StaticBody("Ground2", new Box(200.0f, 10.0f));
			body.SetPosition(345.0f, 270);
			body.Rotation = -0.6f;
			body.Friction = 0;
			world.Add(body);

			// teeter
			body = new Body("Teete", new Box(250.0f, 10.0f), 10);
			body.SetPosition(250.0f, 360);
			//body.setFriction(3.0f);
			world.Add(body);
			j = new BasicJoint(body, ground1, new Vector2f(250, 360));
			world.Add(j);

			// turner
			body = new Body("Turner", new Box(40.0f, 40.0f), 0.1f);
			body.SetPosition(390.0f, 330);
			body.Friction = 0f;
			world.Add(body);
			j = new BasicJoint(ground1, body, new Vector2f(390, 335));
			world.Add(j);
			Body top = new Body("Top", new Box(40.0f, 5.0f), 0.01f);
			top.SetPosition(390.0f, 307.5f);
			top.Friction = 0f;
			world.Add(top);
			j = new BasicJoint(top, body, new Vector2f(410, 310));
			world.Add(j);
		}
	}
}
