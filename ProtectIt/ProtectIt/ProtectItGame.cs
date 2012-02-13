using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProtectIt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ProtectItGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D cursorTexture;

        private TileMap tileMap;

        public ProtectItGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Debug component
            this.Components.Add(new DebugComponent(this));

            // Controller components
            this.Components.Add(new MouseManager(this));
            this.Components.Add(new KeyboardManager(this));

            // TileMap
            this.tileMap = new TileMap(this, 60, 40);
            this.Components.Add(tileMap);

            // FPS counter
            this.Components.Add(new FrameRateCounter(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            cursorTexture = this.Content.Load<Texture2D>("Cursor");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseManager mouseManager = this.Services.GetService<MouseManager>();
            KeyboardManager keyboardManager = this.Services.GetService<KeyboardManager>();

            // Allows the game to exit
            if (keyboardManager.CurrentState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyboardManager.IsKeyPressed(Keys.R) && keyboardManager.CurrentState.IsKeyDown(Keys.LeftControl))
            {
                this.Components.Remove(this.tileMap);
                this.tileMap = new TileMap(this, 60, 40);
                this.Components.Add(this.tileMap);
            }

            if (mouseManager.CurrentState.X > 0 && mouseManager.CurrentState.X < GraphicsDevice.Viewport.Width && mouseManager.CurrentState.Y > 0 && mouseManager.CurrentState.Y < GraphicsDevice.Viewport.Height)
            {
                Tile hitTile = tileMap.GetTileAt(mouseManager.CurrentState.X, mouseManager.CurrentState.Y);
                tileMap.HoverTile = hitTile;

                if (hitTile != null)
                {
                    if (mouseManager.LeftClick)
                    {
                        if (keyboardManager.CurrentState.IsKeyDown(Keys.LeftControl))
                        {
                            if (hitTile.Type == TileType.Free)
                                this.tileMap.AddWall(hitTile.MapX, hitTile.MapY);
                            else if (hitTile.Type == TileType.Wall)
                                this.tileMap.RemoveWall(hitTile.MapX, hitTile.MapY);
                        }
                        else
                        {
                            this.tileMap.PathOrigin = hitTile;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            MouseManager mouseManager = this.Services.GetService<MouseManager>();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(cursorTexture, new Vector2(mouseManager.CurrentState.X, mouseManager.CurrentState.Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
