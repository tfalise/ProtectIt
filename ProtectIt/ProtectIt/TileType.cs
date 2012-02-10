using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProtectIt
{
    public struct TileType
    {
        public string Name { get; set; }
        public bool IsBlocking { get; set; }
        public double Weight { get; set; }
        public Color Color { get; set; }

        #region Constant TileTypes
        public static TileType Free
        {
            get
            {
                return new TileType
                {
                    Name = "Free",
                    IsBlocking = false,
                    Weight = 1,
                    Color = Color.White
                };
            }
        }
        public static TileType Wall
        {
            get
            {
                return new TileType
                {
                    Name = "Wall",
                    IsBlocking = true,
                    Weight = -1,
                    Color = Color.Brown
                };
            }
        }
        public static TileType Entry
        {
            get
            {
                return new TileType
                {
                    Name = "Entry",
                    IsBlocking = false,
                    Weight = 1,
                    Color = Color.LightGreen
                };
            }
        }
        public static TileType Exit
        {
            get
            {
                return new TileType
                {
                    Name = "Exit",
                    IsBlocking = false,
                    Weight = 1,
                    Color = Color.Orange
                };
            }
        }
        #endregion

        public static bool operator ==(TileType a, TileType b)
        {
            return a.Name == b.Name;
        }

        public static bool operator !=(TileType a, TileType b)
        {
            return a.Name != b.Name;
        }
    }
}
