using System.Threading.Tasks;

namespace JaCoCoReader.Core.UI
{
    public delegate Task<bool> CommandTaskCanExecuteDelegate<in T>(T parameter);

    public delegate Task<bool> CommandTaskCanExecuteDelegate();
}