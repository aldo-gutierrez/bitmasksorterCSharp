using System;
using System.Collections.Generic;
using System.Text;

namespace BitMaskSorter
{
    internal class IntSorterUtils
    {
        public static void swap(int[] array, int left, int right)
        {
            int aux = array[left];
            array[left] = array[right];
            array[right] = aux;
        }

        public static int partitionNotStable(int[] array, int start, int end, int mask)
        {
            int left = start;
            int right = end - 1;

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
                            swap(array, left, right);
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

        public static int partitionReverseNotStable(int[] array, int start, int end, int mask)
        {
            int left = start;
            int right = end - 1;

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
                            swap(array, left, right);
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

        public static int partitionStable(int[] array, int start, int end, int mask, int[] aux)
        {
            int left = start;
            int right = 0;
            for (int i = start; i < end; i++)
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

        public static void partitionStableLastBits(int[] array, int start, int end, int mask, int twoPowerK, int[] aux)
        {
            int[] leftX = new int[twoPowerK];
            int[] count = new int[twoPowerK];
            for (int i = start; i < end; i++)
            {
                count[array[i] & mask]++;
            }
            for (int i = 1; i < twoPowerK; i++)
            {
                leftX[i] = leftX[i - 1] + count[i - 1];
            }
            for (int i = start; i < end; i++)
            {
                int element = array[i];
                int elementShiftMasked = element & mask;
                aux[leftX[elementShiftMasked]] = element;
                leftX[elementShiftMasked]++;
            }
            Array.Copy(aux, 0, array, start, end - start);
        }

        public static void partitionStableGroupBits(int[] array, int start, int end, int mask, int shiftRight, int twoPowerK, int[] aux)
        {
            int[] leftX = new int[twoPowerK];
            int[] count = new int[twoPowerK];
             for (int i = start; i < end; i++)
            {
                count[(array[i] & mask) >> shiftRight]++;
            }
            for (int i = 1; i < twoPowerK; i++)
            {
                leftX[i] = leftX[i - 1] + count[i - 1];
            }
            for (int i = start; i < end; i++)
            {
                int element = array[i];
                int elementShiftMasked = (element & mask) >> shiftRight;
                aux[leftX[elementShiftMasked]] = element;
                leftX[elementShiftMasked]++;
            }
            Array.Copy(aux, 0, array, start, end - start);
        }


    }
}
