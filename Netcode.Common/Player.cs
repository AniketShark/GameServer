
namespace Netcode.Common
{
	public class Player
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Guid DeviceId { get; set; }
		public Dictionary<string, string> CustomProperties { get; set; }

	}
}
