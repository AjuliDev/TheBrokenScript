using System.IO;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Content.NPCs.Possessor;
namespace TheBrokenScript.Core;
public static class ModPacketHandler
{
	internal enum PacketType : byte
	{
		SyncWorldData,
		SyncPossessorPlayer,
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
			default:
				ModContent.GetInstance<TheBrokenScript>().Logger.WarnFormat("The Broken Script: Unknown packet type: {0}", packet);
				break;
		}
	}
}