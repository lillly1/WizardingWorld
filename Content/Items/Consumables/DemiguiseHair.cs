using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Demiguise Hair — dropped by the Demiguise.
	/// Used to weave invisibility items. Can also be consumed for 30 seconds of invisibility.
	/// "Demiguise pelts are highly sought after as they can be woven into Invisibility Cloaks."
	/// </summary>
	public class DemiguiseHair : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 1);
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.consumable = true;
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.AddBuff(BuffID.Invisibility, 1800); // 30 seconds invisible
			}
			return true;
		}
	}
}
