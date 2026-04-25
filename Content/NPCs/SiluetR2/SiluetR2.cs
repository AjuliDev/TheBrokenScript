using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Common;
namespace TheBrokenScript.Content.NPCs.SiluetR2;
public class SiluetR2 : ModNPC
{
	public override string Texture => "TheBrokenScript/Common/Textures/transparent_pixel";
	private static Asset<Texture2D> npcSprite;
	float timer = 0f;
	float offset = 0f;
	public override void Load()
	{
		npcSprite = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/SiluetR2/SiluetR2", AssetRequestMode.AsyncLoad);
	}
	public override void Unload()
	{
		npcSprite = null;
	}
	public override void SetDefaults()
	{
		NPC.width = 30;
		NPC.height = 140;
		NPC.damage = 20;
		NPC.defense = 1;
		NPC.dontTakeDamage = true;
		NPC.dontTakeDamageFromHostiles = true;
		NPC.lifeMax = 600;
		NPC.aiStyle = -1;
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.ShowNameOnHover = false;
		NPC.knockBackResist = 0;
		NPC.lavaImmune = false;
		NPC.ai[0] = 0f;
	}
	public override void AI()
	{
		if (timer % 60f == 0f)
		{
			timer = 0f;
		}
		if (timer % 5f == 0f)
		{
			if (Main.rand.NextBool(2))
			{
				offset = Main.rand.NextFloat(-2.5f, 2.5f);
			}
			else
			{
				offset = Main.rand.NextFloat(2.5f, -2.5f);
			}
		}
		timer++;
		// Adjust horizontal velocity based on distance from nearest player
		int closestPlayerID = NPC.FindClosestPlayer();
		Player closestPlayerInstance = Main.player[closestPlayerID];
		float distanceToPlayer = NPC.Center.X - closestPlayerInstance.Center.X;
		if (Math.Abs(distanceToPlayer) > 32f)
		{
			if (NPC.ai[0] == 1f)
			{
				NPC.velocity.X = distanceToPlayer > 0 ?
					MathHelper.Lerp(NPC.velocity.X, -3.5f, 0.1f) :
					MathHelper.Lerp(NPC.velocity.X, 3.5f, 0.1f);
			}
			else if (NPC.ai[0] == 0f)
			{
				NPC.velocity.X = distanceToPlayer > 0 ?
					MathHelper.Lerp(NPC.velocity.X, -1.0f, 0.1f) :
					MathHelper.Lerp(NPC.velocity.X, 1.0f, 0.1f);
			}
			NPC.spriteDirection = distanceToPlayer > 0 ? -1 : 1;
		}
		else
		{
			NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
		}
		if (Math.Abs(distanceToPlayer) < (16f * 20f))
		{
			if (NPC.ai[0] != 1f)
			{
				NPC.ai[0] = 1f;
			}
		}
		if (NPC.ai[0] == 1f)
		{
			float distance = Vector2.Distance(Main.LocalPlayer.Center, NPC.Center);
			Vector2 target = distance < 16f * 20f ? NPC.Center : Main.LocalPlayer.Center;
			Main.instance.CameraModifiers.Add(new CameraSnapTo(target, FullName, () => !NPC.active));
		}
		Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
	}
	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (npcSprite?.Value == null)
		{
			return;
		}
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(offset, -offset);
		Vector2 origin = npcSprite.Value.Size() / 2f;// + new Vector2(0f, 20f);
		spriteBatch.Draw(npcSprite.Value, drawPos, null, Color.White, 0f, origin, 2f, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
	}
}
