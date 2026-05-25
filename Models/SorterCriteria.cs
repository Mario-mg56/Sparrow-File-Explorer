using System;
namespace DynamicFileExplorer.Models;
public class SorterCriteria<T> : ISorterCriteria where T : IComparable<T>
{
    public bool IsAscending { get; set; }

    public bool Compare(object a, object b)
    {
        return Compare((T)a, (T)b);
    }

    public bool Compare(T a, T b)
    {
        int result = a.CompareTo(b);
        return IsAscending ? result < 0 : result > 0;
    }
    
}
public interface ISorterCriteria
    {
        bool Compare(object a, object b);
    }