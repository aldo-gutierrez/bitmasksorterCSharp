using System;
using System.Collections.Generic;
using System.Text;

namespace BitMaskSorter
{
    internal interface Sorter
    {
        String name();

        bool isUnsigned();

        void setUnsigned(bool unsigned);

        bool isStable();

    }
}
