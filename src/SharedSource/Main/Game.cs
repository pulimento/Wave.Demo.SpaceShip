#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace W25SpaceShipDemo
{
    public class Game : WaveEngine.Framework.Game
    {
        public static int score = 0;

        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            this.Load(WaveContent.GameInfo);

			ScreenContext screenContext = new ScreenContext(new MyScene());	
			WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
