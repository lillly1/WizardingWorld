using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Mounts
{
	/// <summary>Firebolt — upgraded flying broomstick. Faster than Nimbus 2000.</summary>
	public class FireboltMount : ModMount
	{
		public override void SetStaticDefaults()
		{
			MountData.jumpHeight = 12;
			MountData.acceleration = 0.35f;
			MountData.jumpSpeed = 10f;
			MountData.blockExtraJumps = false;
			MountData.constantJump = true;
			MountData.heightBoost = 10;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 18f;
			MountData.dashSpeed = 15f;
			MountData.flightTimeMax = 999;
			MountData.fatigueMax = 0;

			MountData.buff = ModContent.BuffType<Buffs.FireboltMountBuff>();
			MountData.spawnDust = DustID.Torch;

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
			MountData.runningFrameDelay = 6;
			MountData.runningFrameStart = 0;
			MountData.flyingFrameCount = 4;
			MountData.flyingFrameDelay = 4;
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
			// Fire trail when fast
			if (player.velocity.Length() > 6f)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Torch, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity = -player.velocity * 0.15f;
			}
		}
	}
}
