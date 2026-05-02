using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Basilisk Fang Dagger — fast melee dagger. Inflicts venom, destroys Horcruxes (lore).</summary>
	public class BasiliskFangDagger : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 40;
			Item.knockBack = 3f;
			Item.crit = 10;
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GreenTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Venom, 300);

			// Extra damage to undead-type enemies
			if (target.buffImmune[BuffID.ShadowFlame] == false)
				target.AddBuff(BuffID.ShadowFlame, 120);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Accessories.BasiliskFang>(), 2)
				.AddIngredient(ItemID.Bone, 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Simpler recipe
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Accessories.BasiliskFang>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
