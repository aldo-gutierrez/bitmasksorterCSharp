using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BitMaskSorter
{
    internal class MaskInfoInt : MaskInfo<int>
    {
        private int pMask;
        private int iMask;

        public int GetUpperBitMask() => 1 << 31;

        public int GetUpperBit() => 31;

        public int GetMask() => pMask & iMask;

        public void SetMaskParts((int, int) parts)
        {
            pMask = parts.Item1;
            iMask = parts.Item2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MaskedEqZero<T>(Func<T, int> convert, T e, int mask)
        {
            return (convert(e) & mask) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GreaterOrEqZero<T>(Func<T, int> convert, T e)
        {
            return (convert(e) >= 0);
        }

        public (int, int) CalculateMask<T>(Func<T, int> convert, T[] array, int start, int endP1)
        {
            int pMask = 0x0000000000000000;
            int iMask = 0x0000000000000000;
            for (int i = start; i < endP1; i++)
            {
                int e = convert(array[i]);
                pMask = pMask | e;
                iMask = iMask | (~e);
            }

            return (pMask, iMask);
        }

        public int[] GetMaskAsArray(int mask)
        {
            List<int> list = new List<int>();
            for (int i = GetUpperBit(); i >= 0; i--)
            {
                if (((mask >> i) & 1) == 1)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }

        public List<(int, int, int)> GetSections(int[] kList, int kIndexStart, int kIndexEnd)
        {
            List<(int, int, int)> parts = new List<(int, int, int)>();
            for (int i = kIndexStart; i >= kIndexEnd; i--)
            {
                int kListI = kList[i];
                int maskI = 1 << kListI;
                int bits = 1;
                int imm = 0;
                for (int j = 1; j <= SorterConstants.MAX_BITS_RADIX_SORT - 1; j++)
                {
                    if (i - j >= kIndexEnd)
                    {
                        int kListIm1 = kList[i - j];
                        if (kListIm1 == kListI + j)
                        {
                            maskI = maskI | 1 << kListIm1;
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