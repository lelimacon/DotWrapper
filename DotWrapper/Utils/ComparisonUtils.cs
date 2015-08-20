using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace DotWrapper.Utils
{
    internal static class ComparisonUtils
    {
        [Pure]
        public static bool DeepEquals<T>(this T left, T right)
        {
            return left.Equals(right);
        }

        [Pure]
        public static bool DeepEquals<T>(this T[] left, T[] right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;
            if (left.Length != right.Length)
                return false;
            for (int i = 0; i < left.Length; i++)
                if (!ComparisonUtils.DeepEquals((dynamic)left[i], (dynamic)right[i]))
                    return false;
            return true;
        }

        [Pure]
        public static bool DeepEquals<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;
            IEnumerator<T> e1 = left.GetEnumerator();
            IEnumerator<T> e2 = right.GetEnumerator();
            bool m1 = false, m2 = false;
            do
            {
                if (!DeepEquals((dynamic) e1.Current, (dynamic) e2.Current))
                    return false;
            } while ((m1 = e1.MoveNext()) && (m2 = e2.MoveNext()));
            return (m1 == m2);
        }
    }
}
