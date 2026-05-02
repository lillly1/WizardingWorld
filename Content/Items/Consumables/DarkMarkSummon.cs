using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Dark Mark (Morsmordre) — manually triggers the Death Eater Invasion event.
	/// Casts the Dark Mark into the sky, summoning waves of Death Eaters.
	/// Only usable at night during Hardmode.
	/// </summary>
	public class DarkMarkSummon : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.maxStack = 20;
			Item.consumable = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item119;
		}

		public override bool CanUseItem(Player player)
		{
			// Only at night, in Hardmode, when no invasion is active
			return !Main.dayTime && Main.hardMode && !Common.Systems.DeathEaterInvasion.invasionActive;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				Common.Systems.DeathEaterInvasion.StartInvasion();

				// Green skull flash in the sky
				for (int i = 0; i < 60; i++)
				{
					Dust dust = Dust.NewDustDirect(
						player.Center + new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-400, -200)),
						8, 8, DustID.CursedTorch, 0f, -2f, 50, default, 2f);
					dust.noGravity = true;
				}

				SoundEngine.PlaySound(SoundID.Roar, player.Center);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 20)
				.AddIngredient(ItemID.SoulofNight, 8)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
