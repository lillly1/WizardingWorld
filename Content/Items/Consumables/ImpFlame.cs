using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Imp Flame — a tiny living ember dropped by Magical Imps.
	/// Crafting ingredient for fire-themed potions and items.
	/// Can also be consumed for 30 seconds of Inferno aura.
	/// </summary>
	public class ImpFlame : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 14;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 5);
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.consumable = true;
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
				player.AddBuff(BuffID.Inferno, 1800);
			return true;
		}
	}
}
