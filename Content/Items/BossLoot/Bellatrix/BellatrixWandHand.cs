using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.BossLoot.Bellatrix
{
	/// <summary>
	/// Bellatrix's Wand Hand — Expert-exclusive accessory from the Bellatrix boss bag.
	/// +20% spell damage, -15% mana cost, but incoming damage increased by 8%.
	/// "She was a true believer."
	/// </summary>
	public class BellatrixWandHand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 15);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// +20% spell damage
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.20f;

			// -15% mana cost
			player.manaCost -= 0.15f;

			// Incoming damage increased by 8% — handled via endurance (negative endurance)
			player.endurance -= 0.08f;

			// Dark magical aura visual
			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.PurpleTorch, 0f, 0f, 150, default, 0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}
	}
}
