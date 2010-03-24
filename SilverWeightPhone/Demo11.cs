using System;
using System.Windows.Input;
using Silver.Weight.Raw;
using Silver.Weight.Raw.Shapes;
using Silver.Weight.Math;
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverWeightHarness
{
	public class Demo11 : AbstractDemo
	{
        public Demo11(PhoneApplicationPage rootCanvas)
            : base(rootCanvas)
		{
			name = "Demo11";
		}

		protected override void HandleKey(KeyHandler handler)
		{
			base.HandleKey(handler);

			if (handler.IsKeyPressed(Key.Space))
			{
				Random r = new Random();
				double rand = r.NextDouble();
				Body body2 = new Body("Mover1", new Circle(20), 300.0f);
				body2.SetPosition(-50, (float)(((rand * 50) + 150)));
				world.Add(body2);
				body2.AdjustAngularVelocity(1);
				body2.AdjustVelocity(new Vector2f(200, (float)(rand * 200)));
			}
		}

		protected override void Init(World world)
		{
			this.world = world;

			Body body1 = new StaticBody("Ground1", new Box(400.0f, 20.0f));
			body1.SetPosition(250.0f, 400);
			body1.Friction = 1;
			world.Add(body1);

			for (int y = 0; y < 5; y++)
			{
				int xbase = 250 - (y * 21);
				for (int x = 0; x < y + 1; x++)
				{
					DynamicShape shape = new Box(40, 40);
					if ((x == 1) && (y == 2))
					{
						shape = new Circle(19);
					}
					if ((x == 1) && (y == 4))
					{
						shape = new Circle(19);
					}
					if ((x == 3) && (y == 4))
					{
						shape = new Circle(19);
					}
					Body body2 = new Body("Mover1", shape, 100.0f);
					body2.SetPosition(xbase + (x * 42), y * 45);
					world.Add(body2);
				}
			}
		}
	}
}
