using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Dragon-Tooth Necklace — crafted from Dragon Scale (Horntail) + Cerberus Fang (Fluffy).
	/// Cross-boss reward: requires beating two different bosses.
	/// +12% all damage, +8 armor penetration, fire trail while moving.
	/// Charlie Weasley would approve.
	/// </summary>
	public class DragonToothNecklace : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 12);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += 0.12f;
			player.GetArmorPenetration(DamageClass.Generic) += 8;

			// Fire trail when moving
			if (!hideVisual && player.velocity.Length() > 3f && Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(player.Bottom, player.width, 4, DustID.Torch, 0f, 0f, 100, default, 0.7f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.CerberusFang>(), 5)
				.AddIngredient(ItemID.GoldBar, 8)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
