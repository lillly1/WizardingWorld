using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	public class ForbiddenForestSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
	{
		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
				fades[i] = i == Slot ? MathHelper.Clamp(fades[i] + transitionSpeed, 0f, 1f) : MathHelper.Clamp(fades[i] - transitionSpeed, 0f, 1f);
		}

		public override int ChooseFarTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/SceneForbiddenForest");
		}

		public override int ChooseMiddleTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/SceneForbiddenForest");
		}

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			scale = 1f;
			parallax = 0.42;
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/SceneForbiddenForest");
		}
	}

	public class BattleOfHogwartsSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
	{
		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
				fades[i] = i == Slot ? MathHelper.Clamp(fades[i] + transitionSpeed, 0f, 1f) : MathHelper.Clamp(fades[i] - transitionSpeed, 0f, 1f);
		}

		public override int ChooseFarTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/SceneHogwartsCastle");
		}

		public override int ChooseMiddleTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/SceneHogwartsCastle");
		}

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			scale = 1f;
			parallax = 0.38;
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/SceneHogwartsCastle");
		}
	}

	public class WizardingGlobalBackgroundStyle : GlobalBackgroundStyle
	{
		public override void ChooseSurfaceBackgroundStyle(ref int style)
		{
			if (Main.dedServ || Main.gameMenu)
				return;

			Player player = Main.LocalPlayer;
			if (player == null || !player.active)
				return;

			if (BattleOfHogwartsSystem.battleActive)
			{
				style = ModContent.GetInstance<BattleOfHogwartsSurfaceBackgroundStyle>().Slot;
				return;
			}

			if (player.InModBiome<Content.Biomes.ForbiddenForestBiome>())
				style = ModContent.GetInstance<ForbiddenForestSurfaceBackgroundStyle>().Slot;
		}
	}
}
