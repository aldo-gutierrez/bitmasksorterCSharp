using System;
using System.Collections.Generic;
using System.Text;

namespace BitMaskSorter
{
    internal class BitSorterUtils
    {
        public static int[] getMaskBit(int[] array, int start, int end)
        {
            int mask = 0x00000000;
            int inv_mask = 0x00000000;
            for (int i = start; i < end; i++)
            {
                int ei = array[i];
                mask = mask | ei;
                inv_mask = inv_mask | (~ei);
            }
            return new int[] { mask, inv_mask };
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
            int[] res = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                res[i] = list[i];
            }
            return res;
        }

        public static int getMaskBit(int k)
        {
            return 1 << k;
        }

        public static int twoPowerX(int k)
        {
            return 1 << k;
        }


    }
}
