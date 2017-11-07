using System;
using System.Windows.Threading;

namespace JaCoCoReader.Core.Threading
{
    public static class ThreadDispatcher
    {
        static ThreadDispatcher()
        {
            // Dispatcher = new DefaultThreadDispatcher();
            Dispatcher = new WpfDispatcher();
        }

        public static IThreadDispatcher Dispatcher { get; set; }

        public static bool ShouldInvoke()
        {
            return Dispatcher.ShouldInvoke();
        }

        public static void Invoke(Action action)
        {
            if (ShouldInvoke())
            {
                Dispatcher.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }

        public static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
    }
}