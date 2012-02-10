using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProtectIt
{
    public static class GameExtensions
    {
        public static TService GetService<TService>(this GameServiceContainer serviceContainer)
        {
            object service = serviceContainer.GetService(typeof(TService));
            return (TService)service;
        }
    }
}
