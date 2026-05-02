using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Spider Silk Cloak — crafted from Aragog's Spider Silk Weave.
	/// Lightweight invisibility-adjacent cloak: -300 aggro, +10% speed, +8% spell damage.
	/// Represents the cunning of surviving the Acromantula colony.
	/// </summary>
	public class SpiderSilkCloak : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.aggro -= 300;
			player.moveSpeed += 0.10f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.08f;

			if (!hideVisual && Main.rand.NextBool(20))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Cloud, 0f, 0f, 180, default, 0.3f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.SpiderSilkWeave>(), 10)
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
