	using System;
	using System.Windows.Media;

namespace SilverUtil
{
	public struct GameLoopEventArgs
	{
		public GameLoopEventArgs(TimeSpan elapsed)
		{
			Elapsed = elapsed;
		}
		public TimeSpan Elapsed;
	}

	public class GameLoop
	{
		protected DateTime lastTick;
		public delegate void UpdateHandler(object sender, GameLoopEventArgs e);
		public event UpdateHandler Update;

		public GameLoop()
		{
			CompositionTarget.Rendering += CompositionTarget_Rendering;
		}

		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			Tick();
		}

		public void Tick()
		{
			DateTime now = DateTime.Now;
			TimeSpan elapsed = now - lastTick;
			lastTick = now;
			if (Update != null) Update(this, new GameLoopEventArgs(elapsed));
		}

		public virtual void Start()
		{
			lastTick = DateTime.Now;
		}

		public virtual void Stop()
		{
			// fix - if Start is called after Stop, we will not be attatched...
			CompositionTarget.Rendering -= CompositionTarget_Rendering;
		}
	}
}
