using System;
using System.Runtime.CompilerServices;

namespace BitMaskSorter
{
    public abstract class RadixBitSorterGeneric<T, TM>
        where TM : struct, IComparable, IComparable<TM>, IConvertible, IEquatable<TM>, IFormattable
    {
        public void Sort(T[] array, int start, int endP1)
        {
            var n = endP1 - start;
            if (n < 2)
            {
                return;
            }

            var maskInfo = GetMaskInfoBuilder();
            maskInfo.SetMaskParts(maskInfo.CalculateMask(MapToMask(), array, start, endP1));
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
                    ? PartitionNotStable(array, start, endP1, e => maskInfo.GreaterOrEqZero(MapToMask(), e))
                    : PartitionReverseNotStable(array, start, endP1, e => maskInfo.GreaterOrEqZero(MapToMask(), e));

                var n1 = finalLeft - start;
                var n2 = endP1 - finalLeft;

                if (n1 > 1)
                {
                    //sort negative numbers
                    maskInfo.SetMaskParts(maskInfo.CalculateMask(MapToMask(), array, start, finalLeft));
                    mask = maskInfo.GetMask();
                    bList = maskInfo.GetMaskAsArray(mask);
                    if (bList.Length > 0)
                    {
                        RadixSort(array, start, finalLeft, bList);
                        if (IsIeee754())
                        {
                            Reverse(array, start, finalLeft);
                        }
                    }
                }

                if (n2 > 1)
                {
                    //sort positive numbers
                    maskInfo.SetMaskParts(maskInfo.CalculateMask(MapToMask(), array, finalLeft, endP1));
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
                    var sortMask = maskInfo.GetUpperBitMask();
                    if (!maskInfo.MaskedEqZero(MapToMask(), array[0], sortMask))
                    {
                        Reverse(array, start, endP1);
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
            var maskInfo = GetMaskInfoBuilder();
            var sections = BitSorterUtils.GetSections(bList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                var bStartIndex = section.Item1;
                var bits = section.Item2;
                var shift = section.Item3;
                var mask = maskInfo.GetMaskRangeBits(bStartIndex, shift);
                if (bits == 1)
                {
                    PartitionStable(array, start, endP1, e => maskInfo.MaskedEqZero(MapToMask(), e, mask), aux);
                }
                else
                {
                    var kRange = 1 << bits;
                    maskInfo.PartitionStableBits(MapToMask(), array, start, endP1, mask, shift, kRange, aux);
                }
            }
        }
        
        private void Swap(T[] array, int left, int right)
        {
            (array[left], array[right]) = (array[right], array[left]);
        }


        private void Reverse(T[] array, int start, int endP1)
        {
            var length = endP1 - start;
            var ld2 = length / 2;
            var end = endP1 - 1;
            for (var i = 0; i < ld2; ++i)
            {
                Swap(array, start + i, end - i);
            }
        }


        public int PartitionNotStable(T[] array, int start, int endP1, Func<T, bool> fMaskEqual)
        {
            var left = start;
            var right = endP1 - 1;

            while (left <= right)
            {
                var element = array[left];
                if (fMaskEqual(element))
                {
                    left++;
                }
                else
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if (fMaskEqual(element))
                        {
                            Swap(array, left, right);
                            left++;
                            right--;
                            break;
                        }
                        else
                        {
                            right--;
                        }
                    }
                }
            }

            return left;
        }

        public int PartitionReverseNotStable(T[] array, int start, int endP1, Func<T, bool> eMaskedEq0)
        {
            var left = start;
            var right = endP1 - 1;

            while (left <= right)
            {
                var element = array[left];
                if (eMaskedEq0(element))
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if (eMaskedEq0(element))
                        {
                            right--;
                        }
                        else
                        {
                            Swap(array, left, right);
                            left++;
                            right--;
                            break;
                        }
                    }
                }
                else
                {
                    left++;
                }
            }

            return left;
        }

        public int PartitionStable(T[] array, int start, int endP1, Func<T, bool> eAndMaskEq0, T[] aux)
        {
            var left = start;
            var right = 0;
            for (var i = start; i < endP1; i++)
            {
                var element = array[i];
                if (eAndMaskEq0(element))
                {
                    array[left] = element;
                    left++;
                }
                else
                {
                    aux[right] = element;
                    right++;
                }
            }

            Array.Copy(aux, 0, array, left, right);
            return left;
        }

        protected abstract bool IsUnsigned();

        protected abstract bool IsIeee754();

        protected abstract Func<T, TM> MapToMask();

        protected abstract IMaskInfo<TM> GetMaskInfoBuilder();

    }


    public class RadixBitSorterGenericInt : RadixBitSorterGeneric<int, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();
        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<int, int> MapToMask() => e => e;

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }
    }

    public class RadixBitSorterGenericFloat : RadixBitSorterGeneric<float, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();

        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override unsafe Func<float, int> MapToMask() => e => *(int*)&e;

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }
    }


    public class RadixBitSorterGenericLong : RadixBitSorterGeneric<long, long>
    {
        private readonly MaskInfoLong _maskInfoLong = new MaskInfoLong();

        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<long, long> MapToMask() => e => e;

        protected override IMaskInfo<long> GetMaskInfoBuilder()
        {
            return _maskInfoLong;
        }
        
    }

    public class RadixBitSorterGenericDouble : RadixBitSorterGeneric<double, long>
    {
        private readonly MaskInfoLong _maskInfoLong = new MaskInfoLong();

        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override unsafe Func<double, long> MapToMask() => e => *(long*)(&e);

        protected override IMaskInfo<long> GetMaskInfoBuilder()
        {
            return _maskInfoLong;
        }
    }

    public class RadixBitSorterGenericShort : RadixBitSorterGeneric<short, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();
        protected override bool IsUnsigned() => false;

        protected override bool IsIeee754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<short, int> MapToMask() => e => e;

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }
    }

    public class RadixBitSorterGenericUShort : RadixBitSorterGeneric<ushort, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();
        protected override bool IsUnsigned() => true;

        protected override bool IsIeee754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<ushort, int> MapToMask() => e => e;

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }
    }

}