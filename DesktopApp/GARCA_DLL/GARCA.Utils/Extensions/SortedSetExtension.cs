namespace GARCA.Utils.Extensions
{
    public static class SortedSetExtension
    {
        public static void AddRange<Transactions>(this SortedSet<Transactions?>? source, SortedSet<Transactions?>? elements)
        {
            if (elements != null)
            {
                foreach (var obj in elements)
                {
                    if (obj != null)
                    {
                        source.Add(obj);
                    }
                }
            }
        }
    }
}
