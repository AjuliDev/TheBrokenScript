using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TheBrokenScript.Core
{
	[BackgroundColor(10, 10, 10, 200)]
	public class ClientConfig : ModConfig
	{
		public static ClientConfig Instance;
		public override ConfigScope Mode => ConfigScope.ClientSide;
		[Header("Rendering")]
		[BackgroundColor(50, 50, 50, 255)]
		[DefaultValue(true)]
		public bool ShowCorruptionVisionAssist { get; set; }
		[BackgroundColor(50, 50, 50, 255)]
		[Range(0f, 1f)]
		[DefaultValue(0.6f)]
		public float CorruptionVisionAssistHeight { get; set; }
		[BackgroundColor(50, 50, 50, 255)]
		[DefaultValue(true)]
		public bool PosterizationShader { get; set; }
		[BackgroundColor(50, 50, 50, 255)]
		[DefaultValue(true)]
		public bool TileAmbientParticles { get; set; }
	}
	[BackgroundColor(10, 10, 10, 200)]
	public class ServerConfig : ModConfig
	{
		public static ServerConfig Instance;
		public override ConfigScope Mode => ConfigScope.ServerSide;
		[Header("Events")]
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(false)]
		public bool DisableRandomEvents { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[Slider]
		[SliderColor(255, 255, 255, 255)]
		[Range(1, 100)]
		[DefaultValue(5)]
		public int MinimumRandomEventCooldown { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[Slider]
		[SliderColor(255, 255, 255, 255)]
		[Range(1, 100)]
		[DefaultValue(5)]
		public int MaximumRandomEventCooldown { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_GiftChest { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_Mangle { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_KernelPanic { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_Faint { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_MaliciousGift { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_ChunkCorruptor { get; set; }
		[BackgroundColor(169, 40, 212, 255)]
		[DefaultValue(true)]
		public bool Event_RandomStructure { get; set; }
		[Header("Entities")]
		[BackgroundColor(200, 0, 0, 255)]
		[Slider]
		[SliderColor(255, 255, 255, 255)]
		[Range(1, 100)]
		[DefaultValue(10)]
		public int MaximumEntitiesAllowed { get; set; }
		[BackgroundColor(200, 0, 0, 255)]
		[Range(400, 9000)]
		[DefaultValue(1600)]
		public int EntitySpawnRate { get; set; }
	}
}