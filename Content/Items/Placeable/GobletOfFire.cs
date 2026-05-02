using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Placeable
{
	/// <summary>
	/// Goblet of Fire — legendary furniture piece.
	/// When used (reusable), grants Battle buff + Wrath to all nearby players.
	/// "The Goblet of Fire selects the champions — and empowers them."
	/// Crafted from endgame materials.
	/// </summary>
	public class GobletOfFire : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.maxStack = 1;
			Item.consumable = false;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 30);
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				// Triwizard Tournament logic
				if (Common.Systems.TriwizardTournamentSystem.CanUnlock())
				{
					Common.Systems.TriwizardTournamentSystem.UnlockTournament();
					return true;
				}

				if (Common.Systems.TriwizardTournamentSystem.CanStartNextTask())
				{
					Common.Systems.TriwizardTournamentSystem.StartTask(player);
					return true;
				}

				if (Common.Systems.TriwizardTournamentSystem.tournamentUnlocked)
				{
					Main.NewText(Common.Systems.TriwizardTournamentSystem.GetStatusText(), new Color(100, 150, 255));
					// Fall through to normal buff behavior
				}

				// Empower all nearby players
				foreach (var p in Main.ActivePlayers)
				{
					if (Vector2.Distance(p.Center, player.Center) < 800f)
					{
						p.AddBuff(BuffID.Battle, 18000);
						p.AddBuff(BuffID.Wrath, 18000);
						p.AddBuff(BuffID.Rage, 18000);
					}
				}

				// Blue flame burst
				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(player.Center + new Vector2(0, 10), 16, 8, DustID.BlueTorch, 0f, -3f, 50, default, 1.5f);
					dust.noGravity = true;
				}

				Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Items.GobletOfFire.UseMessage"), 100, 150, 255);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 15)
				.AddIngredient(ModContent.ItemType<Consumables.GoldenEgg>(), 1)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
				.AddIngredient(ModContent.ItemType<Consumables.UnicornBlood>(), 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
