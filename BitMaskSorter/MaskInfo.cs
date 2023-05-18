using System;
using System.Collections.Generic;

namespace BitMaskSorter
{
    public interface IMaskInfo<TM>
        where TM : struct, IComparable, IComparable<TM>, IConvertible, IEquatable<TM>, IFormattable
    {
        TM GetUpperBitMask();
        int GetUpperBit();

        TM GetMask();
        void SetMaskParts((TM, TM) parts);
        
        (TM, TM) CalculateMask<T>(Func<T, TM> convert, T[] array, int start, int endP1);

        int[] GetMaskAsArray(TM mask);

        TM GetMaskRangeBits(int bStart, int bEnd);

    }
}