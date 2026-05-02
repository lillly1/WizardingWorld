using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Troll
{
	/// <summary>
	/// Troll Hide — Expert-exclusive accessory from the Troll boss bag.
	/// Grants +8 defense and +10% knockback resistance.
	/// "Thick as its owner, and twice as tough."
	/// </summary>
	public class TrollHide : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense += 8;
			player.GetModPlayer<TrollHidePlayer>().trollHideEquipped = true;
		}
	}

	public class TrollHidePlayer : ModPlayer
	{
		public bool trollHideEquipped;

		public override void ResetEffects()
		{
			trollHideEquipped = false;
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (trollHideEquipped)
            modifiers.KnockbackImmunityEffectiveness *= 1.1f;
		}
	}
}
