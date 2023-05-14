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

        public void Sort(int[] array, int start, int endP1)
        {
            var n = endP1 - start;
            if (n < 2)
            {
                return;
            }

            var maskParts = CalculateMaskParts(array, start, endP1);
            var mask = maskParts.Item1 & maskParts.Item2;
            var bList = GetMaskAsArray(mask);
            if (bList.Length == 0)
            {
                return;
            }

            if (bList[0] == 31)
            {
                //there are negative numbers and positive numbers
                var sortMask = 1 << bList[0];
                var finalLeft = IsUnsigned()
                    ? PartitionNotStable(array, start, endP1, sortMask)
                    : PartitionReverseNotStable(array, start, endP1, sortMask);
                var n1 = finalLeft - start;
                var n2 = endP1 - finalLeft;

                if (n1 > 1)
                {
                    //sort negative numbers
                    maskParts = CalculateMaskParts(array, start, finalLeft);
                    mask = maskParts.Item1 & maskParts.Item2;
                    bList = GetMaskAsArray(mask);
                    if (bList.Length > 0)
                    {
                        RadixSort(array, start, finalLeft, bList);
                    }
                }

                if (n2 > 1)
                {
                    //sort positive numbers
                    maskParts = CalculateMaskParts(array, finalLeft, endP1);
                    mask = maskParts.Item1 & maskParts.Item2;
                    bList = GetMaskAsArray(mask);
                    if (bList.Length > 0)
                    {
                        RadixSort(array, finalLeft, endP1, bList);
                    }
                }
            }
            else
            {
                RadixSort(array, start, endP1, bList);
            }
        }

        private void RadixSort(int[] array, int start, int endP1, int[] bList)
        {
            var aux = new int[endP1 - start];
            RadixSort(array, start, endP1, bList, bList.Length - 1, 0, aux);
        }


        private static void RadixSort(int[] array, int start, int endP1, int[] bList, int kIndexStart, int kIndexEnd,
            int[] aux)
        {
            var maskInfo = new MaskInfoInt();
            var sections = BitSorterUtils.GetSections(bList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                var bStartIndex = section.Item1;
                var bits = section.Item2;
                var shift = section.Item3;
                var mask = maskInfo.GetMaskRangeBits(bStartIndex, shift);
                if (bits == 1)
                {
                    PartitionStable(array, start, endP1, mask, aux);
                }
                else
                {
                    var kRange = 1 << bits;
                    if (shift == 0)
                    {
                        PartitionStableLastBits(array, start, endP1, mask, kRange, aux);
                    }
                    else
                    {
                        PartitionStableOneGroupBits(array, start, endP1, mask, shift, kRange, aux);
                    }
                }
            }
        }
    }
}