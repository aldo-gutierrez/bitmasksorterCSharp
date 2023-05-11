using System;
using System.Runtime.CompilerServices;

namespace BitMaskSorter
{
    public abstract class RadixBitSorterGeneric<T, TM>
        where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
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
            var kList = maskInfo.GetMaskAsArray(mask);
            if (kList.Length == 0)
            {
                return;
            }

            if (kList[0] == maskInfo.GetUpperBit())
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
                    kList = maskInfo.GetMaskAsArray(mask);
                    if (kList.Length > 0)
                    {
                        RadixSort(array, start, finalLeft, kList);
                        if (IsIEEE754())
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
                    kList = maskInfo.GetMaskAsArray(mask);
                    if (kList.Length > 0)
                    {
                        RadixSort(array, finalLeft, endP1, kList);
                    }
                }
            }
            else
            {
                RadixSort(array, start, endP1, kList);
                if (IsIEEE754())
                {
                    var sortMask = maskInfo.GetUpperBitMask();
                    if (!maskInfo.MaskedEqZero(MapToMask(), array[0], sortMask))
                    {
                        Reverse(array, start, endP1);
                    }
                }
            }
        }

        private void RadixSort(T[] array, int start, int endP1, int[] kList)
        {
            var aux = new T[endP1 - start];
            RadixSort(array, start, endP1, kList, kList.Length - 1, 0, aux);
        }


        private void RadixSort(T[] array, int start, int endP1, int[] kList, int kIndexStart, int kIndexEnd,
            T[] aux)
        {
            var maskInfo = GetMaskInfoBuilder();
            var sections = maskInfo.GetSections(kList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                var maskI = section.Item1;
                var bits = section.Item2;
                var shift = section.Item3;
                if (bits == 1)
                {
                    PartitionStable(array, start, endP1, e => maskInfo.MaskedEqZero(MapToMask(), e, maskI), aux);
                }
                else
                {
                    var twoPowerBits = 1 << bits;
                    PartitionStableBits(array, start, endP1, maskI, shift, twoPowerBits, aux);
                }
            }
        }

        protected abstract void PartitionStableBits(T[] array, int start, int endP1, TM maskI, int shift,
            int twoPowerBits, T[] aux);


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


        public void PartitionStableBitsInt(Func<T, int> convert, T[] array, int start, int endP1, int mask,
            int shiftRight,
            int twoPowerK, T[] aux)
        {
            var count = new int[twoPowerK];
            for (var i = start; i < endP1; i++)
            {
                count[(convert(array[i]) & mask) >> shiftRight]++;
            }


            for (int i = 0, sum = 0; i < twoPowerK; ++i)
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

        public void PartitionStableBitsLong(Func<T, long> convert, T[] array, int start, int endP1, long mask,
            int shiftRight,
            int twoPowerK, T[] aux)
        {
            var count = new int[twoPowerK];
            for (var i = start; i < endP1; i++)
            {
                count[(convert(array[i]) & mask) >> shiftRight]++;
            }


            for (int i = 0, sum = 0; i < twoPowerK; ++i)
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


        protected abstract bool IsUnsigned();

        protected abstract bool IsIEEE754();

        protected abstract Func<T, TM> MapToMask();

        protected abstract IMaskInfo<TM> GetMaskInfoBuilder();

    }


    public class RadixBitSorterGenericInt : RadixBitSorterGeneric<int, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();
        protected override bool IsUnsigned() => false;

        protected override bool IsIEEE754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<int, int> MapToMask()
        {
            return e => e;
        }

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }

        protected override void PartitionStableBits(int[] array, int start, int endP1,
            int maskI, int shift, int twoPowerBits,
            int[] aux) =>
            PartitionStableBitsInt(MapToMask(), array, start, endP1, maskI, shift, twoPowerBits, aux);
    }

    public class RadixBitSorterGenericFloat : RadixBitSorterGeneric<float, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();

        protected override bool IsUnsigned() => false;

        protected override bool IsIEEE754() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override unsafe Func<float, int> MapToMask()
        {
            return e => *(int*)&e;
        }

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }

        protected override void PartitionStableBits(float[] array, int start, int endP1, int maskI, int shift,
            int twoPowerBits,
            float[] aux) =>
            PartitionStableBitsInt(MapToMask(), array, start, endP1, maskI, shift, twoPowerBits, aux);
    }


    public class RadixBitSorterGenericLong : RadixBitSorterGeneric<long, long>
    {
        private readonly MaskInfoLong _maskInfoLong = new MaskInfoLong();

        protected override bool IsUnsigned() => false;

        protected override bool IsIEEE754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<long, long> MapToMask() => e => e;

        protected override IMaskInfo<long> GetMaskInfoBuilder()
        {
            return _maskInfoLong;
        }


        protected override void PartitionStableBits(long[] array, int start, int endP1,
            long maskI, int shift, int twoPowerBits,
            long[] aux) =>
            PartitionStableBitsLong(MapToMask(), array, start, endP1, maskI, shift, twoPowerBits, aux);
    }

    public class RadixBitSorterGenericDouble : RadixBitSorterGeneric<double, long>
    {
        private readonly MaskInfoLong _maskInfoLong = new MaskInfoLong();

        protected override bool IsUnsigned() => false;

        protected override bool IsIEEE754() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override unsafe Func<double, long> MapToMask()
        {
            return e => *(long*)(&e);
        }

        protected override IMaskInfo<long> GetMaskInfoBuilder()
        {
            return _maskInfoLong;
        }

        protected override void PartitionStableBits(double[] array, int start, int endP1,
            long maskI, int shift, int twoPowerBits,
            double[] aux) =>
            PartitionStableBitsLong(MapToMask(), array, start, endP1, maskI, shift, twoPowerBits, aux);
    }


    public class RadixBitSorterGenericShort : RadixBitSorterGeneric<short, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();
        protected override bool IsUnsigned() => false;

        protected override bool IsIEEE754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<short, int> MapToMask()
        {
            return e => e;
        }

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }

        protected override void PartitionStableBits(short[] array, int start, int endP1,
            int maskI, int shift, int twoPowerBits,
            short[] aux) =>
            PartitionStableBitsInt(MapToMask(), array, start, endP1, maskI, shift, twoPowerBits, aux);
    }

    public class RadixBitSorterGenericUShort : RadixBitSorterGeneric<ushort, int>
    {
        private readonly MaskInfoInt _maskInfoInt = new MaskInfoInt();
        protected override bool IsUnsigned() => true;

        protected override bool IsIEEE754() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Func<ushort, int> MapToMask()
        {
            return e => e;
        }

        protected override IMaskInfo<int> GetMaskInfoBuilder()
        {
            return _maskInfoInt;
        }

        protected override void PartitionStableBits(ushort[] array, int start, int endP1,
            int maskI, int shift, int twoPowerBits,
            ushort[] aux) =>
            PartitionStableBitsInt(MapToMask(), array, start, endP1, maskI, shift, twoPowerBits, aux);
    }

}