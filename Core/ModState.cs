using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace TheBrokenScript.Core;
public class ModState : ModSystem
{
	#region Definitions
	/// <summary>
	/// The state of the current world based on the environment and player actions. <br/>
	/// <b>NewWorld</b> means: The world has just started or is currently before the Skeletron Fight. <br/>
	/// <b>Awakening</b> means: Skeletron was defeated and the gate for Integrity has opened. <br/>
	/// <b>Corrupted</b> means: The meteorite has landed, and corruption has started spreading biomes, structures, entities and more. <br/>
	/// <b>Postponed</b> means: Integrity's access to this world has been restricted. Some artifacts may still appear, but for the most part, Terraria is safe once again. <br/>
	/// </summary>
	public enum WorldState
	{
		NewWorld,
		Awakening,
		Corrupted,
		Postponed // Not used yet, to be used at endgame
	}
	/// <summary>
	/// The state of the moon. <br/>
	/// <b>Normal</b> means: The moon is normal, unaffected. <br/>
	/// <b>CorruptedRandom</b> means: The moon is corrupted, but is not following the sequence, being randomized. <br/>
	/// <b>CorruptedSequence</b> means: The moon is corrupted and follows a numerical sequence <br/>
	/// </summary>
	public enum MoonPhase
	{
		Normal,
		CorruptedRandom,
		CorruptedSequence,
	}
	public struct MoonData
	{
		public MoonPhase MoonPhase;
		public int CorruptedSequenceID;
		public int RandomCorruptedID;
		public int SequenceTotalIndexes;
	}
	public struct Timings
	{
		public int NightsUntilCorrupted;
		public int TotalNightsPassed;
	}
	public struct WorldData
	{
		public WorldState WorldState;
		public MoonData MoonData;
		public Timings Timings;
	}
	#endregion Definitions
	#region Mechanics
	private static WorldData worldData;
	public static WorldData GetWorldData() => worldData;
	private bool nightCounted;
	public override void OnWorldLoad()
	{
		ResetWorldData();
	}
	public override void OnWorldUnload()
	{
		ResetWorldData();
	}
	private void ResetWorldData()
	{
		worldData.Timings.NightsUntilCorrupted = 3;
		worldData.WorldState = WorldState.NewWorld;
		worldData.Timings.TotalNightsPassed = 0;
		worldData.MoonData.MoonPhase = MoonPhase.Normal;
		worldData.MoonData.CorruptedSequenceID = 0;
		worldData.MoonData.SequenceTotalIndexes = 49;
		nightCounted = false;
	}
	public override void PostUpdateTime()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}
		if (NPC.downedBoss3 && worldData.WorldState == WorldState.NewWorld)
		{
			worldData.WorldState = WorldState.Awakening;
		} else if (worldData.WorldState >= WorldState.Awakening)
		{
			if (worldData.Timings.TotalNightsPassed >= worldData.Timings.NightsUntilCorrupted)
			{
				worldData.WorldState = WorldState.Corrupted;
			}
			if (!nightCounted && !Main.IsItDay())
			{
				nightCounted = true;
			}
			if (nightCounted && Main.IsItDay())
			{
				nightCounted = false;
				worldData.Timings.TotalNightsPassed += 1;
				if (worldData.WorldState == WorldState.Corrupted)
				{
					if (Main.rand.NextBool(10))
					{
						worldData.MoonData.MoonPhase = MoonPhase.CorruptedRandom;
						worldData.MoonData.RandomCorruptedID = Main.rand.Next(0, 49);
					}
					else if (Main.rand.NextBool(2))
					{
						worldData.MoonData.MoonPhase = MoonPhase.CorruptedSequence;
						worldData.MoonData.CorruptedSequenceID += 1;
						if (worldData.MoonData.CorruptedSequenceID % worldData.MoonData.SequenceTotalIndexes == 0)
						{
							// Skips the first 40 moons (infecting) once the first cycle is done.
							worldData.MoonData.CorruptedSequenceID = 39;
						}
					}
					else
					{
						worldData.MoonData.MoonPhase = MoonPhase.Normal;
					}
				}
			}
		}
		if (Main.netMode == NetmodeID.Server && Main.GameUpdateCount % 60 == 0)
		{
			ServerBroadcastData(); // 21 bytes per second per client + whatever tml may add
		}
	}
	public override void SaveWorldData(TagCompound tag)
	{
		tag["TBS_TotalNightsPassed"] = worldData.Timings.TotalNightsPassed;
		tag["TBS_CorruptedSequenceID"] = worldData.MoonData.CorruptedSequenceID;
		tag["TBS_RandCorruptedID"] = worldData.MoonData.RandomCorruptedID;
		tag["TBS_WorldState"] = (int)worldData.WorldState;
		tag["TBS_MoonPhase"] = (int)worldData.MoonData.MoonPhase;
	}
	public override void LoadWorldData(TagCompound tag)
	{
		worldData.Timings.TotalNightsPassed = tag.ContainsKey("TBS_TotalNightsPassed") ? tag.GetInt("TBS_TotalNightsPassed") : 0;
		worldData.MoonData.CorruptedSequenceID = tag.ContainsKey("TBS_CorruptedSequenceID") ? tag.GetInt("TBS_CorruptedSequenceID") : 0;
		worldData.MoonData.RandomCorruptedID = tag.ContainsKey("TBS_RandCorruptedID") ? tag.GetInt("TBS_RandCorruptedID") : 0;
		worldData.WorldState = tag.ContainsKey("TBS_WorldState") ? (WorldState)tag.GetInt("TBS_WorldState") : WorldState.NewWorld;
		worldData.MoonData.MoonPhase = tag.ContainsKey("TBS_MoonPhase") ? (MoonPhase)tag.GetInt("TBS_MoonPhase") : MoonPhase.Normal;
	}
	#endregion Mechanics
	#region MultiplayerSync
	public static void ServerBroadcastData()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			return;
		}
		ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
		packet.Write((byte)ModPacketHandler.PacketType.SyncWorldData);
		packet.Write((int)worldData.WorldState);
		packet.Write(worldData.MoonData.RandomCorruptedID);
		packet.Write(worldData.MoonData.CorruptedSequenceID);
		packet.Write((int)worldData.MoonData.MoonPhase);
		packet.Write(worldData.Timings.TotalNightsPassed);
		packet.Send(-1, -1);
	}
	public static void ClientReceiver(BinaryReader reader)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			return;
		}
		worldData.WorldState = (WorldState)reader.ReadInt32();
		worldData.MoonData.RandomCorruptedID = reader.ReadInt32();
		worldData.MoonData.CorruptedSequenceID = reader.ReadInt32();
		worldData.MoonData.MoonPhase = (MoonPhase)reader.ReadInt32();
		worldData.Timings.TotalNightsPassed = reader.ReadInt32();
	}
	#endregion MultiplayerSync
}