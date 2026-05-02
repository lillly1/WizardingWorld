using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables.Potions
{
	/// <summary>
	/// Mandrake Restorative Draught — cures petrification and all debilitating debuffs.
	/// The canonical cure for Basilisk petrification in the HP universe.
	/// Also restores 50 HP and clears Petrified, Jinxed, Dark Curse, Confused, Frozen.
	/// "Professor Sprout has a very healthy crop of Mandrakes. When matured, their cry will revive anyone petrified."
	/// </summary>
	public class MandrakeRestorative : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(silver: 30);
			Item.UseSound = SoundID.Item3;
			Item.healLife = 50;
			Item.potion = true;
		}

		public override void OnConsumeItem(Player player)
		{
			// Cure petrification and all major wizard debuffs
			player.ClearBuff(ModContent.BuffType<Buffs.Debuffs.PetrifiedDebuff>());
			player.ClearBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>());
			player.ClearBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>());
			player.ClearBuff(BuffID.Confused);
			player.ClearBuff(BuffID.Frozen);
			player.ClearBuff(BuffID.Stoned);
			player.ClearBuff(BuffID.Slow);
			player.ClearBuff(BuffID.Weak);

			// Green restoration particles
			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.JungleSpore, 0f, -1f, 50, default, 1.0f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.BottledWater, 3)
				.AddIngredient(ItemID.JungleSpores, 5)
				.AddIngredient(ItemID.Daybloom, 2)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
