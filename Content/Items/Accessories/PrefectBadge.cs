using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Prefect Badge — grants authority-like bonuses.
	/// +5 defense, +5% spell damage, nearby town NPCs deal more damage.
	/// Stacks with Sorting Hat for extra house bonuses.
	/// </summary>
	public class PrefectBadge : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 5);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense += 5;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.05f;

			// Leadership effect — +3% generic damage per nearby town NPC (max +15%)
			int nearbyTownNPCs = 0;
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.townNPC && npc.friendly && Vector2.Distance(npc.Center, player.Center) < 600f)
					nearbyTownNPCs++;
			}

			float leadershipBonus = Math.Min(nearbyTownNPCs * 0.03f, 0.15f);
			player.GetDamage(DamageClass.Generic) += leadershipBonus;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 5)
				.AddIngredient(ItemID.Ruby, 1)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
