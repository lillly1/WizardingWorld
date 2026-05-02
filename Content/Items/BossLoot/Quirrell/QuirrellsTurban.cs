using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.BossLoot.Quirrell
{
	/// <summary>
	/// Quirrell's Turban -- Expert-exclusive accessory from the Quirrell boss bag.
	/// +5% spell damage, reveals nearby enemies (detectCreature), but player takes +3% more damage (Voldemort's influence).
	/// "Something stirs within the folds of this turban..."
	/// </summary>
	public class QuirrellsTurban : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// +5% spell damage
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.05f;

			// Reveals nearby enemies
			player.detectCreature = true;

			// Voldemort's influence: incoming damage increased by 3%
			player.endurance -= 0.03f;

			// Faint dark aura visual
			if (Main.rand.NextBool(15))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.PurpleTorch, 0f, 0f, 180, default, 0.4f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}
	}
}
