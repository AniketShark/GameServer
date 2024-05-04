using System.Net.Sockets;
using System.Net;
using System.Text;
using Netcode.Common;

namespace Netcode.Server
{
    class RelayServer
	{
		
		private TcpListener _listener;
		private List<TcpClient> _clients = new List<TcpClient>();
		private List<Player> _players = new List<Player>();
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
				byte[] buffer = new byte[4096];
				int bytesRead = 0;
				try
				{
					bytesRead = stream.Read(buffer,0,buffer.Length);
				}
				catch
				{
					break;
				}

				if (bytesRead == 0)
					break;

				byte[] cpBuffer = new byte[bytesRead];
				Array.Copy(buffer,cpBuffer, bytesRead);
				Message msg = MessagePack.MessagePackSerializer.Deserialize<Message>(buffer);
				Console.WriteLine($"{client.Client.RemoteEndPoint} received {msg.ToString()}");
				Broadcast(buffer, client);
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
					stream.Write(buffer, 0, buffer.Length);
					stream.Flush();
				}
			}
		}
	}
}
