using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Basilisk
{
	/// <summary>
	/// Basilisk Eye — Expert-exclusive accessory from the Basilisk boss bag.
	/// Grants immunity to Petrified debuff and Stone status.
	/// "Do not look directly into its eyes."
	/// </summary>
	public class BasiliskEye : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 5);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.buffImmune[ModContent.BuffType<Buffs.Debuffs.PetrifiedDebuff>()] = true;
			player.buffImmune[BuffID.Stoned] = true;
			player.GetDamage(ModContent.GetInstance<DamageClasses.SpellDamage>()) += 0.08f;
		}
	}
}
