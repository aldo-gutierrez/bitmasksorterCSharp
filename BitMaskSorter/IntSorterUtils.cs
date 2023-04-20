using System;

namespace BitMaskSorter
{
    internal class IntSorterUtils
    {
        public static void Swap(int[] array, int left, int right)
        {
            int aux = array[left];
            array[left] = array[right];
            array[right] = aux;
        }

        public static int PartitionNotStable(int[] array, int start, int endP1, int mask)
        {
            int left = start;
            int right = endP1 - 1;

            while (left <= right)
            {
                int element = array[left];
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
            int left = start;
            int right = endP1 - 1;

            while (left <= right)
            {
                int element = array[left];
                if ((element & mask) == 0)
                {
                    while (left <= right)
                    {
                        element = array[right];
                        if (((element & mask) == 0))
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
            int left = start;
            int right = 0;
            for (int i = start; i < endP1; i++)
            {
                int element = array[i];
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

        public static void PartitionStableLastBits(int[] array, int start, int endP1, int mask, int twoPowerK,
            int[] aux)
        {
            int[] count = new int[twoPowerK];
            for (int i = start; i < endP1; ++i)
            {
                count[array[i] & mask]++;
            }

            for (int i = 0, sum = 0; i < twoPowerK; ++i)
            {
                int countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (int i = start; i < endP1; ++i)
            {
                int element = array[i];
                aux[count[element & mask]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }

        public static void PartitionStableOneGroupBits(int[] array, int start, int endP1, int mask, int shiftRight,
            int twoPowerK, int[] aux)
        {
            int[] count = new int[twoPowerK];
            for (int i = start; i < endP1; i++)
            {
                count[(array[i] & mask) >> shiftRight]++;
            }

            for (int i = 0, sum = 0; i < twoPowerK; ++i)
            {
                int countI = count[i];
                count[i] = sum;
                sum += countI;
            }

            for (int i = start; i < endP1; i++)
            {
                int element = array[i];
                aux[count[(element & mask) >> shiftRight]++] = element;
            }

            Array.Copy(aux, 0, array, start, endP1 - start);
        }
    }
}