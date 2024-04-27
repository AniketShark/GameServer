using Server.Netcode;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameServer
{
	class Program
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