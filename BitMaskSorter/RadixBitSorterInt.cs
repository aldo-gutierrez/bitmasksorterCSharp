using System;
using System.Collections;
using System.Collections.Generic;
using static BitMaskSorter.BitSorterUtils;
using static BitMaskSorter.IntSorterUtils;

namespace BitMaskSorter
{
    public class RadixBitSorterInt
    {
        public bool IsUnsigned()
        {
            return false;
        }

        public void sort(int[] array, int start, int endP1)
        {
            int n = endP1 - start;
            if (n < 2)
            {
                return;
            }

            var maskParts = CalculateMaskParts(array, start, endP1);
            int mask = maskParts.Item1 & maskParts.Item2;
            int[] kList = GetMaskAsArray(mask);
            if (kList.Length == 0)
            {
                return;
            }

            if (kList[0] == 31)
            {
                //there are negative numbers and positive numbers
                int sortMask = 1 << kList[0];
                int finalLeft = IsUnsigned()
                    ? IntSorterUtils.PartitionNotStable(array, start, endP1, sortMask)
                    : IntSorterUtils.PartitionReverseNotStable(array, start, endP1, sortMask);
                int n1 = finalLeft - start;
                int n2 = endP1 - finalLeft;

                if (n1 > 1)
                {
                    //sort negative numbers
                    maskParts = CalculateMaskParts(array, start, finalLeft);
                    mask = maskParts.Item1 & maskParts.Item2;
                    kList = GetMaskAsArray(mask);
                    if (kList.Length > 0)
                    {
                        RadixSort(array, start, finalLeft, kList);
                    }
                }

                if (n2 > 1)
                {
                    //sort positive numbers
                    maskParts = CalculateMaskParts(array, finalLeft, endP1);
                    mask = maskParts.Item1 & maskParts.Item2;
                    kList = GetMaskAsArray(mask);
                    if (kList.Length > 0)
                    {
                        RadixSort(array, finalLeft, endP1, kList);
                    }
                }
            }
            else
            {
                RadixSort(array, start, endP1, kList);
            }
        }

        private void RadixSort(int[] array, int start, int endP1, int[] kList)
        {
            int[] aux = new int[endP1 - start];
            RadixSort(array, start, endP1, kList, kList.Length - 1, 0, aux);
        }


        private static void RadixSort(int[] array, int start, int endP1, int[] kList, int kIndexStart, int kIndexEnd,
            int[] aux)
        {
            MaskInfoInt maskInfo = new MaskInfoInt();
            List<(int, int, int)> sections = maskInfo.GetSections(kList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                int maskI = section.Item1;
                int bits = section.Item2;
                int shift = section.Item3;
                if (bits == 1)
                {
                    IntSorterUtils.PartitionStable(array, start, endP1, maskI, aux);
                }
                else
                {
                    int twoPowerBits = 1 << bits;
                    if (shift == 0)
                    {
                        PartitionStableLastBits(array, start, endP1, maskI, twoPowerBits, aux);
                    }
                    else
                    {
                        PartitionStableOneGroupBits(array, start, endP1, maskI, shift, twoPowerBits, aux);
                    }
                }
            }
        }
    }
}