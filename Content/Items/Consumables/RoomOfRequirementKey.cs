using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using WizardingWorld.Content.Buffs;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>
	/// Room of Requirement Key — summons a magical room that adapts to your needs.
	///
	/// The Room detects what the player needs and grants an appropriate powerful buff:
	/// - Recovery Room: if HP below 50% -> massive regen + defense
	/// - Training Room: if holding a wand -> +20% spell damage + crit + mana
	/// - Vault Room: if carrying 10+ gold -> luck boost + treasure sense
	/// - Sanctuary Room: default -> balanced defensive buffs
	///
	/// "I need a place to..."
	///
	/// Canon-faithful: The Room of Requirement appears when someone has great need.
	/// Cooldown: cannot be used while any Room buff is active.
	/// </summary>
	public class RoomOfRequirementKey : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.consumable = false; // reusable with cooldown
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item4;
		}

		public override bool CanUseItem(Player player)
		{
			// Cannot use while any Room buff is active
			return !player.HasBuff(ModContent.BuffType<Buffs.RoomRecoveryBuff>())
				&& !player.HasBuff(ModContent.BuffType<Buffs.RoomTrainingBuff>())
				&& !player.HasBuff(ModContent.BuffType<Buffs.RoomVaultBuff>())
				&& !player.HasBuff(ModContent.BuffType<Buffs.RoomSanctuaryBuff>())
				&& !player.HasBuff(ModContent.BuffType<RoomRestorationBuff>())
				&& !player.HasBuff(ModContent.BuffType<RoomResistanceBuff>());
		}

		public override bool? UseItem(Player player)
		{
			// Detect player need and grant appropriate room buff
			int buffType;
			string roomName;
			Color textColor;

			var wp = player.GetModPlayer<Common.Players.WizardPlayer>();

			// Resistance HQ mode: DA members who have defended the castle
			bool hasDAGalleon = false;
			foreach (var item in player.inventory)
				if (item.active && item.type == ModContent.ItemType<DAGalleon>()) { hasDAGalleon = true; break; }
			if (hasDAGalleon && Common.Systems.HogwartsWardSystem.wardsDefended > 0)
			{
				buffType = ModContent.BuffType<RoomResistanceBuff>();
				roomName = Language.GetTextValue("Mods.WizardingWorld.DA.RoomResistance");
				textColor = new Color(200, 180, 140);
			}
			else if (wp.despair > 0.15f)
			{
				buffType = ModContent.BuffType<RoomRestorationBuff>();
				roomName = Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.Restoration");
				textColor = new Color(180, 200, 255);
			}
			else if (player.statLife < player.statLifeMax2 / 2)
			{
				// Recovery Room — dire need for healing
				buffType = ModContent.BuffType<Buffs.RoomRecoveryBuff>();
				roomName = Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.Recovery");
				textColor = new Color(100, 255, 100);
			}
			else if (player.HeldItem.DamageType == ModContent.GetInstance<SpellDamage>())
			{
				// Training Room — practicing magic
				buffType = ModContent.BuffType<Buffs.RoomTrainingBuff>();
				roomName = Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.HiddenThings");
				textColor = new Color(100, 100, 255);
			}
			else if (CountPlayerGold(player) >= 10)
			{
				// Vault Room — wealth-seeking
				buffType = ModContent.BuffType<Buffs.RoomVaultBuff>();
				roomName = Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.ComeAndGo");
				textColor = new Color(255, 215, 0);
			}
			else
			{
				// Sanctuary Room — default safe haven
				buffType = ModContent.BuffType<Buffs.RoomSanctuaryBuff>();
				roomName = Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.Sanctuary");
				textColor = new Color(200, 180, 255);
			}

			player.AddBuff(buffType, 60 * 180); // 3 minutes
			Main.NewText(Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.Manifests", roomName), textColor);

			// Magical visual effect
			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
					DustID.MagicMirror, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
					150, default, 1.2f);
				dust.noGravity = true;
			}

			return true;
		}

		private int CountPlayerGold(Player player)
		{
			int gold = 0;
			foreach (Item item in player.inventory)
			{
				if (item.active && item.type == ItemID.GoldCoin)
					gold += item.stack;
				if (item.active && item.type == ItemID.PlatinumCoin)
					gold += item.stack * 100;
			}
			return gold;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ModContent.ItemType<EssenceOfMagic>(), 25)
				.AddIngredient(ItemID.SoulofLight, 5)
				.AddIngredient(ItemID.SoulofNight, 5)
				.AddTile<Content.Tiles.EnchantingTable>()
				.Register();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(Mod, "RoomInfo",
				Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.TooltipInfo")));
			tooltips.Add(new TooltipLine(Mod, "RoomResistance",
				Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.TooltipResistance")));
			tooltips.Add(new TooltipLine(Mod, "RoomModes0",
				Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.TooltipModes1")));
			tooltips.Add(new TooltipLine(Mod, "RoomModes",
				Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.TooltipModes2")));
			tooltips.Add(new TooltipLine(Mod, "RoomModes2",
				Language.GetTextValue("Mods.WizardingWorld.RoomOfRequirement.TooltipModes3")));
		}
	}
}
