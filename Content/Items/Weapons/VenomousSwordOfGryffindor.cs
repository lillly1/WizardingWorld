using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.Weapons
{
	/// <summary>
	/// Venomous Sword of Gryffindor — upgraded with Spider Silk Weave + Basilisk Fang.
	/// Now swings with AoE poison cloud and lifesteal. The sword that truly imbibes.
	/// "Goblin-made — it imbibes that which makes it stronger. Now with spider venom too."
	/// </summary>
	public class VenomousSwordOfGryffindor : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 52;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 78;
			Item.knockBack = 7f;
			Item.crit = 12;
			Item.value = Item.buyPrice(gold: 25);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Triple venom — Basilisk + Spider + regular
			target.AddBuff(BuffID.Venom, 300);
			target.AddBuff(BuffID.Poisoned, 600);
			target.AddBuff(ModContent.BuffType<Buffs.Debuffs.JinxedDebuff>(), 180);

			// AoE poison cloud — damages nearby enemies
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.whoAmI != target.whoAmI && !npc.friendly && npc.CanBeChasedBy()
					&& Vector2.Distance(npc.Center, target.Center) < 100f)
				{
					npc.AddBuff(BuffID.Venom, 180);
					player.ApplyDamageToNPC(npc, damageDone / 4, 4f, hit.HitDirection, false);
				}
			}

			// Lifesteal — 5% of damage
			int heal = System.Math.Max(1, damageDone / 20);
			heal = System.Math.Min(heal, 8);
			player.statLife = System.Math.Min(player.statLife + heal, player.statLifeMax2);
			player.HealEffect(heal);
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(2))
			{
				int dustType = Main.rand.NextBool() ? DustID.GemRuby : DustID.GreenTorch;
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType, 0f, 0f, 100, default, 1.0f);
				dust.noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<SwordOfGryffindor>(), 1)
				.AddIngredient(ModContent.ItemType<Consumables.SpiderSilkWeave>(), 8)
				.AddIngredient(ModContent.ItemType<Accessories.BasiliskFang>(), 2)
				.AddIngredient(ModContent.ItemType<Consumables.EssenceOfMagic>(), 15)
				.AddTile<Tiles.EnchantingTable>()
				.Register();
		}
	}
}
