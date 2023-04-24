using System.Collections.Generic;

namespace BitMaskSorter
{
    internal class BitSorterUtils
    {
        public static (int, int) CalculateMaskParts(int[] array, int start, int endP1)
        {
            int pMask = 0x00000000;
            int iMask = 0x00000000;
            for (int i = start; i < endP1; i++)
            {
                int e = array[i];
                pMask = pMask | e;
                iMask = iMask | (~e);
            }

            return (pMask, iMask);
        }

        public static int[] GetMaskAsArray(int mask)
        {
            List<int> list = new List<int>();
            for (int i = 31; i >= 0; i--)
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