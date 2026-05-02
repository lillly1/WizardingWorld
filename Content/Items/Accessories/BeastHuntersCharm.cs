using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Accessories
{
	/// <summary>
	/// Beast Hunter's Charm — crafted from 3 different creature boss materials.
	/// Dragon Scale (Horntail) + Cerberus Fang (Fluffy) + Werewolf Pelt (Fenrir).
	/// A hunter's talisman: +12% damage to enemies, +15% crit chance vs bosses,
	/// immune to Bleeding and BrokenArmor.
	/// "Newt Scamander would be horrified. Charlie Weasley would approve."
	/// </summary>
	public class BeastHuntersCharm : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.sellPrice(gold: 15);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += 0.12f;
			player.buffImmune[BuffID.Bleeding] = true;
			player.buffImmune[BuffID.BrokenArmor] = true;

			// Hunter's instinct — detect creatures
			player.detectCreature = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Consumables.DragonScale>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.CerberusFang>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.WerewolfPelt>(), 5)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
