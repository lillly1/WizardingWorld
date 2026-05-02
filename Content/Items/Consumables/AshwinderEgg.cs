using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Ashwinder Egg — intensely hot egg dropped by the Ashwinder serpent.
	/// Key ingredient in Love Potions (Amortentia) and Felix Felicis.
	/// Can also be consumed to grant fire immunity for 2 minutes.
	/// Must be frozen before the egg ignites and burns down your house.
	/// </summary>
	public class AshwinderEgg : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(silver: 50);
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.consumable = true;
			Item.UseSound = SoundID.Item2;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				player.AddBuff(BuffID.ObsidianSkin, 7200); // 2 min fire/lava immunity
				player.AddBuff(BuffID.Inferno, 3600); // 1 min inferno aura
			}
			return true;
		}
	}
}
