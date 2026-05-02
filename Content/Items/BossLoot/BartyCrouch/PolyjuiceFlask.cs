using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.BartyCrouch
{
	/// <summary>
	/// Polyjuice Flask — Expert-exclusive accessory from Barty Crouch Jr.
	/// +12% all damage, +10% movement speed.
	/// Every 30 seconds, briefly become invisible (2 second invis + aggro reduction).
	/// "Constant vigilance!"
	/// </summary>
	public class PolyjuiceFlask : ModItem
	{
		private int invisTimer;

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 15);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// +12% all damage
			player.GetDamage(DamageClass.Generic) += 0.12f;

			// +10% movement speed
			player.moveSpeed += 0.10f;

			// Invisibility mechanic — every 30 seconds (1800 ticks), 2 seconds of invis (120 ticks)
			invisTimer++;

			// Active invisibility window: ticks 1680-1800
			if (invisTimer > 1680 && invisTimer <= 1800)
			{
				// Invisible — reduced aggro
				player.invis = true;
				player.aggro -= 400;

				// Subtle shimmer effect
				if (Main.rand.NextBool(4))
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 0.5f);
					dust.noGravity = true;
					dust.velocity *= 0.1f;
				}
			}

			if (invisTimer >= 1800)
			{
				invisTimer = 0;
			}
		}
	}
}
