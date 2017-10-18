using System.Threading.Tasks;

namespace JaCoCoReader.UI
{
    public delegate Task<bool> CommandTaskCanExecuteDelegate<in T>(T parameter);

    public delegate Task<bool> CommandTaskCanExecuteDelegate();
}