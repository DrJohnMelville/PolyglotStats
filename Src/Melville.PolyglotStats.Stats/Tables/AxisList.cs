using System.Collections.Generic;

namespace Melville.PolyglotStats.Stats.Tables;

public static class AxisList
{
    public static int DimensionLength<TStorage>(this IList<ITableAxis<TStorage>> axis, int start)
    {
        int ret = 1;
        for (int i = start + 1; i < axis.Count; i++)
        {
            ret *= axis[i].Elements.Count;
        }
        return ret;
    }
    public static int DimensionCopies<TStorage>(this IList<ITableAxis<TStorage>> axis, int start)
    {
        int ret = 1;
        for (int i = 0; i < axis.Count && i < start; i++)
        {
            ret *= axis[i].Elements.Count;
        }
        return ret;
    }

}