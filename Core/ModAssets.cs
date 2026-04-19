using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public class ModAssets : ModSystem
{
	public static Dictionary<string, Asset<Texture2D>> Textures { get; private set; } = new();
	public static Dictionary<string, Asset<SoundEffect>> Sounds { get; private set; } = new();
	public static Dictionary<string, Asset<Effect>> ShaderEffects { get; private set; } = new();
	private static readonly string[] AssetPaths = [
		"Common/Textures",
		"Common/Sounds",
		"Common/Effects",
		];
	public override void Load()
	{
		if (!Main.dedServ)
		{
			foreach (string file in Mod.GetFileNames())
			{
				if (!AssetPaths.Any(folder => file.StartsWith(folder)))
					continue;
				string assetPath = file[..file.LastIndexOf('.')];
				if (file.EndsWith(".png"))
				{
					Textures[assetPath] = ModContent.Request<Texture2D>($"{Mod.Name}/{assetPath}", AssetRequestMode.ImmediateLoad);
				} else if (file.EndsWith(".wav") || file.EndsWith(".ogg"))
				{
					Sounds[assetPath] = ModContent.Request<SoundEffect>($"{Mod.Name}/{assetPath}", AssetRequestMode.ImmediateLoad);
				}
				else if (file.EndsWith(".fxc"))
				{
					ShaderEffects[assetPath] = ModContent.Request<Effect>($"{Mod.Name}/{assetPath}", AssetRequestMode.ImmediateLoad);
				}
			}
		}
	}
	public override void Unload()
	{
		Textures = null;
		Sounds = null;
		ShaderEffects = null;
	}
}