using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Common;
namespace TheBrokenScript.Content.NPCs.SubAnomalyOne;

public class SubAnomalyOne : ModNPC
{
	public override string Texture => "TheBrokenScript/Common/Textures/transparent_pixel";
	public static Asset<Texture2D> Quad;
	private Vector2[] quadPositions = new Vector2[12];
	private float[] quadScales = new float[12];
	private float[] quadLifetimes = new float[12];
	float timer = 0f;
	public override void Load()
	{
		Quad = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/SubAnomalyOne/SubAnomalyOne_Quad");
	}
	public override void Unload()
	{
		Quad = null;
	}
	public override void SetDefaults()
	{
		NPC.width = 40;
		NPC.height = 40;
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
	}

	public override void AI()
	{
		timer++;
		if (timer % 60f == 0f)
		{
			for (int i = 0; i < quadPositions.Length; i++)
			{
				quadPositions[i] = NPC.Center + new Vector2(
					Main.rand.NextFloat(-100, 100),
					Main.rand.NextFloat(-100, 100));
				quadScales[i] = 1.5f;
				quadLifetimes[i] = 240f;
			}
		}
		for (int i = 0; i < quadPositions.Length; i++)
		{
			if (quadLifetimes[i] > 0f)
			{
				quadLifetimes[i]--;
				quadScales[i] = 1.5f * (quadLifetimes[i] / 240f);
			}
		}

		// Adjust horizontal velocity based on distance from nearest player
		int closestPlayerID = NPC.FindClosestPlayer();
		Player closestPlayerInstance = Main.player[closestPlayerID];
		float distanceToPlayer = NPC.Center.X - closestPlayerInstance.Center.X;
		if (Math.Abs(distanceToPlayer) > 32f)
		{
			NPC.velocity.X = distanceToPlayer > 0 ?
				MathHelper.Lerp(NPC.velocity.X, -0.5f, 0.1f) :
				MathHelper.Lerp(NPC.velocity.X, 0.5f, 0.1f);
		}
		else
		{
			NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
		}
		Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

		// Check Nearby tiles and convert if possible
		int tileX = (int)(NPC.Bottom.X / 16f);
		int tileY = (int)(NPC.Bottom.Y / 16f);
		int checkRadius = 2;
		if (timer % 30 == 0f && Main.netMode != NetmodeID.MultiplayerClient)
		{
			for (int x = tileX - checkRadius; x < tileX + checkRadius; x++)
			{
				for (int y = tileY; y < tileY + checkRadius; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water)
					{
						tile.LiquidAmount = 0;
						WorldGen.PlaceTile(x, y, TileID.Stone, forced: true);
						WorldGen.SquareTileFrame(x, y);
						NetMessage.SendTileSquare(-1, x, y, 1);
					}
				}
			}
		}
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (Quad?.Value == null)
			return;

		for (int i = 0; i < quadPositions.Length; i++)
		{
			if (!NPC.active ||  Quad?.Value == null)
			{
				return;
			}
			if (quadLifetimes[i] <= 0f)
			{
				continue;
			}
			Vector2 origin = Quad.Value.Size() / 2f;
			spriteBatch.Draw(Quad.Value, quadPositions[i] - screenPos, null, Color.White, 0f, origin, quadScales[i], SpriteEffects.None, 0f);
		}
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}