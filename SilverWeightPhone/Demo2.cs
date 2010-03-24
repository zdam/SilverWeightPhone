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
	public class Demo2 : AbstractDemo
	{
        public Demo2(PhoneApplicationPage rootCanvas)
			: base(rootCanvas)
		{
		}

		protected override void Init(World world)
		{
			Body body1 = new StaticBody("Ground1", new Box(600.0f, 20.0f));
			body1.SetPosition(250.0f, 400);
			world.Add(body1);
			Body body3 = new StaticBody("Ground2", new Box(200.0f, 20.0f));
			body3.SetPosition(360.0f, 340);
			body3.Rotation = 0.4f;
			world.Add(body3);
			Body body9 = new StaticBody("Ground3", new Box(200.0f, 20.0f));
			body9.SetPosition(140.0f, 340);
			body9.Rotation = - 0.4f;
			world.Add(body9);
			Body bodya = new StaticBody("Wall1", new Box(20.0f, 400.0f));
			bodya.SetPosition(20.0f, 190);
			world.Add(bodya);
			Body bodyb = new StaticBody("Wall2", new Box(20.0f, 400.0f));
			bodyb.SetPosition(480.0f, 190);
			world.Add(bodyb);
			
			Body body2 = new Body("Mover1", new Box(50.0f, 50.0f), 100.0f);
			body2.SetPosition(250.0f, 4.0f);
			body2.Rotation = 0.2f;
			world.Add(body2);
			Body body4 = new Body("Mover2", new Box(50.0f, 50.0f), 100.0f);
			body4.SetPosition(230.0f, - 60.0f);
			world.Add(body4);
			Body body8 = new Body("Mover3", new Box(50.0f, 50.0f), 100.0f);
			body8.SetPosition(280.0f, - 120.0f);
			world.Add(body8);
		}
	}
}
