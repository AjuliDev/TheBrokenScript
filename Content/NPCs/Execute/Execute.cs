using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace TheBrokenScript.Content.NPCs.Execute;

public class Execute : ModNPC
{
	public override string Texture => "TheBrokenScript/Common/Textures/transparent_pixel";
	public static Asset<Texture2D>[] BoneTextures;
	Common.BipedRig Armature = new Common.BipedRig();
	private bool isArmatureReady => Armature.BoneChains != null;
	private enum PlantedLimb
	{
		Left,
		LeftSet,
		Right,
		RightSet,
		None
	}
	//private PlantedLimb plantedArm = PlantedLimb.None; -- Unused, at least for now.
	private PlantedLimb plantedLeg = PlantedLimb.None;
	private float stepDistance = 64f;
	private float sineMovement = 0f;
	private Vector2 torsoRoot, torsoLeaf, headRoot, headLeaf, leftArmRoot, leftArmLeaf, rightArmRoot, rightArmLeaf, leftLegRoot, leftLegLeaf, rightLegRoot, rightLegLeaf, leftLegLeafVisual, rightLegLeafVisual;
	public void SetIKDefaults()
	{
		// Roots
		Armature.BoneRoot.BaseTorsoRoot = new Vector2(0f, 30f);
		Armature.BoneRoot.BaseHeadRoot = new Vector2(0f, 0f);
		Armature.BoneRoot.BaseLeftArmRoot = new Vector2(-5f, 0f);
		Armature.BoneRoot.BaseRightArmRoot = new Vector2(5f, 0f);
		Armature.BoneRoot.BaseLeftLegRoot = new Vector2(-5f, 30f);
		Armature.BoneRoot.BaseRightLegRoot = new Vector2(5f, 30f);

		// Targets
		Armature.BoneLeaf.BaseTorsoTarget = new Vector2(0f, 0f);
		Armature.BoneLeaf.BaseHeadTarget = new Vector2(0f, -20f);
		Armature.BoneLeaf.BaseLeftArmTarget = new Vector2(-7f, 80f);
		Armature.BoneLeaf.BaseRightArmTarget = new Vector2(7f, 80f);
		Armature.BoneLeaf.BaseLeftLegTarget = new Vector2(-7f, 130f);
		Armature.BoneLeaf.BaseRightLegTarget = new Vector2(7f, 130f);

		// Lengths
		Armature.BoneLength.TorsoBoneLength = 40f;
		Armature.BoneLength.HeadBoneLength = 50f;
		Armature.BoneLength.LeftUpperArmLength = 35f;
		Armature.BoneLength.LeftLowerArmLength = 50f;
		Armature.BoneLength.RightUpperArmLength = 35f;
		Armature.BoneLength.RightLowerArmLength = 50f;
		Armature.BoneLength.LeftUpperLegLength = 40f;
		Armature.BoneLength.LeftLowerLegLength = 50f;
		Armature.BoneLength.RightUpperLegLength = 40f;
		Armature.BoneLength.RightLowerLegLength = 50f;

		// Chains
		Armature.BoneChains = new Common.FABRIK[6];

		// Initial Positioning
		torsoRoot = NPC.Center + Armature.BoneRoot.BaseTorsoRoot;
		torsoLeaf = NPC.Center + Armature.BoneLeaf.BaseTorsoTarget;
		Armature.BoneChains[0] = new Common.FABRIK(torsoRoot, 1, new float[] { Armature.BoneLength.TorsoBoneLength });
		Armature.BoneChains[0].Solve(torsoRoot, torsoLeaf, 3);

		headRoot = NPC.Center + Armature.BoneRoot.BaseHeadRoot;
		headLeaf = NPC.Center + Armature.BoneLeaf.BaseHeadTarget;
		Armature.BoneChains[1] = new Common.FABRIK(headRoot, 1, new float[] { Armature.BoneLength.HeadBoneLength });
		Armature.BoneChains[1].Solve(headRoot, headLeaf, 3);

		leftArmRoot = NPC.Center + Armature.BoneRoot.BaseLeftArmRoot;
		leftArmLeaf = NPC.Center + Armature.BoneLeaf.BaseLeftArmTarget;
		Armature.BoneChains[2] = new Common.FABRIK(leftArmRoot, 2, new float[]
		{
			Armature.BoneLength.LeftUpperArmLength,
			Armature.BoneLength.LeftLowerArmLength
		});
		Armature.BoneChains[2].Solve(leftArmRoot, leftArmLeaf, 3);

		rightArmRoot = NPC.Center + Armature.BoneRoot.BaseRightArmRoot;
		rightArmLeaf = NPC.Center + Armature.BoneLeaf.BaseRightArmTarget;
		Armature.BoneChains[3] = new Common.FABRIK(rightArmRoot, 2, new float[]
		{
			Armature.BoneLength.RightUpperArmLength,
			Armature.BoneLength.RightLowerArmLength
		});
		Armature.BoneChains[3].Solve(rightArmRoot, rightArmLeaf, 3);

		leftLegRoot = NPC.Center + Armature.BoneRoot.BaseLeftLegRoot;
		leftLegLeaf = NPC.Center + Armature.BoneLeaf.BaseLeftLegTarget;
		leftLegLeafVisual = leftLegLeaf;
		Armature.BoneChains[4] = new Common.FABRIK(leftLegRoot, 2, new float[]
		{
			Armature.BoneLength.LeftUpperLegLength,
			Armature.BoneLength.LeftLowerLegLength
		});
		//Armature.BoneChains[4].PoleTargetStrength = 0.75f;
		Armature.BoneChains[4].PoleTarget = NPC.velocity.X >= 0 ?
			leftLegRoot + new Vector2(-20f, -20f) :
			leftLegRoot + new Vector2(20f, -20f);
		Armature.BoneChains[4].Solve(leftLegRoot, leftLegLeaf, 3);

		rightLegRoot = NPC.Center + Armature.BoneRoot.BaseRightLegRoot;
		rightLegLeaf = NPC.Center + Armature.BoneLeaf.BaseRightLegTarget;
		rightLegLeafVisual = rightLegLeaf;
		Armature.BoneChains[5] = new Common.FABRIK(rightLegRoot, 2, new float[]
		{
			Armature.BoneLength.RightUpperLegLength,
			Armature.BoneLength.RightLowerLegLength
		});
		//Armature.BoneChains[5].PoleTargetStrength = 0.75f;
		Armature.BoneChains[5].PoleTarget = NPC.velocity.X >= 0 ?
			rightLegRoot + new Vector2(-20f, -20f) :
			rightLegRoot + new Vector2(20f, -20f);
		Armature.BoneChains[5].Solve(rightLegRoot, rightLegLeaf, 3);

		if (plantedLeg == PlantedLimb.None)
		{
			plantedLeg = PlantedLimb.Left;
		}
	}
	public override void SetDefaults()
	{
		NPC.width = 40;
		NPC.height = 60;
		NPC.damage = 50;
		NPC.defense = 1;
		NPC.dontTakeDamage = false;
		NPC.dontTakeDamageFromHostiles = true;
		NPC.lifeMax = 1600;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.ShowNameOnHover = false;
		NPC.knockBackResist = 0;
		NPC.lavaImmune = true;
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!isArmatureReady)
		{
			SetIKDefaults();
		}
		else
		{
			for (int i = 0; i < 6; i++)
			{
				Common.IKHelper.DrawChain(spriteBatch, screenPos, drawColor, Armature.BoneChains[i], BoneTextures[i]?.Value, NPC.Center, NPC.rotation);
			}
		}
		return base.PreDraw(spriteBatch, screenPos, drawColor);
	}
	public override void Load()
	{
		BoneTextures = new Asset<Texture2D>[6];
		BoneTextures[0] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Torso");
		BoneTextures[1] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Head");
		BoneTextures[2] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Arm");
		BoneTextures[3] = BoneTextures[2];
		BoneTextures[4] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Execute/Execute_Leg");
		BoneTextures[5] = BoneTextures[4];
	}
	public override void Unload()
	{
		BoneTextures = null;
	}
	public override void AI()
	{
		// Check if IK got initialized.
		if (!isArmatureReady)
		{
			SetIKDefaults();
		}

		// Check for collisions
		bool isColliding = Collision.SolidCollision(NPC.position + new Vector2(NPC.spriteDirection * 20f, -20f), NPC.width, NPC.height);
		bool collisionAtWaist = Collision.SolidCollision(NPC.position + new Vector2(0f, NPC.height) + new Vector2(NPC.spriteDirection * 20f, 0f), NPC.width, NPC.height);

		//Main.NewText($"{NPC.velocity.X} | {isColliding}");
		// Adjust hip height based on environment
		float currentHipHeight = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(NPC.Bottom.X, NPC.Bottom.Y, NPC.Bottom.Y + (
			Armature.BoneLength.LeftUpperLegLength + Armature.BoneLength.LeftLowerLegLength)) - NPC.Bottom.Y;
		float targetHipHeight = (Armature.BoneLength.LeftUpperLegLength + Armature.BoneLength.LeftLowerLegLength) * 0.9f;
		if (isColliding && !collisionAtWaist)
		{
			targetHipHeight = (Armature.BoneLength.LeftUpperLegLength + Armature.BoneLength.LeftLowerLegLength) * 0.1f;
		}
		float heightLerpError = 8f;
		if (currentHipHeight < targetHipHeight - heightLerpError)
		{
			NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -5.5f, 0.1f);
		} else if (currentHipHeight > targetHipHeight + heightLerpError)
		{
			NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 2.5f, 0.1f);
		}
		else
		{
			NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.3f);
		}

		// Adjust horizontal velocity based on distance from nearest player
		int closestPlayerID = NPC.FindClosestPlayer();
		Player closestPlayerInstance = Main.player[closestPlayerID];
		float distanceToPlayer = NPC.Center.X - closestPlayerInstance.Center.X;
		if (Math.Abs(distanceToPlayer) > 32f)
		{
			if (isColliding && !collisionAtWaist)
			{
				NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
				NPC.width = (int)MathHelper.Clamp(NPC.width - 1, 10, 40);
				NPC.height = (int)MathHelper.Clamp(NPC.height - 1, 10, 60);
			}
			else
			{
				NPC.width = (int)MathHelper.Clamp(NPC.width + 1, 10, 40);
				NPC.height = (int)MathHelper.Clamp(NPC.height + 1, 10, 60);
				NPC.velocity.X = distanceToPlayer > 0 ?
					MathHelper.Lerp(NPC.velocity.X, -2f, 0.1f) :
					MathHelper.Lerp(NPC.velocity.X, 2f, 0.1f);
			}
			// Also set sprite direction for other uses
			NPC.spriteDirection = distanceToPlayer > 0 ? -1 : 1;
		} else
		{
			NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
		}

		// Torso Bone Placement
		torsoRoot = NPC.Center + Armature.BoneRoot.BaseTorsoRoot;
		torsoLeaf = NPC.Center + Armature.BoneLeaf.BaseTorsoTarget;
		Armature.BoneChains[0].Solve(torsoRoot, torsoLeaf, 3);

		// Head Bone Placement
		headRoot = NPC.Center + Armature.BoneRoot.BaseHeadRoot;
		headLeaf = NPC.Center + Armature.BoneLeaf.BaseHeadTarget;
		if (Main.rand.NextBool(50))
		{
			headLeaf += new Vector2(Main.rand.NextFloat(-20f, 20f), 0f);
		}
		headLeaf = NPC.velocity.X >= 0 ? Vector2.Lerp(headLeaf, headLeaf + new Vector2(15f, 15f), 0.1f) : Vector2.Lerp(headLeaf, headLeaf + new Vector2(-15f, 15f), 0.1f);
		Armature.BoneChains[1].Solve(headRoot, headLeaf, 3);

		sineMovement += Math.Abs(NPC.velocity.X) * 0.02f;
		float armSwing = MathF.Sin(sineMovement) * 15f;

		// Left Arm Placement
		leftArmRoot = NPC.Center + Armature.BoneRoot.BaseLeftArmRoot;
		leftArmLeaf = NPC.Center + Armature.BoneLeaf.BaseLeftArmTarget + new Vector2(armSwing, 0f);
		float leftArmLeafY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(leftArmRoot.X, NPC.TopLeft.Y, NPC.TopLeft.Y + Armature.BoneLength.LeftUpperArmLength + Armature.BoneLength.LeftLowerArmLength);
		if (Collision.SolidCollision(new Vector2(leftArmRoot.X, leftArmLeafY), 1, 1))
		{
			leftArmLeaf = Vector2.Lerp(leftArmLeaf, new Vector2(leftArmRoot.X, leftArmLeafY), 0.2f);
		}
		if (Vector2.Distance(NPC.Center + Armature.BoneRoot.BaseLeftArmRoot, closestPlayerInstance.Center) < Armature.BoneLength.LeftUpperArmLength + Armature.BoneLength.LeftLowerArmLength)
		{
			leftArmLeaf = Vector2.Lerp(leftArmLeaf, closestPlayerInstance.Center, 0.3f);
		}
		Armature.BoneChains[2].Solve(leftArmRoot, leftArmLeaf, 3);

		// Right Arm Placement
		rightArmRoot = NPC.Center + Armature.BoneRoot.BaseRightArmRoot;
		rightArmLeaf = NPC.Center + Armature.BoneLeaf.BaseRightArmTarget + new Vector2(-armSwing, 0f);
		float rightArmLeafY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(rightArmRoot.X, NPC.TopRight.Y, NPC.TopRight.Y + Armature.BoneLength.RightUpperArmLength + Armature.BoneLength.RightLowerArmLength);
		if (Collision.SolidCollision(new Vector2(rightArmRoot.X, rightArmLeafY), 1, 1))
		{
			rightArmLeaf = Vector2.Lerp(rightArmLeaf, new Vector2(rightArmRoot.X, rightArmLeafY), 0.2f);
		}
		if (Vector2.Distance(NPC.Center + Armature.BoneRoot.BaseRightArmRoot, closestPlayerInstance.Center) < Armature.BoneLength.RightUpperArmLength + Armature.BoneLength.RightLowerArmLength)
		{
			rightArmLeaf = Vector2.Lerp(rightArmLeaf, closestPlayerInstance.Center, 0.3f);
		}
		Armature.BoneChains[3].Solve(rightArmRoot, rightArmLeaf, 3);

		// Left Leg Placement
		leftLegRoot = NPC.Center + Armature.BoneRoot.BaseLeftLegRoot;
		Vector2 oldLeftLegLeaf = leftLegLeaf;
		if (NPC.velocity.X != 0)
		{
			Vector2 leftLegPoleTarget = NPC.velocity.X >= 0 ?
				leftLegRoot + new Vector2(40f, Armature.BoneLength.LeftUpperLegLength) :
				leftLegRoot + new Vector2(-40f, Armature.BoneLength.LeftUpperLegLength);
			Armature.BoneChains[4].PoleTarget = Vector2.Lerp((Vector2)Armature.BoneChains[4].PoleTarget, leftLegPoleTarget, 0.1f);
		}
		if (plantedLeg == PlantedLimb.Left)
		{
			// set the leg step pos
			float leftLegLeafX = NPC.Bottom.X + (NPC.velocity.X * 10f);
			float leftLegLeafY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(leftLegLeafX, NPC.BottomLeft.Y,
				NPC.BottomLeft.Y +
				Armature.BoneLength.LeftUpperLegLength +
				Armature.BoneLength.LeftLowerLegLength);
			leftLegLeaf = new Vector2(leftLegLeafX, leftLegLeafY);
			plantedLeg = PlantedLimb.LeftSet;
		} else if (plantedLeg == PlantedLimb.LeftSet)
		{
			// Check if distance between current leg pos and projected leg pos is over the step distance
			float currProjectedX = NPC.Bottom.X;
			float currProjectedY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(currProjectedX, NPC.BottomLeft.Y,
				NPC.BottomLeft.Y +
				Armature.BoneLength.LeftUpperLegLength +
				Armature.BoneLength.LeftLowerLegLength);
			Vector2 projVector = new Vector2(currProjectedX, currProjectedY);
			if (Vector2.Distance(projVector, leftLegLeaf) > stepDistance * 0.85)
			{
				plantedLeg = PlantedLimb.Right;
			}
		} else if (plantedLeg == PlantedLimb.Right || plantedLeg == PlantedLimb.RightSet)
		{
			// Have leg be up, bent
			float currX = NPC.Bottom.X;
			float groundY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(currX, NPC.BottomLeft.Y,
				NPC.BottomLeft.Y +
				Armature.BoneLength.LeftUpperLegLength +
				Armature.BoneLength.LeftLowerLegLength);
			float floatingY = groundY - 32f;
			leftLegLeaf = Vector2.Lerp(oldLeftLegLeaf, new Vector2(currX, floatingY), 0.1f);
		}
		//Armature.BoneChains[4].PoleTargetStrength = 0.5f;
		leftLegLeafVisual = Vector2.Lerp(leftLegLeafVisual, leftLegLeaf, 0.5f);
		Armature.BoneChains[4].Solve(leftLegRoot, leftLegLeafVisual, 3);

		// Right Leg Placement
		rightLegRoot = NPC.Center + Armature.BoneRoot.BaseRightLegRoot;
		Vector2 oldRightLegLeaf = rightLegLeaf;
		if (NPC.velocity.X != 0)
		{
			Vector2 rightLegPoleTarget = NPC.velocity.X >= 0 ?
				rightLegRoot + new Vector2(40f, Armature.BoneLength.RightUpperLegLength) :
				rightLegRoot + new Vector2(-40f, Armature.BoneLength.RightUpperLegLength);
			Armature.BoneChains[5].PoleTarget = Vector2.Lerp((Vector2)Armature.BoneChains[5].PoleTarget, rightLegPoleTarget, 0.1f);
		}
		if (plantedLeg == PlantedLimb.Right)
		{
			float rightLegLeafX = NPC.Bottom.X + (NPC.velocity.X * 10f);
			float rightLegLeafY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(rightLegLeafX, NPC.BottomRight.Y,
				NPC.BottomRight.Y +
				Armature.BoneLength.RightUpperLegLength +
				Armature.BoneLength.RightLowerLegLength);
			rightLegLeaf = new Vector2(rightLegLeafX, rightLegLeafY);
			plantedLeg = PlantedLimb.RightSet;
		} else if (plantedLeg == PlantedLimb.RightSet)
		{
			float currProjectedX = NPC.Bottom.X;
			float currProjectedY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(currProjectedX, NPC.BottomRight.Y,
				NPC.BottomRight.Y +
				Armature.BoneLength.RightUpperLegLength +
				Armature.BoneLength.RightLowerLegLength);
			Vector2 projVector = new Vector2(currProjectedX, currProjectedY);
			if (Vector2.Distance(projVector, rightLegLeaf) > stepDistance * 0.85)
			{
				plantedLeg = PlantedLimb.Left;
			}
		} else if (plantedLeg == PlantedLimb.Left || plantedLeg == PlantedLimb.LeftSet)
		{
			float currX = NPC.Bottom.X;
			float groundY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(currX, NPC.BottomRight.Y,
				NPC.BottomRight.Y +
				Armature.BoneLength.RightUpperLegLength +
				Armature.BoneLength.RightLowerLegLength);
			float floatingY = groundY - 32f;
			rightLegLeaf = Vector2.Lerp(oldRightLegLeaf, new Vector2(currX, floatingY), 0.1f);
		}
		//Armature.BoneChains[5].PoleTargetStrength = 0f;
		rightLegLeafVisual = Vector2.Lerp(rightLegLeafVisual, rightLegLeaf, 0.5f);
		Armature.BoneChains[5].Solve(rightLegRoot, rightLegLeafVisual, 3);

		// Plant the legs if standing still
		if (NPC.velocity.X >= -0.25 && NPC.velocity.X <= 0.25)
		{
			float leftLegLeafX = NPC.BottomLeft.X;
			float leftLegLeafY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(leftLegLeafX, NPC.BottomLeft.Y,
				NPC.BottomLeft.Y +
				Armature.BoneLength.LeftUpperLegLength +
				Armature.BoneLength.LeftLowerLegLength) + 8f;
			leftLegLeaf = new Vector2(leftLegLeafX, leftLegLeafY);
			//Armature.BoneChains[4].PoleTargetStrength = 0.5f;

			float rightLegLeafX = NPC.BottomRight.X;
			float rightLegLeafY = Common.IKHelper.ReturnFirstTilePixelCoordinateBelow(rightLegLeafX, NPC.BottomRight.Y,
				NPC.BottomRight.Y +
				Armature.BoneLength.RightUpperLegLength +
				Armature.BoneLength.RightLowerLegLength) + 8f;
			rightLegLeaf = new Vector2(rightLegLeafX, rightLegLeafY);
			//Armature.BoneChains[5].PoleTargetStrength = 0f;
		}
	}
	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(leftArmLeaf.X);
		writer.Write(leftArmLeaf.Y);
		writer.Write(rightArmLeaf.X);
		writer.Write(rightArmLeaf.Y);
	}
	public override void ReceiveExtraAI(BinaryReader reader)
	{
		leftArmLeaf = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		rightArmLeaf = new Vector2(reader.ReadSingle(), reader.ReadSingle());
	}
}