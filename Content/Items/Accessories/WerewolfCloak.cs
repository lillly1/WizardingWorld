using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Werewolf Cloak — crafted from Fenrir's Werewolf Pelt.
	/// At night: +20% melee damage, +15% speed, +10 defense.
	/// During day: only +5% damage, +5% speed.
	/// The beast within empowers you under the moon.
	/// </summary>
	public class WerewolfCloak : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (!Main.dayTime)
			{
				player.GetDamage(DamageClass.Melee) += 0.20f;
				player.moveSpeed += 0.15f;
				player.statDefense += 10;

				if (!hideVisual && Main.rand.NextBool(15))
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Smoke, 0f, 0f, 150, default, 0.4f);
					dust.noGravity = true;
				}
			}
			else
			{
				player.GetDamage(DamageClass.Melee) += 0.05f;
				player.moveSpeed += 0.05f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.WerewolfPelt>(), 8)
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
