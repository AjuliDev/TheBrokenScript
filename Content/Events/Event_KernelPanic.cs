using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Common.EventHelpers;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.Events;
public class Event_KernelPanic : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_KernelPanic;
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
			packet.Write((byte)ModPacketHandler.PacketType.KernelPanicCastToClient);
			packet.Send(targetPlayer, -1);
		} else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			KernelPanicModSystem.Enable();
		}
	}
}