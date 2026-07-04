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
}
