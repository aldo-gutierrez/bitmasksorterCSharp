﻿using System.Diagnostics;
using BitMaskSorter;

var rnd = new Random();

const int arraySize = 1000000;
const int range = 1000;
const int iterations = 25;
long totalCSharp = 0;
long totalRadix1 = 0;
long totalRadix2 = 0;


for (var iteration = 0; iteration < iterations; iteration++)
{
    var array = new int[arraySize];

    for (var i = 0; i < arraySize; i++)
    {
        array[i] = rnd.Next(1, range);
    }

    var arrayAux1 = new int[arraySize];
    Array.Copy(array, arrayAux1, arraySize);

    var arrayAux2 = new int[arraySize];
    Array.Copy(array, arrayAux2, arraySize);

    // Sort array in ASC order
    var stopwatch0 = new Stopwatch();
    stopwatch0.Start();
    Array.Sort(array);
    stopwatch0.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch0.ElapsedMilliseconds);
    totalCSharp += stopwatch0.ElapsedMilliseconds;

    var stopwatch1 = new Stopwatch();
    stopwatch1.Start();
    new RadixBitSorterInt().Sort(arrayAux1, 0, arrayAux1.Length);
    stopwatch1.Stop();
    Console.WriteLine("Elapsed Time Radix is  {0} ms", stopwatch1.ElapsedMilliseconds);
    totalRadix1 += stopwatch1.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux1[i] != array[i])
        {
            Console.WriteLine("Arrays are not equal. RadixBitSorterInt");
            break;
        }

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterGenericInt().Sort(arrayAux2, 0, arrayAux2.Length);
    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix Generic is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix2 += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux2[i] != array[i])
        {
            Console.WriteLine("Arrays are not equal. RadixBitSorterGenericInt");
            break;
        }

    Console.WriteLine();
}

Console.WriteLine("AVG<int> C#                       time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG<int> RadixBitSorterInt        time: {0} ms", totalRadix1 / iterations);
Console.WriteLine("AVG<int> RadixBitSorterGenericInt time: {0} ms", totalRadix2 / iterations);
Console.ReadKey();


totalCSharp = 0;
totalRadix1 = 0;
totalRadix2 = 0;

for (var iteration = 0; iteration < iterations; iteration++)
{
    var array = new long[arraySize];

    for (var i = 0; i < arraySize; i++)
    {
        array[i] = rnd.Next(1, range);
    }

    var arrayAux1 = new long[arraySize];
    Array.Copy(array, arrayAux1, arraySize);

    var arrayAux2 = new long[arraySize];
    Array.Copy(array, arrayAux2, arraySize);

    // Sort array in ASC order
    var stopwatch0 = new Stopwatch();
    stopwatch0.Start();
    Array.Sort(array);
    stopwatch0.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch0.ElapsedMilliseconds);
    totalCSharp += stopwatch0.ElapsedMilliseconds;

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterGenericLong().Sort(arrayAux2, 0, arrayAux2.Length);
    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix Generic is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix2 += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux2[i] != array[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();
}

Console.WriteLine("AVG<long>  C#                        time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG<long>  RadixBitSorterGenericLong time: {0} ms", totalRadix2 / iterations);
Console.ReadKey();


totalCSharp = 0;
totalRadix1 = 0;
totalRadix2 = 0;

for (var iteration = 0; iteration < iterations; iteration++)
{
    var array = new float[arraySize];

    for (var i = 0; i < arraySize; i++)
    {
        array[i] = rnd.Next(1, range) + rnd.Next(50) / 1000;
    }

    var arrayAux1 = new float[arraySize];
    Array.Copy(array, arrayAux1, arraySize);

    var arrayAux2 = new float[arraySize];
    Array.Copy(array, arrayAux2, arraySize);

    // Sort array in ASC order
    var stopwatch0 = new Stopwatch();
    stopwatch0.Start();
    Array.Sort(array);
    stopwatch0.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch0.ElapsedMilliseconds);
    totalCSharp += stopwatch0.ElapsedMilliseconds;

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterGenericFloat().Sort(arrayAux2, 0, arrayAux2.Length);
    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix Generic is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix2 += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux2[i] != array[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();
}

Console.WriteLine("AVG<float>  C#                         time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG<float>  RadixBitSorterGenericFloat time: {0} ms", totalRadix2 / iterations);
Console.ReadKey();

totalCSharp = 0;
totalRadix1 = 0;
totalRadix2 = 0;

for (var iteration = 0; iteration < iterations; iteration++)
{
    var array = new double[arraySize];

    for (var i = 0; i < arraySize; i++)
    {
        array[i] = rnd.Next(1, range) + rnd.Next(50) / 1000;
    }

    var arrayAux1 = new double[arraySize];
    Array.Copy(array, arrayAux1, arraySize);

    var arrayAux2 = new double[arraySize];
    Array.Copy(array, arrayAux2, arraySize);

    // Sort array in ASC order
    var stopwatch0 = new Stopwatch();
    stopwatch0.Start();
    Array.Sort(array);
    stopwatch0.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch0.ElapsedMilliseconds);
    totalCSharp += stopwatch0.ElapsedMilliseconds;

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterGenericDouble().Sort(arrayAux2, 0, arrayAux2.Length);
    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix Generic is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix2 += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux2[i] != array[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();
}

Console.WriteLine("AVG<double>  C#                          time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG<double>  RadixBitSorterGenericDouble time: {0} ms", totalRadix2 / iterations);
Console.ReadKey();


totalCSharp = 0;
totalRadix1 = 0;
totalRadix2 = 0;

for (var iteration = 0; iteration < iterations; iteration++)
{
    var array = new short[arraySize];

    for (var i = 0; i < arraySize; i++)
    {
        array[i] = (short)rnd.Next(1, range);
    }

    var arrayAux1 = new short[arraySize];
    Array.Copy(array, arrayAux1, arraySize);

    var arrayAux2 = new short[arraySize];
    Array.Copy(array, arrayAux2, arraySize);

    // Sort array in ASC order
    var stopwatch0 = new Stopwatch();
    stopwatch0.Start();
    Array.Sort(array);
    stopwatch0.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch0.ElapsedMilliseconds);
    totalCSharp += stopwatch0.ElapsedMilliseconds;

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterGenericShort().Sort(arrayAux2, 0, arrayAux2.Length);
    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix Generic is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix2 += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux2[i] != array[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();
}

Console.WriteLine("AVG<short>  C#                         time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG<short>  RadixBitSorterGenericShort time: {0} ms", totalRadix2 / iterations);
Console.ReadKey();



totalCSharp = 0;
totalRadix1 = 0;
totalRadix2 = 0;

for (var iteration = 0; iteration < iterations; iteration++)
{
    var array = new ushort[arraySize];

    for (var i = 0; i < arraySize; i++)
    {
        array[i] = (ushort)rnd.Next(1, range);
    }

    var arrayAux1 = new ushort[arraySize];
    Array.Copy(array, arrayAux1, arraySize);

    var arrayAux2 = new ushort[arraySize];
    Array.Copy(array, arrayAux2, arraySize);

    // Sort array in ASC order
    var stopwatch0 = new Stopwatch();
    stopwatch0.Start();
    Array.Sort(array);
    stopwatch0.Stop();
    Console.WriteLine("Elapsed Time C#    is {0} ms", stopwatch0.ElapsedMilliseconds);
    totalCSharp += stopwatch0.ElapsedMilliseconds;

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();
    new RadixBitSorterGenericUShort().Sort(arrayAux2, 0, arrayAux2.Length);
    stopwatch2.Stop();
    Console.WriteLine("Elapsed Time Radix Generic is  {0} ms", stopwatch2.ElapsedMilliseconds);
    totalRadix2 += stopwatch2.ElapsedMilliseconds;

    // Linearly compare elements
    for (var i = 0; i < arraySize; i++)
        if (arrayAux2[i] != array[i])
            Console.WriteLine("Arrays are not equal.");

    Console.WriteLine();
}

Console.WriteLine("AVG<ushort>  C#                          time: {0} ms", totalCSharp / iterations);
Console.WriteLine("AVG<ushort>  RadixBitSorterGenericUShort time: {0} ms", totalRadix2 / iterations);
Console.ReadKey();