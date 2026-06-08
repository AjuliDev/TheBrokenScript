using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;
namespace TheBrokenScript.Common;
public class CameraSnapTo : ICameraModifier
{
	public Vector2 TargetPosition;
	public string UniqueIdentity { get; private set; }
	public bool Finished { get; private set; }
	private Func<bool> shouldFinish;
	public CameraSnapTo(Vector2 position, string uniqueIdentity, Func<bool>_shouldFinish = null)
	{
		TargetPosition = position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
		UniqueIdentity = uniqueIdentity;
		shouldFinish = _shouldFinish;
	}
	public void Update(ref CameraInfo cameraInfo)
	{
		if (shouldFinish != null && shouldFinish())
		{
			Finished = true;
			return;
		}
		cameraInfo.CameraPosition = TargetPosition;
	}
}