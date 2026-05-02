using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Projectiles.Spells
{
	/// <summary>
	/// Apparition — teleport spell. Teleports the player to the cursor position.
	/// Creates a crack-like disapparition effect at departure and arrival.
	/// Has a 3-second cooldown. Cannot teleport through solid tiles for balance.
	/// "Destination, Determination, Deliberation."
	/// </summary>
	public class ApparitionProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 5;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			if (Projectile.ai[0] != 0)
				return;

			Projectile.ai[0] = 1;
			Player owner = Main.player[Projectile.owner];

			if (owner.whoAmI != Main.myPlayer)
				return;

			Vector2 targetPos = Main.MouseWorld;

			// Check line of sight — can't apparate through solid walls
			bool canReach = Collision.CanHitLine(owner.position, owner.width, owner.height,
				targetPos, 1, 1);

			if (!canReach)
			{
				Main.NewText("You can't Apparate there — too much solid matter in the way!", 200, 50, 50);
				return;
			}

			// Max range: 600 pixels (~37 tiles)
			float maxRange = 600f;
			float distance = Vector2.Distance(owner.Center, targetPos);
			if (distance > maxRange)
			{
				// Clamp to max range
				targetPos = owner.Center + (targetPos - owner.Center).SafeNormalize(Vector2.Zero) * maxRange;
			}

			// Departure effect — purple crack
			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(owner.position, owner.width, owner.height, DustID.PurpleTorch, 0f, 0f, 50, default, 1.5f);
				dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
				dust.noGravity = true;
			}

			// Teleport!
			owner.Teleport(targetPos, 1); // Style 1 = Rod of Discord style
			owner.velocity = Vector2.Zero;

			// Arrival effect — purple crack
			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(owner.position, owner.width, owner.height, DustID.PurpleTorch, 0f, 0f, 50, default, 1.5f);
				dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
				dust.noGravity = true;
			}

			// Cooldown — apply Chaos State like Rod of Discord
			owner.AddBuff(BuffID.ChaosState, 180); // 3 second cooldown
		}
	}
}
