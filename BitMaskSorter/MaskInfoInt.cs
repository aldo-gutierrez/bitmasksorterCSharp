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

        public List<(int, int, int)> GetSections(int[] bList, int bListStart, int bListEnd)
        {
            var parts = new List<(int, int, int)>();
            for (var i = bListStart; i >= bListEnd; i--)
            {
                var bIndex = bList[i];
                var mask = 1 << bIndex;
                var bits = 1;
                var imm = 0;
                for (var j = 1; j <= SorterConstants.RadixSortMaxBits - 1; j++)
                {
                    if (i - j >= bListEnd)
                    {
                        var bIndexJ = bList[i - j];
                        if (bIndexJ == bIndex + j)
                        {
                            mask = mask | 1 << bIndexJ;
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
                parts.Add((mask, bits, bIndex));
            }

            return parts;
        }
    }
}