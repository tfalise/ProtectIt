using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProtectIt
{
    public class KeyboardManager : GameComponent
    {
        private KeyboardState previousState;
        public KeyboardState CurrentState { get; private set; }

        public KeyboardManager(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            this.Game.Services.AddService(typeof(KeyboardManager), this);
        }

        public override void Update(GameTime gameTime)
        {
            this.previousState = this.CurrentState;
            this.CurrentState = Keyboard.GetState();
        }

        public bool IsKeyPressed(Keys key)
        {
            return this.previousState.IsKeyUp(key) && this.CurrentState.IsKeyDown(key);
        }
    }
}
