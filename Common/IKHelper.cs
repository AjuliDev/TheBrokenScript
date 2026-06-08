using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
namespace TheBrokenScript.Common;
public class IKHelper
{
	public static float ReturnFirstTilePixelCoordinateBelow(float x_row, float y_column, float range)
	{
		for (float i=(y_column / 16); i <= (y_column + range) / 16; i++)
		{
			if ((x_row / 16) < 0 || ((x_row / 16) >= Terraria.Main.maxTilesX) || i < 0 || i >= Terraria.Main.maxTilesY)
			{
				break;
			}
			Terraria.Tile tile = Terraria.Main.tile[(int)(x_row / 16), (int)i];
			if (tile.HasTile && tile.HasUnactuatedTile)
			{
				if (!Main.tileSolid[tile.TileType])
				{
					continue;
				}
				if (tile.IsHalfBlock)
				{
					return i * 16f + 8f;
				} else if (tile.Slope != Terraria.ID.SlopeType.Solid)
				{
					switch (tile.Slope)
					{
						case Terraria.ID.SlopeType.SlopeDownLeft:
							return i * 16f + (x_row - ((int)(x_row / 16) * 16f));
						case Terraria.ID.SlopeType.SlopeDownRight:
							return i * 16f + (16f - (x_row - ((int)(x_row / 16) * 16f)));
					}
				}
				return i * 16f;
			}
		}
		return System.Math.Min(y_column + range, Terraria.Main.maxTilesY * 16f);
	}
	public static void DrawChain(SpriteBatch spriteBatch, Vector2 screenPosition, Color chainColor, Common.FABRIK fabrikChain, Texture2D texture, Vector2 npcCenter, float npcRotation, float rotationOffset = 0f)
	{
		for (int i = 0; i < fabrikChain.BoneCount; i++)
		{
			Vector2 startPosition = IKHelper.RotateAround(fabrikChain.Joints[i], npcCenter, npcRotation) - screenPosition;
			Vector2 endPosition = IKHelper.RotateAround(fabrikChain.Joints[i + 1], npcCenter, npcRotation) - screenPosition;
			Vector2 difference = endPosition - startPosition;
			float angle = System.MathF.Atan2(difference.Y, difference.X) + rotationOffset;
			float length = difference.Length();
			Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
			Vector2 origin = new Vector2(0f, texture.Height / 2f);
			float scaleX = length / texture.Width;
			spriteBatch.Draw(texture, startPosition, sourceRectangle, chainColor, angle, origin, new Vector2(scaleX, 1f), SpriteEffects.None, 0f);
		}
	}

	private static Vector2 RotateAround(Vector2 point, Vector2 pivot, float rotation)
	{
		Vector2 offset = point - pivot;
		float cos = System.MathF.Cos(rotation);
		float sin = System.MathF.Sin(rotation);
		return pivot + new Vector2(offset.X * cos - offset.Y * sin, offset.X * sin + offset.Y * cos);
	}

	// Collision.TileCollision broken
}
