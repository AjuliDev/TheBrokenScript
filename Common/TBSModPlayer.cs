using Terraria;
using Terraria.ModLoader;
using TheBrokenScript.Content.NPCs.SiluetR2;
using TheBrokenScript.Core;
namespace TheBrokenScript.Common;
public class TBSModPlayer : ModPlayer
{
	public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
	{
		base.ModifyHitByNPC(npc, ref modifiers);
		if (npc.type == ModContent.NPCType<SiluetR2>())
		{
			ModSounds.PlaySound("SiluetAttackSimple", 0.4f, (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, 0.4f);
		}
	}
}
