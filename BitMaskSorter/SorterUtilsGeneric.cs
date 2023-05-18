namespace BitMaskSorter
{
    public class SorterUtilsGeneric
    {
        public static void Swap<T>(T[] array, int left, int right)
        {
            (array[left], array[right]) = (array[right], array[left]);
        }


        public static void Reverse<T>(T[] array, int start, int endP1)
        {
            var length = endP1 - start;
            var ld2 = length / 2;
            var end = endP1 - 1;
            for (var i = 0; i < ld2; ++i)
            {
                Swap(array, start + i, end - i);
            }
        }
        
    }
}