using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs.Debuffs
{
	/// <summary>
	/// Petrified — the Basilisk's signature debuff.
	/// Player is frozen in place, cannot move or attack, takes 3 damage/sec.
	/// Much scarier than vanilla Confused. Short duration but devastating.
	/// </summary>
	public class PetrifiedDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// Frozen in place — cannot move
			player.velocity = Vector2.Zero;
			player.noKnockback = true;
			player.AddBuff(BuffID.Stoned, 2); // Uses vanilla Stoned buff for full immobility

			// Stone grey overlay effect
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Stone, 0f, 0f, 150, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			// Enemy version: frozen, takes damage over time
			npc.velocity = Vector2.Zero;

			// Stone dust
			if (Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Stone, 0f, 0f, 150, default, 0.8f);
				dust.noGravity = true;
			}
		}
	}
}
