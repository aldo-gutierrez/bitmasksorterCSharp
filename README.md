# Mask Bit Sorters C#

This project use a bitmask for speeding Radix Sorting

See the initial implementation in java for more information.
[Java Version and Documentation] (https://github.com/aldo-gutierrez/bitmasksorter)

Only RadixBitSorter is implemented for in this project

## RadixBitSorter:
RadixBitSorter is a Radix Sorter that uses the bitmask to make a LSD sorting using bits instead of bytes
upto 11 bits at a time.

# Speed
Comparison for sorting 1 Million int elements with range from 0 to 1000
AMD Ryzen 7 4800H processor, VisualStudio 2022, Net 6.0, Power Options: High Performance

| Algorithm                  | AVG CPU time [ms] Debug Mode | AVG CPU time [ms] Release Mode |
|----------------------------|-----------------------------:|-------------------------------:|
| C# sort                    |                           34 |                             34 |
| RadixBitSorterInt          |                           13 |                              4 |
| RadixBitSorterGenericInt   |                           29 |                              8 |
| RadixBitSorterGeneric<int> |                           76 |                             31 |


Comparison for sorting 40 Million int elements with range from 0 to 1000 Million
AMD Ryzen 7 4800H processor, VisualStudio 2022, Net 6.0, Power Options: High Performance

| Algorithm         | AVG CPU time [ms] Debug Mode | AVG CPU time [ms] Release Mode |
|-------------------|-----------------------------:|-------------------------------:|
| C# sort           |                         2797 |                           2785 |
| RadixBitSorterInt |                         1560 |                            731 |

# TODO
- Make more graphs
- Port other algorithms from Java repository
