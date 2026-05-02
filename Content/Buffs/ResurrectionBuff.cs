using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>
	/// Resurrection — prevents one death. When you would die, instead heal to 50%.
	/// One-time use buff that lasts 10 minutes.
	/// </summary>
	public class ResurrectionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
		}
	}

	/// <summary>ModPlayer that handles the resurrection trigger.</summary>
	public class ResurrectionPlayer : ModPlayer
	{
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
		{
			if (Player.HasBuff(ModContent.BuffType<ResurrectionBuff>()))
			{
				// Prevent death! Heal to 50%
				Player.statLife = Player.statLifeMax2 / 2;
				Player.HealEffect(Player.statLifeMax2 / 2);

				// Remove the buff — one-time use
				Player.ClearBuff(ModContent.BuffType<ResurrectionBuff>());

				// Phoenix fire visual
				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height,
						Terraria.ID.DustID.Torch, 0f, -2f, 50, default, 2f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(4f, 6f);
					dust.velocity.Y -= 3f;
				}

				if (Player.whoAmI == Main.myPlayer)
					Main.NewText(Language.GetTextValue("Mods.WizardingWorld.Resurrection.RiseFromAshes"), 255, 150, 50);

				return false; // Prevent death
			}

			return true;
		}
	}
}
