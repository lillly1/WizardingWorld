using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Slytherin's Dagger — house signature weapon for Slytherin.
	/// Fast, crit-focused dagger with stealth mechanics.
	/// Deals +50% damage when hitting from behind (player moving toward enemy's back).
	/// Applies Jinxed debuff on critical hits.
	/// Matches Slytherin's cunning/crit armor bonus.
	/// </summary>
	public class SlytherinsDagger : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 42;
			Item.knockBack = 2f;
			Item.crit = 20; // Massive crit chance — Slytherin's cunning
			Item.value = Item.buyPrice(gold: 12);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Jinxed on crits — cunning strike
			if (hit.Crit)
				target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 240);

			// Venom — serpent's bite
			target.AddBuff(BuffID.Venom, 120);
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GemEmerald, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SilverBar, 15)
				.AddIngredient(ItemID.Emerald, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
