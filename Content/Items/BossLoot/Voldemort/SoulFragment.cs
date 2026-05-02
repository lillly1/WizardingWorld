using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.BossLoot.Voldemort
{
	/// <summary>
	/// Fragment of Voldemort's Soul — Expert-exclusive accessory.
	/// The most powerful accessory in the mod. Grants:
	/// +20% all damage, +15% spell damage, +10 defense, life steal on spell hits.
	/// But corrupts: -20 max HP, Darkness visual.
	/// "He split his soul seven times... but this fragment was the last."
	/// </summary>
	public class SoulFragment : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 25);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Immense power
			player.GetDamage(DamageClass.Generic) += 0.20f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.15f;
			player.statDefense += 10;

			// Life steal on spell damage — 5% of damage dealt
			// (Handled via OnHitNPC in a separate GlobalProjectile or the ModPlayer)

			// Corruption cost
			player.statLifeMax2 -= 20;

			// Dark aura visual
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Shadowflame, 0f, 0f, 150, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}
	}
}
