using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBrokenScript.Core;
namespace TheBrokenScript.Content.NPCs.Possessor;
public class Possessor : ModNPC
{
	public override string Texture => "TheBrokenScript/Common/Textures/transparent_pixel";
	private static Asset<Texture2D>[] Textures;
	private ref float aiState => ref NPC.ai[0];
	private ref float aiAnchorX => ref NPC.ai[1];
	private ref float aiAnchorY => ref NPC.ai[2];
	private ref float aiTimer => ref NPC.ai[3];
	private const int SEARCH_RADIUS = 50; // Tiles
	private const int SEARCH_COOLDOWN = 60;
	public override void Load()
	{
		Textures = new Asset<Texture2D>[2];
		Textures[0] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Possessor/Possessor_Head");
		Textures[1] = ModContent.Request<Texture2D>("TheBrokenScript/Content/NPCs/Possessor/Possessor_Leak");
	}
	public override void Unload()
	{
		Textures = null;
	}
	public override void SetDefaults()
	{
		NPC.width = 40;
		NPC.height = 40;
		NPC.damage = 1;
		NPC.defense = 1;
		NPC.lifeMax = 150;
		NPC.dontTakeDamage = false;
		NPC.dontTakeDamageFromHostiles = true;
		NPC.value = 0f;
		NPC.knockBackResist = 0f;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.ShowNameOnHover = false;
		NPC.lavaImmune = true;
	}
	public override void AI()
	{
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC otherNPC = Main.npc[i];
			if (otherNPC.active && otherNPC.whoAmI != NPC.whoAmI && otherNPC.ModNPC is Possessor)
			{
				NPC.active = false;
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
				return;
			}
		}
		if (aiState == 0)
		{
			aiTimer++;
			if (aiTimer == 1 || aiTimer % SEARCH_COOLDOWN == 0)
			{
				if (findAnchor(out int x, out int y))
				{
					aiAnchorX = x;
					aiAnchorY = y;
					aiState = 1;
					aiTimer = 0;
					NPC.netUpdate = true;
				}
			}
		}
		else
		{
			NPC.position.X = aiAnchorX * 16f + 8f - NPC.width / 2f;
			NPC.position.Y = aiAnchorY * 16f + 8f - NPC.height / 2f;
			NPC.velocity = Vector2.Zero;
			Tile tile = Framing.GetTileSafely((int)aiAnchorX, (int)aiAnchorY);
			if (tile.WallType != WallID.Glass)
			{
				aiState = 0;
				aiTimer = 0;
				NPC.netUpdate = true;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player p = Main.player[i];
					if (!p.active || p.dead) continue;
					if (NPC.Distance(p.Center) >= 80f) continue;

					if (Main.netMode == NetmodeID.SinglePlayer)
						p.GetModPlayer<PossessorPlayer>().Possessed = true;
					else
					{
						ModPacket packet = ModContent.GetInstance<TheBrokenScript>().GetPacket();
						packet.Write((byte)ModPacketHandler.PacketType.SyncPossessorPlayer);
						packet.Write(i);
						packet.Send(toClient: i);
					}

					NPC.active = false;
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
					break;
				}
			}
		}
	}

	private bool findAnchor(out int tilex, out int tiley)
	{
		int npcCenterX = (int)(NPC.Center.X / 16f);
		int npcCenterY = (int)(NPC.Center.Y / 16f);
		for (int y = -SEARCH_RADIUS; y <= SEARCH_RADIUS; y++)
		{
			for (int x = -SEARCH_RADIUS; x <= SEARCH_RADIUS; x++)
			{
				tilex = npcCenterX + x;
				tiley = npcCenterY + y;
				if (isSquareGlass(tilex, tiley) && surroundedByWalls(tilex, tiley))
				{
					return true;
				}
			}
		}
		tilex = tiley = 0;
		return false;
	}
	private bool surroundedByWalls(int worldX, int worldY)
	{
		for (int column = worldX - 1; column <= worldX + 2; column++)
		{
			if (Framing.GetTileSafely(column, worldY - 1).WallType == WallID.None)
			{
				return false;
			}
			if (Framing.GetTileSafely(column, worldY + 1).WallType == WallID.None)
			{
				return false;
			}
		}
		for (int row = worldY; row < worldY + 2; row++)
		{
			if (Framing.GetTileSafely(worldX - 1, row).WallType == WallID.None)
			{
				return false;
			}
			if (Framing.GetTileSafely(worldX + 2, row).WallType == WallID.None)
			{
				return false;
			}
		}
		return true;
	}

	private bool isSquareGlass(int worldX, int worldY)
	{
		for (int row = 0; row < 2; row++)
		{
			for (int column = 0; column < 2; column++)
			{
				if (Framing.GetTileSafely(worldX + column, worldY + row).WallType != WallID.Glass)
				{
					return false;
				}
			}
		}
		return true;
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (aiState != 1 || Textures[0]?.Value == null)
		{
			return false;
		}
		Vector2 center = new Vector2(aiAnchorX * 16f + 16f, aiAnchorY * 16f + 16f) - screenPos;
		spriteBatch.Draw(Textures[0].Value, center, null, Color.White, 0f, Textures[0].Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
		return false;
	}
	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (aiState != 1 || Textures[1]?.Value == null)
		{
			return;
		}
		Vector2 origin = Textures[1].Value.Size() / 2f;
		(int destX, int destY, float rot)[] borders = new[]
		{
			(0, -1, MathHelper.Pi),
			(0, 2, 0f),
			(-1, 0, MathHelper.PiOver2),
			(2, 0, -MathHelper.PiOver2)
		};
		int ancX = (int)aiAnchorX;
		int ancY = (int)aiAnchorY;
		for (int i = 0; i < 2; i++)
		{
			foreach (var (destX, destY, rot) in borders)
			{
				int tileX = ancX + destX + (destY == 0 ? 0 : i);
				int tileY = ancY + destY + (destX == 0 ? 0 : i);
				Tile tile = Framing.GetTileSafely(tileX, tileY);
				if (tile.WallType == WallID.Glass)
				{
					continue;
				}
				Vector2 worldPos = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f) - screenPos;
				spriteBatch.Draw(
					Textures[1].Value,
					worldPos,
					null,
					Color.White * 0.75f,
					rot,
					origin,
					1f,
					SpriteEffects.None,
					0f);
			}
		}
	}
	public override void DrawBehind(int index)
	{
		Main.instance.DrawCacheNPCsMoonMoon.Add(index);
	}
}