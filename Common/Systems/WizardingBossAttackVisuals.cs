using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.NPCs.Bosses.Aragog;
using WizardingWorld.Content.NPCs.Bosses.BartyCrouch;
using WizardingWorld.Content.NPCs.Bosses.Basilisk;
using WizardingWorld.Content.NPCs.Bosses.Bellatrix;
using WizardingWorld.Content.NPCs.Bosses.DementorKing;
using WizardingWorld.Content.NPCs.Bosses.Fenrir;
using WizardingWorld.Content.NPCs.Bosses.Fluffy;
using WizardingWorld.Content.NPCs.Bosses.Horntail;
using WizardingWorld.Content.NPCs.Bosses.Quirrell;
using WizardingWorld.Content.NPCs.Bosses.Troll;
using WizardingWorld.Content.NPCs.Bosses.Umbridge;
using WizardingWorld.Content.NPCs.Bosses.Voldemort;

namespace WizardingWorld.Common.Systems
{
	public enum WizardBossAttackStyle
	{
		None,
		Troll,
		Quirrell,
		Basilisk,
		Aragog,
		Fluffy,
		Horntail,
		Umbridge,
		Fenrir,
		Bellatrix,
		BartyCrouch,
		DementorKing,
		Voldemort
	}

	public static class WizardingBossAttackVisuals
	{
		public static int SpawnProjectile(WizardBossAttackStyle style, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = 255, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
		{
			int projectileIndex = Projectile.NewProjectile(source, position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
			TagProjectile(projectileIndex, style);
			EmitCastBurst(style, position, velocity, 10);
			return projectileIndex;
		}

		public static void TagProjectile(int projectileIndex, WizardBossAttackStyle style)
		{
			if (projectileIndex < 0 || projectileIndex >= Main.maxProjectiles)
				return;

			Main.projectile[projectileIndex].GetGlobalProjectile<WizardingBossAttackProjectile>().Style = style;
		}

		public static bool TryGetStyleForNpc(int type, out WizardBossAttackStyle style)
		{
			if (type == ModContent.NPCType<TrollBoss>())
				style = WizardBossAttackStyle.Troll;
			else if (type == ModContent.NPCType<QuirrellBoss>())
				style = WizardBossAttackStyle.Quirrell;
			else if (type == ModContent.NPCType<BasiliskBoss>())
				style = WizardBossAttackStyle.Basilisk;
			else if (type == ModContent.NPCType<AragogBoss>())
				style = WizardBossAttackStyle.Aragog;
			else if (type == ModContent.NPCType<FluffyBoss>())
				style = WizardBossAttackStyle.Fluffy;
			else if (type == ModContent.NPCType<HorntailBoss>())
				style = WizardBossAttackStyle.Horntail;
			else if (type == ModContent.NPCType<UmbridgeBoss>())
				style = WizardBossAttackStyle.Umbridge;
			else if (type == ModContent.NPCType<FenrirBoss>())
				style = WizardBossAttackStyle.Fenrir;
			else if (type == ModContent.NPCType<BellatrixBoss>())
				style = WizardBossAttackStyle.Bellatrix;
			else if (type == ModContent.NPCType<BartyCrouchBoss>())
				style = WizardBossAttackStyle.BartyCrouch;
			else if (type == ModContent.NPCType<DementorKingBoss>())
				style = WizardBossAttackStyle.DementorKing;
			else if (type == ModContent.NPCType<VoldemortBoss>())
				style = WizardBossAttackStyle.Voldemort;
			else
			{
				style = WizardBossAttackStyle.None;
				return false;
			}

			return true;
		}

		public static void EmitProjectileTrail(Projectile projectile, WizardBossAttackStyle style)
		{
			if (Main.dedServ || style == WizardBossAttackStyle.None)
				return;

			Vector2 direction = projectile.velocity.SafeNormalize(Vector2.UnitX);
			Vector2 normal = direction.RotatedBy(MathHelper.PiOver2);
			Vector2 center = projectile.Center;
			float wave = (float)System.Math.Sin((projectile.timeLeft + projectile.whoAmI) * 0.35f);
			Vector2 ribbonOffset = normal * wave * 7f;

			switch (style)
			{
				case WizardBossAttackStyle.Troll:
					EmitDust(center - direction * 8f, DustID.Stone, new Color(120, 105, 80), 1.1f, projectile.velocity * -0.12f, false);
					if (Main.rand.NextBool(3))
						EmitDust(center + Main.rand.NextVector2Circular(5f, 5f), DustID.Dirt, new Color(80, 65, 45), 0.9f, projectile.velocity * -0.08f, false);
					break;

				case WizardBossAttackStyle.Quirrell:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.PurpleTorch : DustID.CursedTorch, new Color(110, 70, 150), 1.0f, projectile.velocity * -0.10f, true);
					if (Main.rand.NextBool(4))
						EmitDust(center - ribbonOffset * 0.6f, DustID.Smoke, new Color(75, 65, 88), 0.8f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Basilisk:
					EmitDust(center + ribbonOffset, DustID.GreenTorch, new Color(40, 220, 90), 1.25f, projectile.velocity * -0.05f, true);
					if (Main.rand.NextBool(3))
						EmitDust(center - ribbonOffset, DustID.GemEmerald, new Color(10, 130, 70), 0.65f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Aragog:
					EmitDust(center + ribbonOffset * 0.5f, DustID.Web, new Color(210, 210, 195), 1.0f, projectile.velocity * -0.08f, false);
					if (Main.rand.NextBool(3))
						EmitDust(center - ribbonOffset, DustID.Smoke, new Color(90, 75, 65), 0.7f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Fluffy:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.Smoke : DustID.GoldCoin, new Color(120, 75, 35), 1.15f, projectile.velocity * -0.12f, true);
					if (Main.rand.NextBool(4))
						EmitDust(center - ribbonOffset, DustID.Blood, new Color(110, 35, 25), 0.75f, Vector2.Zero, false);
					break;

				case WizardBossAttackStyle.Horntail:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.Torch : DustID.SolarFlare, new Color(255, 115, 30), 1.35f, projectile.velocity * -0.10f, true);
					if (Main.rand.NextBool(3))
						EmitDust(center - direction * 10f, DustID.Smoke, new Color(70, 50, 40), 0.85f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Umbridge:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.PinkTorch : DustID.GoldCoin, new Color(255, 125, 205), 1.05f, projectile.velocity * -0.05f, true);
					if (Main.rand.NextBool(5))
						EmitDust(center - ribbonOffset, DustID.TreasureSparkle, new Color(255, 225, 150), 0.6f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Fenrir:
					EmitDust(center + ribbonOffset, DustID.Blood, new Color(170, 25, 25), 1.1f, projectile.velocity * -0.10f, false);
					if (Main.rand.NextBool(3))
						EmitDust(center - ribbonOffset, DustID.Smoke, new Color(55, 55, 60), 0.8f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Bellatrix:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.Shadowflame : DustID.PurpleTorch, new Color(165, 55, 230), 1.2f, projectile.velocity * -0.08f, true);
					if (Main.rand.NextBool(4))
						EmitDust(center - ribbonOffset, DustID.CrimsonTorch, new Color(125, 20, 75), 0.85f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.BartyCrouch:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.MagicMirror : DustID.CursedTorch, new Color(60, 180, 115), 1.0f, projectile.velocity * -0.06f, true);
					if (Main.rand.NextBool(4))
						EmitDust(center - ribbonOffset, DustID.Shadowflame, new Color(80, 60, 135), 0.85f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.DementorKing:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.Shadowflame : DustID.SilverCoin, new Color(120, 160, 190), 1.05f, projectile.velocity * -0.06f, true);
					if (Main.rand.NextBool(2))
						EmitDust(center - ribbonOffset, DustID.Smoke, new Color(15, 22, 35), 1.0f, Vector2.Zero, true);
					break;

				case WizardBossAttackStyle.Voldemort:
					EmitDust(center + ribbonOffset, Main.rand.NextBool() ? DustID.CursedTorch : DustID.Shadowflame, new Color(70, 255, 85), 1.25f, projectile.velocity * -0.08f, true);
					if (Main.rand.NextBool(3))
						EmitDust(center - ribbonOffset, DustID.Smoke, new Color(15, 25, 20), 0.9f, Vector2.Zero, true);
					break;
			}

			AddStyleLight(center, style, 0.45f);
		}

		public static void EmitNpcAttackAura(NPC npc, WizardBossAttackStyle style)
		{
			if (Main.dedServ || style == WizardBossAttackStyle.None)
				return;

			float speed = npc.velocity.Length();
			bool charging = speed > 3.5f;
			bool castingPulse = npc.ai[1] % 45f < 2f;
			if (!charging && !castingPulse && !Main.rand.NextBool(18))
				return;

			int count = charging ? 4 : 2;
			for (int i = 0; i < count; i++)
			{
				Vector2 pos = npc.Center + Main.rand.NextVector2Circular(npc.width * 0.65f, npc.height * 0.65f);
				Vector2 vel = charging ? -npc.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.5f) * Main.rand.NextFloat(1.0f, 2.5f) : Main.rand.NextVector2Circular(1.2f, 1.2f);
				EmitStyleDust(style, pos, vel, charging ? 1.2f : 0.8f);
			}

			AddStyleLight(npc.Center, style, charging ? 0.35f : 0.18f);
		}

		public static void EmitCastBurst(WizardBossAttackStyle style, Vector2 center, Vector2 velocity, int count)
		{
			if (Main.dedServ || style == WizardBossAttackStyle.None)
				return;

			Vector2 direction = velocity.SafeNormalize(Vector2.UnitY);
			for (int i = 0; i < count; i++)
			{
				Vector2 ring = direction.RotatedBy(MathHelper.TwoPi * i / count) * Main.rand.NextFloat(1.0f, 3.4f);
				EmitStyleDust(style, center + ring * 3f, ring, 1.15f);
			}
		}

		private static void EmitStyleDust(WizardBossAttackStyle style, Vector2 position, Vector2 velocity, float scale)
		{
			switch (style)
			{
				case WizardBossAttackStyle.Troll:
					EmitDust(position, Main.rand.NextBool() ? DustID.Stone : DustID.Dirt, new Color(115, 95, 70), scale, velocity, false);
					break;
				case WizardBossAttackStyle.Quirrell:
					EmitDust(position, Main.rand.NextBool() ? DustID.PurpleTorch : DustID.CursedTorch, new Color(115, 70, 155), scale, velocity, true);
					break;
				case WizardBossAttackStyle.Basilisk:
					EmitDust(position, Main.rand.NextBool() ? DustID.GreenTorch : DustID.GemEmerald, new Color(40, 210, 90), scale, velocity, true);
					break;
				case WizardBossAttackStyle.Aragog:
					EmitDust(position, Main.rand.NextBool() ? DustID.Web : DustID.Smoke, new Color(205, 205, 190), scale, velocity, false);
					break;
				case WizardBossAttackStyle.Fluffy:
					EmitDust(position, Main.rand.NextBool() ? DustID.Smoke : DustID.Blood, new Color(120, 65, 40), scale, velocity, true);
					break;
				case WizardBossAttackStyle.Horntail:
					EmitDust(position, Main.rand.NextBool() ? DustID.Torch : DustID.SolarFlare, new Color(255, 120, 25), scale, velocity, true);
					break;
				case WizardBossAttackStyle.Umbridge:
					EmitDust(position, Main.rand.NextBool() ? DustID.PinkTorch : DustID.GoldCoin, new Color(255, 135, 205), scale, velocity, true);
					break;
				case WizardBossAttackStyle.Fenrir:
					EmitDust(position, Main.rand.NextBool() ? DustID.Blood : DustID.Smoke, new Color(165, 30, 25), scale, velocity, false);
					break;
				case WizardBossAttackStyle.Bellatrix:
					EmitDust(position, Main.rand.NextBool() ? DustID.Shadowflame : DustID.CrimsonTorch, new Color(170, 55, 220), scale, velocity, true);
					break;
				case WizardBossAttackStyle.BartyCrouch:
					EmitDust(position, Main.rand.NextBool() ? DustID.MagicMirror : DustID.CursedTorch, new Color(60, 180, 115), scale, velocity, true);
					break;
				case WizardBossAttackStyle.DementorKing:
					EmitDust(position, Main.rand.NextBool() ? DustID.Shadowflame : DustID.SilverCoin, new Color(120, 160, 190), scale, velocity, true);
					break;
				case WizardBossAttackStyle.Voldemort:
					EmitDust(position, Main.rand.NextBool() ? DustID.CursedTorch : DustID.Shadowflame, new Color(75, 255, 90), scale, velocity, true);
					break;
			}
		}

		private static void EmitDust(Vector2 position, int dustType, Color color, float scale, Vector2 velocity, bool noGravity)
		{
			Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 80, color, scale);
			dust.noGravity = noGravity;
			if (noGravity)
				dust.velocity *= 0.85f;
		}

		private static void AddStyleLight(Vector2 center, WizardBossAttackStyle style, float strength)
		{
			Vector3 color = style switch
			{
				WizardBossAttackStyle.Troll => new Vector3(0.45f, 0.32f, 0.16f),
				WizardBossAttackStyle.Quirrell => new Vector3(0.35f, 0.18f, 0.52f),
				WizardBossAttackStyle.Basilisk => new Vector3(0.08f, 0.55f, 0.20f),
				WizardBossAttackStyle.Aragog => new Vector3(0.35f, 0.32f, 0.28f),
				WizardBossAttackStyle.Fluffy => new Vector3(0.45f, 0.22f, 0.12f),
				WizardBossAttackStyle.Horntail => new Vector3(0.80f, 0.30f, 0.08f),
				WizardBossAttackStyle.Umbridge => new Vector3(0.75f, 0.28f, 0.65f),
				WizardBossAttackStyle.Fenrir => new Vector3(0.55f, 0.05f, 0.04f),
				WizardBossAttackStyle.Bellatrix => new Vector3(0.55f, 0.15f, 0.80f),
				WizardBossAttackStyle.BartyCrouch => new Vector3(0.12f, 0.45f, 0.36f),
				WizardBossAttackStyle.DementorKing => new Vector3(0.18f, 0.30f, 0.42f),
				WizardBossAttackStyle.Voldemort => new Vector3(0.15f, 0.75f, 0.18f),
				_ => Vector3.Zero
			};

			Lighting.AddLight(center, color * strength);
		}
	}

	public class WizardingBossAttackProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		public WizardBossAttackStyle Style { get; set; }

		public override void AI(Projectile projectile)
		{
			WizardingBossAttackVisuals.EmitProjectileTrail(projectile, Style);
		}

		public override void OnKill(Projectile projectile, int timeLeft)
		{
			WizardingBossAttackVisuals.EmitCastBurst(Style, projectile.Center, projectile.velocity, 8);
		}
	}

	public class WizardingBossAttackNpcVisuals : GlobalNPC
	{
		public override void PostAI(NPC npc)
		{
			if (WizardingBossAttackVisuals.TryGetStyleForNpc(npc.type, out WizardBossAttackStyle style))
				WizardingBossAttackVisuals.EmitNpcAttackAura(npc, style);
		}
	}
}
