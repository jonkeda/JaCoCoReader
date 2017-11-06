using System.Collections.ObjectModel;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestModelCollection<T, TP> : ObservableCollection<T>
        where T : TestModel<TP> 
        where TP : TestModel
    {
        private readonly TP _parent;

        public TestModelCollection(TP parent)
        {
            _parent = parent;
        }

        protected override void InsertItem(int index, T item)
        {
            item.Parent = _parent;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            item.Parent = _parent;
            base.SetItem(index, item);
        }
    }
}