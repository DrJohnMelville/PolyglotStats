using System.Collections;
using System.Collections.Generic;
using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Stats.Tables;

public class ObjectIdentitySet<T>: IEnumerable<T>
{
    private readonly IList<T> items;
    public ObjectIdentitySet() : this (new List<T>())
    {
    }

    public ObjectIdentitySet(IEnumerable<T> collection)
    {
        items = collection.AsList();
    }
      
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => items.Count;

    public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

    public IEnumerable<T> Intersect(ObjectIdentitySet<T> other)
    {
        foreach (var item in items)
        {
            foreach (var otherItem in other.items)
            {
                if (object.ReferenceEquals(item, otherItem))
                {
                    yield return item;
                }
            }
        }
    }
}