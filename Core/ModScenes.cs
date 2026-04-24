using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public class ModScenes : ModSystem
{
	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}
		Filters.Scene["Posterize"] = new Filter(
			new ScreenShaderData(ModAssets.ShaderEffects["Posterize"], "One"),
			EffectPriority.Medium);
		Filters.Scene["Posterize"].GetShader().Shader.Parameters["uPosterizeLevels"].SetValue(16f);
		Filters.Scene["Posterize"].Load();
	}
	public override void Unload()
	{
		if (Main.dedServ)
		{
			return;
		}
		if (Filters.Scene["Posterize"] != null && Filters.Scene["Posterize"].IsActive())
		{
			Filters.Scene["Posterize"].Deactivate();
		}
	}
	public static void Activate(string filterName)
	{
		if (!Filters.Scene[filterName].IsActive())
		{
			Filters.Scene.Activate(filterName);
		}
	}
	public static void Deactivate(string filterName)
	{
		if (Filters.Scene[filterName].IsActive())
		{
			Filters.Scene.Deactivate(filterName);
		}
	}
}