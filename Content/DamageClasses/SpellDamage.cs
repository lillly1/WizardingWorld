using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.DamageClasses
{
	public class SpellDamage : DamageClass
	{
		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
		{
			if (damageClass == DamageClass.Generic)
				return StatInheritanceData.Full;

			return StatInheritanceData.None;
		}

		public override bool GetEffectInheritance(DamageClass damageClass)
		{
			return false;
		}

		public override void SetDefaultStats(Player player)
		{
			player.GetCritChance(this) += 4;
		}
	}
}
