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
	public class Demo4 : AbstractDemo
	{
        public Demo4(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
			name = "Demo4";
		}

		protected override void Init(World world)
		{
			Body body1 = new StaticBody("Ground1", new Box(500.0f, 20.0f));
			body1.SetPosition(250.0f, 400);
			world.Add(body1);

			Body body2 = new Body("Teeter", new Box(250.0f, 10.0f), 5);
			body2.Friction = 0.5f;
			body2.SetPosition(250.0f, 370);
			world.Add(body2);

			Body body3 = new Body("light1", new Box(10.0f, 10.0f), 10);
			body3.SetPosition(135, 360);
			world.Add(body3);
			Body body4 = new Body("light2", new Box(10.0f, 10.0f), 10);
			body4.SetPosition(150, 360);
			world.Add(body4);

			Body body5 = new Body("weight", new Box(25.0f, 25.0f), 30);
			body5.SetPosition(350, 50);
			world.Add(body5);

			BasicJoint j = new BasicJoint(body1, body2, new Vector2f(250, 370));
			world.Add(j);
		}
	}
}
