using System;

namespace BitMaskSorter
{
    public static class SorterUtilsGenericInt
    {
        public static int PartitionNotStableUpperBit<T>(T[] array, int start, int endP1, Func<T, int> mapper)
        {
            var left = start;
            var right = endP1 - 1;

            while (left <= right)
            {
                var element = array[left];
                if (mapper(element) >= 0)
                {
                    left++;
                }
                else
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if (mapper(element) >= 0)
                        {
                            SorterUtilsGeneric.Swap(array, left, right);
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

        public static int PartitionReverseNotStableUpperBit<T>(T[] array, int start, int endP1, Func<T, int> mapper)
        {
            var left = start;
            var right = endP1 - 1;

            while (left <= right)
            {
                var element = array[left];
                if (mapper(element) >= 0)
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if (mapper(element) >= 0)
                        {
                            right--;
                        }
                        else
                        {
                            SorterUtilsGeneric.Swap(array, left, right);
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

        public static int PartitionStable<T>(T[] array, int start, int endP1, int mask, Func<T, int> mapper, T[] aux)
        {
            var left = start;
            var right = 0;
            for (var i = start; i < endP1; i++)
            {
                var element = array[i];
                if ((mapper(element) & mask) == 0)
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

        public static void PartitionStableBits<T>(Func<T, int> mapper, T[] array, int start, int endP1, int mask,
            int shiftRight,
            int kRange, T[] aux)
        {
            var count = new int[kRange];
            for (var i = start; i < endP1; i++)
            {
                count[(mapper(array[i]) & mask) >> shiftRight]++;
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
                aux[count[(mapper(element) & mask) >> shiftRight]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }
    }
}