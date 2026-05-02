using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Puking Pastille — Weasleys' Wizard Wheezes.
	/// Eat one to instantly purge ALL debuffs, but lose 30 HP.
	/// Emergency debuff clear when you don't have Finite Incantatem.
	/// Cheaper and faster than the Larch Wand but costs health.
	/// </summary>
	public class PukingPastille : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 5; // Very fast — emergency use
			Item.useAnimation = 5;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 15);
			Item.UseSound = SoundID.Item3;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			// Purge ALL debuffs
			for (int i = 0; i < Player.MaxBuffs; i++)
			{
				if (player.buffType[i] > 0 && Main.debuff[player.buffType[i]])
				{
					player.DelBuff(i);
					i--;
				}
			}

			// Cost: lose 30 HP
			player.statLife -= 30;
			if (player.statLife < 1)
				player.statLife = 1;

			// Green sick visual
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GreenTorch, Main.rand.NextFloat(-2f, 2f), -2f, 100, default, 1.2f);
				dust.noGravity = true;
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.VileMushroom, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			CreateRecipe(5)
				.AddIngredient(ItemID.Mushroom, 2)
				.AddIngredient(ItemID.ViciousMushroom, 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
