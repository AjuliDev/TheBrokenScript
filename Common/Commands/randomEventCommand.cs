using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Content.Events;
using TheBrokenScript.Core;

namespace TheBrokenScript.Common.Commands;

public class randomEventCommand : ModCommand // COMMANDS ARE RUNNING IN CLIENT CONTEXT, DO NOT FORGET, I SPENT TWO DAYS BEFORE I FIGURED THIS BULLSHIT
{
	public override string Command => "randomevent";
	public override CommandType Type => CommandType.Console | CommandType.Chat;
	public override string Description => "Triggers a random event.";
	public override string Usage => "/randomevent";
	public override void Action(CommandCaller caller, string input, string[] args)
	{
		var config = ServerConfig.Instance;
		if (config == null)
		{
			caller.Reply("Config not found.");
			return;
		}
		var enabled = EventRegistry.GetEnabled(config);
		if (enabled.Count == 0)
		{
			caller.Reply("No events are currently enabled.");
			return;
		}
		var modEvent = enabled[Main.rand.Next(enabled.Count)];
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
			packet.Write((byte)ModPacketHandler.PacketType.TriggerEvent);
			packet.Send(); // Client to Server, parameters irrelevant
			caller.Reply($"Asking Server for a random event.");
			return;
		}
		ModEvents.TriggerEvent(modEvent);
		caller.Reply($"Triggered random event.");
	}
}
