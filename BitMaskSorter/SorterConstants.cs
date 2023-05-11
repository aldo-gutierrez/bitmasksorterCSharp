using System;

namespace BitMaskSorter
{
    internal static class SorterConstants
    {
        public static int MaxBitsRadixSort = 8;

        static SorterConstants()
        {
            var cores = Environment.ProcessorCount;
            if (cores <= 4)
            {
                //8bits looks faster on Single Thread on Core i5-5200U
                MaxBitsRadixSort = 8;
            }
            else if (cores <= 6)
            {
                MaxBitsRadixSort = 9;
            }
            else if (cores <= 8)
            {
                MaxBitsRadixSort = 10;
            }
            else if (cores <= 16)
            {
                //11bits looks faster than 8 on AMD 4800H, 15 is slower
                MaxBitsRadixSort = 11;
            }
            else
            {
                MaxBitsRadixSort = 12;
            }
        }
    }
}