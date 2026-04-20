using System.IO;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public static class ModPacketHandler
{
	internal enum PacketType : byte
	{
		SyncWorldData,
	} 
	public static void Handle(BinaryReader reader, int whoAmI) // Multiplayer packet handling
	{
		PacketType packet = (PacketType)reader.ReadByte();
		switch (packet)
		{
			case PacketType.SyncWorldData:
				ModState.ClientReceiver(reader);
				break;
			default:
				ModContent.GetInstance<TheBrokenScript>().Logger.WarnFormat("The Broken Script: Unknown packet type: {0}", packet);
				break;
		}
	}
}