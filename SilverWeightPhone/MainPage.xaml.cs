using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SilverWeightHarness;
using SilverUtil;

namespace SilverWeightPhone
{
    public partial class MainPage : PhoneApplicationPage
    {

        private IDrawer currentDrawer;
        AbstractDemo currentDemo;
        int current = 0;
        readonly IList<AbstractDemo> demos = new List<AbstractDemo>();

        public MainPage()
        {
            
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;

            currentDrawer = new SpriteDrawer(this.ObjectCanvas, "LargeTestGraphic.jpg");

            demos.Add(new Demo1(this));
            demos.Add(new Demo1a(this));
            demos.Add(new Demo2(this));
            demos.Add(new Demo6(this));
            demos.Add(new Demo7(this));
            demos.Add(new Demo10(this));
            demos.Add(new Demo11(this));
            demos.Add(new Demo4(this));
            demos.Add(new Demo12(this));

            KeyUp += Page_KeyUp;

            StartDemo();
        }

        void StartDemo()
        {
            if (currentDemo != null)
            {
                currentDemo.End();
            }
            currentDemo = demos[current];
            currentDemo.Drawer = currentDrawer;
            currentDemo.Start();
        }

        void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N) // nect demo
            {
                current++;
                if (current >= demos.Count)
                {
                    current = 0;
                }
                StartDemo();
            }
            if (e.Key == Key.D) //switch between sprite and line drawer
            {
                ToggleCurrentDrawer();
                StartDemo();
            }
        }

        private void ToggleCurrentDrawer()
        {
            if (currentDrawer is SpriteDrawer)
            {
                currentDrawer = new BaseDrawer(ObjectCanvas);
            }
            else
            {
                currentDrawer = new SpriteDrawer(ObjectCanvas, "LargeTestGraphic.jpg");
            }
        }

        public TextBlock StatsBlock
        {
            get { return WorldCanvas.FindName("statisticsTextBlock") as TextBlock; }
        }
        public Canvas WorldCanvas
        {
            get { return this.FindName("worldCanvas") as Canvas; }
        }
        public Canvas ObjectCanvas
        {
            get { return WorldCanvas.FindName("objectCanvas") as Canvas; }
        }
    }
}