using System.Windows.Input;
using Silver.Weight.Raw;
using Silver.Weight.Math;
using Silver.Weight.Raw.Strategies;
using System.Windows.Controls;
using SilverUtil;
using Microsoft.Phone.Controls;

namespace SilverUtil
{
	public abstract class AbstractDemo
	{
		protected string name;// = "root";
		protected PhoneApplicationPage page;

		private GameLoop gameLoop;

		KeyHandler keyHandler = new KeyHandler();

		/// <summary>The world containing the physics model </summary>
		protected internal World world;

		public AbstractDemo(PhoneApplicationPage page)
		{
			this.page = page;
			gameLoop = new GameLoop();
		}

		private void AddDebugText(string debugText)
		{
			StatsBlock.Text = debugText;
		}

		public void Start()
		{
			InitDemo();

			keyHandler.Attach(page);

			gameLoop.Update += gameLoop_Update;
			gameLoop.Start();
		}

		public void End()
		{
			keyHandler.Detach(page);
			
			gameLoop.Update -= gameLoop_Update;
		}

		private void InitDemo()
		{
			ObjectCanvas.Children.Clear();
			world = new World(new Vector2f(0.0f, 10.0f), 10, new QuadSpaceStrategy(20, 5));
			world.SetGravity(0, 10);
			shouldStop = false;
			// Let subclasses do stuff
			Init(world);
		}

		void gameLoop_Update(object sender, GameLoopEventArgs e)
		{
			float fps = 60;
			if (e.Elapsed.TotalMilliseconds != 0)
			{
				fps = 1000f / (float)e.Elapsed.TotalMilliseconds;
			}

			HandleKeys();

			Drawer.DrawWorld(world);

			if (!shouldStop)
			{
				float? dt = 1 / fps;
				 
				StepWorld(2, dt);
                //NewStepWorld(dt);
			}
			AddDebugText("space = throw, r = restart, s = stop, g = go, n = Next, d = Switch Drawer, fps = " + fps.ToString());
		}

        private void NewStepWorld(float? dt)
        {
            world.Step(dt.Value);
        }

		private void StepWorld(int numTimes, float? dt)
		{
			if (dt == null)
			{
				dt = 1 / 60.0f;
			}
			for (int i = 0; i < numTimes; i++)
			{
				world.Step(dt.Value);
			}
		}

		private void HandleKeys()
		{
			if (keyHandler.IsKeyPressed(Key.R))
			{
				InitDemo();
			}
			if (keyHandler.IsKeyPressed(Key.S))
			{
				Stop();
			}
			if (keyHandler.IsKeyPressed(Key.G))
			{
				Go();
			}
			HandleKey(keyHandler);
		}

		private void Go()
		{
			shouldStop = false;
		}

		private bool shouldStop = false;
		private void Stop()
		{
			shouldStop = true;
		}

		protected internal abstract void Init(World world);

		protected virtual void HandleKey(KeyHandler handler)
		{
		}

		private IDrawer drawer;
		public IDrawer Drawer
		{
			get 
			{
				if (drawer == null)
				{
					drawer = new BaseDrawer(WorldCanvas);
				}
				return drawer; 
			}
			set { drawer = value; }
		}


        public TextBlock StatsBlock
        {
            get { return WorldCanvas.FindName("statisticsTextBlock") as TextBlock; }
        }
        public Canvas WorldCanvas
        {
            get { return page.FindName("worldCanvas") as Canvas; }
        }
        public Canvas ObjectCanvas
        {
            get { return WorldCanvas.FindName("objectCanvas") as Canvas; }
        }
	}
}
