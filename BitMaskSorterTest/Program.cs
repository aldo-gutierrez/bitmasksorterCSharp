// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using BitMaskSorter;

Stopwatch stopwatch = new Stopwatch();
Console.WriteLine("Hello, World!");
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

    for (int i = 0; i < arraySize; i++)
    {
        intArray[i] = rnd.Next(1, range);
    }

    int[] intArrayAux = new int[arraySize];

    Array.Copy(intArray, intArrayAux, arraySize);

    // Sort array in ASC order
    stopwatch.Start();
    Array.Sort(intArray);
    stopwatch.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch.ElapsedMilliseconds);
    totalCSharp += stopwatch.ElapsedMilliseconds;

    intArrayAux = new int[arraySize];
    Array.Copy(intArray, intArrayAux, arraySize);
    stopwatch = new Stopwatch();
    stopwatch.Start();
    RadixBitSorterInt radixBiSorterInt = new RadixBitSorterInt();
    radixBiSorterInt.sort(intArray);
    stopwatch.Stop();
    Console.WriteLine("Elapsed Time Radix is  {0} ms", stopwatch.ElapsedMilliseconds);
    totalRadix += stopwatch.ElapsedMilliseconds;

    // Linearly compare elements
    for (int i = 0; i < arraySize; i++)
        if (intArrayAux[i] != intArray[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();

}

Console.WriteLine("AVG C#                time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG RadixBitSorterInt time: {0} ms", totalRadix / iterations);
