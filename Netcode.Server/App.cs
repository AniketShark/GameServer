using Netcode.Server;

namespace GameServer
{
	class App
	{
		static void Main(string[] args)
		{
			NewServer();
		}

		static void NewServer()
		{
			int port = 5000;
			//RelayServerTCP server = new RelayServerTCP(port);
			//server.Start();
			RelayServerUDP serverUDP = new RelayServerUDP();
			serverUDP.Start(port);
		}
	}

}