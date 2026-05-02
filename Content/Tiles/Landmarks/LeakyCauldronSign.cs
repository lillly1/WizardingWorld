using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WizardingWorld.Content.Tiles.Landmarks
{
    /// <summary>
    /// Leaky Cauldron Sign — 2x2 pub sign.
    /// Diagon Alley gateway identity marker.
    /// </summary>
    public class LeakyCauldronSign : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 90, 60), CreateMapEntryName());
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
