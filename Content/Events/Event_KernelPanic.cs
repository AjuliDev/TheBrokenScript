using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using thebrokenscript.Common.EventHelpers;
using thebrokenscript.Core;
namespace thebrokenscript.Content.Events;
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
			ModPacket packet = ModContent.GetInstance<thebrokenscript>().GetPacket();
			packet.Write((byte)ModPacketHandler.PacketType.KernelPanicCastToClient);
			packet.Send(targetPlayer, -1);
		} else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			KernelPanicModSystem.Enable();
		}
	}
}