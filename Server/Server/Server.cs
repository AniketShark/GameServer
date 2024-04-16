using Server.Netcode;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	class Server
	{
		private static List<TcpClient> _clients = new List<TcpClient>();

		static void Main(string[] args)
		{
			var server = new TcpListener(IPAddress.Any, 5000);
			server.Start();
			Console.WriteLine("Server has started on 127.0.0.1:5000");
			try
			{
				while (true)
				{
					TcpClient clientSocket = server.AcceptTcpClient();
					lock (_clients)
					{
						_clients.Add(clientSocket);
					}
					Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
					clientThread.Start(clientSocket);
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				server.Stop();
			}
		}

		private static void HandleClient(object obj)
		{
			TcpClient client = (TcpClient)obj;
			Console.WriteLine("Client connected. Waiting for data.");
			NetworkStream stream = client.GetStream();

			try
			{
				byte[] buffer = new byte[1024];
				int byte_count;

				while ((byte_count = stream.Read(buffer, 0, buffer.Length)) > 0)
				{
					Packet packet = new Packet();
					packet.WriteBytes(buffer);
					Console.WriteLine(packet.ReadString());
					Broadcast(packet, client);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"A client disconnected or an error occurred: {ex.Message}");
			}
			finally
			{
				stream.Close();
				client.Close();
				lock (_clients)
				{
					_clients.Remove(client);
					Console.WriteLine("Client disconnected.");
				}
			}
		}

		private static void Broadcast(Packet data, TcpClient excludeClient)
		{
			byte[] buffer = data.ToArray();//Encoding.ASCII.GetBytes(data);

			lock (_clients)
			{
				foreach (var client in _clients)
				{
					if (client != excludeClient && client.Connected)
					{
						try
						{
							NetworkStream stream = client.GetStream();
							stream.Write(buffer, 0, buffer.Length);
							stream.Flush();
						}
						catch(Exception ex) 
						{
							// Handle any exceptions (like disconnected clients) here
							Console.WriteLine($"A client disconnected or an error occurred: {ex.Message}");
						}
						finally
						{
							client.Close();
						}
					}
				}
			}
		}
	}
}