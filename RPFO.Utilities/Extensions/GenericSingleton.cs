using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Utilities.Extensions
{
    public sealed class GenericSingleton<T> where T : class, new()
    {
        private static T instance;

        public static T GetInstance()
        {
            lock (typeof(T))
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}
