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
			ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
			packet.Write((byte)ModPacketHandler.PacketType.KernelPanicCastToClient);
			packet.Send(-1, -1);
		} else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			KernelPanicModSystem.Enable();
		}
	}
}