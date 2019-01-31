using System;
using System.Runtime.InteropServices;

namespace AltV.Net.Native
{
    internal static partial class Alt
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Position
        {
            public static Position Zero = new Position
            {
                x = 0,
                y = 0,
                z = 0
            };
            public float x;
            public float y;
            public float z;
        }
    }
}