using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	public class QATestWand : ModItem
	{
		public override string Texture => "WizardingWorld/Content/Items/Weapons/Wands/ElderWand";

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 600;
			Item.knockBack = 8f;
			Item.mana = 0;
			Item.crit = 100;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<StupefyProjectile>();
			Item.shootSpeed = 18f;
			Item.value = 0;
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = WizardSoundStyles.Stupefy;
		}
	}
}
