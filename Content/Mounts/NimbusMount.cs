using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Mounts
{
	/// <summary>Nimbus 2000 — flying broomstick mount with high speed.</summary>
	public class NimbusMount : ModMount
	{
		public override void SetStaticDefaults()
		{
			MountData.jumpHeight = 10;
			MountData.acceleration = 0.25f;
			MountData.jumpSpeed = 8f;
			MountData.blockExtraJumps = false;
			MountData.constantJump = true;
			MountData.heightBoost = 10;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 14f;
			MountData.dashSpeed = 10f;
			MountData.flightTimeMax = 999; // Unlimited flight
			MountData.fatigueMax = 0;

			MountData.buff = ModContent.BuffType<Buffs.NimbusMountBuff>();
			MountData.spawnDust = DustID.Cloud;

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
			MountData.runningFrameDelay = 8;
			MountData.runningFrameStart = 0;
			MountData.flyingFrameCount = 4;
			MountData.flyingFrameDelay = 6;
			MountData.flyingFrameStart = 0;
			MountData.inAirFrameCount = 4;
			MountData.inAirFrameDelay = 8;
			MountData.inAirFrameStart = 0;
			MountData.idleFrameCount = 1;
			MountData.idleFrameDelay = 12;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = false;
		}

		public override void UpdateEffects(Player player)
		{
			// Wind dust when flying fast
			if (player.velocity.Length() > 5f && Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Cloud, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity = -player.velocity * 0.2f;
			}
		}
	}
}
