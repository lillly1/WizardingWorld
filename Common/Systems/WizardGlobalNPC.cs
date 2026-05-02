using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// GlobalNPC that adds wizard-themed drops to vanilla enemies.
	/// This integrates the mod into the base game's loot flow so players
	/// naturally discover wizard content during normal gameplay.
	/// </summary>
	public class WizardGlobalNPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			// === PRE-HARDMODE ===

			// Zombies/Skeletons — small chance for basic wand materials
			if (npc.type == NPCID.Zombie || npc.type == NPCID.Skeleton)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.Potions.Butterbeer>(), 25));
			}

			// Demon Eyes — chance for wizard loot
			if (npc.type == NPCID.DemonEye)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Accessories.Remembrall>(), 50));
			}

			// Tim (the rare wizard enemy) — always drops a wand
			if (npc.type == NPCID.Tim)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.Wands.AshWand>(), 3));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Accessories.SortingHat>(), 5));
			}

			// Dungeon enemies — Dark wizard drops
			if (npc.type == NPCID.DarkCaster || npc.type == NPCID.CursedSkull)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.BasiliskSummonItem>(), 30));
			}

			// === HARDMODE ===

			// Hardmode Dungeon — Dark wizard gear
			if (npc.type == NPCID.Necromancer || npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.Wands.DragonHeartstringWand>(), 30));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Accessories.RiddlesDiary>(), 50));
			}

			// Pixies in the Hallow — chance for Cornish Pixie loot
			if (npc.type == NPCID.Pixie)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.Potions.FelixFelicis>(), 20));
			}

			// Werewolves — Wolfsbane loot
			if (npc.type == NPCID.Werewolf)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.Potions.WolfsbanePotion>(), 8));
			}

			// Ice enemies — Patronus-related drops
			if (npc.type == NPCID.IceElemental || npc.type == NPCID.IcyMerman)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.Wands.UnicornHairWand>(), 40));
			}

			// Unicorns — fitting thematic drop
			if (npc.type == NPCID.Unicorn)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.Wands.UnicornHairWand>(), 15));
			}

			// === POST-PLANTERA / GOLEM ===

			// Lunatic Cultist's minions — Dark Mark drops
			if (npc.type == NPCID.CultistArcherBlue || npc.type == NPCID.CultistArcherWhite)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.VoldemortSummonItem>(), 20));
			}

			// === BOSS DROPS ===

			// Eye of Cthulhu — basic wizard reward
			if (npc.type == NPCID.EyeofCthulhu)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.Wands.PhoenixFeatherWand>(), 4));
			}

			// Skeletron — dungeon wand
			if (npc.type == NPCID.SkeletronHead)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.Wands.DragonHeartstringWand>(), 3));
			}

			// Plantera — mid-hardmode wizard gear
			if (npc.type == NPCID.Plantera)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.GoldenEgg>(), 4));
			}
		}
	}
}
