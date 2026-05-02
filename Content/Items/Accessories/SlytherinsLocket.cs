using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>Slytherin's Locket — Horcrux. Grants poison immunity and increased defense, but reduces speed.</summary>
	public class SlytherinsLocket : ModItem
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
			player.statDefense += 12;
			player.buffImmune[BuffID.Poisoned] = true;
			player.buffImmune[BuffID.Venom] = true;
			player.endurance += 0.08f; // 8% damage reduction

			// Dark cost: slightly slower
			player.moveSpeed -= 0.08f;

			// HORCRUX CORRUPTION — the locket breeds distrust and aggression
			if (Main.GameUpdateCount % 300 == 0)
			{
				var darkPlayer = player.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
				darkPlayer.AddCorruption(0.005f, "Horcrux influence");
			}
			// The locket whispers — occasional aggression
			if (Main.rand.NextBool(2400))
				player.AddBuff(Terraria.ID.BuffID.Confused, 90);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ItemID.Emerald, 3)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
