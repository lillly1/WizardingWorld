using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.Fenrir
{
	/// <summary>
	/// Lycan's Bite Mark — Expert-exclusive accessory from the Fenrir boss bag.
	/// +20% melee damage, +15% movement speed at night, but -5% damage during day.
	/// "The bite that changes everything."
	/// </summary>
	public class LycansBiteMark : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 8);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// +20% melee damage always
			player.GetDamage(DamageClass.Melee) += 0.20f;

			if (!Main.dayTime)
			{
				// Night bonus: +15% movement speed
				player.moveSpeed += 0.15f;
				player.maxRunSpeed *= 1.15f;

				// Subtle blood dust at night while moving
				if (player.velocity.Length() > 3f && Main.rand.NextBool(5))
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood, 0f, 0f, 100, default, 0.7f);
					dust.noGravity = true;
					dust.velocity *= 0.2f;
				}
			}
			else
			{
				// Day penalty: -5% all damage
				player.GetDamage(DamageClass.Generic) -= 0.05f;
			}
		}
	}
}
