using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtectIt
{
    public class FrameRateCounter : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        Vector2 textPosition;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game)
            : base(game)
        {
            this.Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Game.Services.AddService(typeof(FrameRateCounter), this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.spriteFont = this.Game.Content.Load<SpriteFont>("FrameRateFont");

            Vector2 textSize = spriteFont.MeasureString("FPS: 1000");
            this.textPosition = new Vector2(this.Game.GraphicsDevice.Viewport.Width - textSize.X, 2);
        }

        public override void Update(GameTime gameTime)
        {
            this.elapsedTime += gameTime.ElapsedGameTime;

            if (this.elapsedTime > TimeSpan.FromSeconds(1))
            {
                this.elapsedTime -= TimeSpan.FromSeconds(1);
                this.frameRate = this.frameCounter;
                this.frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.frameCounter++;

            if (!this.Visible) return;

            string fps = string.Format("FPS: {0}", this.frameRate);

            this.spriteBatch.Begin();
            this.spriteBatch.DrawString(spriteFont, fps, this.textPosition, Color.White);

            this.spriteBatch.End();
        }
    }
}
