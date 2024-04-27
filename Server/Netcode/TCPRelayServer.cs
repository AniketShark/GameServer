using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Server.Netcode;

namespace Server.Netcode
{
	class RelayServer
	{
		private TcpListener _listener;
		private List<TcpClient> _clients = new List<TcpClient>();

		public RelayServer(int port)
		{
			_listener = new TcpListener(IPAddress.Any, port);
		}

		public void Start()
		{
			_listener.Start();
			Console.WriteLine("Server started. Waiting for connections...");

			while (true)
			{
				TcpClient client = _listener.AcceptTcpClient();
				_clients.Add(client);

				Thread clientThread = new Thread(HandleClient);
				clientThread.Start(client);
			}
		}

		private void HandleClient(object obj)
		{
			TcpClient client = (TcpClient)obj;
			Console.WriteLine("Client connected: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address);

			NetworkStream stream = client.GetStream();

			while (true)
			{
				byte[] buffer = new byte[1024];
				int bytesRead = 0;
				try
				{
					bytesRead = stream.Read(buffer, 0, buffer.Length);
				}
				catch
				{
					break;
				}
				

				if (bytesRead == 0)
					break;
				string s = Encoding.ASCII.GetString(buffer);
				Console.WriteLine($"{client.Client.RemoteEndPoint} received {s}");
				byte[] cpBuffer = new byte[bytesRead];
				Array.Copy(buffer,cpBuffer, bytesRead);
				Broadcast(cpBuffer, client);
			}

			lock (_clients)
			{
				Console.WriteLine($"Disconnecting Client : {client.Client.RemoteEndPoint}");
				_clients.Remove(client);
				client.Close();
			}
		}

		private void Broadcast(string data, TcpClient sender)
		{
			foreach (TcpClient client in _clients)
			{
				if (client != sender)
				{
					NetworkStream stream = client.GetStream();
					byte[] buffer = Encoding.ASCII.GetBytes(data);
					stream.Write(buffer, 0, buffer.Length);
					stream.Flush();
				}
			}
		}

		private void Broadcast(byte[] buffer, TcpClient sender)
		{
			foreach (TcpClient client in _clients)
			{
				if (client != sender)
				{
					NetworkStream stream = client.GetStream();
					//byte[] buffer = Encoding.ASCII.GetBytes(data);
					stream.Write(buffer, 0, buffer.Length);
					stream.Flush();
				}
			}
		}
	}
}
