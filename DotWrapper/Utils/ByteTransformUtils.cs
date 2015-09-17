using System;

namespace DotWrapper.Utils
{
    /// <summary>
    ///     A set of byte transforms.
    ///     All transforms are involutions such as T(T(b)) = b
    /// </summary>
    public enum ByteTransform
    {
        None,
        Xor, // Invert every bit.
        EvenXor, // Invert every other bit (even position).
        OddXor, // Invert every other bit (odd position).
        Reverse
    }

    internal static class ByteTransformUtils
    {
        private static readonly byte evenMask8 = 170;
        private static readonly byte oddMask8 = 85;

        /// <summary>
        ///     Reads a byte array from a stream.
        /// </summary>
        /// <param name="b">The byte.</param>
        /// <param name="t">The function to apply to the byte.</param>
        /// <returns></returns>
        public static byte Transform(this byte b, ByteTransform t)
        {
            switch (t)
            {
                case ByteTransform.None:
                    return b;
                case ByteTransform.Xor:
                    return (byte) (255 - b);
                case ByteTransform.EvenXor:
                    return (byte) (b ^ evenMask8);
                case ByteTransform.OddXor:
                    return (byte) (b ^ oddMask8);
                case ByteTransform.Reverse:
                    return (byte) ((b * 0x0202020202UL & 0x010884422010UL) % 1023);
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }
}
