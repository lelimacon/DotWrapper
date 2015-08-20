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
        /// <summary>
        ///     Reads a byte array from a stream.
        /// </summary>
        /// <param name="s">the stream (must be valid)</param>
        /// <param name="t">the transform to apply to the byte</param>
        /// <returns></returns>
        public static byte Transform(this byte b, ByteTransform t)
        {
            // TODO: apply transform here.
            return b;
        }
    }
}
