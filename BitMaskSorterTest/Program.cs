using System.Diagnostics;
using BitMaskSorter;

Random rnd = new Random();

int arraySize = 1000000;
int range = 1000;
int iterations = 100;
long totalCSharp = 0;
long totalRadix = 0;


for (int j = 0; j < iterations; j++)
{
    // Array of integers
    int[] intArray = new int[arraySize];
    //float[] intArray = new float[arraySize];

    for (int i = 0; i < arraySize; i++)
    {
        intArray[i] = rnd.Next(1, range);
        //intArray[i] = rnd.Next(1, range) - range/2 + rnd.Next(50)/1000;
    }

    //float[] intArrayAux = new float[arraySize];
    int[] intArrayAux = new int[arraySize];

    Array.Copy(intArray, intArrayAux, arraySize);

    // Sort array in ASC order
    Stopwatch stopwatch1 = new Stopwatch();
    stopwatch1.Start();
    Array.Sort(intArray);
    stopwatch1.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch1.ElapsedMilliseconds);
    totalCSharp += stopwatch1.ElapsedMilliseconds;

    Stopwatch stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterInt().sort(intArrayAux, 0, intArrayAux.Length);

    //new RadixBitSorterGenericInt().sort(intArrayAux, 0, intArrayAux.Length);
    //new RadixBitSorterGenericFloat().sort(intArrayAux, 0, intArrayAux.Length);

    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (int i = 0; i < arraySize; i++)
        if (intArrayAux[i] != intArray[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();

}

Console.WriteLine("AVG C#                time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG RadixBitSorterInt time: {0} ms", totalRadix / iterations);
