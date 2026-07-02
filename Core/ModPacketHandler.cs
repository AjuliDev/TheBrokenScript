using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using thebrokenscript.Common;
using thebrokenscript.Common.EventHelpers;
using thebrokenscript.Content.Events;
using thebrokenscript.Content.NPCs.Possessor;
namespace thebrokenscript.Core;
public static class ModPacketHandler
{
	internal enum PacketType : byte
	{
		SyncWorldData,
		SyncPossessorPlayer,
		TriggerEvent,
		RandomEvent,
		KernelPanicCastToClient,
		FaintCastToClient,
		RequestCaveSound,
		SubworldCounter,
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
			case PacketType.FaintCastToClient:
				FaintModSystem.Enable();
				break;
			case PacketType.RequestCaveSound:
				int i = reader.ReadInt32();
				int j = reader.ReadInt32();
				ModSounds.PlayCaveNoise(i, j);
				break;
			case PacketType.SubworldCounter:
				SubworldCounter.NowhereEntryCooldown = 5;
				break;
			default:
				ModContent.GetInstance<thebrokenscript>().Logger.WarnFormat("The Broken Script: Unknown packet type: {0}", packet);
				break;
		}
	}
}