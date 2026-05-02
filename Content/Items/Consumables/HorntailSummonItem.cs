using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Dragon Egg (Cracked) — summons the Hungarian Horntail boss.</summary>
	public class HorntailSummonItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.Pink;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.value = Item.buyPrice(gold: 5);
		}

		public override bool CanUseItem(Player player)
		{
			return Main.hardMode && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Horntail.HorntailBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.Horntail.HorntailBoss>();

				if (Main.netMode != NetmodeID.MultiplayerClient)
					NPC.SpawnOnPlayer(player.whoAmI, type);
				else
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.HellstoneBar, 10)
				.AddIngredient(ItemID.SoulofMight, 5)
				.AddIngredient(ModContent.ItemType<DragonScale>(), 3)
				.AddTile<Tiles.EnchantingTable>()
				.Register();

			// Alternative recipe without Dragon Scale for first summon
			CreateRecipe()
				.AddIngredient(ItemID.HellstoneBar, 15)
				.AddIngredient(ItemID.SoulofMight, 10)
				.AddIngredient(ItemID.SoulofFlight, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
