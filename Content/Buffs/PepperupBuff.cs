using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Pepperup Potion effect — massive speed boost, fire immunity, steam from ears.</summary>
	public class PepperupBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.moveSpeed += 0.25f;
			player.maxRunSpeed += 3f;
			player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.OnFire3] = true;
			player.buffImmune[BuffID.Chilled] = true;
			player.buffImmune[BuffID.Frozen] = true;

			// Steam from ears visual
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(
					player.position + new Vector2(Main.rand.NextBool() ? 2 : player.width - 4, 4),
					4, 4, DustID.Smoke, 0f, -2f, 100, default, 0.8f);
				dust.noGravity = true;
			}
		}
	}
}
