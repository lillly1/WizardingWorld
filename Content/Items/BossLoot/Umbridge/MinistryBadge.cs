using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.Buffs.Debuffs;

namespace WizardingWorld.Content.Items.BossLoot.Umbridge
{
	/// <summary>
	/// Ministry Badge -- Expert-exclusive accessory from the Umbridge boss bag.
	/// Immune to Jinxed and Confused debuffs, +5 defense, but -5% damage (bureaucracy dampens your spirit).
	/// "By order of the Ministry of Magic."
	/// </summary>
	public class MinistryBadge : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Immune to Jinxed and Confused
			player.buffImmune[ModContent.BuffType<JinxedDebuff>()] = true;
			player.buffImmune[BuffID.Confused] = true;

			// +5 defense
			player.statDefense += 5;

			// Bureaucracy dampens your spirit: -5% damage
			player.GetDamage(DamageClass.Generic) -= 0.05f;
		}
	}
}
