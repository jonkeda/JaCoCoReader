using System.Collections;
using System.Collections.Generic;

namespace JaCoCoReader.UI
{
    public abstract class NodeCollectionViewModel<T, TV> : NodesViewModel<T, TV>
        where T : IEnumerable
        where TV : ViewModel
    {
        protected NodeCollectionViewModel()
        { }

        protected NodeCollectionViewModel(T model) : base(model)
        {
        }
    }
}