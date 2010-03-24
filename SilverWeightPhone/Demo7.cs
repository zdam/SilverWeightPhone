using System;
using System.Windows.Input;
using Silver.Weight.Raw;
using Silver.Weight.Raw.Shapes;
using Silver.Weight.Math;
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverWeightHarness
{
	public class Demo7 : AbstractDemo
	{
		public Demo7(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
		}

		protected override void HandleKey(KeyHandler handler)
		{
			base.HandleKey(handler);

			if(handler.IsKeyPressed(Key.Space))
			{
				Random r = new Random();
				double rand = r.NextDouble();
				Body body2 = new Body("Mover1", new Box(40.0f, 40.0f), 300.0f);
				body2.SetPosition(- 50, (float) (((rand * 50) + 150)));
				world.Add(body2);
				body2.AdjustAngularVelocity(1);
				body2.AdjustVelocity(new Vector2f(200, (float) (rand * 200)));
			}
		}

		protected override void Init(World world)
		{
			Body body1 = new StaticBody("Ground1", new Box(400.0f, 20.0f));
			body1.SetPosition(250.0f, 400);
			body1.Friction = 1;
			world.Add(body1);

			for (int y = 0; y < 3; y++)
			{
				int xbase = 250 - (y * 21);
				for (int x = 0; x < y + 1; x++)
				{
					Body body2 = new Body("Mover1", new Box(40.0f, 40.0f), 100.0f);
					body2.SetPosition(xbase + (x * 42), y * 45);
					world.Add(body2);
				}
			}
		}
	}
}
