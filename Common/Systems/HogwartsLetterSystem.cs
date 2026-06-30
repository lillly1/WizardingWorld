using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
	/// <summary>
	/// Hogwarts Letter System — the mod's intro moment.
	/// When a new player first reaches 120 max HP (after using a Life Crystal)
	/// or defeats their first boss, an owl delivers a Hogwarts Acceptance Letter.
	/// The letter grants a free Oak Wand and points the player toward early crafting.
	/// This solves the "how does a new player discover the mod?" problem.
	/// </summary>
	public class HogwartsLetterSystem : ModPlayer
	{
		public bool receivedLetter;

		public override void SaveData(TagCompound tag)
		{
			tag["receivedLetter"] = receivedLetter;
		}

		public override void LoadData(TagCompound tag)
		{
			receivedLetter = tag.GetBool("receivedLetter");
		}

		public override void PostUpdate()
		{
			if (receivedLetter)
				return;

			// Trigger conditions: used a Life Crystal OR defeated an early boss.
			bool ready = Player.statLifeMax >= 120 || NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedSlimeKing;

			if (!ready)
				return;

			if (Player.whoAmI != Main.myPlayer)
				return;

			// Deliver the letter!
			receivedLetter = true;
			SoundEngine.PlaySound(WizardSoundStyles.OwlHoot, Player.Center);

			// Owl delivery visual — feather dust from above
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(
					Player.Center + new Vector2(Main.rand.Next(-50, 50), -100 - Main.rand.Next(50)),
					4, 4, DustID.Cloud, 0f, 2f, 100, default, 1.0f);
				dust.noGravity = false;
			}

			// Give the letter item
			Player.QuickSpawnItem(Player.GetSource_GiftOrReward(),
				ModContent.ItemType<Content.Items.Consumables.HogwartsLetter>(), 1);

			// Give a free starter wand
			Player.QuickSpawnItem(Player.GetSource_GiftOrReward(),
				ModContent.ItemType<Content.Items.Weapons.Wands.OakWand>(), 1);

			// Chat message
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HogwartsLetterEvent.OwlArrives"), 200, 180, 100);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.HogwartsLetterEvent.Accepted"), 255, 215, 0);
		}
	}
}
