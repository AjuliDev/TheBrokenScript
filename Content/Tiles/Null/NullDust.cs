using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace thebrokenscript.Content.Tiles.Null;
public class NullDust : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = true;
		dust.scale = 8f;
	}
	public override bool Update(Dust dust)
	{
		dust.position += Vector2.Multiply(new Vector2((float)Math.Sin(dust.velocity.Y), 0), dust.velocity);
		dust.rotation += dust.velocity.X;
		dust.scale -= 0.1f;
		if (dust.scale < 0.5f)
		{
			dust.active = false;
		}
		return false;
	}
}
