using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Magical Koi — a glowing fish caught while fishing.
	/// Can be eaten for a temporary luck + mana regen buff,
	/// or used as a potion ingredient for Felix Felicis.
	/// </summary>
	public class MagicalKoi : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 30);
			Item.UseSound = SoundID.Item2;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.AddBuff(BuffID.Lucky, 7200); // 2 min luck
				player.AddBuff(BuffID.ManaRegeneration, 7200);
			}
			return true;
		}
	}
}
