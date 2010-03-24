using Silver.Weight.Raw;
using Silver.Weight.Raw.Shapes;
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverWeightHarness
{
	public class Demo10: AbstractDemo
	{
        public Demo10(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
			name = "Demo10";
		}

		protected override void Init(World world)
		{
			Body body1 = new StaticBody("Ground1", new Box(400.0f, 20.0f));
			body1.SetPosition(250.0f, 400);
			world.Add(body1);
			Body body1b = new StaticBody("Ground1", new Box(20.0f, 400.0f));
			body1b.SetPosition(20.0f, 200);
			world.Add(body1b);

			Body body3 = new Body("Mover2", new Circle(25), 50.0f);
			body3.SetPosition(225.0f, 365);
			world.Add(body3);
			Body body2 = new Body("Mover1", new Circle(25), 50.0f);
			body2.SetPosition(275.0f, 365);
			world.Add(body2);
			Body body3a = new Body("Mover2", new Circle(25), 50.0f);
			body3a.SetPosition(175.0f, 365);
			world.Add(body3a);
			Body body2a = new Body("Mover1", new Circle(25), 50.0f);
			body2a.SetPosition(325.0f, 365);
			world.Add(body2a);

			Body faller = new Body("Faller", new Circle(25), 200.0f);
			faller.SetPosition(250.0f, -20f);
			world.Add(faller);
		}
	}
}
