using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Riddikulus — the anti-Boggart charm. Turns fear into laughter.
	/// Deals crushing bonus damage to Boggarts and dispels fear-heavy debuffs on cast.
	/// Patronus remains the primary anti-Dementor answer.
	/// </summary>
	public class RiddikulusProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = ModContent.GetInstance<SpellDamage>();
			Projectile.penetrate = 3;
			Projectile.timeLeft = 300;
			Projectile.light = 0.6f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			// Rainbow/multicolor cheerful trail — laughter magic
			int[] colors = { DustID.YellowStarDust, DustID.PinkTorch, DustID.BlueTorch, DustID.GreenTorch };
			int dustType = colors[(int)(Projectile.ai[0]++ / 5) % colors.Length];
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.0f);
			dust.noGravity = true;
			dust.velocity *= 0.2f;

			// On first frame: cleanse fear debuffs from caster
			if (Projectile.ai[1] == 0)
			{
				Projectile.ai[1] = 1;
				Player owner = Main.player[Projectile.owner];
				int masteryLevel = owner.GetModPlayer<Common.Players.WizardPlayer>()
					.GetWandMasteryLevel(ModContent.ItemType<Content.Items.Weapons.Wands.RedOakWand>());
				owner.ClearBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>());
				owner.ClearBuff(ModContent.BuffType<Buffs.Debuffs.DarkCurseDebuff>());
				owner.ClearBuff(BuffID.Confused);
				owner.ClearBuff(BuffID.Darkness);
				if (masteryLevel >= 2)
				{
					owner.ClearBuff(BuffID.Slow);
					owner.ClearBuff(BuffID.Blackout);
				}
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// CANON FIX: Riddikulus counters BOGGARTS and fear illusions.
			// Patronus counters Dementors. These are separate mechanics.
			// "Riddikulus forces the Boggart to assume a form the caster finds amusing."
			if (target.type == ModContent.NPCType<NPCs.Enemies.Boggart>()
				|| target.type == ModContent.NPCType<NPCs.Enemies.GrimmauldBoggart>())
			{
				modifiers.FinalDamage *= 5f; // Devastating against Boggarts (its true purpose)
			}
			// Moderate bonus against other fear/nightmare creatures (NOT Dementors)
			// Dementors removed — use Patronus instead
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player owner = Main.player[Projectile.owner];
			int masteryLevel = owner.GetModPlayer<Common.Players.WizardPlayer>()
				.GetWandMasteryLevel(ModContent.ItemType<Content.Items.Weapons.Wands.RedOakWand>());

			// Turn Boggarts ridiculous — confuse them
			target.AddBuff(BuffID.Confused, 300);
			owner.GetModPlayer<Common.Players.WizardPlayer>().RelieveDespair(0.03f);

			if (masteryLevel >= 3 && (target.type == ModContent.NPCType<NPCs.Enemies.Boggart>()
				|| target.type == ModContent.NPCType<NPCs.Enemies.GrimmauldBoggart>()))
			{
				foreach (NPC npc in Main.ActiveNPCs)
				{
					if (npc.CanBeChasedBy() && npc.whoAmI != target.whoAmI &&
						Vector2.Distance(npc.Center, target.Center) < 180f)
					{
						npc.AddBuff(BuffID.Confused, 180);
					}
				}
			}

			// Laughter burst
			for (int i = 0; i < 10; i++)
			{
				int dustType = Main.rand.Next(new[] { DustID.YellowStarDust, DustID.PinkTorch, DustID.GreenTorch });
				Dust d = Dust.NewDustDirect(target.Center + Main.rand.NextVector2Circular(10, 10), 4, 4, dustType, 0f, -2f, 50, default, 1.0f);
				d.noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				int dustType = Main.rand.Next(new[] { DustID.YellowStarDust, DustID.PinkTorch, DustID.BlueTorch, DustID.GreenTorch });
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 1.3f);
				dust.velocity *= 2.5f;
				dust.noGravity = true;
			}
		}
	}
}
