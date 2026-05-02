using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Dark Lord's Bane — the ultimate cross-boss accessory.
	/// Crafted from materials of ALL wizard bosses: Troll, Basilisk, Aragog, Fluffy,
	/// Horntail, Fenrir, Bellatrix, plus Essence and Unicorn Blood.
	/// The combined power of every challenge overcome.
	/// +15% all damage, +10% spell, +10 defense, +4 regen, poison/venom/fire immune.
	/// </summary>
	public class DarkLordsBane : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(platinum: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += 0.15f;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.10f;
			player.statDefense += 10;
			player.lifeRegen += 4;
			player.buffImmune[BuffID.Poisoned] = true;
			player.buffImmune[BuffID.Venom] = true;
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.OnFire3] = true;

			// Golden heroic aura
			if (!hideVisual && Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GoldCoin, 0f, -0.5f, 50, default, 0.6f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<BasiliskFang>(), 3)
				.AddIngredient(ModContent.ItemType<Consumables.SpiderSilkWeave>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.CerberusFang>(), 3)
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.WerewolfPelt>(), 3)
				.AddIngredient(ModContent.ItemType<Consumables.DarkArtsTome>(), 3)
				.AddIngredient(ModContent.ItemType<Consumables.UnicornBlood>(), 3)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 30)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
