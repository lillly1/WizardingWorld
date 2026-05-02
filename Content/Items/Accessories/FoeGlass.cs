using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Foe-Glass — a dark detector mirror that shows your enemies.
	/// Reveals all enemies on minimap + shows boss HP percentage in tooltip.
	/// "The shadowy figures in the glass grow clearer as your enemies approach."
	/// Moody's favorite Dark detector.
	/// </summary>
	public class FoeGlass : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 6);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.detectCreature = true;
			player.dangerSense = true;
			player.nightVision = true;
			player.GetDamage(DamageClass.Generic) += 0.03f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Show nearby boss HP
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.boss && npc.active)
				{
					float hpPercent = (float)npc.life / npc.lifeMax * 100f;
					tooltips.Add(new TooltipLine(Mod, "BossHP",
						$"[c/FF5555:{npc.FullName}: {hpPercent:F0}% HP remaining]"));
				}
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Glass, 10)
				.AddIngredient(ItemID.SilverBar, 5)
				.AddIngredient(ItemID.Lens, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 8)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
