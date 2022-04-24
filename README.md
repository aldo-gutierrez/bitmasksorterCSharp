# Mask Bit Sorters C#
This project tests different ideas for sorting algorithms.
We use a bitmask as a way to get statistical information about the numbers to be sorted.
All the algorithms use this bitmask.

See the initial implementation in java for more information.
[Java Version and Documentation] (https://github.com/aldo-gutierrez/bitmasksorter)

Only RadixBitSorter is implemented for now in this project

## RadixBitSorter:
RadixBitSorter is a Radix Sorter that uses the bitmask to make a LSD sorting using bits instead of bytes
upto 11 bits at a time.

# Speed
Comparison for sorting 1 Million int elements with range from 0 to 1000 in an AMD Ryzen 7 4800H processor,
Debug Mode, VisualStudio 2022, Net 6.0

| Algorithm         | AVG CPU time [ms] |
|-------------------|------------------:|
| C# sort           |                48 |
| RadixBitSorterInt |                11 |


Comparison for sorting 40 Million int elements with range from 0 to 1000 Million in an AMD Ryzen 7 4800H processor,
Debug Mode, VisualStudio 2022, Net 6.0


| Algorithm         | AVG CPU time [ms] |
|-------------------|------------------:|
| C# sort           |              4676 |
| RadixBitSorterInt |              1750 |

# TODO
- Make more graphs
- Port other algorithms from Java repository
