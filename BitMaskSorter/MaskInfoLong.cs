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

        public List<(long, int, int)> GetSections(int[] bList, int bListStart, int bListEnd)
        {
            var parts = new List<(long, int, int)>();
            for (var i = bListStart; i >= bListEnd; i--)
            {
                var bIndex = bList[i];
                var mask = 1L << bIndex;
                var length = 1;
                var imm = 0;
                for (var j = 1; j <= SorterConstants.RadixSortMaxBits - 1; j++)
                {
                    if (i - j >= bListEnd)
                    {
                        var bIndexJ = bList[i - j];
                        if (bIndexJ == bIndex + j)
                        {
                            mask = mask | 1L << bIndexJ;
                            length++;
                            imm++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                i -= imm;
                parts.Add((mask, length, bIndex));
            }

            return parts;
        }
    }
}