using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Netcode.Common;
using System.IO;

namespace Netcode.Server
{
	
	class RelayServerUDP
	{
		private static UdpClient _udpServer;
		private static List<IPEndPoint> _clients = new List<IPEndPoint>();

		public void Start(int port)
		{
			_udpServer = new UdpClient(port);
			Console.WriteLine("UDP Server started on port 5000.");

			Thread listenThread = new Thread(new ThreadStart(ListenForMessages));
			listenThread.Start();

			Console.WriteLine("Press any key to terminate the server...");
			Console.ReadKey();
		}

		private static void ListenForMessages()
		{
			try
			{
				while (true)
				{
					IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
					byte[] receivedBytes = _udpServer.Receive(ref clientEndPoint);

					// Add client if not already in list
					if (!_clients.Contains(clientEndPoint))
					{
						_clients.Add(clientEndPoint);
						Console.WriteLine($"New client: {clientEndPoint}");
					}

					Message msg = MessagePack.MessagePackSerializer.Deserialize<Message>(receivedBytes);
					Console.WriteLine($"{clientEndPoint.Address} received {msg.ToString()}");

					Broadcast(receivedBytes, clientEndPoint);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
		}

		private static void Broadcast(byte[] data, IPEndPoint excludeClient)
		{
			foreach (var client in _clients)
			{
				if (!client.Equals(excludeClient))
				{
					_udpServer.Send(data, data.Length, client);
				}
			}
		}
	}
}
