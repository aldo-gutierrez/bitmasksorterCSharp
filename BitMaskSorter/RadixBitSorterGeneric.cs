using System;
using System.Collections.Generic;

namespace BitMaskSorter
{
    public class RadixBitSorterGeneric<T>
        where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        public void sort(T[] array, int start, int endP1)
        {
            int n = endP1 - start;
            if (n < 2)
            {
                return;
            }

            var maskInfo = getMaskBit(array, start, endP1);
            int mask = maskInfo.Item1 & maskInfo.Item2;
            int[] kList = getMaskAsArray(mask);
            if (kList.Length == 0)
            {
                return;
            }

            if (kList[0] == 31)
            {
                //there are negative numbers and positive numbers
                int sortMask = 1 << kList[0];
                int finalLeft = isUnsigned()
                    ? PartitionNotStable(array, start, endP1, e => eMaskedEq0(e, sortMask))
                    : PartitionReverseNotStable(array, start, endP1, e => eMaskedEq0(e, sortMask));
                int n1 = finalLeft - start;
                int n2 = endP1 - finalLeft;

                if (n1 > 1)
                {
                    //sort negative numbers
                    maskInfo = getMaskBit(array, start, finalLeft);
                    mask = maskInfo.Item1 & maskInfo.Item2;
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
                    mask = maskInfo.Item1 & maskInfo.Item2;
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
                    int sortMask = 1 << 31;
                    if (!eMaskedEq0(array[0], sortMask))
                    {
                        reverse(array, start, endP1);
                    }
                }
            }
        }


        private void radixSort(T[] array, int start, int endP1, int[] kList)
        {
            T[] aux = new T[endP1 - start];
            radixSort(array, start, endP1, kList, kList.Length - 1, 0, aux);
        }


        private void radixSort(T[] array, int start, int endP1, int[] kList, int kIndexStart, int kIndexEnd,
            T[] aux)
        {
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
                if (bits == 1)
                {
                    PartitionStable(array, start, endP1, e => eMaskedEq0(e, maskI), aux);
                }
                else
                {
                    int twoPowerBits = 1 << bits;
                    PartitionStableBits(array, start, endP1, e => eMaskedShifted(e, maskI, kListI),
                        twoPowerBits, aux);
                }
            }
        }

        public (int, int) getMaskBit(T[] array, int start, int endP1)
        {
            int p_mask = 0x00000000;
            int i_mask = 0x00000000;
            for (int i = start; i < endP1; i++)
            {
                int e = convertToInt(array[i]);
                p_mask = p_mask | e;
                i_mask = i_mask | (~e);
            }

            return (p_mask, i_mask);
        }

        protected virtual int convertToInt(T ei)
        {
            return Convert.ToInt32(ei);
        }

        public int[] getMaskAsArray(int mask)
        {
            List<int> list = new List<int>();
            for (int i = 31; i >= 0; i--)
            {
                if (((mask >> i) & 1) == 1)
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }


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

        public int PartitionStable(T[] array, int start, int endP1, Func<T, bool> xAndMaskEq0, T[] aux)
        {
            int left = start;
            int right = 0;
            for (int i = start; i < endP1; i++)
            {
                T element = array[i];
                if (xAndMaskEq0(element))
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

        public void PartitionStableBits(T[] array, int start, int endP1, Func<T, int> eMaskedShifted,
            int twoPowerK, T[] aux)
        {
            int[] count = new int[twoPowerK];
            for (int i = start; i < endP1; i++)
            {
                count[eMaskedShifted(array[i])]++;
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
                aux[count[eMaskedShifted(element)]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }


        protected virtual bool IsIEEE754()
        {
            if (typeof(T) == typeof(float))
            {
                return true;
            }

            if (typeof(T) == typeof(double))
            {
                return true;
            }

            return false;
        }

        protected virtual bool isUnsigned()
        {
            if (typeof(T) == typeof(UInt32))
            {
                return true;
            }

            if (typeof(T) == typeof(UInt64))
            {
                return true;
            }

            if (typeof(T) == typeof(UInt16))
            {
                return true;
            }

            if (typeof(T) == typeof(Byte))
            {
                return true;
            }

            return false;
        }

        //x & eMaskedShifted;
        protected virtual int eMaskedShifted(T x, int mask, int shiftRight)
        {
            if (typeof(T) == typeof(int))
            {
                return (Convert.ToInt32(x) & mask) >> shiftRight;
            }

            if (typeof(T) == typeof(float))
            {
                return (Convert.ToInt32(x) & mask) >> shiftRight;
            }

            return 0;
        }

        //(x & eMaskedShifted) == 0;
        protected virtual bool eMaskedEq0(T x, int mask)
        {
            if (typeof(T) == typeof(int))
            {
                return (Convert.ToInt32(x) & mask) == 0;
            }

            return false;
        }

        protected int FloatToInt(float f)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
        }

        protected long DoubleToLong(double d)
        {
            return BitConverter.DoubleToInt64Bits(d);
        }
    }

    public class RadixBitSorterGenericInt : RadixBitSorterGeneric<int>
    {
        protected override bool isUnsigned()
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

        protected override bool IsIEEE754()
        {
            return false;
        }

        protected override int convertToInt(int ei)
        {
            return ei;
        }


    }

    public class RadixBitSorterGenericFloat : RadixBitSorterGeneric<float>
    {
        protected override bool isUnsigned()
        {
            return false;
        }

        protected override int eMaskedShifted(float x, int mask, int shiftRight)
        {
            return (FloatToInt(x) & mask) >> shiftRight;
        }

        protected override bool eMaskedEq0(float x, int mask)
        {
            return (FloatToInt(x) & mask) == 0;
        }

        protected override bool IsIEEE754()
        {
            return true;
        }
    }
}