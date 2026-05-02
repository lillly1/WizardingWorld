using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Mounts
{
	/// <summary>
	/// Thestral Mount — dark skeletal winged horse. Only those who have seen death can ride it.
	/// Unlocked after defeating the Basilisk (seen death).
	/// Moderate speed flying mount — between Nimbus and Firebolt.
	/// Grants night vision while mounted and slight enemy detection.
	/// Dark and eerie — opposite aesthetic to the bright Hippogriff.
	/// </summary>
	public class ThestralMount : ModMount
	{
		public override void SetStaticDefaults()
		{
			MountData.jumpHeight = 10;
			MountData.acceleration = 0.28f;
			MountData.jumpSpeed = 8f;
			MountData.blockExtraJumps = false;
			MountData.constantJump = true;
			MountData.heightBoost = 16;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 15f;
			MountData.dashSpeed = 12f;
			MountData.flightTimeMax = 999;
			MountData.fatigueMax = 0;

			MountData.buff = ModContent.BuffType<Buffs.ThestralMountBuff>();
			MountData.spawnDust = DustID.Smoke;

			MountData.totalFrames = 4;
			MountData.playerYOffsets = Enumerable.Repeat(14, MountData.totalFrames).ToArray();
			MountData.xOffset = 6;
			MountData.yOffset = -8;
			MountData.playerHeadOffset = 16;
			MountData.bodyFrame = 3;

			MountData.standingFrameCount = 1;
			MountData.standingFrameDelay = 12;
			MountData.standingFrameStart = 0;
			MountData.runningFrameCount = 4;
			MountData.runningFrameDelay = 6;
			MountData.runningFrameStart = 0;
			MountData.flyingFrameCount = 4;
			MountData.flyingFrameDelay = 5;
			MountData.flyingFrameStart = 0;
			MountData.inAirFrameCount = 4;
			MountData.inAirFrameDelay = 6;
			MountData.inAirFrameStart = 0;
			MountData.idleFrameCount = 1;
			MountData.idleFrameDelay = 12;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = false;
		}

		public override void UpdateEffects(Player player)
		{
			// Night vision while mounted
			player.nightVision = true;
			player.detectCreature = true;

			// Dark mist trail
			if (player.velocity.Length() > 4f && Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Smoke, 0f, 0f, 180, default, 0.6f);
				dust.noGravity = true;
				dust.velocity = -player.velocity * 0.1f;
			}
		}
	}
}
