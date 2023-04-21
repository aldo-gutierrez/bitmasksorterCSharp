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
                V sortMask = getUpperBitMask();
                int finalLeft = isUnsigned()
                    ? PartitionNotStable(array, start, endP1, e => eMaskedEq0(e, sortMask))
                    : PartitionReverseNotStable(array, start, endP1, e => eMaskedEq0(e, sortMask));
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
                    PartitionStableBits(array, start, endP1, e => eMaskedShifted(e, maskI, shift),
                        twoPowerBits, aux);
                }
            }
        }

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

        public void PartitionStableBits(T[] array, int start, int endP1, Func<T, int> eAndMaskShift,
            int twoPowerK, T[] aux)
        {
            int[] count = new int[twoPowerK];
            for (int i = start; i < endP1; i++)
            {
                count[eAndMaskShift(array[i])]++;
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
                aux[count[eAndMaskShift(element)]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }


        protected abstract bool IsIEEE754();
        protected abstract bool isUnsigned();

        //x & eAndMaskShift;
        protected abstract int eMaskedShifted(T x, V mask, int shiftRight);

        //(x & eAndMaskShift) == 0;
        protected abstract bool eMaskedEq0(T x, V mask);



        //https://stackoverflow.com/questions/27237776/convert-int-bits-to-float-bits
        //https://stackoverflow.com/questions/26394616/c-sharp-equivalent-to-javas-float-floattointbits
        public static unsafe int FloatToInt(float value)
        {
            //return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            return *(int*)(&value);
        }

        protected long DoubleToLong(double d)
        {
            return BitConverter.DoubleToInt64Bits(d);
        }

        protected (int, int) getMaskBitInt(T[] array, int start, int endP1, Func<T, int> convert)
        {
            int p_mask = 0x0000000000000000;
            int i_mask = 0x0000000000000000;
            for (int i = start; i < endP1; i++)
            {
                int e = convert(array[i]);
                p_mask = p_mask | e;
                i_mask = i_mask | (~e);
            }

            return (p_mask, i_mask);
        }

        protected (long, long) getMaskBitLong(T[] array, int start, int endP1, Func<T, long> convert)
        {
            long p_mask = 0x0000000000000000;
            long i_mask = 0x0000000000000000;
            for (int i = start; i < endP1; i++)
            {
                long e = convert(array[i]);
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
        protected override bool isUnsigned()
        {
            return false;
        }

        protected override bool IsIEEE754()
        {
            return false;
        }

        protected override int eMaskedShifted(int x, int mask, int shiftRight)
        {
            return (x & mask) >> shiftRight;
        }

        protected override bool eMaskedEq0(int x, int mask)
        {
            return (x & mask) == 0;
        }

        protected override int getUpperBitMask()
        {
            return 1 << 31;
        }

        protected override int getMask((int, int) maskInfo)
        {
            return maskInfo.Item1 & maskInfo.Item2;
        }

        protected override int getUpperBit()
        {
            return 31;
        }

        protected override List<(int, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd)
        {
            return getSectionsInt(kList, kIndexStart, kIndexEnd);
        }

        protected override (int, int) getMaskBit(int[] array, int start, int endP1)
        {
            return getMaskBitInt(array, start, endP1, x => x);
        }

        protected override int[] getMaskAsArray(int mask)
        {
            return getIntMaskAsArray(mask);
        }
    }

    public class RadixBitSorterGenericFloat : RadixBitSorterGeneric<float, int>
    {
        protected override bool isUnsigned() => false;

        protected override int eMaskedShifted(float x, int mask, int shiftRight)
        {
            return (FloatToInt(x) & mask) >> shiftRight;
        }

        protected override bool eMaskedEq0(float x, int mask)
        {
            return (FloatToInt(x) & mask) == 0;
        }

        protected override int getUpperBitMask()
        {
            return 1 << 31;
        }

        protected override int getMask((int, int) maskInfo)
        {
            return maskInfo.Item1 & maskInfo.Item2;
        }

        protected override int getUpperBit()
        {
            return 31;
        }

        protected override List<(int, int, int)> getSections(int[] kList, int kIndexStart, int kIndexEnd)
        {
            return getSectionsInt(kList, kIndexStart, kIndexEnd);
        }

        protected override (int, int) getMaskBit(float[] array, int start, int endP1)
        {
            return getMaskBitInt(array, start, endP1, x => FloatToInt(x));
        }

        protected override int[] getMaskAsArray(int mask)
        {
            return getIntMaskAsArray(mask);
        }

        protected override bool IsIEEE754()
        {
            return true;
        }
    }
}