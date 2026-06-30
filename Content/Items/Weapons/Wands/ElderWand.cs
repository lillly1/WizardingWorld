using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Elder Wand — the Deathstick. Drops from Voldemort. Casts Avada Kedavra.</summary>
	public class ElderWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 150;
			Item.knockBack = 6f;
			Item.mana = 40;
			Item.crit = 15;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<AvadaKedavraProjectile>();
			Item.shootSpeed = 18f;
			Item.value = Item.buyPrice(platinum: 1);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = WizardSoundStyles.AvadaKedavra;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();
			int masteryLevel = wp.GetWandMasteryLevel(Type);

			// 20% chance to also fire a Stupefy for crowd control
			int stupefyChance = masteryLevel >= 3 ? 4 : 5;
			if (Main.rand.NextBool(stupefyChance))
			{
				Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(15)),
					ModContent.ProjectileType<StupefyProjectile>(), damage / 2, knockback, player.whoAmI);
			}

			return true;
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
		{
			// Elder Wand scales slightly with missing mana (more desperate = more powerful)
			float manaRatio = 1f - (float)player.statMana / player.statManaMax2;
			damage += manaRatio * 0.2f;
		}

		public override void UpdateInventory(Player player)
		{
			player.GetModPlayer<Common.Players.WizardPlayer>().hasElderWand = true;
		}
	}
}
