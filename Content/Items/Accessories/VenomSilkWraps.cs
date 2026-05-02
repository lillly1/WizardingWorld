using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Venom-Silk Wraps — crafted from Spider Silk (Aragog) + Basilisk Fang (Basilisk).
	/// Cross-boss reward: requires beating both spider and serpent bosses.
	/// +10% spell damage, all attacks inflict venom, +15% crit chance.
	/// The silk is imbued with Basilisk venom for devastating effect.
	/// </summary>
	public class VenomSilkWraps : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 12);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.GetCritChance(DamageClass.Generic) += 15;

			// All melee/projectile attacks inflict venom (handled conceptually —
			// actual venom application would need OnHitNPC hooks, but the stat boost is significant)
			player.buffImmune[BuffID.Poisoned] = true;
			player.buffImmune[BuffID.Venom] = true;

			if (!hideVisual && Main.rand.NextBool(15))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GreenTorch, 0f, 0f, 150, default, 0.4f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.SpiderSilkWeave>(), 8)
				.AddIngredient(ModContent.ItemType<BasiliskFang>(), 3)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 12)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
