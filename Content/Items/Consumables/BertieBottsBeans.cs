using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Bertie Bott's Every Flavour Beans — random buff OR debuff. A gamble!</summary>
	public class BertieBottsBeans : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 5);
			Item.UseSound = SoundID.Item2;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			int roll = Main.rand.Next(20);

			switch (roll)
			{
				// GOOD flavours (60% chance)
				case 0: // Toffee
					player.AddBuff(BuffID.Ironskin, 3600);
					break;
				case 1: // Chocolate
					player.AddBuff(BuffID.WellFed3, 3600);
					break;
				case 2: // Peppermint
					player.AddBuff(BuffID.Swiftness, 3600);
					break;
				case 3: // Cherry
					player.AddBuff(BuffID.Regeneration, 3600);
					break;
				case 4: // Blueberry
					player.AddBuff(BuffID.ManaRegeneration, 3600);
					break;
				case 5: // Cinnamon
					player.AddBuff(BuffID.Rage, 1800);
					break;
				case 6: // Honey
					player.AddBuff(BuffID.Honey, 3600);
					break;
				case 7: // Grape
					player.AddBuff(BuffID.Wrath, 1800);
					break;
				case 8: // Strawberry
					player.AddBuff(BuffID.Endurance, 1800);
					break;
				case 9: // Vanilla
					player.AddBuff(BuffID.MagicPower, 3600);
					break;
				case 10: // Banana
					player.AddBuff(BuffID.Lifeforce, 1800);
					break;
				case 11: // Coconut
					player.AddBuff(BuffID.Lucky, 3600);
					break;

				// BAD flavours (40% chance)
				case 12: // Earwax
					player.AddBuff(BuffID.Confused, 300);
					break;
				case 13: // Vomit
					player.AddBuff(BuffID.Stinky, 600);
					break;
				case 14: // Pepper (too spicy!)
					player.AddBuff(BuffID.OnFire, 300);
					break;
				case 15: // Dirt
					player.AddBuff(BuffID.Slow, 600);
					break;
				case 16: // Soap
					player.AddBuff(BuffID.Weak, 600);
					break;
				case 17: // Rotten Egg
					player.AddBuff(BuffID.Stinky, 600);
					break;
				case 18: // Booger
					player.AddBuff(BuffID.Darkness, 600);
					break;
				case 19: // Ghost Pepper
					player.AddBuff(BuffID.OnFire3, 180);
					break;
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(10)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.FallenStar, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
