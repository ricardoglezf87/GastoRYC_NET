namespace GARCA.Utlis.Extensions
{
    public static class DoubleExtension
    {
        public static int CompareTo(this double? source, double? other)
        {
            return source < other ? -1 : source > other ? 1 : 0;
        }
    }
}
