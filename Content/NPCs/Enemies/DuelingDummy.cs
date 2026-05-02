using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.NPCs.Enemies
{
	/// <summary>
	/// Dueling Dummy — a training target. Spawned by placing a Dueling Dummy item.
	/// Cannot be killed (extremely high HP, regens to full instantly).
	/// Shows damage numbers so players can test their DPS.
	/// Does not attack. Does not move. Does not despawn.
	/// The Practice Dummy of the wizarding world.
	/// </summary>
	public class DuelingDummy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;
			NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 44;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 999999;
			NPC.HitSound = SoundID.NPCHit15;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.immortal = true;
			NPC.dontTakeDamage = false; // Takes damage for numbers, but won't die
			NPC.friendly = false; // So you can hit it
			NPC.npcSlots = 0f;
		}

		public override void AI()
		{
			// Stay still
			NPC.velocity = Vector2.Zero;

			// Regen to full
			NPC.life = NPC.lifeMax;

			// Apply gravity (so it sits on the ground)
			if (!Collision.SolidCollision(NPC.position + new Vector2(0, NPC.height), NPC.width, 4))
				NPC.position.Y += 4f;
		}

		public override bool CheckDead() => false; // Can't die

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false; // No health bar — it's a training dummy
		}
	}
}
