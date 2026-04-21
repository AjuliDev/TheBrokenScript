using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TheBrokenScript.Core
{
	[BackgroundColor(10, 10, 10, 200)]
	public class ClientConfig : ModConfig
	{
		public static ClientConfig Instance;
		public override ConfigScope Mode => ConfigScope.ClientSide;
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
	}
}