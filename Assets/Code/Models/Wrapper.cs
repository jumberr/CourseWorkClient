using System;

namespace Code.Models
{
    [Serializable]
    public class Wrapper<T>
    {
        public T[] Items;
    }
}