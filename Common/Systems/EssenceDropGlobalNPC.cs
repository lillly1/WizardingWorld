using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Adds Essence of Magic drops to ALL mod enemies.
	/// Drop rate scales with enemy difficulty:
	/// - Critters: 0 (no drop)
	/// - Normal enemies: 1-2 (50% chance)
	/// - Mini-boss (Nagini): 5-10 (100%)
	/// - Bosses: 15-30 (100%)
	/// </summary>
	public class EssenceDropGlobalNPC : GlobalNPC
	{
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			// Only apply to mod NPCs
			return entity.ModNPC != null && entity.ModNPC.Mod == ModContent.GetInstance<WizardingWorld>();
		}

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			int essenceType = ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>();

			// Skip critters
			if (Terraria.ID.NPCID.Sets.CountsAsCritter[npc.type])
				return;

			// Skip town NPCs
			if (npc.townNPC)
				return;

			if (npc.boss)
			{
				// Bosses: guaranteed 15-30
				npcLoot.Add(ItemDropRule.Common(essenceType, 1, 15, 30));
			}
			else if (npc.lifeMax >= 2000)
			{
				// Mini-bosses (Nagini, etc.): guaranteed 5-10
				npcLoot.Add(ItemDropRule.Common(essenceType, 1, 5, 10));
			}
			else if (npc.lifeMax >= 100)
			{
				// Normal enemies: 50% chance, 1-3
				npcLoot.Add(ItemDropRule.Common(essenceType, 2, 1, 3));
			}
			else
			{
				// Weak enemies: 25% chance, 1
				npcLoot.Add(ItemDropRule.Common(essenceType, 4, 1, 1));
			}
		}
	}
}
