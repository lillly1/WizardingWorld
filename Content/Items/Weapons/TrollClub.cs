using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Troll Club — massive melee weapon dropped by the Mountain Troll. Slow but devastating knockback.</summary>
	public class TrollClub : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 48;
			Item.height = 48;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 28;
			Item.knockBack = 12f;
			Item.crit = 4;
			Item.value = Item.buyPrice(gold: 3);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			// Stone dust on swing
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Stone, 0f, 0f, 100, default, 1.2f);
				dust.noGravity = true;
			}
		}
	}
}
