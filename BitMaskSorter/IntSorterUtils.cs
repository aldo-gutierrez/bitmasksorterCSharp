using System;

namespace BitMaskSorter
{
    internal static class IntSorterUtils
    {
        public static void Swap(int[] array, int left, int right)
        {
            (array[left], array[right]) = (array[right], array[left]);
        }

        public static int PartitionNotStable(int[] array, int start, int endP1, int mask)
        {
            var left = start;
            var right = endP1 - 1;

            while (left <= right)
            {
                var element = array[left];
                if ((element & mask) == 0)
                {
                    left++;
                }
                else
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if ((element & mask) == 0)
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

        public static int PartitionReverseNotStable(int[] array, int start, int endP1, int mask)
        {
            var left = start;
            var right = endP1 - 1;

            while (left <= right)
            {
                var element = array[left];
                if ((element & mask) == 0)
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if ((element & mask) == 0)
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

        public static int PartitionStable(int[] array, int start, int endP1, int mask, int[] aux)
        {
            var left = start;
            var right = 0;
            for (var i = start; i < endP1; i++)
            {
                var element = array[i];
                if ((element & mask) == 0)
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

        public static void PartitionStableLastBits(int[] array, int start, int endP1, int mask, int kRange,
            int[] aux)
        {
            var count = new int[kRange];
            for (var i = start; i < endP1; ++i)
            {
                count[array[i] & mask]++;
            }

            for (int i = 0, sum = 0; i < kRange; ++i)
            {
                var countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (var i = start; i < endP1; ++i)
            {
                var element = array[i];
                aux[count[element & mask]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }

        public static void PartitionStableOneGroupBits(int[] array, int start, int endP1, int mask, int shiftRight,
            int kRange, int[] aux)
        {
            var count = new int[kRange];
            for (var i = start; i < endP1; i++)
            {
                count[(array[i] & mask) >> shiftRight]++;
            }

            for (int i = 0, sum = 0; i < kRange; ++i)
            {
                var countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (var i = start; i < endP1; i++)
            {
                var element = array[i];
                aux[count[(element & mask) >> shiftRight]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }
    }
}