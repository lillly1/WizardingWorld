using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Diadem of Ravenclaw — Horcrux. Massive mana boost and spell power, but reduced max life.</summary>
	public class DiademOfRavenclaw : ModItem
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
			player.statManaMax2 += 80;
			player.manaRegen += 8;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.15f;
			player.manaCost -= 0.15f;

			// Dark cost: reduced max life
			player.statLifeMax2 -= 40;

			// HORCRUX CORRUPTION — the diadem clouds judgment
			if (Main.GameUpdateCount % 300 == 0)
			{
				var darkPlayer = player.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.005f, "Horcrux influence");
			}
			// Occasional mana drain — the diadem feeds on magical energy
			if (Main.GameUpdateCount % 180 == 0 && player.statMana > 10)
				player.statMana -= 5;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SilverBar, 10)
				.AddIngredient(ItemID.Sapphire, 5)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.ManaCrystal, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
