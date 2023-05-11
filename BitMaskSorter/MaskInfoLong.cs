using System;
using System.Collections.Generic;

namespace BitMaskSorter
{
    public class MaskInfoLong : IMaskInfo<long>
    {
        private long _pMask;
        private long _iMask;

        public long GetUpperBitMask() => 1L << 63;

        public int GetUpperBit() => 63;

        public long GetMask() => _pMask & _iMask;

        public void SetMaskParts((long, long) parts)
        {
            _pMask = parts.Item1;
            _iMask = parts.Item2;
        }


        public bool MaskedEqZero<T>(Func<T, long> convert, T e, long mask)
        {
            return (convert(e) & mask) == 0L;
        }

        public bool GreaterOrEqZero<T>(Func<T, long> convert, T e)
        {
            return convert(e) >= 0L;
        }

        public (long, long) CalculateMask<T>(Func<T, long> convert, T[] array, int start, int endP1)
        {
            long pMask = 0x0000000000000000;
            long iMask = 0x0000000000000000;
            for (var i = start; i < endP1; i++)
            {
                var e = convert(array[i]);
                pMask = pMask | e;
                iMask = iMask | ~e;
            }

            return (pMask, iMask);
        }

        public int[] GetMaskAsArray(long mask)
        {
            var list = new List<int>();
            for (var i = GetUpperBit(); i >= 0; i--)
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
            var parts = new List<(long, int, int)>();
            for (var i = kIndexStart; i >= kIndexEnd; i--)
            {
                var kListI = kList[i];
                var maskI = 1L << kListI;
                var bits = 1;
                var imm = 0;
                for (var j = 1; j <= SorterConstants.MaxBitsRadixSort - 1; j++)
                {
                    if (i - j >= kIndexEnd)
                    {
                        var kListIm1 = kList[i - j];
                        if (kListIm1 == kListI + j)
                        {
                            maskI = maskI | 1L << kListIm1;
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