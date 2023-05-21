namespace BOLib.Services
{
    public class InstanceBase<T>
        where T : class, new()
    {
        private static T _instance = new();
        public static T Instance => _instance;
    }
}
