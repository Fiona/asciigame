using System;

namespace asciigame.Core
{
    public static class ExMath
    {
        public static float ToRadians(this float val)
        {
            return (float)((Math.PI / 180) * val);
        }
    }
}