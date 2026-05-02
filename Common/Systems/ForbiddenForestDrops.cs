using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Adds Unicorn Blood as a rare exclusive drop from enemies killed
	/// while the player is in the Forbidden Forest biome.
	/// This makes the Forbidden Forest worth visiting beyond just the unique spawns.
	/// </summary>
	public class ForbiddenForestDrops : GlobalNPC
	{
		public override void OnKill(NPC npc)
		{
			if (npc.friendly || npc.townNPC || npc.lifeMax < 50)
				return;

			// Check if the nearest player is in the Forbidden Forest
			Player nearest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
			if (nearest == null || !nearest.active)
				return;

			if (!nearest.InModBiome<Content.Biomes.ForbiddenForestBiome>())
				return;

			// 5% chance to drop Unicorn Blood from any non-trivial enemy in the Forest
			if (Main.rand.NextBool(20))
			{
				Item.NewItem(npc.GetSource_Loot(), npc.getRect(),
					ModContent.ItemType<Content.Items.Consumables.UnicornBlood>(), 1);
			}
		}
	}
}
