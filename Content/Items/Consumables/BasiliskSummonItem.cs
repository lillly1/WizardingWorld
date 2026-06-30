using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Common.Systems;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Serpent's Diary — summons the Basilisk boss.</summary>
	public class BasiliskSummonItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.Orange;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override bool CanUseItem(Player player)
		{
			return DownedBossSystem.downedQuirrell
				&& NPC.downedBoss3
				&& !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Basilisk.BasiliskBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.Basilisk.BasiliskBoss>();

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else
				{
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Book, 1)
				.AddIngredient(ItemID.Bone, 10)
				.AddIngredient(ItemID.Cobweb, 20)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
