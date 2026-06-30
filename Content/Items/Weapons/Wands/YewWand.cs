using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;
using WizardingWorld.Content.DamageClasses;
using WizardingWorld.Content.Projectiles.Spells;

namespace WizardingWorld.Content.Items.Weapons.Wands
{
	/// <summary>Yew Wand — Voldemort's original wand. Casts Crucio (channeled beam) and Reducto (explosion).</summary>
	public class YewWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<SpellDamage>();
			Item.damage = 48;
			Item.knockBack = 2f;
			Item.mana = 5;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<CrucioProjectile>();
			Item.shootSpeed = 16f;
			Item.channel = true;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = null;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// 30% chance to fire Reducto instead
			if (Main.rand.NextBool(3))
			{
				Projectile.NewProjectile(source, position, velocity,
					ModContent.ProjectileType<ReductoProjectile>(), (int)(damage * 1.5f), knockback * 2, player.whoAmI);
				SoundEngine.PlaySound(WizardSoundStyles.Reducto, player.Center);
				return false;
			}

			SoundEngine.PlaySound(WizardSoundStyles.Crucio, player.Center);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SpookyWood, 15)
				.AddIngredient(ItemID.SoulofNight, 8)
				.AddIngredient(ItemID.DarkShard, 1)
				.AddIngredient(ItemID.Diamond, 2)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
