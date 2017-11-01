using System.Threading.Tasks;

namespace JaCoCoReader.Core.UI
{
    public delegate Task CommandTaskDelegate();


    public delegate Task CommandTaskDelegate<in T>(T parameter);
}