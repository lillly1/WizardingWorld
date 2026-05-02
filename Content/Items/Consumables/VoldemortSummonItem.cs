using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Dark Mark — summons Lord Voldemort.</summary>
	public class VoldemortSummonItem : ModItem
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
			// Post-Cultist — Voldemort is the TRUE final boss of the mod
			return NPC.downedAncientCultist && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Voldemort.VoldemortBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.Center);
				int type = ModContent.NPCType<NPCs.Bosses.Voldemort.VoldemortBoss>();

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
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddIngredient(ItemID.Ectoplasm, 10)
				.AddIngredient(ItemID.LunarTabletFragment, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
