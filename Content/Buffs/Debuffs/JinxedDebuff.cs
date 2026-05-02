using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs.Debuffs
{
	/// <summary>
	/// Jinxed — a hex that weakens the target.
	/// -20% damage, -15% movement speed, -5 defense.
	/// Applied by Impedimenta, Boggart, Pixie attacks.
	/// </summary>
	public class JinxedDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) -= 0.20f;
			player.moveSpeed -= 0.15f;
			player.statDefense -= 5;

			// Purple jinx particles
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.PurpleTorch, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			// Enemies: reduced damage and speed
			npc.damage = (int)(npc.defDamage * 0.8f);
			npc.velocity *= 0.85f;

			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.PurpleTorch, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
			}
		}
	}
}
