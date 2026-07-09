using System.Collections;
using System.Collections.Generic;

public class TechList : IReadOnlyList<Tech>
{
    public const int Capacity = 3;

    private readonly List<Tech> _techs = new();

    public int Count => _techs.Count;
    public Tech this[int index] => _techs[index];

    public bool TryAdd(Tech tech)
    {
        if (_techs.Count >= Capacity)
        {
            return false;
        }

        _techs.Add(tech);
        return true;
    }

    public IEnumerator<Tech> GetEnumerator() => _techs.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
