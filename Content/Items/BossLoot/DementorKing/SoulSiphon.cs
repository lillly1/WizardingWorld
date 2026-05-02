using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Items.BossLoot.DementorKing
{
	/// <summary>
	/// Soul Siphon — Expert-exclusive accessory from the Dementor King boss bag.
	/// All damage heals you for 3% of damage dealt. Enemies near you have -5 defense.
	/// "It feeds on despair." Dark aura visual.
	/// </summary>
	public class SoulSiphon : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.sellPrice(gold: 15);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Life steal is handled via OnHitNPCWithAnything in the ModPlayer
			player.GetModPlayer<SoulSiphonPlayer>().soulSiphonActive = true;

			// Defense aura — reduce defense of nearby enemies
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.active && !npc.friendly && !npc.dontTakeDamage
						&& Vector2.Distance(player.Center, npc.Center) < 300f)
					{
						npc.defense = System.Math.Max(0, npc.defDefense - 5);
					}
				}
			}

			// Dark aura visual
			if (!hideVisual && Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Shadowflame, 0f, 0f, 150, default, 0.7f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}
	}

	/// <summary>ModPlayer hook to handle Soul Siphon's 3% life steal on all damage.</summary>
	public class SoulSiphonPlayer : ModPlayer
	{
		public bool soulSiphonActive;

		public override void ResetEffects()
		{
			soulSiphonActive = false;
		}

		private void ApplySoulSiphon(int damageDone)
		{
			if (!soulSiphonActive)
				return;

			int healAmount = (int)(damageDone * 0.03f);
			if (healAmount < 1)
				healAmount = 1;

			Player.Heal(healAmount);
		}

		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			ApplySoulSiphon(damageDone);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			ApplySoulSiphon(damageDone);
		}
	}
}
