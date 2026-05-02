using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Umbridge's Quill -- Spell weapon dropped by Dolores Umbridge.
	/// 25 damage, fast fire rate, shoots pink projectiles that apply Bleeding + Jinxed.
	/// "I must not tell lies."
	/// </summary>
	public class UmbridgesQuill : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 25;
			Item.knockBack = 2f;
			Item.mana = 6;
			Item.noMelee = true;
			Item.shoot = ProjectileID.PinkLaser;
			Item.shootSpeed = 14f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item12;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int proj = Projectile.NewProjectile(source, position, velocity, ProjectileID.PinkLaser, damage, knockback, player.whoAmI);

			if (proj >= 0 && proj < Main.maxProjectiles)
			{
				Main.projectile[proj].friendly = true;
				Main.projectile[proj].hostile = false;
			}

			return false;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Apply Bleeding and Jinxed on hit
			target.AddBuff(BuffID.Bleeding, 180); // 3 seconds
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 180); // 3 seconds
		}

		public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Bleeding, 180);
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 180);
		}
	}
}
