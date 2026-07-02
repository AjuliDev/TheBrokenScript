using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using thebrokenscript.Content.NPCs.Follow;
using thebrokenscript.Content.NPCs.SiluetR2;
using thebrokenscript.Content.Subworlds.Nowhere;
using thebrokenscript.Core;
namespace thebrokenscript.Common;
public class TBSModPlayer : ModPlayer
{
	public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
	{
		base.ModifyHitByNPC(npc, ref modifiers);
		if (npc.type == ModContent.NPCType<SiluetR2>())
		{
			ModSounds.PlaySound("SiluetAttackSimple", 0.4f, (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, 0.4f);
		} else if (npc.type == ModContent.NPCType<Follow>())
		{
			if (SubworldCounter.NowhereEntryCooldown > 0) // Quick Patch to prevent crashes when multiple people attempt to join a starting server at the same time.
			{
				return;
			}
			else
			{
				SubworldSystem.Enter<Nowhere>();
			}
		}
	}
}
