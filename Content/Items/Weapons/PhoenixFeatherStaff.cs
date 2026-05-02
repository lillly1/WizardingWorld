using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Phoenix Feather Staff — summons a Phoenix minion that attacks and heals.</summary>
	public class PhoenixFeatherStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Type] = true;
			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.damage = 45;
			Item.knockBack = 3f;
			Item.mana = 15;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<Buffs.PhoenixMinionBuff>();
			Item.shoot = ModContent.ProjectileType<Projectiles.PhoenixMinion>();
			Item.value = Item.buyPrice(gold: 20);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item44;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.HellstoneBar, 12)
				.AddIngredient(ItemID.Feather, 10)
				.AddIngredient(ItemID.SoulofLight, 8)
				.AddIngredient(ModContent.ItemType<Items.Consumables.GoldenEgg>(), 1)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
