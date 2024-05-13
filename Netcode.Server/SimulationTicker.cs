using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netcode.Server
{
	public class SimulationTicker
	{
		public static bool running;

		public void Start()
		{

			const int TICK_RATE = 100; // 100 milliseconds per tick
			long lastTickTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

			while (running)
			{
				long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				long elapsedTime = currentTime - lastTickTime;

				if (elapsedTime >= TICK_RATE)
				{
					// Perform game logic updates
					UpdateGame();

					lastTickTime = currentTime;
				}

				Thread.Sleep(1); // Ensure the thread yields to prevent CPU hogging
			}
		}

		public void Stop()
		{
			running = false;
		}

		private void UpdateGame()
		{
			// Perform game logic updates here
			// This method is called at a fixed tick rate
		}


	}
}
