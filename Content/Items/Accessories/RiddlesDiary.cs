using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Riddle's Diary — Horcrux accessory. Grants spell damage but slowly drains life.</summary>
	public class RiddlesDiary : ModItem
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
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.18f;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 8;

			// HORCRUX CORRUPTION — Canon Tier A.
			// "The locket made Ron violent, Harry reckless, Hermione exhausted."
			// Drains HP, adds corruption, triggers paranoia
			if (Main.GameUpdateCount % 60 == 0 && player.statLife > 1)
				player.statLife -= 2; // 2 HP per second (doubled from 1 per 2s)

			// Add dark corruption every 5 seconds
			if (Main.GameUpdateCount % 300 == 0)
			{
				var darkPlayer = player.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.005f, "Horcrux influence");
			}

			// Random paranoia — the diary whispers
			if (Main.rand.NextBool(1800)) // ~Every 30 seconds
				player.AddBuff(Terraria.ID.BuffID.Confused, 60);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Book, 1)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.DarkShard, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
