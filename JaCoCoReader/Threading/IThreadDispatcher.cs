using System;

namespace JaCoCoReader.Threading
{
    public interface IThreadDispatcher
    {
        bool ShouldInvoke();

        void Invoke(Action action);
    }
}