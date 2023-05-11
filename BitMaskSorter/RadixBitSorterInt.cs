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
            var kList = GetMaskAsArray(mask);
            if (kList.Length == 0)
            {
                return;
            }

            if (kList[0] == 31)
            {
                //there are negative numbers and positive numbers
                var sortMask = 1 << kList[0];
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
            var aux = new int[endP1 - start];
            RadixSort(array, start, endP1, kList, kList.Length - 1, 0, aux);
        }


        private static void RadixSort(int[] array, int start, int endP1, int[] kList, int kIndexStart, int kIndexEnd,
            int[] aux)
        {
            var maskInfo = new MaskInfoInt();
            var sections = maskInfo.GetSections(kList, kIndexStart, kIndexEnd);
            foreach (var section in sections)
            {
                var maskI = section.Item1;
                var bits = section.Item2;
                var shift = section.Item3;
                if (bits == 1)
                {
                    PartitionStable(array, start, endP1, maskI, aux);
                }
                else
                {
                    var twoPowerBits = 1 << bits;
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