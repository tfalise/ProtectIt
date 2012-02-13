using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtectIt
{
    internal class DebugComponent : DrawableGameComponent
    {
        private SpriteFont spriteFont;
        private SpriteBatch spriteBatch;
        private Vector2 textPosition;

        private List<string> messages;

        public DebugComponent(Game game)
            : base(game)
        {
            // Debug not visible by default
            this.Visible = false;
            this.messages = new List<string>(5);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Game.Services.AddService(typeof(DebugComponent), this);

            // Compute text position
            this.textPosition = new Vector2(5, this.GraphicsDevice.Viewport.Height - 6 * this.spriteFont.LineSpacing);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            this.spriteFont = this.Game.Content.Load<SpriteFont>("DebugFont");
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.Visible) return;

            this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            Vector2 offset = Vector2.Zero;
            this.spriteBatch.DrawString(this.spriteFont, "[Debug]", this.textPosition + offset, Color.White);

            foreach (string message in this.messages)
            {
                offset += new Vector2(0, this.spriteFont.LineSpacing);
                this.spriteBatch.DrawString(this.spriteFont, message, this.textPosition + offset, Color.White);
            }

            this.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardManager keyboardManager = this.Game.Services.GetService<KeyboardManager>();

            // Switch visible status on CTRL + D
            if (keyboardManager.IsKeyPressed(Keys.D) && keyboardManager.CurrentState.IsKeyDown(Keys.LeftControl))
            {
                this.Visible = !this.Visible;
            }
        }

        internal void Print(string message)
        {
            if (messages.Count == 5)
                this.messages.RemoveAt(0);

            this.messages.Add(message);
        }

        internal void PrintFormat(string format, params object[] args)
        {
            this.Print(string.Format(format, args));
        }

    }
}
