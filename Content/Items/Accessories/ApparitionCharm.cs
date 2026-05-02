using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Apparition Charm — teleport to cursor position on double-tap down (like Rod of Discord).</summary>
	public class ApparitionCharm : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.sellPrice(gold: 15);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Grant Rod of Discord-style teleportation
			// This is done through the ModPlayer hook instead
			player.GetModPlayer<Common.Players.WizardPlayer>().spellPowerBonus += 0.05f;

			// Faster movement — apparition readiness
			player.moveSpeed += 0.08f;
			player.maxRunSpeed += 1f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.RodofDiscord, 1)
				.AddIngredient(ItemID.SoulofLight, 15)
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
