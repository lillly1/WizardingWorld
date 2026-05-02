using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Consumables
{
	/// <summary>Bowtruckle (caught) — a tiny tree guardian in a jar. Can be released or used as bait.</summary>
	public class BowtruckleCatch : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 20;
			Item.maxStack = 99;
			Item.value = Item.buyPrice(silver: 10);
			Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.noMelee = true;
			Item.consumable = true;
			Item.makeNPC = ModContent.NPCType<NPCs.Enemies.Bowtruckle>();
			Item.bait = 25; // Can be used as fishing bait!
		}
	}
}
