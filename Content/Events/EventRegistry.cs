using System.Collections.Generic;
using System.Linq;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;
public static class EventRegistry
{
	public static readonly List<IModEvent> All = new()
	{
		// Events go here
		new Event_GiftChest(),
		new Event_Mangle(),
		new Event_KernelPanic(),
		new Event_Faint(),
		new Event_MaliciousGift(),
		new Event_ChunkCorruptor(),
		new Event_RandomStructure()
	};
	public static List<IModEvent> GetEnabled(ServerConfig config) => All.Where(e => e.IsEnabled(config)).ToList();
}