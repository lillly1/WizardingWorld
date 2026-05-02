using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Patronus Charm — summons a permanent Patronus Guardian minion.
	/// Costs 2 minion slots. Provides +10 defense and +2 life regen while active.
	/// Excels against Dementors, Azkaban horrors, and despair-heavy encounters.
	/// </summary>
	public class PatronusCharm : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Type] = true;
			ItemID.Sets.StaffMinionSlotsRequired[Type] = 2f;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;
			Item.damage = 65;
			Item.knockBack = 5f;
			Item.mana = 25;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<Buffs.PatronusCharmBuff>();
			Item.shoot = ModContent.ProjectileType<Projectiles.PatronusGuardian>();
			Item.value = Item.buyPrice(gold: 25);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item44;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			// LIGHT MAGIC CLEANSES DARKNESS — Canon Tier A.
			// "Expecto Patronum is the most powerful defense against Dementors and
			// other dark creatures. It requires focusing on your happiest memory."
			// Casting Patronus actively pushes back corruption.
			var darkPlayer = player.GetModPlayer<Common.Players.DarkArtsCorruptionPlayer>();
			darkPlayer.CleansCorruption(0.05f); // Significant cleansing per cast

			// Kill existing Patronus first (only one allowed)
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == type)
					Main.projectile[i].Kill();
			}

			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SoulofLight, 15)
				.AddIngredient(ItemID.UnicornHorn, 3)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 20)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
