using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using thebrokenscript.Content.Events;
using thebrokenscript.Core;
namespace thebrokenscript.Core;
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
		//ChatHelper.BroadcastChatMessage(NetworkText.FromFormattable($"{randEventTimer} out of {randEventTargetTime}"), color: Color.White, -1);
		//Main.NewText($"{randEventTimer} out of {randEventTargetTime}");
		ModState.WorldData worldData = ModState.GetWorldData();
		if (worldData.WorldState >= ModState.WorldState.Awakening)
		{
			if (++randEventTimer < randEventTargetTime)
			{
				return;
			}
			TriggerRandom();
			ResetTimer();
		}
	}
	public static void TriggerRandom() // Private to Public Static
	{
		var config = ServerConfig.Instance;
		if (config == null)
		{
			return;
		}
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
		//Main.NewText($"[DEBUG]: Executed random event.");
	}
	public static void TriggerEvent(IModEvent modEvent)
	{
		var config = ServerConfig.Instance;
		if (config == null)
		{
			return;
		}
		if (!modEvent.IsEnabled(config))
		{
			return;
		}
		modEvent.StartEvent();
		//Main.NewText($"[DEBUG]: Executed event.");
	}
	private void ResetTimer()
	{
		var config = ServerConfig.Instance;
		int min = 60, max = 60;
		if (config == null)
		{
			min = 60 * 60 * 1;
			max = 60 * 60 * 1;
		}
		else
		{
			// 60 ticks (second) * 60 * ...
			min = 60 * 60 * config.MinimumRandomEventCooldown;
			max = 60 * 60 * config.MaximumRandomEventCooldown;
		}
		randEventTargetTime = Main.rand.Next(min, max + 1);
		randEventTimer = 0;
		//Main.NewText($"[DEBUG]: Reset timer to {randEventTargetTime}");
	}
}