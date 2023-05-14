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
        
        public static List<(int, int, int)> GetSections(int[] bList, int bListStart, int bListEnd)
        {
            var parts = new List<(int, int, int)>();
            for (var i = bListStart; i >= bListEnd; i--)
            {
                var bIndex = bList[i];
                var bits = 1;
                var imm = 0;
                for (var j = 1; j <= SorterConstants.RadixSortMaxBits - 1; j++)
                {
                    if (i - j >= bListEnd)
                    {
                        var bIndexJ = bList[i - j];
                        if (bIndexJ == bIndex + j)
                        {
                            bits++;
                            imm++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                i -= imm;
                parts.Add((bIndex + bits - 1, bits, bIndex));
            }

            return parts;
        }
        
    }
}