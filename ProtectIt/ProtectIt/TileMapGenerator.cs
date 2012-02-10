using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProtectIt
{
    public static class TileMapGenerator
    {
        private static readonly Random random = new Random();

        public static void RandomizeTiles(TileMap tileMap)
        {
            // Randomize entry & exit
            tileMap.Entry = tileMap[random.Next(0, tileMap.Width), random.Next(0, tileMap.Height)];
            tileMap.Exit = tileMap.Entry;

            while (tileMap.Exit == tileMap.Entry)
            {
                tileMap.Exit = tileMap[random.Next(0, tileMap.Width), random.Next(0, tileMap.Height)];
            }

            tileMap.Entry.Type = TileType.Entry;
            tileMap.Exit.Type = TileType.Exit;

            AddRandomWalls(tileMap);

            tileMap.PathOrigin = tileMap.Entry;
        }

        private static void AddRandomWalls(TileMap tileMap)
        {
            do
            {
                ClearWalls(tileMap);

                foreach (Tile tile in tileMap.Tiles)
                {
                    if (tile.Type == TileType.Entry || tile.Type == TileType.Exit)
                        continue;

                    if (random.NextDouble() < 0.3)
                        tile.Type = TileType.Wall;
                }

                tileMap.UpdateTiles();
            } while (tileMap.Entry.Next == null);

            foreach (Tile tile in tileMap.Tiles)
            {
                if (tile.Type == TileType.Free && tile.Next == null)
                    tile.Type = TileType.Wall;
            }
        }

        private static void ClearWalls(TileMap tileMap)
        {
            foreach (Tile tile in tileMap.Tiles)
            {
                tile.IsVisited = false;
                tile.Ancestors.Clear();
                tile.Next = null;

                if (tile.Type == TileType.Exit)
                {
                    tile.DistanceToExit = 0;
                }
                else
                {
                    tile.DistanceToExit = double.MaxValue;
                }

                if (tile.Type == TileType.Wall)
                {
                    tile.Type = TileType.Free;
                }
            }
        }
    }
}
