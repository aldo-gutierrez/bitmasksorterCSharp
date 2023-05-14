using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BitMaskSorter
{
    internal class MaskInfoInt : IMaskInfo<int>
    {
        private int _pMask;
        private int _iMask;

        public int GetUpperBitMask() => 1 << 31;

        public int GetUpperBit() => 31;

        public int GetMask() => _pMask & _iMask;

        public void SetMaskParts((int, int) parts)
        {
            _pMask = parts.Item1;
            _iMask = parts.Item2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MaskedEqZero<T>(Func<T, int> convert, T e, int mask)
        {
            return (convert(e) & mask) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GreaterOrEqZero<T>(Func<T, int> convert, T e)
        {
            return convert(e) >= 0;
        }

        public (int, int) CalculateMask<T>(Func<T, int> convert, T[] array, int start, int endP1)
        {
            var pMask = 0x0000000000000000;
            var iMask = 0x0000000000000000;
            for (var i = start; i < endP1; i++)
            {
                var e = convert(array[i]);
                pMask = pMask | e;
                iMask = iMask | ~e;
            }

            return (pMask, iMask);
        }

        public int[] GetMaskAsArray(int mask)
        {
            var list = new List<int>();
            for (var i = GetUpperBit(); i >= 0; i--)
            {
                if (((mask >> i) & 1) == 1)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }

        public int GetMaskRangeBits(int bStart, int bEnd)
        {
            return ((1 << bStart + 1 - bEnd) - 1) << bEnd;
        }
        
        public void PartitionStableBits<T>(Func<T, int> convert, T[] array, int start, int endP1, int mask,
            int shiftRight,
            int kRange, T[] aux)
        {
            var count = new int[kRange];
            for (var i = start; i < endP1; i++)
            {
                count[(convert(array[i]) & mask) >> shiftRight]++;
            }


            for (int i = 0, sum = 0; i < kRange; ++i)
            {
                var countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (var i = start; i < endP1; i++)
            {
                var element = array[i];
                aux[count[(convert(element) & mask) >> shiftRight]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }
    }
}