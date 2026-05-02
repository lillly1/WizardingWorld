using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Master Wizard's Banner — the ultimate "you beat the mod" trophy.
	/// Crafted from all 3 boss trophies + Triwizard Cup + Unicorn Blood.
	/// Grants a permanent zone buff while equipped:
	/// +10% all damage, +5% spell damage, +8 defense, +20 max mana, +2 life regen.
	/// Also emits a permanent golden aura around the player.
	/// "The greatest witch or wizard of their age."
	/// </summary>
	public class MasterWizardBanner : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.rare = ItemRarityID.Purple;
			Item.value = Item.sellPrice(platinum: 2);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// The ultimate wizard buff
			player.GetDamage(DamageClass.Generic) += 0.10f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.05f;
			player.statDefense += 8;
			player.statManaMax2 += 20;
			player.lifeRegen += 2;
			player.moveSpeed += 0.05f;

			// Golden master wizard aura
			if (!hideVisual && Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GoldCoin, 0f, -1f, 50, default, 0.7f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}

			Terraria.Lighting.AddLight(player.Center, 0.4f, 0.35f, 0.2f);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<BossLoot.Basilisk.BasiliskTrophy>(), 1)
				.AddIngredient(ModContent.ItemType<BossLoot.Horntail.HorntailTrophy>(), 1)
				.AddIngredient(ModContent.ItemType<BossLoot.Voldemort.VoldemortTrophy>(), 1)
				.AddIngredient(ModContent.ItemType<ChampionsMedallion>(), 1)
				.AddIngredient(ModContent.ItemType<Consumables.UnicornBlood>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
