using System;

namespace Common.ConfigSystem
{
    public interface IConfigProvider<out T>
    {
        event Action OnUpdated;
        T Get();
    }
}