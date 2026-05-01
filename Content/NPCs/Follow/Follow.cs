using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Common;
namespace TheBrokenScript.Content.NPCs.Follow;
public class Follow : ModNPC
{
	public static Asset<Texture2D> BoneTexture;
	private FABRIK[] boneChains;
	private bool isArmatureReady => boneChains != null;
	private Vector2[] tentacleRoots, tentacleTargets;
	private float[] tentaclePhases, tentacleSpeeds;
	private float tentacleBoneLength, amountOfBones, timer;
	public void SetIKDefaults()
	{
		boneChains = new FABRIK[3];
		tentacleBoneLength = 16f;
		tentacleRoots = new Vector2[3];
		tentacleTargets = new Vector2[3];
		tentaclePhases = new float[3];
		tentacleSpeeds = new float[3];
		for (int i = 0; i < 3; i++)
		{
			tentacleRoots[i] = NPC.Top +
				new Vector2(
					Main.rand.NextFloat(-5, 5),
					0);
			tentacleTargets[i] = NPC.Top +
				new Vector2(
					Main.rand.NextFloat(-30, 30),
					Main.rand.NextFloat(-110, -150)
					);
			int maxBones = 5;
			float distance = Vector2.Distance(tentacleTargets[i], tentacleRoots[i]);
			amountOfBones = Math.Min(maxBones, (int)(distance / tentacleBoneLength));
			boneChains[i] = new FABRIK(tentacleRoots[i], (int)amountOfBones, Enumerable.Repeat(tentacleBoneLength, (int)amountOfBones).ToArray());
			boneChains[i].Solve(tentacleRoots[i], tentacleTargets[i], 3);
			tentaclePhases[i] = Main.rand.NextFloat(0, MathHelper.TwoPi);
			tentacleSpeeds[i] = Main.rand.NextFloat(0.03f, 0.07f);
		}
	}
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 12;
		NPCID.Sets.ShimmerTransformToNPC[Type] = ModContent.NPCType<NPCs.SubAnomalyOne.SubAnomalyOne>();
		NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
		{
			Velocity = 1f
		};
		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
	}
	public override void SetDefaults()
	{
		NPC.width = 18;
		NPC.height = 90;
		NPC.damage = 10;
		NPC.defense = 20;
		NPC.lifeMax = 50;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.dontTakeDamageFromHostiles = true;
		NPC.dontTakeDamage = false;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.value = 0f;
		NPC.knockBackResist = 0f;
		NPC.aiStyle = -1;
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.ShowNameOnHover = false;
		NPC.lavaImmune = false;
	}
	public override void AI()
	{
		Lighting.AddLight(NPC.Center, 0.9f, 0.05f, 0.87f); // Purple glow behind NPC
		NPC.spriteDirection = NPC.direction;
		// Adjust horizontal velocity based on distance from nearest player
		int closestPlayerID = NPC.FindClosestPlayer();
		Player closestPlayerInstance = Main.player[closestPlayerID];
		float distanceToPlayer = NPC.Center.X - closestPlayerInstance.Center.X;
		if (Math.Abs(distanceToPlayer) > 16f)
		{
			NPC.velocity.X = distanceToPlayer > 0 ?
				MathHelper.Lerp(NPC.velocity.X, -1.0f, 0.1f) :
				MathHelper.Lerp(NPC.velocity.X, 1.0f, 0.1f);
			NPC.direction = distanceToPlayer > 0 ? -1 : 1;
		}
		else
		{
			NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
		}
		Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
		timer++;
		if (!isArmatureReady)
		{
			SetIKDefaults();
		}
		for (int i = 0; i < 3; i++)
		{
			tentaclePhases[i] += tentacleSpeeds[i];
			Vector2 wriggleOffset = new Vector2(
				MathF.Sin(tentaclePhases[i]) * 30f,
				MathF.Cos(tentaclePhases[i] * 0.7f) * 50f);
			tentacleRoots[i] = NPC.Top +
				new Vector2(
					0,
					15f);
			Vector2 tentacleLeaf = NPC.Center + new Vector2(0f, -tentacleBoneLength * Main.rand.NextFloat(1, amountOfBones)) + wriggleOffset;
			tentacleTargets[i] = Vector2.Lerp(tentacleTargets[i], tentacleLeaf, 0.1f);
			boneChains[i].Solve(tentacleRoots[i], tentacleTargets[i], 10);
		}
	}
	public override void FindFrame(int frameHeight)
	{
		if (Math.Abs(NPC.velocity.X) > 0.1f || Math.Abs(NPC.velocity.Y) > 0.1f)
		{
			NPC.frameCounter++;
			if (NPC.frameCounter >= 9)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;
				if (NPC.frame.Y >= frameHeight * 12)
				{
					NPC.frame.Y = 0;
				}
			}
		}
		else
		{
			NPC.frame.Y = frameHeight * 3;
			NPC.frameCounter = 0;
		}
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
		else
		{
			IKHelper.DrawChain(spriteBatch, screenPos, drawColor, boneChains[0], BoneTexture?.Value, NPC.Center, NPC.rotation);
			for (int i = 1; i < boneChains.Length; i++)
			{
				IKHelper.DrawChain(spriteBatch, screenPos, drawColor, boneChains[i], BoneTexture.Value, NPC.Center, NPC.rotation);
			}
		}
		Texture2D texture = TextureAssets.Npc[Type].Value;
		Vector2 drawOrigin = new Vector2(texture.Width / 2, NPC.frame.Height / 2);
		Vector2 drawPos = NPC.Center - screenPos;
		spriteBatch.Draw(
			texture,
			drawPos,
			NPC.frame,
			drawColor,
			NPC.rotation,
			drawOrigin,
			.8f,
			NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
			0f
		);
		return false;
	}
	public override void Load()
	{
		BoneTexture = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Follow/Follow_Tentacle");
	}
	public override void Unload()
	{
		BoneTexture = null;
	}
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		// Send player to Null Torture Dimension
		base.OnHitPlayer(target, hurtInfo);
	}
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 0.1f;
	}
}