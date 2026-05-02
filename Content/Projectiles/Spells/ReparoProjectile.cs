using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Reparo — object and ward repair magic.
	/// Rather than healing living targets, it refreshes nearby magical protections,
	/// stabilizes the Room of Requirement buffs, and strengthens anti-despair wards.
	/// </summary>
	public class ReparoProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 360;
			Projectile.light = 0.65f;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.ai[0] += 0.07f;
			float radius = 64f;
			Vector2 orbitPos = owner.Center + new Vector2(
				(float)Math.Cos(Projectile.ai[0]) * radius,
				(float)Math.Sin(Projectile.ai[0]) * radius
			);
			Projectile.Center = orbitPos;
			Projectile.velocity = Vector2.Zero;

			owner.statDefense += 4;
			owner.endurance += 0.04f;

			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald, 0f, 0f, 100, default, 0.9f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			Projectile.ai[1]++;
			if (Projectile.ai[1] >= 60)
			{
				Projectile.ai[1] = 0;
				RepairNearbyMagic(owner);
			}

			Projectile.rotation += 0.1f;
		}

		private void RepairNearbyMagic(Player owner)
		{
			int elmType = ModContent.ItemType<Content.Items.Weapons.Wands.ElmWand>();
			int masteryLevel = owner.GetModPlayer<Common.Players.WizardPlayer>().GetWandMasteryLevel(elmType);
			int repairStrength = masteryLevel >= 3 ? 90 : 60;

			foreach (Projectile proj in Main.ActiveProjectiles)
			{
				if (!proj.active || proj.owner != owner.whoAmI)
					continue;

				if (proj.type == ModContent.ProjectileType<ProtegoProjectile>() &&
					Vector2.Distance(owner.Center, proj.Center) < 140f)
				{
					proj.timeLeft = Math.Max(proj.timeLeft, 120);
					proj.timeLeft += repairStrength;
				}
			}

			ExtendBuff(owner, ModContent.BuffType<Buffs.RoomRecoveryBuff>(), repairStrength);
			ExtendBuff(owner, ModContent.BuffType<Buffs.RoomTrainingBuff>(), repairStrength);
			ExtendBuff(owner, ModContent.BuffType<Buffs.RoomVaultBuff>(), repairStrength);
			ExtendBuff(owner, ModContent.BuffType<Buffs.RoomSanctuaryBuff>(), repairStrength);
			ExtendBuff(owner, ModContent.BuffType<Buffs.WardOfHopeBuff>(), repairStrength);

			owner.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(masteryLevel >= 2 ? 0.05f : 0.03f);

			for (int i = 0; i < 12; i++)
			{
				Dust spark = Dust.NewDustDirect(owner.position, owner.width, owner.height, DustID.GemEmerald, 0f, -1f, 80, default, 1.1f);
				spark.noGravity = true;
				spark.velocity = Main.rand.NextVector2Circular(2f, 2f);
			}
		}

		private static void ExtendBuff(Player player, int buffType, int amount)
		{
			int index = player.FindBuffIndex(buffType);
			if (index >= 0)
				player.buffTime[index] = Math.Min(player.buffTime[index] + amount, 60 * 60 * 5);
		}
	}
}
