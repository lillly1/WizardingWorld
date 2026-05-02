using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Hogwarts Acceptance Letter — the intro item for the entire mod.
	/// When used, displays a welcome message explaining the mod's systems,
	/// grants the Enchanting Table recipe, and gives a small Essence of Magic starter pack.
	/// "Dear Mr/Ms [Player], We are pleased to inform you that you have been
	/// accepted at Hogwarts School of Witchcraft and Wizardry."
	/// </summary>
	public class HogwartsLetter : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.maxStack = 1;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = 0;
			Item.UseSound = SoundID.Item4;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterHeader"), 255, 215, 0);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterDear", player.name), 200, 180, 140);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWelcome"), 200, 180, 140);
			Main.NewText("", 200, 180, 140);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterStartHeader"), 255, 200, 100);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterStart1"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterStart2"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterStart3"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterStart4"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterStart5"), 180, 180, 180);
			Main.NewText("", 200, 180, 140);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWorldHeader"), 255, 200, 100);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWorld1"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWorld2"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWorld3"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWorld4"), 180, 180, 180);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterWorld5"), 180, 180, 180);
			Main.NewText("", 200, 180, 140);
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.ItemMessages.LetterClosing"), 255, 215, 0);

			// Starter pack
			player.QuickSpawnItem(player.GetSource_GiftOrReward(),
				ModContent.ItemType<EssenceOfMagic>(), 10);

			return true;
		}
	}
}
