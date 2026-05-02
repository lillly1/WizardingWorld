using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Mounts
{
	/// <summary>
	/// Nimbus 2001 — the ultimate broomstick. Faster than Firebolt.
	/// Malfoy money can buy speed. Post-Moon Lord mount.
	/// Grants bonus damage while mounted (aerial superiority).
	/// </summary>
	public class Nimbus2001Mount : ModMount
	{
		public override void SetStaticDefaults()
		{
			MountData.jumpHeight = 15;
			MountData.acceleration = 0.4f;
			MountData.jumpSpeed = 12f;
			MountData.blockExtraJumps = false;
			MountData.constantJump = true;
			MountData.heightBoost = 10;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 22f; // Fastest mount in the mod
			MountData.dashSpeed = 18f;
			MountData.flightTimeMax = 999;
			MountData.fatigueMax = 0;

			MountData.buff = ModContent.BuffType<Buffs.Nimbus2001MountBuff>();
			MountData.spawnDust = DustID.SilverCoin;

			MountData.totalFrames = 4;
			MountData.playerYOffsets = Enumerable.Repeat(10, MountData.totalFrames).ToArray();
			MountData.xOffset = 5;
			MountData.yOffset = -5;
			MountData.playerHeadOffset = 14;
			MountData.bodyFrame = 3;

			MountData.standingFrameCount = 1;
			MountData.standingFrameDelay = 12;
			MountData.standingFrameStart = 0;
			MountData.runningFrameCount = 4;
			MountData.runningFrameDelay = 4;
			MountData.runningFrameStart = 0;
			MountData.flyingFrameCount = 4;
			MountData.flyingFrameDelay = 3;
			MountData.flyingFrameStart = 0;
			MountData.inAirFrameCount = 4;
			MountData.inAirFrameDelay = 4;
			MountData.inAirFrameStart = 0;
			MountData.idleFrameCount = 1;
			MountData.idleFrameDelay = 12;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = false;
		}

		public override void UpdateEffects(Player player)
		{
			// Speed demon trail — silver particles
			if (player.velocity.Length() > 8f)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.SilverCoin, 0f, 0f, 100, default, 0.7f);
				dust.noGravity = true;
				dust.velocity = -player.velocity * 0.12f;
			}

			// Aerial superiority — damage boost while mounted and moving fast
			if (player.velocity.Length() > 10f)
				player.GetDamage(Terraria.ModLoader.DamageClass.Generic) += 0.08f;
		}
	}
}
