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

        public long GetMaskRangeBits(int bStart, int bEnd)
        {
            return ((1L << bStart + 1 - bEnd) - 1L) << bEnd;
        }
    }
}