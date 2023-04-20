using System.Collections.Generic;

namespace BitMaskSorter
{
    internal class BitSorterUtils
    {
        public static (int, int) getMaskBit(int[] array, int start, int endP1)
        {
            int mask = 0x00000000;
            int inv_mask = 0x00000000;
            for (int i = start; i < endP1; i++)
            {
                int ei = array[i];
                mask = mask | ei;
                inv_mask = inv_mask | (~ei);
            }

            return ( mask, inv_mask );
        }

        public static int[] getMaskAsArray(int mask)
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