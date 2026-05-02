using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizardingWorld.Content.DamageClasses;

namespace WizardingWorld.Content.Items.BossLoot.Horntail
{
	/// <summary>
	/// Dragon Heart — Expert-exclusive accessory from the Hungarian Horntail boss bag.
	/// Grants fire/lava immunity, +12% spell damage, and a fire trail when moving fast.
	/// "Dragon heartstring produces wands of the most power."
	/// </summary>
	public class DragonHeart : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 10);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.OnFire3] = true;
			player.buffImmune[BuffID.Burning] = true;
			player.lavaImmune = true;
			player.GetDamage(ModContent.GetInstance<SpellDamage>()) += 0.12f;

			// Fire trail when moving fast
			if (player.velocity.Length() > 5f && Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(player.Bottom, player.width, 4, DustID.Torch, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}
	}
}
