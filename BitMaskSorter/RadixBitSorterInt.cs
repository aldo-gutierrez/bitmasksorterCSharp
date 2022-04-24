using System;
using System.Collections.Generic;
using System.Text;
using static BitMaskSorter.BitSorterUtils;
using static BitMaskSorter.IntSorterUtils;

namespace BitMaskSorter
{
    public class RadixBitSorterInt
    {
        bool unsigned = false;

        public bool isUnsigned()
        {
            return unsigned;
        }

        public void setUnsigned(bool unsigned)
        {
            this.unsigned = unsigned;
        }

        public void sort(int[] array)
        {
            if (array.Length < 2)
            {
                return;
            }
            int start = 0;
            int end = array.Length;
//            int ordered = isUnsigned() ? listIsOrderedUnSigned(array, start, end) : listIsOrderedSigned(array, start, end);
//            if (ordered == AnalysisResult.DESCENDING)
//            {
//                IntSorterUtils.reverse(array, start, end);
//            }
//            if (ordered != AnalysisResult.UNORDERED) return;

            int[] maskParts = getMaskBit(array, start, end);
            int mask = maskParts[0] & maskParts[1];
            int[] kList = getMaskAsArray(mask);
            if (kList.Length == 0)
            {
                return;
            }
            if (kList[0] == 31)
            { //there are negative numbers and positive numbers
                int sortMask = BitSorterUtils.getMaskBit(kList[0]);
                int finalLeft = isUnsigned()
                        ? IntSorterUtils.partitionNotStable(array, start, end, sortMask)
                        : IntSorterUtils.partitionReverseNotStable(array, start, end, sortMask);
                if (finalLeft - start > 1)
                { //sort negative numbers
                    int[] aux = new int[finalLeft - start];
                    maskParts = getMaskBit(array, start, finalLeft);
                    mask = maskParts[0] & maskParts[1];
                    kList = getMaskAsArray(mask);
                    radixSort(array, start, finalLeft, kList, kList.Length - 1, 0, aux);
                }
                if (end - finalLeft > 1)
                { //sort positive numbers
                    int[] aux = new int[end - finalLeft];
                    maskParts = getMaskBit(array, finalLeft, end);
                    mask = maskParts[0] & maskParts[1];
                    kList = getMaskAsArray(mask);
                    radixSort(array, finalLeft, end, kList, kList.Length - 1, 0, aux);
                }
            }
            else
            {
                int[] aux = new int[end - start];
                radixSort(array, start, end, kList, kList.Length - 1, 0, aux);
            }
        }

        public static void radixSort(int[] array, int start, int end, int[] kList, int kIndexStart, int kIndexEnd, int[] aux)
        {
            for (int i = kIndexStart; i >= kIndexEnd; i--)
            {
                int kListI = kList[i];
                int maskI = BitSorterUtils.getMaskBit(kListI);
                int bits = 1;
                int imm = 0;
                for (int j = 1; j <= 11; j++)
                { //11bits looks faster than 8 on AMD 4800H, 15 is slower
                    if (i - j >= kIndexEnd)
                    {
                        int kListIm1 = kList[i - j];
                        if (kListIm1 == kListI + j)
                        {
                            int maskIm1 = BitSorterUtils.getMaskBit(kListIm1);
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
                    IntSorterUtils.partitionStable(array, start, end, maskI, aux);
                }
                else
                {
                    int twoPowerBits = BitSorterUtils.twoPowerX(bits);
                    if (kListI == 0)
                    {
                        partitionStableLastBits(array, start, end, maskI, twoPowerBits, aux);
                    }
                    else
                    {
                        partitionStableGroupBits(array, start, end, maskI, kListI, twoPowerBits, aux);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // Display the number of command line arguments.
            Console.WriteLine(args.Length);
        }

    }
}
