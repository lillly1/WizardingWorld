using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WizardingWorld.Content.Tiles.Landmarks
{
    /// <summary>
    /// Castle Ward Stone — 2x2 decorative ward stone.
    /// Battle of Hogwarts defense identity marker.
    /// </summary>
    public class CastleWardStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(140, 140, 160), CreateMapEntryName());
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
