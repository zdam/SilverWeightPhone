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
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverWeightHarness
{
	public class Demo1 : AbstractDemo
	{
        public Demo1(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
			name = "Demo1";
		}

		protected override void Init(World world)
		{
			Body body1 = new StaticBody("Ground1", new Box(400.0f, 20.0f));

			body1.SetPosition(250.0f, 400);
			world.Add(body1);
			Body body3 = new StaticBody("Ground2", new Box(200.0f, 20.0f));
			body3.SetPosition(360.0f, 380);
			world.Add(body3);
			Body body5 = new StaticBody("Ground3", new Box(20.0f, 100.0f));
			body5.SetPosition(200.0f, 300);
			world.Add(body5);
			Body body6 = new StaticBody("Ground3", new Box(20.0f, 100.0f));
			body6.SetPosition(400.0f, 300);
			world.Add(body6);

			Body body2 = new Body("Mover1", new Box(50.0f, 50.0f), 100.0f);
			body2.SetPosition(250.0f, 4.0f);
			world.Add(body2);
			Body body4 = new Body("Mover2", new Box(50.0f, 50.0f), 100.0f);
			body4.SetPosition(230.0f, -60.0f);
			world.Add(body4);
			Body body8 = new Body("Mover3", new Box(50.0f, 50.0f), 100.0f);
			body8.SetPosition(280.0f, -120.0f);
			world.Add(body8);
		}
	}
}
