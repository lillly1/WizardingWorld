using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WizardingWorld.Content.Tiles.Landmarks
{
    /// <summary>
    /// Gringotts Facade — 3x3 marble bank facade prop.
    /// Gringotts Wizarding Bank identity marker.
    /// </summary>
    public class GringottsFacade : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(230, 220, 200), CreateMapEntryName());
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
