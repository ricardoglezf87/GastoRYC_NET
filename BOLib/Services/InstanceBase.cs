using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOLib.Services
{
    public class InstanceBase<T>
        where T : class, new()
    {
        private static readonly T _instance = new();
        public static T Instance => _instance;
    }
}
