using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;

namespace DotWrapper.Utils
{
    internal static class ArrayUtils
    {
        /// <summary>
        ///     Checks whether an array is null or has any elements.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">the array to check</param>
        /// <returns>true if the array is null or empty</returns>
        [Pure]
        public static bool IsNullOrEmpty<T>(T[] array)
        {
            return (array == null || array.Length == 0);
        }

        /// <summary>
        ///     Copies the current array starting from startIndex to the end.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">the array to copy</param>
        /// <param name="startIndex">the index to start from in the given array</param>
        /// <returns>the new array</returns>
        [Pure]
        public static T[] Subset<T>(this T[] array, int startIndex)
        {
            Debug.Assert(!IsNullOrEmpty(array));
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex < array.Length);
            long count = array.Length - startIndex;
            var newArray = new T[count];
            Array.Copy(array, startIndex, newArray, 0, count);
            return newArray;
        }

        /// <summary>
        ///     Copies the current array starting from startIndex with the specified size.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">the array to copy</param>
        /// <param name="startIndex">the index to start from in the given array</param>
        /// <param name="count">the number of elements to copy</param>
        /// <returns>the new array</returns>
        [Pure]
        public static T[] Subset<T>(this T[] array, int startIndex, int count)
        {
            Debug.Assert(!IsNullOrEmpty(array));
            Debug.Assert(startIndex >= 0 && count >= 0);
            Debug.Assert(startIndex + count < array.Length);
            var newArray = new T[count];
            Array.Copy(array, startIndex, newArray, 0, count);
            return newArray;
        }

        /// <summary>
        ///     Compares t
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static bool ShallowEquals<T>(this T[] left, T[] right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;
            if (left.Length != right.Length)
                return false;
            return !left.Where((t, i) => !t.Equals(right[i])).Any();
        }

        /// <summary>
        ///     Replaces all occurrences of a value in an array.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The model array.</param>
        /// <param name="oldValue">Value to be replaced.</param>
        /// <param name="newValue">The new value to replace oldValue with.</param>
        /// <returns>The new array with replaced values.</returns>
        [Pure]
        public static T[] Replace<T>(this T[] array, T oldValue, T newValue)
        {
            Debug.Assert(array != null);
            // As we can see here, speed is not our first priority...
            return (from e in array select e.DeepEquals(oldValue) ? newValue : e).ToArray();
        }
    }
}
