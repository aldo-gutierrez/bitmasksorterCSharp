﻿using System;
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

        bool MaskedEqZero<T>(Func<T, TM> convert, T e, TM mask);

        bool GreaterOrEqZero<T>(Func<T, TM> convert, T e);

        (TM, TM) CalculateMask<T>(Func<T, TM> convert, T[] array, int start, int endP1);

        int[] GetMaskAsArray(TM mask);

        TM GetMaskRangeBits(int bStart, int bEnd);

        void PartitionStableBits<T>(Func<T, TM> convert, T[] array, int start, int endP1,
            TM maskI, int shift, int kRange,
            T[] aux);
    }
}