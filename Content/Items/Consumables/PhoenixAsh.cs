using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Phoenix Ash — the remains of a phoenix after Burning Day.
	/// Crafting material for resurrection/healing items.
	/// Can also be consumed to grant 30 seconds of fire immunity + regen.
	/// </summary>
	public class PhoenixAsh : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(silver: 50);
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
				player.AddBuff(BuffID.ObsidianSkin, 1800);
				player.AddBuff(BuffID.Regeneration, 1800);
			}
			return true;
		}
	}
}
