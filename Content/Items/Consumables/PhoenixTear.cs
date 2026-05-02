using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Phoenix Tear — the rarest healing item in the wizarding world.
	/// Instantly restores ALL health and clears ALL debuffs.
	/// "Phoenix tears have immense healing powers — the only known antidote to Basilisk venom."
	/// Extremely rare craft from Essence + Unicorn Blood + Life Crystals.
	/// Does NOT trigger potion sickness.
	/// </summary>
	public class PhoenixTear : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.maxStack = 5; // Very limited stacking — it's precious
			Item.consumable = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			// Full heal
			player.statLife = player.statLifeMax2;
			player.HealEffect(player.statLifeMax2);

			// Full mana restore
			player.statMana = player.statManaMax2;
			player.ManaEffect(player.statManaMax2);

			// Clear ALL debuffs
			for (int i = 0; i < Player.MaxBuffs; i++)
			{
				if (player.buffType[i] > 0 && Main.debuff[player.buffType[i]])
				{
					player.DelBuff(i);
					i--;
				}
			}

			// Golden phoenix healing visual
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GoldCoin, 0f, -2f, 50, default, 1.5f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(3f, 4f);
				dust.velocity.Y -= 2f;
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<UnicornBlood>(), 2)
				.AddIngredient(ItemID.LifeCrystal, 1)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
