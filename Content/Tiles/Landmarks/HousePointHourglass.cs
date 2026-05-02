using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WizardingWorld.Content.Tiles.Landmarks
{
    /// <summary>
    /// House Point Hourglass — 2x3 decorative landmark.
    /// Great Hall identity prop. The four hourglasses that track house points.
    /// </summary>
    public class HousePointHourglass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 180, 100), CreateMapEntryName());
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
