using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using thebrokenscript.Content.Events;
using thebrokenscript.Core;

namespace thebrokenscript.Common.Commands;

public class eventCommand : ModCommand // COMMANDS ARE RUNNING IN CLIENT CONTEXT, DO NOT FORGET, I SPENT TWO DAYS BEFORE I FIGURED THIS BULLSHIT
{
	public override string Command => "event";
	public override CommandType Type => CommandType.Console | CommandType.Chat;
	public override string Description => "Triggers a specific event by name.";
	public override string Usage => "/event <eventname>";
	public override void Action(CommandCaller caller, string input, string[] args)
	{
		if (args.Length == 0)
		{
			caller.Reply("Usage: " + Usage);
			return;
		}
		var config = ServerConfig.Instance;
		if (config == null)
		{
			caller.Reply("Config not found.");
			return;
		}
		string eventName = args[0].ToLower();
		var modEvent = EventRegistry.GetEnabled(config).FirstOrDefault(e => e.GetType().Name.ToLower() == eventName);
		if (modEvent == null)
		{
			caller.Reply($"Unknown or disabled event: '{eventName}'");
			return;
		}
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			ModPacket packet = ModContent.GetInstance<thebrokenscript>().GetPacket();
			packet.Write((byte)ModPacketHandler.PacketType.TriggerEvent);
			packet.Write(eventName);
			packet.Send(); // Client to Server, parameters irrelevant
			caller.Reply($"Asking Server for event... {eventName}");
			return;
		}
		ModEvents.TriggerEvent(modEvent);
		caller.Reply($"Triggered event: {eventName}");
	}
}
