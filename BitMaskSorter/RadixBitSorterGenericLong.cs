using System;
using System.Runtime.CompilerServices;

namespace BitMaskSorter
{
    public abstract class RadixBitSorterGenericLong<T>
    {
        private readonly MaskInfoLong _maskInfo = new MaskInfoLong();

        public void Sort(T[] array, int start, int endP1)
        {
            var n = endP1 - start;
            if (n < 2)
            {
                return;
            }

            var maskInfo = _maskInfo;
            maskInfo.SetMaskParts(maskInfo.CalculateMask(Mapper(), array, start, endP1));
            var mask = maskInfo.GetMask();
            var bList = maskInfo.GetMaskAsArray(mask);
            if (bList.Length == 0)
            {
                return;
            }

            if (bList[0] == maskInfo.GetUpperBit())
            {
                //there are negative numbers and positive numbers
                var finalLeft = IsUnsigned()
                    ? SorterUtilsGenericLong.PartitionNotStableUpperBit(array, start, endP1, Mapper())
                    : SorterUtilsGenericLong.PartitionReverseNotStableUpperBit(array, start, endP1, Mapper());

                var n1 = finalLeft - start;
                var n2 = endP1 - finalLeft;

                if (n1 > 1)
                {
                    //sort negative numbers
                    maskInfo.SetMaskParts(maskInfo.CalculateMask(Mapper(), array, start, finalLeft));
                    mask = maskInfo.GetMask();
                    bList = maskInfo.GetMaskAsArray(mask);
                    if (bList.Length > 0)
                    {
                        RadixSort(array, start, finalLeft, bList);
                        if (IsIeee754())
                        {
                            SorterUtilsGeneric.Reverse(array, start, finalLeft);
                        }
                    }
                }

                if (n2 > 1)
                {
                    //sort positive numbers
                    maskInfo.SetMaskParts(maskInfo.CalculateMask(Mapper(), array, finalLeft, endP1));
                    mask = maskInfo.GetMask();
                    bList = maskInfo.GetMaskAsArray(mask);
                    if (bList.Length > 0)
                    {
                        RadixSort(array, finalLeft, endP1, bList);
                    }
                }
            }
            else
            {
                RadixSort(array, start, endP1, bList);
                if (IsIeee754())
                {
                    if (Mapper().Invoke(array[0]) < 0L)
                    {
                        SorterUtilsGeneric.Reverse(array, start, endP1);
                    }
                }
            }
        }

        private void RadixSort(T[] array, int start, int endP1, int[] bList)
        {
            var aux = new T[endP1 - start];
            RadixSort(array, start, endP1, bList, bList.Length - 1, 0, aux);
        }


        private void RadixSort(T[] array, int start, int endP1, int[] bList, int kIndexStart, int kIndexEnd,
            T[] aux)
        {
            var maskInfo = _maskInfo;
            var sections = BitSorterUtils.GetSections(bList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                var bStartIndex = section.Item1;
                var bits = section.Item2;
                var shift = section.Item3;
                var mask = maskInfo.GetMaskRangeBits(bStartIndex, shift);
                if (bits == 1)
                {
                    SorterUtilsGenericLong.PartitionStable(array, start, endP1, mask, Mapper(), aux);
                }
                else
                {
                    var kRange = 1 << bits;
                    SorterUtilsGenericLong.PartitionStableBits(Mapper(), array, start, endP1, mask, shift, kRange, aux);
                }
            }
        }

        protected abstract bool IsUnsigned();

        protected abstract bool IsIeee754();

        protected abstract Func<T, long> Mapper();
    }

    public class RadixBitSorterGenericLong : RadixBitSorterGenericLong<long>
    {
        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<long, long> Mapper() => e => e;
    }

    public class RadixBitSorterGenericDouble : RadixBitSorterGenericLong<double>
    {
        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override unsafe Func<double, long> Mapper() => e => *(long*)(&e);
    }
    
}