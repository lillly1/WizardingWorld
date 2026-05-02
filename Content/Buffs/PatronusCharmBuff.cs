using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Content.Buffs
{
	/// <summary>Patronus Charm active — silver guardian protects you.</summary>
	public class PatronusCharmBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.PatronusGuardian>()] > 0)
				player.buffTime[buffIndex] = 18000;
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}

			// Passive defense while Patronus is active
			player.statDefense += 10;
			player.lifeRegen += 2;
		}
	}
}
