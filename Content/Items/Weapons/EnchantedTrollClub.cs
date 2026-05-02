using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Enchanted Troll Club — upgraded version of the Troll Club.
	/// Slower but MUCH harder hitting. Shockwave on hit that damages nearby enemies.
	/// Crafted from Troll Club + Essence of Magic.
	/// "Somehow, enchanting a troll's club made it even more brutish."
	/// </summary>
	public class EnchantedTrollClub : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 44;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 48; // Big upgrade from 28
			Item.knockBack = 14f;
			Item.crit = 6;
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Shockwave — damage nearby enemies
			float shockwaveRadius = 120f;
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.whoAmI != target.whoAmI && !npc.friendly && npc.CanBeChasedBy()
					&& Vector2.Distance(npc.Center, target.Center) < shockwaveRadius)
				{
					player.ApplyDamageToNPC(npc, damageDone / 3, hit.Knockback, hit.HitDirection, false);
				}
			}

			// Shockwave dust ring
			for (int i = 0; i < 20; i++)
			{
				float angle = MathHelper.TwoPi / 20 * i;
				Vector2 pos = target.Center + angle.ToRotationVector2() * 40f;
				Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.PurpleTorch, 0f, 0f, 50, default, 1.0f);
				dust.velocity = angle.ToRotationVector2() * 2f;
				dust.noGravity = true;
			}
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.PurpleTorch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<TrollClub>(), 1)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddIngredient(ItemID.Amethyst, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
