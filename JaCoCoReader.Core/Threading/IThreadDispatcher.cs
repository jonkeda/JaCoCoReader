using System;

namespace JaCoCoReader.Core.Threading
{
    public interface IThreadDispatcher
    {
        bool ShouldInvoke();

        void Invoke(Action action);
    }
}