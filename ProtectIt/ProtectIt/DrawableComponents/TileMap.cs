using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtectIt
{
    public class TileMap : DrawableGameComponent
    {
        private Texture2D tileTexture;
        private Texture2D hoverTexture;

        private SpriteBatch spriteBatch;

        private DebugComponent debug;

        public int Width { get; private set; }
        public int Height { get; private set; }

        internal Tile[] Tiles { get; set; }

        public LinkedList<Tile> Path { get; private set; }

        public Tile HoverTile { get; set; }

        private Tile pathOrigin;
        public Tile PathOrigin {
            get { return this.pathOrigin; }
            set
            {
                if (this.debug != null && value != null) this.debug.PrintFormat("[TileMap] Changed path origin to ({0}, {1})", value.MapX, value.MapY);

                this.pathOrigin = value;
                this.Path = this.GetPathForTile(this.pathOrigin);
            }
        }

        public Tile Entry { get; set; }
        public Tile Exit { get; set; }

        public Tile this[int x, int y]
        {
            get { return this.Tiles[y * this.Width + x]; }
        }

        public TileMap(Game game, int width, int height)
            : base(game)
        {
            this.Width = width;
            this.Height = height;
        }

        public override void Initialize()
        {
            base.Initialize();

            this.debug = this.Game.Services.GetService<DebugComponent>();

            InitializeTiles();

            TileMapGenerator.RandomizeTiles(this);
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);

            this.tileTexture = this.Game.Content.Load<Texture2D>("Tile");
            this.hoverTexture = this.Game.Content.Load<Texture2D>("HoverTile");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // Draw game tiles
            for (int h = 0; h < this.Height; h++)
            {
                for (int w = 0; w < this.Width; w++)
                {
                    Tile tile = this[w, h];

                    Color tintColor = tile.Type.Color;

                    if (tile == this.PathOrigin && tile.Type == TileType.Free)
                    {
                        tintColor = new Color(114, 159, 207);
                    }
                    else if (this.Path != null && this.Path.Contains(tile) && tile.Type == TileType.Free)
                    {
                        tintColor = new Color(252, 233, 79);
                    }

                    this.spriteBatch.Draw(this.tileTexture, tile.Position, tintColor);
                }
            }

            this.spriteBatch.End();

            this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // Draw hover tile
            if (this.HoverTile != null)
                this.spriteBatch.Draw(this.hoverTexture, this.HoverTile.Position, Color.White);

            this.spriteBatch.End();
        }

        private void InitializeTiles()
        {
            this.Tiles = new Tile[this.Width * this.Height];

            for (int h = 0; h < this.Height; h++)
            {
                for (int w = 0; w < this.Width; w++)
                {
                    Tile tile = new Tile()
                    {
                        Position = new Vector2(w * tileTexture.Width, h * tileTexture.Height),
                        Type = TileType.Free,
                        MapX = w,
                        MapY = h
                    };

                    this.Tiles[h * this.Width + w] = tile;
                }
            }
        }

        private Tile GetUpdatableTile()
        {
            // Do not udpate if we dont have an exit
            if (this.Exit == null)
                throw new Exception("Cannot update tiles if no exit exists");

            // Always start from exit if possible
            if (!this.Exit.IsVisited)
            {
                this.Exit.DistanceToExit = 0;
                return this.Exit;
            }

            // Return the non visited tile having the lowest distance from exit
            return this.Tiles
                .Where(tile => !tile.IsVisited)
                .OrderBy(tile => tile.DistanceToExit)
                .FirstOrDefault();
        }

        public void AddWall(int x, int y)
        {
            Tile targetTile = this[x, y];

            // Cannot turn entry & exit into walls
            if (targetTile.Type == TileType.Entry || targetTile.Type == TileType.Exit)
                return;

            targetTile.Type = TileType.Wall;
            targetTile.Next = null;

            this.ResetTile(targetTile);

            this.UpdateTiles();

            if (this.Path != null && this.Path.Contains(targetTile))
                this.Path = this.GetPathForTile(this.pathOrigin);
        }

        public void RemoveWall(int x, int y)
        {
            Tile targetTile = this[x, y];

            // Only walls can be removed
            if(targetTile.Type != TileType.Wall) return;
        
            // Mark the tile as not being a wall
            targetTile.Type = TileType.Free;
        
            // find next
            Tile nextTile = null;
            foreach (Tile tile in targetTile.GetNeighbours(this))
            {
                if (tile.Type == TileType.Wall) continue;
                if (nextTile == null || tile.DistanceToExit < nextTile.DistanceToExit)
                    nextTile = tile;
            }

            // If we dont have a next tile, we are surrounded by walls
            if (nextTile == null) return;

            // compute dExit = distance of tile to exit
            this.BackwardUpdate(nextTile, targetTile);
        
            // Update path
            this.Path = this.GetPathForTile(this.PathOrigin);
        }

        private void BackwardUpdate(Tile source, Tile target)
        {
            // Check if we should use the source as new path
            if(source.DistanceToExit + target.Type.Weight < target.DistanceToExit) {
                if(target.Next != null) target.Next.Ancestors.Remove(target);
                target.Next = source;
                source.Ancestors.Add(target);
                target.DistanceToExit = source.DistanceToExit + target.Type.Weight;
            
                foreach (Tile tile in target.GetNeighbours(this))
                {
                    if(tile.Type == TileType.Wall) continue;
                    if(tile == target.Next) continue;

                    this.BackwardUpdate(target, tile);
                }
            }
        }

        private LinkedList<Tile> GetPathForTile(Tile tile)
        {
            LinkedList<Tile> newPath = new LinkedList<Tile>();

            while (tile != null)
            {
                newPath.AddLast(tile);
                tile = tile.Next;
            }

            return newPath;
        }

        internal void UpdateTiles()
        {
            Tile tile = this.GetUpdatableTile();

            while (tile != null)
            {
                tile.IsVisited = true;
                foreach (Tile neighbour in tile.GetNeighbours(this))
                {
                    tile.UpdateAdjacentTile(neighbour);
                }
                tile = this.GetUpdatableTile();
            }
        }

        private void ResetTile(Tile targetTile)
        {
            targetTile.Next = null;
            targetTile.DistanceToExit = double.MaxValue;

            foreach (Tile tile in targetTile.GetNeighbours(this))
            {
                if (tile.Type != TileType.Wall)
                    tile.IsVisited = false;
            }
        
            targetTile.Ancestors.ForEach(
                tile => this.ResetTile(tile)
            );

            targetTile.Ancestors.Clear();
        }

        internal Tile GetTileAt(int x, int y)
        {
            x = x / 10;
            y = y / 10;

            if (x >= Width || x < 0) return null;
            if (y >= Height || y < 0) return null;

            return this[x, y];
        }
    }
}
