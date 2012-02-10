using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtectIt
{
    public class Tile
    {
        public int MapX { get; set; }
        public int MapY { get; set; }

        public Vector2 Position { get; set; }
        public TileType Type { get; set; }

        public List<Tile> Ancestors { get; set; }
        public Tile Next { get; set; }

        public bool IsVisited { get; set; }
        public double DistanceToExit { get; set; }

        public IEnumerable<Tile> GetNeighbours(TileMap map)
        {
            if (this.MapX > 0) yield return map[this.MapX - 1, this.MapY];
            if (this.MapX < map.Width - 1) yield return map[this.MapX + 1, this.MapY];
            if (this.MapY > 0) yield return map[this.MapX, this.MapY - 1];
            if (this.MapY < map.Height - 1) yield return map[this.MapX, this.MapY + 1];
        }

        public void UpdateAdjacentTile(Tile adjacentTile)
        {
            if (adjacentTile.IsVisited) return;
            if (adjacentTile.Type == TileType.Wall) return;

            // Compute adjacente tile distance to exit
            double distance = this.DistanceToExit + adjacentTile.Type.Weight;
            if (distance < adjacentTile.DistanceToExit)
            {
                // Udpate distance
                adjacentTile.DistanceToExit = distance;

                // Reallocate path
                if (adjacentTile.Next != null)
                    adjacentTile.Next.Ancestors.Remove(adjacentTile);

                adjacentTile.Next = this;
                this.Ancestors.Add(adjacentTile);
            }
        }

        public Tile()
        {
            this.Position = Vector2.Zero;
            this.Type = TileType.Free;
            this.IsVisited = false;
            this.DistanceToExit = double.MaxValue;
            this.Ancestors = new List<Tile>();
        }
    }
}
