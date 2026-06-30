using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Suspicious Flask — summons Barty Crouch Jr in disguise. Requires Bellatrix and Plantera defeated.</summary>
	public class BartyCrouchSummonItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.LightPurple;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.value = Item.buyPrice(gold: 5);
		}

		public override bool CanUseItem(Player player)
		{
			return Common.Systems.WizardConditions.BartyGateOpen
				&& !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.BartyCrouch.BartyCrouchBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.BartyCrouch.BartyCrouchBoss>();

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
				.AddIngredient(ItemID.Bottle, 1)
				.AddIngredient(ItemID.Deathweed, 5)
				.AddIngredient(ItemID.SoulofNight, 5)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 12)
				.AddCondition(Common.Systems.WizardConditions.PostBellatrixAndPlantera)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
