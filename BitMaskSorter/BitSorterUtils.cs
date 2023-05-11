using System.Collections.Generic;

namespace BitMaskSorter
{
    internal static class BitSorterUtils
    {
        public static (int, int) CalculateMaskParts(int[] array, int start, int endP1)
        {
            var pMask = 0x00000000;
            var iMask = 0x00000000;
            for (var i = start; i < endP1; i++)
            {
                var e = array[i];
                pMask = pMask | e;
                iMask = iMask | ~e;
            }

            return (pMask, iMask);
        }

        public static int[] GetMaskAsArray(int mask)
        {
            var list = new List<int>();
            for (var i = 31; i >= 0; i--)
            {
                if (((mask >> i) & 1) == 1)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }
    }
}