using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Common;
namespace TheBrokenScript.Content.NPCs.Observe;

public class Observe : ModNPC
{
	public static Asset<Texture2D>[] BoneTextures;
	private Vector2 baseHeadRoot, baseHeadTarget;
	private FABRIK[] boneChains;
	private bool isArmatureReady => boneChains != null;
	private float headBoneLength, tentacleBoneLength;
	private float timer = 0f;
	private Vector2 headRoot, headLeaf;
	private Vector2[] tentacleRoots, tentacleTargets;
	private float[] tentaclePhases, tentacleSpeeds;
	public void SetIKDefaults()
	{
		boneChains = new FABRIK[14];
		baseHeadRoot = new Vector2(0f, -20f);
		baseHeadTarget = new Vector2(0f, -60f);
		headBoneLength = 70f;
		tentacleBoneLength = 15f;

		// Head Initial Positioning
		headRoot = NPC.Center + baseHeadRoot;
		headLeaf = NPC.Center + baseHeadTarget;
		boneChains[0] = new FABRIK(headRoot, 1, new float[]
		{
			headBoneLength
		});
		boneChains[0].Solve(headRoot, headLeaf, 3);

		tentacleRoots = new Vector2[14];
		tentacleTargets = new Vector2[14];
		tentaclePhases = new float[14];
		tentacleSpeeds = new float[14];
		for (int i = 1; i < 14; i++)
		{
			tentacleRoots[i] = NPC.Bottom +
				new Vector2(
					Main.rand.NextFloat(-60, 60),
					Main.rand.NextFloat(0, 15));
			tentacleTargets[i] = NPC.Center +
				new Vector2(
					Main.rand.NextFloat(-80, 80),
					Main.rand.NextFloat(-80, -160));
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
		NPC.width = 30;
		NPC.height = 80;
		NPC.damage = 1000;
		NPC.defense = 1;
		NPC.dontTakeDamage = true;
		NPC.dontTakeDamageFromHostiles = true;
		NPC.lifeMax = 600;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
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
		else
		{
			IKHelper.DrawChain(spriteBatch, screenPos, drawColor, boneChains[0], BoneTextures[0]?.Value, NPC.Center, NPC.rotation);
			for (int i = 1; i < boneChains.Length; i++)
			{
				IKHelper.DrawChain(spriteBatch, screenPos, drawColor, boneChains[i], BoneTextures[1]?.Value, NPC.Center, NPC.rotation);
			}
		}
		return base.PreDraw(spriteBatch, screenPos, drawColor);
	}
	public override void Load()
	{
		BoneTextures = new Asset<Texture2D>[2];
		BoneTextures[0] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Observe/Observe_Head");
		BoneTextures[1] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Observe/Observe_Tentacle");
	}
	public override void Unload()
	{
		BoneTextures = null;
	}
	public override void AI()
	{
		if (!isArmatureReady)
		{
			SetIKDefaults();
		}
		timer++;
		for (int i = 1; i < 14; i++)
		{
			tentaclePhases[i] += tentacleSpeeds[i];
			Vector2 wriggleOffset = new Vector2(
				MathF.Sin(tentaclePhases[i]) * 120f,
				MathF.Cos(tentaclePhases[i] * 0.7f) * 180f);
			Vector2 tentacleLeaf = NPC.Center + new Vector2(0f, -80f) + wriggleOffset;
			tentacleTargets[i] = Vector2.Lerp(tentacleTargets[i], tentacleLeaf, 0.1f);
			boneChains[i].Solve(tentacleRoots[i], tentacleTargets[i], 10);
		}
		headRoot = NPC.Center + baseHeadRoot;
		headLeaf = NPC.Center + baseHeadTarget;
		if (Main.rand.NextBool(50))
		{
			headLeaf += new Vector2(Main.rand.NextFloat(-20f, 20f), 0f);
		}
		boneChains[0].Solve(headRoot, headLeaf, 3);
	}
}