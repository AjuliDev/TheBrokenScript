using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
namespace TheBrokenScript.Core;
public class ModSounds : ModSystem
{
	public static HashSet<string> ActiveSounds = new HashSet<string>();

	public static void PlayCaveNoise(int i = 0, int j = 0) // To be played locally via a packet.
	{
		Point16 caveNoiseEntries = new Point16(1, 17); // min index, max index
		int randEntry = Main.rand.Next(caveNoiseEntries.X, caveNoiseEntries.Y + 1);
		if (ActiveSounds.Contains($"CaveNoise{randEntry}"))
		{
			return;
		}
		else
		{
			ActiveSounds.Add($"CaveNoise{randEntry}");
		}
		if (i == 0 && j == 0)
		{
			ModAssets.Sounds[$"CaveNoise{randEntry}"].Value.Play(volume: 1f, pitch: 0f, pan: 0f);
		}
		else
		{
			if (i > (Main.LocalPlayer.position.X / 16) + 20)
			{
				ModAssets.Sounds[$"CaveNoise{randEntry}"].Value.Play(volume: 1f, pitch: 0f, pan: 0.5f);
			}
			else if (i < (Main.LocalPlayer.position.X / 16) - 20)
			{
				ModAssets.Sounds[$"CaveNoise{randEntry}"].Value.Play(volume: 1f, pitch: 0f, pan: -0.5f);
			}
			else
			{
				ModAssets.Sounds[$"CaveNoise{randEntry}"].Value.Play(volume: 1f, pitch: 0f, pan: 0f);
			}
		}
		Task.Delay(10000).ContinueWith(task => ActiveSounds.Remove($"CaveNoise{randEntry}"));
	}

	public static void PlaySound(string sound, float soundTimerSeconds = 1f, int i = 0, int j = 0, float pitchVariance = 0f)
	{
		if (ActiveSounds.Contains(sound))
		{
			return;
		}
		else
		{
			ActiveSounds.Add(sound);
		}
		float newPitch = Main.rand.NextFloat(0, 1) * pitchVariance;
		newPitch = Math.Clamp(newPitch, 0f, 1f);
		if (i == 0 && j == 0)
		{
			ModAssets.Sounds[sound].Value.Play(volume: 1f, newPitch, pan: 0f);
		}
		else
		{
			if (i > (Main.LocalPlayer.position.X / 16) + 20)
			{
				ModAssets.Sounds[sound].Value.Play(volume: 1f, newPitch, pan: 0.5f);
			}
			else if (i < (Main.LocalPlayer.position.X / 16) - 20)
			{
				ModAssets.Sounds[sound].Value.Play(volume: 1f, newPitch, pan: -0.5f);
			}
			else
			{
				ModAssets.Sounds[sound].Value.Play(volume: 1f, newPitch, pan: 0f);
			}
		}
		Task.Delay((int)(1000f * soundTimerSeconds)).ContinueWith(task => ActiveSounds.Remove(sound));
	}
}
