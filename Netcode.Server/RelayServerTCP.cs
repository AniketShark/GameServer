using System.Net.Sockets;
using System.Net;
using System.Text;
using Netcode.Common;

namespace Netcode.Server
{
    class RelayServerTCP
	{
		private TcpListener _listener;
		private List<TcpClient> _clients = new List<TcpClient>();
		private List<Player> _players = new List<Player>();
		private Queue<Message> _commandQueue = new Queue<Message>();
		public RelayServerTCP(int port)
		{
			_listener = new TcpListener(IPAddress.Any, port);
		}

		public void Start()
		{
			_listener.Start();
			Console.WriteLine("Server started. Waiting for connections...");
			Thread commandProcessingThread = new Thread(ProcessCommands);
			commandProcessingThread.Start();

			while (true)
			{
				TcpClient client = _listener.AcceptTcpClient();
				_clients.Add(client);

				Thread clientThread = new Thread(HandleClient);
				clientThread.Start(client);
			}
		}
		#region Command Processing

		private void EnqueueCommand(Message message)
		{
			lock (_commandQueue)
			{
				_commandQueue.Enqueue(message);
			}
		}

		private void ProcessCommands()
		{
			while (true)
			{
				Message command = DequeueCommand();
				if (command != null)
				{
					// Process command here
					Console.WriteLine("Received command: " + command.Type);
				}
				Thread.Sleep(10); // Optional: Adjust sleep time as needed
			}
		}

		private Message DequeueCommand()
		{
			lock (_commandQueue)
			{
				if (_commandQueue.Count > 0)
				{
					return _commandQueue.Dequeue();
				}
				else
				{
					return null;
				}
			}
		}


		#endregion


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

				if(msg.Type == MessageType.Command)
					EnqueueCommand(msg);

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
