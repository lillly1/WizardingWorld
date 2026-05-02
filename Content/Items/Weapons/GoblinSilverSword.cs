using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Goblin-Wrought Silver Sword — crafted using Goblin Rebel drops + silver.
	/// Goblin-made weapons imbibe that which makes them stronger.
	/// Gains +1 damage for every 10 enemies killed while holding it (session-only, caps at +30).
	/// "Goblins are the finest metalworkers in the wizarding world."
	/// </summary>
	public class GoblinSilverSword : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 44;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 45;
			Item.knockBack = 5f;
			Item.crit = 8;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.SilverCoin, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Goblin-made — absorbs enemy strength
			target.AddBuff(BuffID.Midas, 180);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SilverBar, 15)
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
