using System;
using System.Collections.Generic;

namespace BitMaskSorter
{
    public abstract class RadixBitSorterGeneric<T, V>
        where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        where V : struct, IComparable, IComparable<V>, IConvertible, IEquatable<V>, IFormattable
    {
        public void sort(T[] array, int start, int endP1)
        {
            int n = endP1 - start;
            if (n < 2)
            {
                return;
            }

            var maskInfo = getMaskBit(array, start, endP1);
            V mask = getMask(maskInfo);
            int[] kList = getMaskAsArray(mask);
            if (kList.Length == 0)
            {
                return;
            }

            if (kList[0] == getUpperBit())
            {
                //there are negative numbers and positive numbers
//                V sortMask = getUpperBitMask();
                int finalLeft = isUnsigned()
//                    ? PartitionNotStable(array, start, endP1, e => eMaskedEq0(e, sortMask))
//                    : PartitionReverseNotStable(array, start, endP1, e => eMaskedEq0(e, sortMask));
                    ? PartitionNotStable(array, start, endP1, e => eGreEq0(e))
                    : PartitionReverseNotStable(array, start, endP1, e => eGreEq0(e));

                int n1 = finalLeft - start;
                int n2 = endP1 - finalLeft;

                if (n1 > 1)
                {
                    //sort negative numbers
                    maskInfo = getMaskBit(array, start, finalLeft);
                    mask = getMask(maskInfo);
                    kList = getMaskAsArray(mask);
                    if (kList.Length > 0)
                    {
                        radixSort(array, start, finalLeft, kList);
                        if (IsIEEE754())
                        {
                            reverse(array, start, finalLeft);
                        }
                    }
                }

                if (n2 > 1)
                {
                    //sort positive numbers
                    maskInfo = getMaskBit(array, finalLeft, endP1);
                    mask = getMask(maskInfo);
                    kList = getMaskAsArray(mask);
                    if (kList.Length > 0)
                    {
                        radixSort(array, finalLeft, endP1, kList);
                    }
                }
            }
            else
            {
                radixSort(array, start, endP1, kList);
                if (IsIEEE754())
                {
                    V sortMask = getUpperBitMask();
                    if (!eMaskedEq0(array[0], sortMask))
                    {
                        reverse(array, start, endP1);
                    }
                }
            }
        }

        protected abstract bool eGreEq0(T e);

        protected abstract V getUpperBitMask();

        protected abstract V getMask((V, V) maskInfo);

        protected abstract int getUpperBit();


        private void radixSort(T[] array, int start, int endP1, int[] kList)
        {
            T[] aux = new T[endP1 - start];
            radixSort(array, start, endP1, kList, kList.Length - 1, 0, aux);
        }


        private void radixSort(T[] array, int start, int endP1, int[] kList, int kIndexStart, int kIndexEnd,
            T[] aux)
        {
            List<(V, int, int)> sections = getSections(kList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                V maskI = section.Item1;
                int bits = section.Item2;
                int shift = section.Item3;
                if (bits == 1)
                {
                    PartitionStable(array, start, endP1, e => eMaskedEq0(e, maskI), aux);
                }
                else
                {
                    int twoPowerBits = 1 << bits;
                    PartitionStableBitsX(array, start, endP1, maskI, shift, twoPowerBits, aux);
                }
            }
        }

        protected abstract void PartitionStableBitsX(T[] array, int start, int endP1, V maskI, int shift,
            int twoPowerBits, T[] aux);

        protected abstract List<(V, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd);

        protected List<(int, int, int)> getSectionsInt(int[] kList, int kIndexStart, int kIndexEnd)
        {
            List<(int, int, int)> parts = new List<(int, int, int)>();
            for (int i = kIndexStart; i >= kIndexEnd; i--)
            {
                int kListI = kList[i];
                int maskI = 1 << kListI;
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
                            int maskIm1 = 1 << kListIm1;
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

        protected List<(long, int, int)> getSectionsLong(int[] kList, int kIndexStart, int kIndexEnd)
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


        protected abstract (V, V) getMaskBit(T[] array, int start, int endP1);

        protected abstract int[] getMaskAsArray(V mask);


        private void Swap(T[] array, int left, int right)
        {
            T aux = array[left];
            array[left] = array[right];
            array[right] = aux;
        }


        private void reverse(T[] array, int start, int endP1)
        {
            int length = endP1 - start;
            int ld2 = length / 2;
            int end = endP1 - 1;
            for (int i = 0; i < ld2; ++i)
            {
                Swap(array, start + i, end - i);
            }
        }


        public int PartitionNotStable(T[] array, int start, int endP1, Func<T, bool> fMaskEqual)
        {
            int left = start;
            int right = endP1 - 1;

            while (left <= right)
            {
                T element = array[left];
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
            int left = start;
            int right = endP1 - 1;

            while (left <= right)
            {
                T element = array[left];
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
            int left = start;
            int right = 0;
            for (int i = start; i < endP1; i++)
            {
                T element = array[i];
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


        public void PartitionStableBitsInt(T[] array, int start, int endP1, int mask, int shiftRight,
            int twoPowerK, T[] aux)
        {
            int[] count = new int[twoPowerK];
            for (int i = start; i < endP1; i++)
            {
                count[(convertToInt(array[i]) & mask) >> shiftRight]++;
            }


            for (int i = 0, sum = 0; i < twoPowerK; ++i)
            {
                int countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (int i = start; i < endP1; i++)
            {
                T element = array[i];
                aux[count[(convertToInt(element) & mask) >> shiftRight]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }

        public void PartitionStableBitsLong(T[] array, int start, int endP1, long mask, int shiftRight,
            int twoPowerK, T[] aux)
        {
            int[] count = new int[twoPowerK];
            for (int i = start; i < endP1; i++)
            {
                count[(convertToLong(array[i]) & mask) >> shiftRight]++;
            }


            for (int i = 0, sum = 0; i < twoPowerK; ++i)
            {
                int countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (int i = start; i < endP1; i++)
            {
                T element = array[i];
                aux[count[(convertToLong(element) & mask) >> shiftRight]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }

        protected abstract long convertToLong(T e);

        protected abstract int convertToInt(T e);
        protected abstract bool IsIEEE754();
        protected abstract bool isUnsigned();

        //(e & eAndMaskShift) == 0;
        protected abstract bool eMaskedEq0(T e, V mask);

        protected long DoubleToLong(double d)
        {
            return BitConverter.DoubleToInt64Bits(d);
        }

        protected (int, int) getMaskBitInt(T[] array, int start, int endP1)
        {
            int p_mask = 0x0000000000000000;
            int i_mask = 0x0000000000000000;
            for (int i = start; i < endP1; i++)
            {
                int e = convertToInt(array[i]);
                p_mask = p_mask | e;
                i_mask = i_mask | (~e);
            }

            return (p_mask, i_mask);
        }

        protected (long, long) getMaskBitLong(T[] array, int start, int endP1)
        {
            long p_mask = 0x0000000000000000;
            long i_mask = 0x0000000000000000;
            for (int i = start; i < endP1; i++)
            {
                long e = convertToLong(array[i]);
                p_mask = p_mask | e;
                i_mask = i_mask | (~e);
            }

            return (p_mask, i_mask);
        }

        public int[] getIntMaskAsArray(int mask)
        {
            List<int> list = new List<int>();
            for (int i = getUpperBit(); i >= 0; i--)
            {
                if (((mask >> i) & 1) == 1)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }

        public int[] getLongMaskAsArray(long mask)
        {
            List<int> list = new List<int>();
            for (int i = getUpperBit(); i >= 0; i--)
            {
                if (((mask >> i) & 1L) == 1L)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }
    }


    public class RadixBitSorterGenericInt : RadixBitSorterGeneric<int, int>
    {
        protected override bool isUnsigned() => false;

        protected override bool IsIEEE754() => false;

        protected override int convertToInt(int e) => e;
        protected override long convertToLong(int e) => e;

        protected override bool eMaskedEq0(int e, int mask) => (e & mask) == 0;

        protected override bool eGreEq0(int e) => e >= 0;

        protected override int getUpperBitMask() => 1 << 31;

        protected override int getMask((int, int) maskInfo) => maskInfo.Item1 & maskInfo.Item2;

        protected override int getUpperBit() => 31;

        protected override void PartitionStableBitsX(int[] array, int start, int endP1,
            int maskI, int shift, int twoPowerBits,
            int[] aux) =>
            PartitionStableBitsInt(array, start, endP1, maskI, shift, twoPowerBits, aux);

        protected override List<(int, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd) =>
            getSectionsInt(kList, kIndexStart, kIndexEnd);

        protected override (int, int) getMaskBit(int[] array, int start, int endP1) =>
            getMaskBitInt(array, start, endP1);

        protected override int[] getMaskAsArray(int mask) => getIntMaskAsArray(mask);
    }

    public class RadixBitSorterGenericFloat : RadixBitSorterGeneric<float, int>
    {
        protected override bool isUnsigned() => false;

        protected override bool IsIEEE754() => true;

        protected override unsafe long convertToLong(float e) => throw new NotImplementedException();

        protected override unsafe int convertToInt(float e) => *(int*)(&e);

        protected override bool eMaskedEq0(float e, int mask) => (convertToInt(e) & mask) == 0;

        protected override bool eGreEq0(float e) => convertToInt(e) >= 0;

        protected override int getUpperBitMask() => 1 << 31;

        protected override int getMask((int, int) maskInfo) => maskInfo.Item1 & maskInfo.Item2;

        protected override int getUpperBit() => 31;

        protected override void PartitionStableBitsX(float[] array, int start, int endP1, int maskI, int shift,
            int twoPowerBits,
            float[] aux) =>
            PartitionStableBitsInt(array, start, endP1, maskI, shift, twoPowerBits, aux);


        protected override List<(int, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd) =>
            getSectionsInt(kList, kIndexStart, kIndexEnd);

        protected override (int, int) getMaskBit(float[] array, int start, int endP1) =>
            getMaskBitInt(array, start, endP1);

        protected override int[] getMaskAsArray(int mask) => getIntMaskAsArray(mask);
    }


    public class RadixBitSorterGenericLong : RadixBitSorterGeneric<long, long>
    {
        protected override bool isUnsigned() => false;

        protected override bool IsIEEE754() => false;
        protected override long convertToLong(long e) => e;

        protected override int convertToInt(long e) => throw new NotImplementedException();

        protected override bool eMaskedEq0(long e, long mask) => (e & mask) == 0L;

        protected override bool eGreEq0(long e) => e >= 0L;

        protected override long getUpperBitMask() => 1L << 63;

        protected override long getMask((long, long) maskInfo) => maskInfo.Item1 & maskInfo.Item2;

        protected override int getUpperBit() => 63;

        protected override void PartitionStableBitsX(long[] array, int start, int endP1,
            long maskI, int shift, int twoPowerBits,
            long[] aux) =>
            PartitionStableBitsLong(array, start, endP1, maskI, shift, twoPowerBits, aux);

        protected override List<(long, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd) =>
            getSectionsLong(kList, kIndexStart, kIndexEnd);

        protected override (long, long) getMaskBit(long[] array, int start, int endP1) =>
            getMaskBitLong(array, start, endP1);

        protected override int[] getMaskAsArray(long mask) => getLongMaskAsArray(mask);
    }

    public class RadixBitSorterGenericDouble : RadixBitSorterGeneric<double, long>
    {
        protected override bool isUnsigned() => false;

        protected override bool IsIEEE754() => true;

        protected override unsafe long convertToLong(double e) => *(long*)(&e);

        protected override int convertToInt(double e) => throw new NotImplementedException();

        protected override bool eMaskedEq0(double e, long mask) => (convertToLong(e) & mask) == 0;

        protected override bool eGreEq0(double e) => convertToLong(e) >= 0;

        protected override long getUpperBitMask() => 1L << 63;

        protected override long getMask((long, long) maskInfo) => maskInfo.Item1 & maskInfo.Item2;

        protected override int getUpperBit() => 63;

        protected override void PartitionStableBitsX(double[] array, int start, int endP1,
            long maskI, int shift, int twoPowerBits,
            double[] aux) =>
            PartitionStableBitsLong(array, start, endP1, maskI, shift, twoPowerBits, aux);

        protected override List<(long, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd) =>
            getSectionsLong(kList, kIndexStart, kIndexEnd);

        protected override (long, long) getMaskBit(double[] array, int start, int endP1) =>
            getMaskBitLong(array, start, endP1);

        protected override int[] getMaskAsArray(long mask) => getLongMaskAsArray(mask);
    }
}