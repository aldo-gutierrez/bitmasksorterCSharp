using System;
using System.Collections.Generic;

namespace BitMaskSorter
{
    public class MaskInfoLong : MaskInfo<long>
    {
        private long pMask;
        private long iMask;

        public long GetUpperBitMask() => 1L << 63;

        public int GetUpperBit() => 63;

        public long GetMask() => pMask & iMask;

        public void SetMaskParts((long, long) parts)
        {
            pMask = parts.Item1;
            iMask = parts.Item2;
        }


        public bool MaskedEqZero<T>(Func<T, long> convert, T e, long mask)
        {
            return (convert(e) & mask) == 0L;
        }

        public bool GreaterOrEqZero<T>(Func<T, long> convert, T e)
        {
            return (convert(e) >= 0L);
        }

        public (long, long) CalculateMaskParts<T>(Func<T, long> convert, T[] array, int start, int endP1)
        {
            long pMask = 0x0000000000000000;
            long iMask = 0x0000000000000000;
            for (int i = start; i < endP1; i++)
            {
                long e = convert(array[i]);
                pMask = pMask | e;
                iMask = iMask | (~e);
            }

            return (pMask, iMask);
        }

        public int[] GetMaskAsArray(long mask)
        {
            List<int> list = new List<int>();
            for (int i = GetUpperBit(); i >= 0; i--)
            {
                if (((mask >> i) & 1L) == 1L)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }

        public List<(long, int, int)> GetSections(int[] kList, int kIndexStart, int kIndexEnd)
        {
            List<(long, int, int)> parts = new List<(long, int, int)>();
            for (int i = kIndexStart; i >= kIndexEnd; i--)
            {
                int kListI = kList[i];
                long maskI = 1L << kListI;
                int bits = 1;
                int imm = 0;
                for (int j = 1; j <= 11; j++)
                {
                    //11bits looks faster than 8 on AMD 4800H, 15 is slower
                    if (i - j >= kIndexEnd)
                    {
                        int kListIm1 = kList[i - j];
                        if (kListIm1 == kListI + j)
                        {
                            long maskIm1 = 1L << kListIm1;
                            maskI = maskI | maskIm1;
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
                parts.Add((maskI, bits, kListI));
            }

            return parts;
        }
    }
}