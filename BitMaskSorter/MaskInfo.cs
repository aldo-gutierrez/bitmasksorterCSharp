using System;
using System.Collections.Generic;

namespace BitMaskSorter
{
    public interface MaskInfo<M>
        where M : struct, IComparable, IComparable<M>, IConvertible, IEquatable<M>, IFormattable
    {
        M GetUpperBitMask();
        int GetUpperBit();

        M GetMask();
        void SetMaskParts((M, M) parts);

        bool MaskedEqZero<T>(Func<T, M> convert, T e, M mask);

        bool GreaterOrEqZero<T>(Func<T, M> convert, T e);

        (M, M) CalculateMask<T>(Func<T, M> convert, T[] array, int start, int endP1);

        int[] GetMaskAsArray(M mask);

        List<(M, int, int)> GetSections(int[] kList, int kIndexStart, int kIndexEnd);

    }
}