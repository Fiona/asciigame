using System;

using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;

namespace asciigame
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var window = new Core.Window {IsFixedTimeStep = false};
            window.Run();
        }
    }
}