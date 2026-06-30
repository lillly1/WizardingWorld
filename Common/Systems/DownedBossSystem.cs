using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
	public class DownedBossSystem : ModSystem
	{
		public static bool downedTroll;
		public static bool downedQuirrell;
		public static bool downedBasilisk;
		public static bool downedAragog;
		public static bool downedFluffy;
		public static bool downedHorntail;
		public static bool downedBellatrix;
		public static bool downedFenrir;
		public static bool downedUmbridge;
		public static bool downedVoldemort;
		public static bool downedBartyCrouch;
		public static bool downedDementorKing;

		public override void ClearWorld()
		{
			downedTroll = false;
			downedQuirrell = false;
			downedBasilisk = false;
			downedAragog = false;
			downedFluffy = false;
			downedHorntail = false;
			downedBellatrix = false;
			downedFenrir = false;
			downedUmbridge = false;
			downedVoldemort = false;
			downedBartyCrouch = false;
			downedDementorKing = false;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["downedTroll"] = downedTroll;
			tag["downedQuirrell"] = downedQuirrell;
			tag["downedBasilisk"] = downedBasilisk;
			tag["downedAragog"] = downedAragog;
			tag["downedFluffy"] = downedFluffy;
			tag["downedHorntail"] = downedHorntail;
			tag["downedBellatrix"] = downedBellatrix;
			tag["downedFenrir"] = downedFenrir;
			tag["downedUmbridge"] = downedUmbridge;
			tag["downedVoldemort"] = downedVoldemort;
			tag["downedBartyCrouch"] = downedBartyCrouch;
			tag["downedDementorKing"] = downedDementorKing;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			downedTroll = tag.GetBool("downedTroll");
			downedQuirrell = tag.GetBool("downedQuirrell");
			downedBasilisk = tag.GetBool("downedBasilisk");
			downedAragog = tag.GetBool("downedAragog");
			downedFluffy = tag.GetBool("downedFluffy");
			downedHorntail = tag.GetBool("downedHorntail");
			downedBellatrix = tag.GetBool("downedBellatrix");
			downedFenrir = tag.GetBool("downedFenrir");
			downedUmbridge = tag.GetBool("downedUmbridge");
			downedVoldemort = tag.GetBool("downedVoldemort");
			downedBartyCrouch = tag.GetBool("downedBartyCrouch");
			downedDementorKing = tag.GetBool("downedDementorKing");
		}

		public override void NetSend(BinaryWriter writer)
		{
			var flags1 = new BitsByte();
			flags1[0] = downedTroll;
			flags1[1] = downedQuirrell;
			flags1[2] = downedBasilisk;
			flags1[3] = downedAragog;
			flags1[4] = downedFluffy;
			flags1[5] = downedHorntail;
			flags1[6] = downedBellatrix;
			flags1[7] = downedFenrir;
			writer.Write(flags1);

			var flags2 = new BitsByte();
			flags2[0] = downedUmbridge;
			flags2[1] = downedVoldemort;
			flags2[2] = downedBartyCrouch;
			flags2[3] = downedDementorKing;
			writer.Write(flags2);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags1 = reader.ReadByte();
			downedTroll = flags1[0];
			downedQuirrell = flags1[1];
			downedBasilisk = flags1[2];
			downedAragog = flags1[3];
			downedFluffy = flags1[4];
			downedHorntail = flags1[5];
			downedBellatrix = flags1[6];
			downedFenrir = flags1[7];

			BitsByte flags2 = reader.ReadByte();
			downedUmbridge = flags2[0];
			downedVoldemort = flags2[1];
			downedBartyCrouch = flags2[2];
			downedDementorKing = flags2[3];
		}
	}

	public static class WizardConditions
	{
		public static bool AnyMechBossDowned =>
			NPC.downedMechBossAny || NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3;

		public static bool UmbridgeGateOpen =>
			Main.hardMode && AnyMechBossDowned && DownedBossSystem.downedHorntail;

		public static bool FenrirGateOpen =>
			Main.hardMode && DownedBossSystem.downedUmbridge;

		public static bool BellatrixGateOpen =>
			Main.hardMode && DownedBossSystem.downedFenrir && NPC.downedPlantBoss;

		public static bool BartyGateOpen =>
			Main.hardMode && DownedBossSystem.downedBellatrix && NPC.downedPlantBoss;

		public static bool DementorGateOpen =>
			Main.hardMode && DownedBossSystem.downedBartyCrouch && NPC.downedGolemBoss;

		public static readonly Condition DownedBasilisk =
			new Condition("Mods.WizardingWorld.Conditions.DownedBasilisk", () => DownedBossSystem.downedBasilisk);

		public static readonly Condition DownedAnyMechBoss =
			new Condition("Mods.WizardingWorld.Conditions.DownedAnyMechBoss", () => AnyMechBossDowned);

		public static readonly Condition DownedHorntail =
			new Condition("Mods.WizardingWorld.Conditions.DownedHorntail", () => DownedBossSystem.downedHorntail);

		public static readonly Condition DownedHorntailAndAnyMechBoss =
			new Condition("Mods.WizardingWorld.Conditions.DownedHorntailAndAnyMechBoss", () => UmbridgeGateOpen);

		public static readonly Condition DownedUmbridge =
			new Condition("Mods.WizardingWorld.Conditions.DownedUmbridge", () => DownedBossSystem.downedUmbridge);

		public static readonly Condition DownedVoldemort =
			new Condition("Mods.WizardingWorld.Conditions.DownedVoldemort", () => DownedBossSystem.downedVoldemort);

		public static readonly Condition DownedFenrir =
			new Condition("Mods.WizardingWorld.Conditions.DownedFenrir", () => DownedBossSystem.downedFenrir);

		public static readonly Condition PostFenrirAndPlantera =
			new Condition("Mods.WizardingWorld.Conditions.PostFenrirAndPlantera", () => BellatrixGateOpen);

		public static readonly Condition PostBellatrixAndPlantera =
			new Condition("Mods.WizardingWorld.Conditions.PostBellatrixAndPlantera", () => BartyGateOpen);

		public static readonly Condition PostBartyAndGolem =
			new Condition("Mods.WizardingWorld.Conditions.PostBartyAndGolem", () => DementorGateOpen);
	}
}
