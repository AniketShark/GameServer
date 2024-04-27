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
			RelayServer server = new RelayServer(port);
			server.Start();
		}
	}

}