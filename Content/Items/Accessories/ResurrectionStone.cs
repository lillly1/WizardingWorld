using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Resurrection Stone — Deathly Hallow extracted from the purified Gaunt's Ring.
	/// Obtained by destroying all Horcruxes and purifying the cursed ring.
	/// Canon-faithful: The Stone was set into Marvolo Gaunt's ring.
	/// </summary>
	public class ResurrectionStone : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(gold: 20);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.lifeRegen += 6;
			player.statLifeMax2 += 40;
			player.GetModPlayer<Common.Players.WizardPlayer>().hasResurrectionStone = true;
		}
	}
}
