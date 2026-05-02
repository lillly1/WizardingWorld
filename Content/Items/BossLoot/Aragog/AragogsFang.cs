using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Aragog
{
	/// <summary>
	/// Aragog's Fang — Expert-exclusive accessory from the Aragog boss bag.
	/// Grants +15% critical strike chance and enemies killed have a 10% chance to drop extra loot.
	/// "The fang drips with ancient venom and an insatiable hunger for treasure."
	/// </summary>
	public class AragogsFang : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// +15% critical strike chance across all damage types
			player.GetCritChance(DamageClass.Generic) += 15f;

			// Extra loot chance is handled by the AragogsFangPlayer class below
			player.GetModPlayer<AragogsFangPlayer>().aragogsFangEquipped = true;
		}
	}

	/// <summary>
	/// ModPlayer that handles the 10% extra loot drop from Aragog's Fang.
	/// When an enemy is killed, there is a 10% chance it drops extra coins.
	/// </summary>
	public class AragogsFangPlayer : ModPlayer
	{
		public bool aragogsFangEquipped;

		public override void ResetEffects()
		{
			aragogsFangEquipped = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (aragogsFangEquipped && target.life <= 0 && Main.rand.NextBool(10))
			{
				// Drop extra loot — bonus gold coins mimicking spider hoarding behavior
				int goldAmount = Main.rand.Next(1, 4);
				Item.NewItem(target.GetSource_Loot(), target.getRect(), ItemID.GoldCoin, goldAmount);

				// Also chance for bonus spider fang
				if (Main.rand.NextBool(3))
				{
					Item.NewItem(target.GetSource_Loot(), target.getRect(), ItemID.SpiderFang, Main.rand.Next(1, 3));
				}
			}
		}
	}
}
