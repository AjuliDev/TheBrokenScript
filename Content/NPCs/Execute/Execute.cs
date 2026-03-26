using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Common;
namespace TheBrokenScript.Content.NPCs.Execute;
public class Execute : ModNPC
{
	public override string Texture => "TheBrokenScript/Content/NPCs/Execute/Execute_Torso";
	private static Asset<Texture2D>[] npcTextures;
	private int[] headOffsets = [-5, 5];
	private int[] armOffsets = [-2, 2];
	private int universalBoneLength = 40;
	private int gravityScale = 5;
	private int walkSpeed = 5;
	private float legStepDist = 20f;
	private float lerpStepDist = 0.05f;
	private float lerpHead = 0.05f;
	private float lerpArm = 0.05f;
	private float legStepHeight = 10f;
	private float[] legStepProg;
	private int legStepTimer;
	private bool[] isLegStepping;
	private int torsoHeightOffset = 40;
	private int bonesPerLeg = 2;
	private Vector2 velocityVector;
	private FABRIK[] chains;
	private Vector2 currHeadPos;
	private Vector2 destHeadPos;
	private Vector2[] currArmPos;
	private Vector2[] destArmPos;
	private Vector2[] currLegPos;
	private Vector2[] destLegPos;
	private Vector2[] rootJointPos =
	{
		// x, y
		new(0f, 0f), //head
		new(-5f, 5f), //armL
		new(5f, 5f), //armR
		new(-5f, 15f), //legL
		new(5f, 15f), //legR
	};
	private Vector2[] leafJointPos =
	{
		new(0f, 10f), //head
		new(-5f, 10f), //armL
		new(5f, 10f), //armR
		new(-5f, 25f), //legL
		new(5f, 25f), //legR
	};
	private bool chainsReady => chains != null;
	public override void OnSpawn(IEntitySource source) => InitIK();
	public override void PostAI() { if (!chainsReady) InitIK(); else UpdateIK(); }
	public override bool PreAI()
	{
		int tileX = (int)(NPC.Center.X / 16f);
		int tileY = (int)(NPC.Center.Y / 16f);
		int groundTileY = tileY + 20;
		for (int i = tileY; i < groundTileY; i++)
		{
			if (!WorldGen.InWorld(tileX, i))
			{
				continue;
			}
			Tile tile = Main.tile[tileX, i];
			if (tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType])
			{
				groundTileY = i;
				break;
			}
		}
		float targetY = groundTileY * 16f - 75;
		NPC.Center = new Vector2(NPC.Center.X, MathHelper.Lerp(NPC.Center.Y, targetY, 0.5f));

		// Go To Nearest Player
		int targetPlayerID = NPC.FindClosestPlayer();
		if (targetPlayerID == -1) { return true; }
		Player targetPlayer = Main.player[targetPlayerID];
		float distance = Vector2.Distance(targetPlayer.Center, NPC.Center);
		Vector2 playerDirection = targetPlayer.Center - NPC.Center;
		if (distance >= 50)
		{
			NPC.velocity = playerDirection.SafeNormalize(Vector2.Zero) * walkSpeed;
		}
		else
		{
			NPC.velocity = Vector2.Zero;
		}
		return true;
	}

	private void InitIK()
	{
		chains = new FABRIK[5];
		currArmPos = new Vector2[2];
		currLegPos = new Vector2[2];
		destArmPos = new Vector2[2];
		destLegPos = new Vector2[2];
		legStepProg = new float[2];
		isLegStepping = new bool[2];
		float maxYCoordLookup = 80f;
		for (int legIndex = 0; legIndex < 2; legIndex++)
		{
			int index = legIndex + 3;
			Vector2 worldRootPos = NPC.Center + rootJointPos[index];
			chains[index] = new FABRIK(worldRootPos, bonesPerLeg, universalBoneLength);
			Vector2 worldLeafPos = NPC.Center + leafJointPos[index];
			worldLeafPos.Y = TileBelow(worldLeafPos.X, worldLeafPos.Y, worldLeafPos.Y + maxYCoordLookup);
			currLegPos[legIndex] = worldLeafPos;
			destLegPos[legIndex] = worldLeafPos;
			legStepProg[legIndex] = 1f;
		}
		for (int armIndex = 0; armIndex < 2; armIndex++)
		{
			int index = armIndex + 1;
			Vector2 worldRootPos = NPC.Center + rootJointPos[index];
			chains[index] = new FABRIK(worldRootPos, 1, universalBoneLength);
			Vector2 worldLeafPos = NPC.Center + leafJointPos[index];
			currArmPos[armIndex] = worldLeafPos;
			destArmPos[armIndex] = worldLeafPos;
		}
		Vector2 headWorldRootPos = NPC.Center + rootJointPos[0];
		chains[0] = new FABRIK(headWorldRootPos, 1, universalBoneLength);
		currHeadPos = NPC.Center + leafJointPos[0];
		destHeadPos = currHeadPos;
	}

	private float TileBelow(float x, float y, float maxY)
	{
		float tilePixelSize = 16f;
		int tileCoordX = (int)(x / tilePixelSize);
		int tileCoordY = (int)(y / tilePixelSize);
		int maxTileCoordY = (int)(maxY / tilePixelSize);
		for (int tileY = tileCoordY; tileY <= maxTileCoordY; tileY++)
		{
			if (!WorldGen.InWorld(tileCoordX, tileY)) { continue; }
			Tile tile = Main.tile[tileCoordX, tileY];
			if (tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType]) { return tileY * tilePixelSize + tilePixelSize / 2; }
		}
		return maxY;
	}

	private void UpdateIK()
	{
		// Head
		Vector2 headWorldRoot = NPC.Center + rootJointPos[0];
		destHeadPos = headWorldRoot + new Vector2(headOffsets[NPC.direction > 0 ? 1 : 0], 0f);
		currHeadPos = Vector2.Lerp(currHeadPos, destHeadPos, lerpHead);
		chains[0].Solve(headWorldRoot, currHeadPos);

		// Arms
		for (int armIndex = 0; armIndex < 2; armIndex++)
		{
			int index = armIndex + 1;
			Vector2 armWorldRoot = NPC.Center + rootJointPos[index];
			// Swing in opposite directions based on NPC velocity
			float swingX = armOffsets[armIndex] + NPC.velocity.X * 0.5f;
			destArmPos[armIndex] = armWorldRoot + new Vector2(swingX, 0f);
			currArmPos[armIndex] = Vector2.Lerp(currArmPos[armIndex], destArmPos[armIndex], lerpArm);
			chains[index].Solve(armWorldRoot, currArmPos[armIndex]);
		}

		// Legs
		legStepTimer++;
		for (int legIndex = 0; legIndex < 2; legIndex++)
		{
			int index = legIndex + 3;
			Vector2 legWorldRoot = NPC.Center + rootJointPos[index];
			Vector2 targetWorldPos = NPC.Center + leafJointPos[index];
			float legStepSpeed = 0.05f;
			targetWorldPos.Y = TileBelow(targetWorldPos.X, targetWorldPos.Y, targetWorldPos.Y + 40f);
			chains[index].PoleTarget = legWorldRoot + new Vector2(NPC.direction * 15f, -15f);
			chains[index].PoleTargetStrength = Math.Abs(NPC.velocity.X) < 0.5f ? 0f : MathHelper.Clamp(Math.Abs(NPC.velocity.X) / 6f, 0f, 0.4f);
			if (isLegStepping[legIndex])
			{
				legStepProg[legIndex] = MathHelper.Clamp(legStepProg[legIndex] + legStepSpeed, 0f, 1f);
				float smoothStep = legStepProg[legIndex] * legStepProg[legIndex] * (3f - (2f * legStepProg[legIndex]));
				float arcLift = MathF.Sin(smoothStep * MathF.PI) * legStepHeight;
				currLegPos[legIndex] = Vector2.Lerp(currLegPos[legIndex], destLegPos[legIndex], smoothStep) + new Vector2(0f, -arcLift);
				if (legStepProg[legIndex] >= 1f)
				{
					currLegPos[legIndex] = destLegPos[legIndex];
					isLegStepping[legIndex] = false;
				}
			} else if (!isLegStepping[1 - legIndex] && Vector2.Distance(currLegPos[legIndex], targetWorldPos) > legStepDist && legStepTimer % 2 == index % 2)
			{
				isLegStepping[legIndex] = true;
				legStepProg[legIndex] = 0f;
				Vector2 destination = targetWorldPos + new Vector2(NPC.velocity.X * legStepDist, 0f);
				destination.Y = TileBelow(destination.X, destination.Y, destination.Y + 40f);
				destLegPos[legIndex] = destination;
			}
			chains[index].Solve(legWorldRoot, currLegPos[legIndex]);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!chainsReady) return false;
		Texture2D armTex = npcTextures[1]?.Value;
		Texture2D legTex = npcTextures[2]?.Value;
		Texture2D headTex = npcTextures[0]?.Value;
		for (int legIndex = 0; legIndex < 2; legIndex++)
		{
			DrawChain(spriteBatch, screenPos, drawColor, chains[legIndex + 3], legTex);
		}
		for (int armIndex = 0; armIndex < 2; armIndex++)
		{
			DrawChain(spriteBatch, screenPos, drawColor, chains[armIndex + 1], armTex);
		}
		DrawChain(spriteBatch, screenPos, drawColor, chains[0], headTex);
		return base.PreDraw(spriteBatch, screenPos, drawColor);
	}

	private void DrawChain(SpriteBatch sb, Vector2 screenPos, Color color, FABRIK chain, Texture2D texture)
	{
		for (int i = 0; i < chain.BoneCount; i++)
		{
			Vector2 start = chain.Joints[i] - screenPos;
			Vector2 end = chain.Joints[i + 1] - screenPos;
			Vector2 diff = end - start;
			float angle = MathF.Atan2(diff.Y, diff.X);
			float length = diff.Length();
			//Scale to fit bone length
			Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
			Vector2 origin = new Vector2(0f, texture.Height / 2f);
			float scaleX = length / texture.Width;
			sb.Draw(texture, start, sourceRect, color, angle, origin, new Vector2(scaleX, 1f), SpriteEffects.None, 0f);
		}
	}

	public override void Load()
	{
		npcTextures = new Asset<Texture2D>[3];
		npcTextures[0] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Head");
		npcTextures[1] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Arm");
		npcTextures[2] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Leg");
	}
	public override void Unload()
	{
		npcTextures = null;
	}

	public override void SetDefaults()
	{
		NPC.width = 18;
		NPC.height = 40;
		NPC.damage = 14;
		NPC.defense = 6;
		NPC.lifeMax = 10;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.knockBackResist = 0f;
	}
}
