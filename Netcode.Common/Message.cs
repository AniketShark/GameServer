using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;

namespace Netcode.Common
{
	public enum MessageType
	{
		Chat,
		Move,
		Action,
	}


	[MessagePackObject]
	public class Message
	{
		[Key(0)]
		public MessageType Type { get; set; }
		[Key(1)]
		public Dictionary<string, object> Payload { get; set; }

		public override string ToString()
		{
			string str = $"Type : {Type} ";
			foreach (var kv in Payload)
			{
				str += $"\n{kv.Key} {kv.Value}";
			}
			return str;
		}
	}
}
