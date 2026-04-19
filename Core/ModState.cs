using Terraria;
using Terraria.ModLoader;
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
		Postponed
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
	private WorldData worldData;
	private bool nightCounted;
	public override void SetStaticDefaults()
	{
		worldData.Timings.NightsUntilCorrupted = 3;
		worldData.WorldState = WorldState.NewWorld;
		worldData.Timings.TotalNightsPassed = 0;
		worldData.MoonData.MoonPhase = MoonPhase.Normal;
		worldData.MoonData.CorruptedSequenceID = 0;
		worldData.MoonData.SequenceTotalIndexes = 50;
		nightCounted = false;
	}
	public override void PostUpdateTime()
	{
		if (NPC.downedBoss3 && worldData.WorldState == WorldState.NewWorld)
		{
			worldData.WorldState = WorldState.Awakening;
		} else if (worldData.WorldState >= WorldState.Awakening && worldData.Timings.TotalNightsPassed >= worldData.Timings.NightsUntilCorrupted)
		{
			worldData.WorldState = WorldState.Corrupted;
		} else if (worldData.WorldState >= WorldState.Awakening)
		{
			if (!nightCounted && !Main.IsItDay())
			{
				nightCounted = true;
				worldData.Timings.TotalNightsPassed += 1;
				if (worldData.WorldState == WorldState.Corrupted)
				{
					worldData.MoonData.CorruptedSequenceID += 1;
					if (worldData.MoonData.CorruptedSequenceID % worldData.MoonData.SequenceTotalIndexes == 0)
					{
						worldData.MoonData.CorruptedSequenceID = 40;
					}
				}
			}
			if (nightCounted && Main.IsItDay())
			{
				nightCounted = false;
			}
		}
	}
	#endregion Mechanics
}