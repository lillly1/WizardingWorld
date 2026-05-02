using Terraria;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Room of Hidden Things — spell damage, crit chance, and bonus mana for training wizards.</summary>
	public class RoomTrainingBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.20f;
			player.GetCritChance(ModContent.GetInstance<SpellDamage>()) += 10;
			player.statManaMax2 += 40;

			// Spawn a training dummy on first tick if none nearby
			if (player.buffTime[buffIndex] >= 60 * 179) // first ~1 second of buff
			{
				bool dummyNearby = false;
				foreach (NPC npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<Content.NPCs.Enemies.DuelingDummy>() &&
						Microsoft.Xna.Framework.Vector2.Distance(npc.Center, player.Center) < 300f)
					{
						dummyNearby = true;
						break;
					}
				}
				if (!dummyNearby && Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
				{
					NPC.NewNPC(player.GetSource_Buff(buffIndex), (int)player.Center.X + 100, (int)player.Center.Y,
						ModContent.NPCType<Content.NPCs.Enemies.DuelingDummy>());
				}
			}
		}
	}
}
