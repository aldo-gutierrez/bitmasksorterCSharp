using System;

namespace BitMaskSorter
{
    internal static class SorterConstants
    {
        public static int RadixSortMaxBits = 8;

        static SorterConstants()
        {
            var cores = Environment.ProcessorCount;
            if (cores <= 4)
            {
                RadixSortMaxBits = 8;
            }
            else if (cores <= 6)
            {
                RadixSortMaxBits = 9;
            }
            else if (cores <= 8)
            {
                RadixSortMaxBits = 10;
            }
            else if (cores <= 16)
            {
                RadixSortMaxBits = 11;
            }
            else
            {
                RadixSortMaxBits = 12;
            }
        }
    }
}