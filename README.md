# BitMask Sorters C#

This project explores various sorting algorithms employing a BitMask approach.
One of the algorithms is a Radix Sort utilizing a BitMask to minimize the number of Count Sort iterations required.

The following code demonstrates the calculation of the BitMask:

```
    public static int CalculateMaskParts(int[] array, int start, int endP1)
    {
        var pMask = 0x00000000;
        var iMask = 0x00000000;
        for (var i = start; i < endP1; i++)
        {
            var e = array[i];
            pMask = pMask | e;
            iMask = iMask | ~e;
        }
    
        return pMask & iMask;
    }
```

For further details, refer to the initial Java implementation
[Java Version and Documentation] (https://github.com/aldo-gutierrez/bitmasksorter)


## RadixBitSorter:

RadixBitSorter is the implementation of a Radix Sort utilizing a BitMask to minimize the number of Count Sort iterations required.

RadixBitSorter is an LSD Radix Sorter.
The number of bits per iteration has been increased to 11, departing from the standard 8.
For a dual-core machine or lower, it is recommended to use 8 bits.

# Speed
### Comparison for sorting 1 million elements ranging from 0 to 1000.
AMD Ryzen 7 4800H processor, VisualStudio 2022, Net 6.0, Power Options: High Performance

int elements

| Algorithm                  | avg. CPU time [ms] debug mode  | avg. CPU time [ms] release mode  |
|----------------------------|-------------------------------:|---------------------------------:|
| C# sort                    |                             34 |                               34 |
| RadixBitSorterInt          |                             13 |                                4 |
| RadixBitSorterGenericInt   |                             20 |                                8 |

long elements

| Algorithm                  | avg. CPU time [ms] debug mode  | avg. CPU time [ms] release mode  |
|----------------------------|-------------------------------:|---------------------------------:|
| C# sort                    |                             36 |                               36 |
| RadixBitSorterGenericLong  |                             20 |                                9 |

float elements

| Algorithm                  | avg. CPU time [ms] debug mode  | avg. CPU time [ms] release mode  |
|----------------------------|-------------------------------:|---------------------------------:|
| C# sort                    |                             46 |                               46 |
| RadixBitSorterGenericFloat |                             36 |                               14 |

double elements

| Algorithm                  | avg. CPU time [ms] debug mode  | avg. CPU time [ms] release mode   |
|----------------------------|-------------------------------:|----------------------------------:|
| C# sort                    |                             46 |                                46 |
| RadixBitSorterGenericDouble|                             36 |                                14 |


### Comparison for sorting 40 million integer elements ranging from 0 to 1000 million.
AMD Ryzen 7 4800H processor, VisualStudio 2022, Net 6.0, Power Options: High Performance

| Algorithm         | avg. CPU time [ms] debug mode | avg. CPU time [ms] release mode |
|-------------------|------------------------------:|--------------------------------:|
| C# sort           |                          2797 |                            2785 |
| RadixBitSorterInt |                          1560 |                             731 |

# TODO
- Make more graphs
- Port other algorithms from Java repository
