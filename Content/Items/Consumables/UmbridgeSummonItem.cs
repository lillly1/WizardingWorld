using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Educational Decree -- summons the Dolores Umbridge boss. Requires mech boss defeated.</summary>
	public class UmbridgeSummonItem : ModItem
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
			// Requires the post-Horntail route plus at least one mechanical boss defeated.
			return Common.Systems.WizardConditions.UmbridgeGateOpen
				&& !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Umbridge.UmbridgeBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.Umbridge.UmbridgeBoss>();

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
				.AddIngredient(ItemID.Book, 3)
				.AddIngredient(ItemID.PinkGel, 10)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
