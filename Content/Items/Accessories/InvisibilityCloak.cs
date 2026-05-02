using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>True Invisibility Cloak — Deathly Hallow heirloom of the Peverell line.
	/// Obtained through the Hallows Attunement questline, not crafting.</summary>
	public class InvisibilityCloak : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(gold: 20);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.invis = true;
			player.aggro -= 400;
			player.GetDamage(DamageClass.Generic) += 0.05f; // Stealth bonus damage

			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			wp.hasInvisibilityCloak = true;
		}
	}
}
