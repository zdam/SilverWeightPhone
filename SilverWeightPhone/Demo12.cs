using Silver.Weight.Raw;
using Silver.Weight.Raw.Shapes;
using Silver.Weight.Math;
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverWeightHarness
{
	public class Demo12 : AbstractDemo
	{
        public Demo12(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
			name = "Demo12";
		}

		protected override void Init(World world)
		{
			Body body1 = new StaticBody("Ground1", new Box(400.0f, 20.0f));
			body1.SetPosition(250.0f, 400);
			world.Add(body1);
			Body body3 = new StaticBody("Ground2", new Box(200.0f, 20.0f));
			body3.SetPosition(250.0f, 100);
			world.Add(body3);

			Body swing = new Body("Swing", new Circle(10), 50);
			swing.SetPosition(160.0f, 300);
			world.Add(swing);
			Body swing2 = new Body("Swing", new Circle(10), 50);
			swing2.SetPosition(340.0f, 300);
			world.Add(swing2);
			Body swing3 = new Body("Swing", new Box(250.0f, 10.0f), 50);
			swing3.SetPosition(250.0f, 285);
			swing3.Friction = 4.0f;
			world.Add(swing3);

			Body box = new Body("Resting", new Box(30, 30), 1);
			box.SetPosition(250.0f, 200);
			box.Rotation = 0.15f;
			world.Add(box);

			BasicJoint j1 = new BasicJoint(body3, swing, new Vector2f(160, 110));
			world.Add(j1);
			BasicJoint j2 = new BasicJoint(body3, swing2, new Vector2f(340, 110));
			world.Add(j2);
			BasicJoint j3 = new BasicJoint(swing, swing3, new Vector2f(160, 300));
			world.Add(j3);
			BasicJoint j4 = new BasicJoint(swing2, swing3, new Vector2f(340, 300));
			world.Add(j4);

			swing.AdjustVelocity(new Vector2f(-100, 0));
		}
	}
}
