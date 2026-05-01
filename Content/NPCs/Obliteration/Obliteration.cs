using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Common;
using TheBrokenScript.Content.Tiles.Null;
namespace TheBrokenScript.Content.NPCs.Obliteration;

public class Obliteration : ModNPC
{
	public static Asset<Texture2D> BoneTexture;
	private FABRIK[] boneChains;
	private bool isArmatureReady => boneChains != null;
	private Vector2[] tentacleRoots, tentacleTargets;
	private float[] tentaclePhases, tentacleSpeeds;
	private float tentacleBoneLength, closeTimer;
	private float timer;
	public void SetIKDefaults()
	{
		boneChains = new FABRIK[31];
		tentacleBoneLength = 20f;
		tentacleRoots = new Vector2[boneChains.Length];
		tentacleTargets = new Vector2[boneChains.Length];
		tentaclePhases = new float[boneChains.Length];
		tentacleSpeeds = new float[boneChains.Length];
		for (int i = 1; i < boneChains.Length; i++)
		{
			tentacleRoots[i] = NPC.Center;
			tentacleTargets[i] = NPC.Center +
				new Vector2(
					Main.rand.NextFloat(-80, 80),
					Main.rand.NextFloat(-80, -160)
					);
			float amountOfBones = Math.Max(1, Vector2.Distance(tentacleTargets[i], tentacleRoots[i]) / tentacleBoneLength);
			boneChains[i] = new FABRIK(tentacleRoots[i],
				(int)amountOfBones,
				Enumerable.Repeat(tentacleBoneLength, (int)amountOfBones).ToArray());
			boneChains[i].Solve(tentacleRoots[i], tentacleTargets[i], 3);
			tentaclePhases[i] = Main.rand.NextFloat(0, MathHelper.TwoPi);
			tentacleSpeeds[i] = Main.rand.NextFloat(0.03f, 0.07f);
		}
	}
	public override void SetDefaults()
	{
		NPC.width = 60;
		NPC.height = 60;
		NPC.damage = 50;
		NPC.defense = 1;
		NPC.dontTakeDamage = true;
		NPC.dontTakeDamageFromHostiles = true;
		NPC.lifeMax = 600;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.ShowNameOnHover = false;
		NPC.knockBackResist = 0;
		NPC.lavaImmune = true;
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!NPC.active || NPC.Center == Vector2.Zero)
		{
			return false;
		}
		if (!isArmatureReady)
		{
			SetIKDefaults();
		}
		for (int i = 1; i < boneChains.Length; i++)
		{
			IKHelper.DrawChain(spriteBatch, screenPos, drawColor, boneChains[i], BoneTexture?.Value, NPC.Center, NPC.rotation);
		}
		return base.PreDraw(spriteBatch, screenPos, drawColor);
	}
	public override void Load()
	{
		BoneTexture = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Obliteration/Obliteration_Tentacle");
	}
	public override void Unload()
	{
		BoneTexture = null;
	}
	public override void AI()
	{
		timer++;
		// Go towards player
		int closestPlayerID = NPC.FindClosestPlayer();
		Player closestPlayerInstance = Main.player[closestPlayerID];
		float distanceToPlayer = Vector2.Distance(NPC.Center, closestPlayerInstance.Center);
		if (Math.Abs(distanceToPlayer) > 32f)
		{
			if (closeTimer < 300)
			{
				NPC.velocity.X = NPC.Center.X - closestPlayerInstance.Center.X > 0 ?
					MathHelper.Lerp(NPC.velocity.X, -1f, 0.1f) :
					MathHelper.Lerp(NPC.velocity.X, 1f, 0.1f);
				NPC.velocity.Y = NPC.Center.Y - closestPlayerInstance.Center.Y > 0 ?
					MathHelper.Lerp(NPC.velocity.Y, -0.5f, 0.1f) :
					MathHelper.Lerp(NPC.velocity.Y, 0.5f, 0.1f);
			}
			else
			{
				NPC.noTileCollide = true;
				NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.Center.X - closestPlayerInstance.Center.X > 0 ? 4f : -4f, 0.05f);
				NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -6f, 0.05f);
				if (distanceToPlayer > 16f * 50f)
				{
					NPC.active = false;
				}
			}
		}
		Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		if (distanceToPlayer < 170f)
		{
			closestPlayerInstance.Center = Vector2.Lerp(closestPlayerInstance.Center, NPC.Center, 0.04f);
		}
		if (!isArmatureReady)
		{
			SetIKDefaults();
		}
		for (int i = 1; i < boneChains.Length; i++)
		{
			tentacleRoots[i] = NPC.Center;
			tentaclePhases[i] += tentacleSpeeds[i];
			float baseAngle = MathHelper.TwoPi * (i - 1) / (boneChains.Length - 1);
			float radius = 80f + MathF.Sin(tentaclePhases[i] * 2f) * 20f;
			Vector2 tentacleLeaf = NPC.Center +
				new Vector2(
					MathF.Cos(baseAngle + tentaclePhases[i]) * radius,
					MathF.Sin(baseAngle + tentaclePhases[i]) * radius);
			tentacleTargets[i] = Vector2.Lerp(tentacleTargets[i], tentacleLeaf, 0.1f);
			boneChains[i].Solve(tentacleRoots[i], tentacleTargets[i], 10);
		}
		if (distanceToPlayer < 170f)
		{
			float targetRotation = NPC.Center.AngleTo(closestPlayerInstance.Center) + MathHelper.Pi;
			NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
			closeTimer += 1;
			Math.Clamp(closeTimer, 0, 300);
			Main.NewText(closeTimer);
		}
		else
		{
			NPC.rotation += 0.01f;
		}

		// Check Nearby tiles and convert if possible
		int tileX = (int)(NPC.Center.X / 16f);
		int tileY = (int)(NPC.Center.Y / 16f);
		int checkRadius = 5;
		if (timer % 30 == 0f && Main.netMode != NetmodeID.MultiplayerClient)
		{
			for (int x = tileX - checkRadius; x < tileX + checkRadius; x++)
			{
				for (int y = tileY - checkRadius; y < tileY + checkRadius; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.TileType != ModContent.TileType<Null>())
					{
						WorldGen.PlaceTile(x, y, ModContent.TileType<Null>(), forced: true);
						WorldGen.SquareTileFrame(x, y);
						NetMessage.SendTileSquare(-1, x, y, 1);
					}
				}
			}
		}
		if (Main.IsItDay() && Main.netMode != NetmodeID.MultiplayerClient)
		{
			NPC.life = 0;
			NPC.HitEffect();
			NPC.active = false;
			NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
		}
	}
}