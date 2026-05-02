using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Beater's Bat — melee weapon with massive knockback. Sends enemies flying.</summary>
	public class BeatersBat : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 35;
			Item.knockBack = 15f; // Massive knockback!
			Item.crit = 4;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Extra upward launch
			if (!target.boss && target.knockBackResist > 0)
				target.velocity.Y -= 8f;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.BorealWood, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 20)
				.AddIngredient(ItemID.IronBar, 5)
				.AddIngredient(ItemID.Leather, 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
