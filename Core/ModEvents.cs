using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Content.Events;
namespace TheBrokenScript.Core;
public class ModEvents : ModSystem
{
	private int randEventTimer, randEventTargetTime;
	public override void OnWorldLoad() => ResetTimer();
	public override void PostUpdateTime()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}
		if (++randEventTimer < randEventTargetTime)
		{
			return;
		}
		TriggerRandom();
		ResetTimer();
	}
	private void TriggerRandom()
	{
		var config = ServerConfig.Instance;
		if (config.DisableRandomEvents)
		{
			return;
		}
		var enabled = EventRegistry.GetEnabled(config);
		if (enabled.Count == 0)
		{
			return;
		}
		enabled[Main.rand.Next(enabled.Count)].StartEvent();
	}
	public static void TriggerEvent(IModEvent modEvent)
	{
		var config = ServerConfig.Instance;
		if (!modEvent.IsEnabled(config))
		{
			return;
		}
		modEvent.StartEvent();
	}
	private void ResetTimer()
	{
		var config = ServerConfig.Instance;
		// 60 ticks (second) * 60 * ...
		int min = 60 * 60 * config.MinimumRandomEventCooldown;
		int max = 60 * 60 * config.MaximumRandomEventCooldown;
		randEventTargetTime = Main.rand.Next(min, max + 1);
		randEventTimer = 0;
	}
}