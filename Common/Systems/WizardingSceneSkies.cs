using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace WizardingWorld.Common.Systems
{
	public class WizardingLayeredSky : CustomSky
	{
		private readonly string farPath;
		private readonly string middlePath;
		private readonly string closePath;
		private readonly Color tint;
		private readonly float opacity;

		private Asset<Texture2D> farTexture;
		private Asset<Texture2D> middleTexture;
		private Asset<Texture2D> closeTexture;
		private bool active;
		private float intensity;

		public WizardingLayeredSky(string farPath, string middlePath, string closePath, Color tint, float opacity)
		{
			this.farPath = farPath;
			this.middlePath = middlePath;
			this.closePath = closePath;
			this.tint = tint;
			this.opacity = opacity;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			active = true;
		}

		public override void Deactivate(params object[] args)
		{
			active = false;
		}

		public override void Reset()
		{
			active = false;
			intensity = 0f;
		}

		public override bool IsActive()
		{
			return active || intensity > 0f;
		}

		public override void Update(GameTime gameTime)
		{
			float target = active ? 1f : 0f;
			intensity = MathHelper.Lerp(intensity, target, 0.06f);
			if (!active && intensity < 0.01f)
				intensity = 0f;
		}

		public override Color OnTileColor(Color inColor)
		{
			return Color.Lerp(inColor, tint, 0.08f * intensity);
		}

		public override float GetCloudAlpha()
		{
			return 1f - 0.55f * intensity;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (intensity <= 0f || Main.gameMenu)
				return;

			EnsureTextures();

			DrawLayer(spriteBatch, farTexture.Value, 0.12f, 0.82f);
			DrawLayer(spriteBatch, middleTexture.Value, 0.24f, 0.92f);
			DrawLayer(spriteBatch, closeTexture.Value, 0.36f, 1f);
		}

		private void EnsureTextures()
		{
			farTexture ??= ModContent.Request<Texture2D>(farPath, AssetRequestMode.ImmediateLoad);
			middleTexture ??= ModContent.Request<Texture2D>(middlePath, AssetRequestMode.ImmediateLoad);
			closeTexture ??= ModContent.Request<Texture2D>(closePath, AssetRequestMode.ImmediateLoad);
		}

		private void DrawLayer(SpriteBatch spriteBatch, Texture2D texture, float parallax, float scaleMultiplier)
		{
			float scale = Main.screenHeight / (float)texture.Height * scaleMultiplier;
			int drawWidth = (int)(texture.Width * scale);
			int drawHeight = (int)(texture.Height * scale);
			int y = Main.screenHeight - drawHeight;
			float scroll = Main.screenPosition.X * parallax;
			int startX = -(int)(scroll % drawWidth) - drawWidth;
			Color color = Color.White * (opacity * intensity);

			for (int x = startX; x < Main.screenWidth + drawWidth; x += drawWidth)
				spriteBatch.Draw(texture, new Rectangle(x, y, drawWidth, drawHeight), color);
		}
	}

	public static class WizardingSurfaceSceneRules
	{
		public const string SurfaceSkyKey = "WizardingWorld:SurfaceScenes";

		public static bool ShouldShow(Player player)
		{
			return !Main.gameMenu
				&& player != null
				&& player.active
				&& !player.dead
				&& player.ZoneOverworldHeight
				&& !player.ZoneSkyHeight;
		}
	}

	public class WizardingSurfaceSceneSky : CustomSky
	{
		private const int SceneCount = 5;

		private static readonly string[] ScenePaths =
		{
			"WizardingWorld/Assets/Backgrounds/ScenePlatformNineThreeQuarters",
			"WizardingWorld/Assets/Backgrounds/SceneBlackLakeDock",
			"WizardingWorld/Assets/Backgrounds/SceneHogwartsCastle",
			"WizardingWorld/Assets/Backgrounds/SceneQuidditchPitch",
			"WizardingWorld/Assets/Backgrounds/SceneForbiddenForest"
		};

		private static readonly Color[] SceneTints =
		{
			new Color(80, 68, 58),
			new Color(45, 70, 92),
			new Color(68, 58, 84),
			new Color(72, 86, 52),
			new Color(28, 70, 42)
		};

		private readonly Asset<Texture2D>[] textures = new Asset<Texture2D>[SceneCount];
		private bool active;
		private float intensity;

		public override void Activate(Vector2 position, params object[] args)
		{
			active = true;
		}

		public override void Deactivate(params object[] args)
		{
			active = false;
		}

		public override void Reset()
		{
			active = false;
			intensity = 0f;
		}

		public override bool IsActive()
		{
			return active || intensity > 0f;
		}

		public override void Update(GameTime gameTime)
		{
			float target = active ? 1f : 0f;
			intensity = MathHelper.Lerp(intensity, target, 0.08f);
			if (!active && intensity < 0.01f)
				intensity = 0f;
		}

		public override Color OnTileColor(Color inColor)
		{
			Player player = Main.LocalPlayer;
			Color tint = SceneTints[GetPrimarySceneIndex(player)];
			return Color.Lerp(inColor, tint, 0.06f * intensity);
		}

		public override float GetCloudAlpha()
		{
			return 1f - 0.5f * intensity;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (intensity <= 0f || Main.gameMenu)
				return;

			EnsureTextures();

			Player player = Main.LocalPlayer;
			float worldProgress = GetWorldProgress(player);
			float[] weights = GetSceneWeights(player, worldProgress);

			for (int i = 0; i < SceneCount; i++)
			{
				if (weights[i] > 0.01f)
					DrawScene(spriteBatch, textures[i].Value, weights[i] * intensity, worldProgress);
			}
		}

		private void EnsureTextures()
		{
			for (int i = 0; i < SceneCount; i++)
				textures[i] ??= ModContent.Request<Texture2D>(ScenePaths[i], AssetRequestMode.ImmediateLoad);
		}

		private static float[] GetSceneWeights(Player player, float worldProgress)
		{
			float[] weights = new float[SceneCount];
			if (TryGetForcedScene(player, out int forcedScene))
			{
				weights[forcedScene] = 1f;
				return weights;
			}

			weights[GetSceneIndexForProgress(worldProgress)] = 1f;
			return weights;
		}

		private static int GetPrimarySceneIndex(Player player)
		{
			if (TryGetForcedScene(player, out int forcedScene))
				return forcedScene;

			return GetSceneIndexForProgress(GetWorldProgress(player));
		}

		private static int GetSceneIndexForProgress(float worldProgress)
		{
			return Math.Clamp((int)(MathHelper.Clamp(worldProgress, 0f, 0.9999f) * SceneCount), 0, SceneCount - 1);
		}

		private static bool TryGetForcedScene(Player player, out int scene)
		{
			if (player != null)
			{
				if (player.InModBiome<Content.Biomes.ForbiddenForestBiome>() || ForestExpeditionSystem.missionActive)
				{
					scene = 4;
					return true;
				}

				if (QuidditchCupSystem.matchActive)
				{
					scene = 3;
					return true;
				}

				if (BattleOfHogwartsSystem.battleActive)
				{
					scene = 2;
					return true;
				}

				if (player.ZoneBeach || player.wet)
				{
					scene = 1;
					return true;
				}
			}

			scene = 0;
			return false;
		}

		private static float GetWorldProgress(Player player)
		{
			float worldWidth = Main.maxTilesX * 16f;
			if (worldWidth <= 0f)
				return 0.5f;

			float x = player?.Center.X ?? Main.screenPosition.X + Main.screenWidth * 0.5f;
			return MathHelper.Clamp(x / worldWidth, 0f, 1f);
		}

		private static void DrawScene(SpriteBatch spriteBatch, Texture2D texture, float alpha, float worldProgress)
		{
			float scale = Math.Max(Main.screenWidth / (float)texture.Width, Main.screenHeight / (float)texture.Height);
			int drawWidth = (int)(texture.Width * scale);
			int drawHeight = (int)(texture.Height * scale);
			int extraWidth = Math.Max(0, drawWidth - Main.screenWidth);
			int x = -Math.Clamp((int)(extraWidth * worldProgress), 0, extraWidth);
			int y = Main.screenHeight - drawHeight;
			Color color = GetTimeTint() * MathHelper.Clamp(alpha, 0f, 1f);

			spriteBatch.Draw(texture, new Rectangle(x, y, drawWidth, drawHeight), color);
		}

		private static Color GetTimeTint()
		{
			return Main.dayTime ? Color.White : new Color(165, 175, 205);
		}
	}

	public class WizardingSingleSceneSky : CustomSky
	{
		private readonly string texturePath;
		private readonly Color tint;
		private readonly float opacity;
		private Asset<Texture2D> texture;
		private bool active;
		private float intensity;

		public WizardingSingleSceneSky(string texturePath, Color tint, float opacity)
		{
			this.texturePath = texturePath;
			this.tint = tint;
			this.opacity = opacity;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			active = true;
		}

		public override void Deactivate(params object[] args)
		{
			active = false;
		}

		public override void Reset()
		{
			active = false;
			intensity = 0f;
		}

		public override bool IsActive()
		{
			return active || intensity > 0f;
		}

		public override void Update(GameTime gameTime)
		{
			float target = active ? 1f : 0f;
			intensity = MathHelper.Lerp(intensity, target, 0.08f);
			if (!active && intensity < 0.01f)
				intensity = 0f;
		}

		public override Color OnTileColor(Color inColor)
		{
			return Color.Lerp(inColor, tint, 0.08f * intensity);
		}

		public override float GetCloudAlpha()
		{
			return 1f - 0.55f * intensity;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (intensity <= 0f || Main.gameMenu)
				return;

			texture ??= ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad);
			float worldProgress = Main.maxTilesX > 0 ? MathHelper.Clamp((Main.screenPosition.X + Main.screenWidth * 0.5f) / (Main.maxTilesX * 16f), 0f, 1f) : 0.5f;
			float scale = Math.Max(Main.screenWidth / (float)texture.Value.Width, Main.screenHeight / (float)texture.Value.Height);
			int drawWidth = (int)(texture.Value.Width * scale);
			int drawHeight = (int)(texture.Value.Height * scale);
			int extraWidth = Math.Max(0, drawWidth - Main.screenWidth);
			int x = -Math.Clamp((int)(extraWidth * worldProgress), 0, extraWidth);
			int y = Main.screenHeight - drawHeight;
			Color color = (Main.dayTime ? Color.White : new Color(165, 175, 205)) * (opacity * intensity);

			spriteBatch.Draw(texture.Value, new Rectangle(x, y, drawWidth, drawHeight), color);
		}
	}
}
