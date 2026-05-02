using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Mounts
{
	/// <summary>Hippogriff — fast flying mount. Must bow before riding (earned from Hagrid).</summary>
	public class HippogriffMount : ModMount
	{
		public override void SetStaticDefaults()
		{
			MountData.jumpHeight = 12;
			MountData.acceleration = 0.3f;
			MountData.jumpSpeed = 10f;
			MountData.blockExtraJumps = false;
			MountData.constantJump = true;
			MountData.heightBoost = 20;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 16f;
			MountData.dashSpeed = 14f;
			MountData.flightTimeMax = 999;
			MountData.fatigueMax = 0;

			MountData.buff = ModContent.BuffType<Buffs.HippogriffMountBuff>();
			MountData.spawnDust = DustID.Cloud;

			MountData.totalFrames = 4;
			MountData.playerYOffsets = Enumerable.Repeat(16, MountData.totalFrames).ToArray();
			MountData.xOffset = 8;
			MountData.yOffset = -10;
			MountData.playerHeadOffset = 18;
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
			// Feather dust when flying
			if (player.velocity.Length() > 6f && Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.BorealWood, 0f, 0f, 100, default, 0.7f);
				dust.noGravity = true;
				dust.velocity = -player.velocity * 0.15f;
			}

			// Boost attack while mounted
			player.GetDamage(DamageClass.Generic) += 0.05f;
		}
	}
}
