using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Hufflepuff's Mace of Loyalty — house signature weapon for Hufflepuff.
	/// Slow, heavy mace that heals the player on hit (life steal).
	/// Also grants +5 defense while swinging.
	/// Matches Hufflepuff's tank/regen armor bonus.
	/// </summary>
	public class HufflepuffsMace : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 44;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 28;
			Item.useAnimation = 28;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 58;
			Item.knockBack = 10f; // Heavy smash
			Item.crit = 4;
			Item.value = Item.buyPrice(gold: 12);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Loyalty: heal on hit — the badger protects its own
			int healAmount = System.Math.Min(damageDone / 10, 8);
			if (healAmount > 0)
			{
				player.statLife += healAmount;
				if (player.statLife > player.statLifeMax2)
					player.statLife = player.statLifeMax2;
				player.HealEffect(healAmount);
			}
		}

		public override void HoldItem(Player player)
		{
			// Steadfast: +5 defense while holding
			player.statDefense += 5;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GemTopaz, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.IronBar, 15)
				.AddIngredient(ItemID.Topaz, 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
