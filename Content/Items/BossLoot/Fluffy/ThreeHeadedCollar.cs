using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Fluffy
{
	/// <summary>
	/// Three-Headed Collar — Expert-exclusive accessory from the Fluffy boss bag.
	/// +3 max minions but -10% movement speed.
	/// "Even Fluffy can be tamed... sort of."
	/// </summary>
	public class ThreeHeadedCollar : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.maxMinions += 3;
			player.moveSpeed -= 0.10f;

			// Subtle dust effect — three small paw-print dust trails
			if (player.velocity.Length() > 2f && Main.rand.NextBool(6))
			{
				Dust dust = Dust.NewDustDirect(player.Bottom, player.width, 4, DustID.Blood, 0f, 0f, 100, default, 0.6f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}
	}
}
