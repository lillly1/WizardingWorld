using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Hufflepuff's Cup — Horcrux. Grants life regen and potion effectiveness, but darkness.</summary>
	public class HufflepuffsCup : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 8);
			Item.consumable = false;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			var system = ModContent.GetInstance<Common.Systems.HorcruxHuntSystem>();
			if (!system.AttemptDestroyHorcrux(Type, player))
			{
				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HorcruxHunt.NeedTool"),
					Microsoft.Xna.Framework.Color.Red);
			}
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.lifeRegen += 8;
			player.statLifeMax2 += 60;
			player.pStone = true; // Reduced potion sickness duration

			// Dark cost: reduced vision
			player.AddBuff(BuffID.Darkness, 2);

			// HORCRUX CORRUPTION — the cup drains vitality
			if (Main.GameUpdateCount % 300 == 0)
			{
				var darkPlayer = player.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.005f, "Horcrux influence");
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ItemID.Topaz, 3)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.LifeCrystal, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
