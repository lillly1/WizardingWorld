using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>
	/// Legacy Wand of Destiny stub.
	/// Preserved only so old worlds migrate cleanly into the Elder Wand mastery path.
	/// </summary>
	[Obsolete("Merged into Elder Wand mastery")]
	public class WandOfDestiny : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 44;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.Purple;
			Item.value = 0;
		}

		public override bool CanUseItem(Player player) => false;

		public override void UpdateInventory(Player player)
		{
			Item.SetDefaults(ModContent.ItemType<ElderWand>());
		}
	}
}
