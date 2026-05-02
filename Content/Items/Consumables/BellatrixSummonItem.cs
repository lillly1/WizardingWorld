using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Azkaban Prisoner Tag — summons Bellatrix Lestrange. Requires Plantera defeated.</summary>
	public class BellatrixSummonItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.Yellow;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.value = Item.buyPrice(gold: 8);
		}

		public override bool CanUseItem(Player player)
		{
			return NPC.downedPlantBoss && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Bellatrix.BellatrixBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.Bellatrix.BellatrixBoss>();

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
				.AddIngredient(ItemID.Bone, 30)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.DarkShard, 2)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
