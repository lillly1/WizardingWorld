using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs.Debuffs
{
	/// <summary>
	/// Dark Curse — applied by Voldemort, Death Eaters, and Unforgivable Curses.
	/// Damage over time (8 HP/sec), blocks healing, reduces defense by 10.
	/// The nastiest debuff in the mod.
	/// </summary>
	public class DarkCurseDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true; // Nurse can't cure dark magic
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// Dark damage over time
			player.lifeRegen -= 16; // ~8 HP/sec damage

			// Block healing
			player.moonLeech = true; // Prevents life regen from non-natural sources

			// Reduced defense
			player.statDefense -= 10;

			// Dark curse visual — green-black particles
			if (Main.rand.NextBool(4))
			{
				int dustType = Main.rand.NextBool() ? DustID.CursedTorch : DustID.Shadowflame;
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, dustType, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			// Enemy version: damage over time, weakened
			npc.lifeRegen -= 16;
			npc.defense -= 10;

			if (Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.CursedTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}
	}
}
