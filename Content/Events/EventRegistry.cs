using System.Collections.Generic;
using System.Linq;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;
public static class EventRegistry
{
	public static readonly List<IModEvent> All = new()
	{
		// Events go here
	};
	public static List<IModEvent> GetEnabled(ServerConfig config) => All.Where(e => e.IsEnabled(config)).ToList();
}