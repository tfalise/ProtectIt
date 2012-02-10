using System;

namespace ProtectIt
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ProtectItGame game = new ProtectItGame())
            {
                game.Run();
            }
        }
    }
#endif
}

