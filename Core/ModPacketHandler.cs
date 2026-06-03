using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Common.EventHelpers;
using TheBrokenScript.Content.Events;
using TheBrokenScript.Content.NPCs.Possessor;
namespace TheBrokenScript.Core;
public static class ModPacketHandler
{
	internal enum PacketType : byte
	{
		SyncWorldData,
		SyncPossessorPlayer,
		TriggerEvent,
		RandomEvent,
		KernelPanicCastToClient,
	} 
	public static void Handle(BinaryReader reader, int whoAmI) // Multiplayer packet handling
	{
		PacketType packet = (PacketType)reader.ReadByte();
		switch (packet)
		{
			case PacketType.SyncWorldData:
				ModState.ClientReceiver(reader);
				break;
			case PacketType.SyncPossessorPlayer:
				int targetPlayer = reader.ReadInt32();
				if (targetPlayer == Main.myPlayer)
				{
					Main.LocalPlayer.GetModPlayer<PossessorPlayer>().Possessed = true;
				}
				break;
			case PacketType.TriggerEvent:
				string eventName = reader.ReadString();
				var config = ServerConfig.Instance;
				var modEvent = EventRegistry.GetEnabled(config).FirstOrDefault(e => e.GetType().Name.ToLower() == eventName);
				if (modEvent != null)
				{
					ModEvents.TriggerEvent(modEvent);
				}
				break;
			case PacketType.RandomEvent:
				ModEvents.TriggerRandom();
				break;
			case PacketType.KernelPanicCastToClient:
				KernelPanicModSystem.Enable();
				break;
			default:
				ModContent.GetInstance<TheBrokenScript>().Logger.WarnFormat("The Broken Script: Unknown packet type: {0}", packet);
				break;
		}
	}
}