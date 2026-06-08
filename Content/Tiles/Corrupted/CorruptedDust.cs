using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace TheBrokenScript.Content.Tiles.Corrupted;
public class CorruptedDust : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = true;
		dust.scale = 5f;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
	}
	public override bool Update(Dust dust)
	{
		//dust.position += Vector2.Multiply(new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), dust.velocity);
		//dust.position += new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2));
		dust.scale -= 0.1f;
		dust.alpha = Math.Clamp(200 + 1, 128, 255);
		if (dust.scale < 1f)
		{
			dust.active = false;
		}
		return false;
	}
}
