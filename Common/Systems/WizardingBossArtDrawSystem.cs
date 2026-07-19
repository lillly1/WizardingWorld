using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizardingWorld.Content.NPCs.Bosses.Aragog;
using WizardingWorld.Content.NPCs.Bosses.BartyCrouch;
using WizardingWorld.Content.NPCs.Bosses.Basilisk;
using WizardingWorld.Content.NPCs.Bosses.Bellatrix;
using WizardingWorld.Content.NPCs.Bosses.DementorKing;
using WizardingWorld.Content.NPCs.Bosses.Fenrir;
using WizardingWorld.Content.NPCs.Bosses.Fluffy;
using WizardingWorld.Content.NPCs.Bosses.Horntail;
using WizardingWorld.Content.NPCs.Bosses.Quirrell;
using WizardingWorld.Content.NPCs.Bosses.Troll;
using WizardingWorld.Content.NPCs.Bosses.Umbridge;
using WizardingWorld.Content.NPCs.Bosses.Voldemort;

namespace WizardingWorld.Common.Systems
{
	public class WizardingBossArtDrawSystem : GlobalNPC
	{
		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (!TryGetArtLayout(npc.type, out Vector2 displayFrameSize, out int atlasColumns))
				return true;

			Texture2D texture = TextureAssets.Npc[npc.type].Value;
			int frameCount = System.Math.Max(1, Main.npcFrameCount[npc.type]);
			int atlasRows = (frameCount + atlasColumns - 1) / atlasColumns;
			int frameWidth = texture.Width / atlasColumns;
			int frameHeight = texture.Height / atlasRows;
			int logicalFrameHeight = System.Math.Max(1, texture.Height / frameCount);
			int frameIndex = System.Math.Clamp(npc.frame.Y / logicalFrameHeight, 0, frameCount - 1);
			Rectangle frame = new Rectangle(
				(frameIndex % atlasColumns) * frameWidth,
				(frameIndex / atlasColumns) * frameHeight,
				frameWidth,
				frameHeight);

			float drawScale = System.MathF.Min(displayFrameSize.X / frame.Width, displayFrameSize.Y / frame.Height) * npc.scale;
			Vector2 drawPosition = npc.Center - screenPos + new Vector2(0f, npc.gfxOffY);
			Vector2 origin = frame.Size() * 0.5f;
			SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Color spriteColor = new Color(drawColor.R, drawColor.G, drawColor.B, 255);
			spriteBatch.Draw(texture, drawPosition, frame, spriteColor, npc.rotation, origin, drawScale, effects, 0f);
			return false;
		}

		private static bool TryGetArtLayout(int type, out Vector2 size, out int atlasColumns)
		{
			atlasColumns = 4;
			if (type == ModContent.NPCType<TrollBoss>())
				size = new Vector2(300f, 340f);
			else if (type == ModContent.NPCType<QuirrellBoss>())
				size = new Vector2(185f, 285f);
			else if (type == ModContent.NPCType<BasiliskBoss>())
			{
				size = new Vector2(480f, 200f);
				atlasColumns = 3;
			}
			else if (type == ModContent.NPCType<AragogBoss>())
				size = new Vector2(360f, 290f);
			else if (type == ModContent.NPCType<FluffyBoss>())
				size = new Vector2(420f, 340f);
			else if (type == ModContent.NPCType<HorntailBoss>())
				size = new Vector2(540f, 380f);
			else if (type == ModContent.NPCType<UmbridgeBoss>())
				size = new Vector2(185f, 285f);
			else if (type == ModContent.NPCType<FenrirBoss>())
				size = new Vector2(230f, 315f);
			else if (type == ModContent.NPCType<BellatrixBoss>())
				size = new Vector2(200f, 310f);
			else if (type == ModContent.NPCType<BartyCrouchBoss>())
				size = new Vector2(200f, 310f);
			else if (type == ModContent.NPCType<DementorKingBoss>())
				size = new Vector2(320f, 430f);
			else if (type == ModContent.NPCType<VoldemortBoss>())
				size = new Vector2(570f, 840f);
			else
			{
				size = default;
				atlasColumns = 0;
				return false;
			}

			return true;
		}
	}
}
