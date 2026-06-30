using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Enchanted Flute — summons Fluffy. "Music soothes the savage beast."</summary>
	public class FluffySummonItem : ModItem
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
			// Requires Hardmode + at least one mechanical boss killed
			return Common.Systems.WizardConditions.AnyMechBossDowned
				&& !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Fluffy.FluffyBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.Fluffy.FluffyBoss>();

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
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ItemID.SoulofSight, 5)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 10)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
