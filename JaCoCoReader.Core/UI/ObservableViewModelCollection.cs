using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace JaCoCoReader.Core.UI
{
    public abstract class ObservableViewModelCollection<TVm, TM> : ObservableCollection<TVm>
        where TVm : ViewModel<TM>
        where TM : class, new()
    {
        private readonly ObservableCollection<TM> _models;
        

        protected ObservableViewModelCollection(ObservableCollection<TM> models)
        {
            _models = models;
            Models = new ReadOnlyCollection<TM>(models);
            _models.CollectionChanged += ModelsOnCollectionChanged;
        }

        public ReadOnlyCollection<TM> Models { get; }

        protected abstract TVm CreatViewModel(TM model);

        private void ModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int i = e.NewStartingIndex;
                foreach (object item in e.NewItems)
                {
                    TM model = item as TM;
                    Insert(i, CreatViewModel(model));
                    i++;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
               // TODO
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                int i = e.OldStartingIndex;
                foreach (object item in e.OldItems)
                {
                    RemoveAt(i);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                // TODO
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            Clear();
            foreach (TM model in _models)
            {
                Add(CreatViewModel(model));
            }
        }
    }
}