using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using thebrokenscript.Common.EventHelpers;
using thebrokenscript.Core;

namespace thebrokenscript.Content.Events;
public class Event_MaliciousGift : IModEvent
{
	public bool IsEnabled(ServerConfig config) => config.Event_Faint;
	public static int[] Projectiles = new int[]
	{
		ProjectileID.Bomb,
		ProjectileID.BombFish,
		ProjectileID.Dynamite,
		ProjectileID.ExplosiveBunny,
		ProjectileID.BouncyBoulder,
		ProjectileID.Boulder,
		ProjectileID.RollingCactus
	};
	public void StartEvent()
	{
		if (Main.dedServ)
		{
			List<int> activePlayers = [];
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i].active)
				{
					activePlayers.Add(i);
				}
			}
			if (activePlayers.Count == 0)
			{
				return;
			}
			int targetPlayer = activePlayers[Main.rand.Next(activePlayers.Count)];
			int selectedProjectile = Projectiles[Main.rand.Next(Projectiles.Length)];
			Projectile.NewProjectile(Main.player[targetPlayer].GetSource_FromThis(),
				Main.player[targetPlayer].Center - new Vector2(0, 10 * 16),
				Vector2.Zero,
				selectedProjectile,
				80,
				5f,
				-1
				);
			Terraria.Chat.ChatHelper.BroadcastChatMessage(
				NetworkText.FromLiteral("< > Have a present, on me."),
				Color.Red,
				-1);

		}
		else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			int selectedProjectile = Projectiles[Main.rand.Next(Projectiles.Length)];
			Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(),
				Main.LocalPlayer.Center - new Vector2(0, 10 * 16),
				Vector2.Zero,
				selectedProjectile,
				80,
				5f,
				-1
				);
			Terraria.Chat.ChatHelper.BroadcastChatMessage(
				NetworkText.FromLiteral("< > Have a present, on me."),
				Color.Red,
				-1);
		}
	}
}
