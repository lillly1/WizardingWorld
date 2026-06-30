using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Frozen Soul Lantern — summons Azkaban's Despair. Requires Barty and Golem defeated. Night only.</summary>
	public class DementorKingSummonItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.Red;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.value = Item.buyPrice(gold: 10);
		}

		public override bool CanUseItem(Player player)
		{
			return Common.Systems.WizardConditions.DementorGateOpen
				&& !Main.dayTime
				&& !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.DementorKing.DementorKingBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.DementorKing.DementorKingBoss>();

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
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddIngredient(ItemID.Ectoplasm, 10)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 25)
				.AddIngredient(ModContent.ItemType<UnicornBlood>(), 3)
				.AddCondition(Common.Systems.WizardConditions.PostBartyAndGolem)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
