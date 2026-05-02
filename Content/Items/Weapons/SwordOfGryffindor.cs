using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>Sword of Gryffindor — legendary goblin-made sword. Drops from Basilisk.</summary>
	public class SwordOfGryffindor : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 48;
			Item.height = 48;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 55;
			Item.knockBack = 6f;
			Item.crit = 8;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			// Ruby-red sparkle
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GemRuby, 0f, 0f, 100, default, 1.2f);
				dust.noGravity = true;
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Imbued with Basilisk venom
			target.AddBuff(BuffID.Venom, 180);
		}
	}
}
