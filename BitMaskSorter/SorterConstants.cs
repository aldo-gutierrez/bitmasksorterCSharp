using System;

namespace BitMaskSorter
{
    internal class SorterConstants
    {
        public static int MAX_BITS_RADIX_SORT = 8;

        static SorterConstants()
        {
            int cores = Environment.ProcessorCount;
            if (cores <= 4)
            {
                //8bits looks faster on Single Thread on Core i5-5200U
                MAX_BITS_RADIX_SORT = 8;
            }
            else if (cores <= 6)
            {
                MAX_BITS_RADIX_SORT = 9;
            }
            else if (cores <= 8)
            {
                MAX_BITS_RADIX_SORT = 10;
            }
            else if (cores <= 16)
            {
                //11bits looks faster than 8 on AMD 4800H, 15 is slower
                MAX_BITS_RADIX_SORT = 11;
            }
            else
            {
                MAX_BITS_RADIX_SORT = 12;
            }
        }
    }
}