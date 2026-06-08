using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheBrokenScript.Common.EventHelpers;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;

public class Event_Faint : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_Faint;
	public void StartEvent()
	{
		if (Main.dedServ)
		{
			List<int> activePlayers = [];
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i].active)
				{
					activePlayers.Add(i);
				}
			}
			if (activePlayers.Count == 0)
			{
				return;
			}
			int targetPlayer = activePlayers[Main.rand.Next(activePlayers.Count)];
			ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
			packet.Write((byte)ModPacketHandler.PacketType.FaintCastToClient);
			packet.Send(targetPlayer, -1);
			Terraria.Chat.ChatHelper.BroadcastChatMessage(
				NetworkText.FromLiteral($"<{Main.player[targetPlayer].name}> What is this heavy feeling...?"),
				Color.White,
				-1);
			
		}
		else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			FaintModSystem.Enable();
			Terraria.Chat.ChatHelper.BroadcastChatMessage(
				NetworkText.FromLiteral($"<{Main.LocalPlayer.name}> What is this heavy feeling...?"),
				Color.White,
				-1);
		}
	}
}