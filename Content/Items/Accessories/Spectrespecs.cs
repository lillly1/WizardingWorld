using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Spectrespecs — Luna Lovegood's signature rainbow glasses.
	/// Reveals hidden enemies (Hunter effect), grants spell crit bonus,
	/// and highlights all enemies with danger sense.
	/// "They make Wrackspurts visible, you know."
	/// Vanity + functional hybrid.
	/// </summary>
	[AutoloadEquip(EquipType.Face)]
	public class Spectrespecs : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 14;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(gold: 4);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.detectCreature = true;
			player.dangerSense = true;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 5;

			// Rainbow tint visual — the Spectrespecs glow
			if (!hideVisual && Main.rand.NextBool(20))
			{
				int[] colors = { DustID.PinkTorch, DustID.BlueTorch, DustID.YellowStarDust, DustID.PurpleTorch };
				Dust dust = Dust.NewDustDirect(
					player.position + new Vector2(6, 6), 8, 4,
					colors[Main.rand.Next(colors.Length)], 0f, 0f, 100, default, 0.3f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Glass, 5)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddIngredient(ItemID.Lens, 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
