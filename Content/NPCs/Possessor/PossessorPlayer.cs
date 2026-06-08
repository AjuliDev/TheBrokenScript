using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace TheBrokenScript.Content.NPCs.Possessor;
public class PossessorPlayer : ModPlayer
{
	public bool Possessed;
	public int PossessTimer;
	private const int POSSESS_DURATION = 5 * 60; // Seconds
	private const int TARGET_SEARCH_RADIUS = 30; // Tiles
	public override void PreUpdate()
	{
		if (!Possessed)
		{
			return;
		}
		Player.isControlledByFilm = true;
		PossessTimer++;
		if (PossessTimer >= POSSESS_DURATION)
		{
			Possessed = false;
			PossessTimer = 0;
			Player.isControlledByFilm = false;
			return;
		}
		Player.AddBuff(BuffID.Darkness, 2);
		clearControls();
		moveToDanger();
	}
	private void moveToDanger()
	{
		Vector2? target = findDanger();
		if (target == null)
		{
			if (Player.direction == 1)
			{
				Player.controlRight = true;
			}
			else
			{
				Player.controlLeft = true;
			}
			return;
		}
		float coordDiffX = target.Value.X - Player.Center.X;
		if (coordDiffX > 8f)
		{
			Player.controlRight = true;
		} else if (coordDiffX < -8f)
		{
			Player.controlLeft = true;
		}
		if (target.Value.Y < Player.Center.Y - 32f || isPlayerBlocked(coordDiffX))
		{
			Player.controlJump = true;
		} else if (target.Value.Y > Player.Center.Y + 32f)
		{
			Player.controlDownHold = true;
		}
	}
	private bool isPlayerBlocked(float coordinateX)
	{
		int checkXCoord = (int)((Player.Center.X + (coordinateX > 0 ? 20 : -20)) / 16f);
		int checkYCoord = (int)((Player.Center.Y + 8f) / 16f);
		Tile tile = Framing.GetTileSafely(checkXCoord, checkYCoord);
		return tile.HasTile;
	}
	private Vector2? findDanger()
	{
		Vector2? bestTarget = null;
		float bestDist = float.MaxValue;
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC npc = Main.npc[i];
			if (!npc.active || npc.friendly || npc.dontTakeDamage)
			{
				continue;
			}
			float distance = Player.Distance(npc.Center);
			if (distance < TARGET_SEARCH_RADIUS * 16f && distance < bestDist)
			{
				bestDist = distance;
				bestTarget = npc.Center;
			}
		}
		Vector2? lava = findLava();
		if (lava.HasValue)
		{
			float dist = Player.Distance(lava.Value);
			if (dist < bestDist)
			{
				bestTarget = lava.Value;
			}
		}
		return bestTarget;
	}
	private Vector2? findLava()
	{
		int playerCenterX = (int)(Player.Center.X / 16f);
		int playerCenterY = (int)(Player.Center.Y / 16f);
		Vector2? bestTarget = null;
		float bestDistance = float.MaxValue;
		for (int y = -TARGET_SEARCH_RADIUS; y <= TARGET_SEARCH_RADIUS; y++)
		{
			for (int x = -TARGET_SEARCH_RADIUS; x <= TARGET_SEARCH_RADIUS; x++)
			{
				Tile tile = Framing.GetTileSafely(playerCenterX + x, playerCenterY + y);
				if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava)
				{
					Vector2 position = new Vector2((playerCenterX + x) * 16f + 8f, (playerCenterY + y) * 16f + 8f);
					float distance = Player.Distance(position);
					if (distance < bestDistance)
					{
						bestDistance = distance;
						bestTarget = position;
					}
				}
			}
		}
		return bestTarget;
	}
	private void clearControls()
	{
		Player.controlLeft = false;
		Player.controlRight = false;
		Player.controlJump = false;
		Player.controlDown = false;
		Player.controlUseItem = false;
		Player.controlUseTile = false;
		Player.controlDownHold = false;
	}
}