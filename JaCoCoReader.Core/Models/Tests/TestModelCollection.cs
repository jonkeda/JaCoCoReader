using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestModelCollection<T> : ObservableCollection<T>
        where T : TestModel
    {
        public void Merge(TestModelCollection<T> models)
        {
            if (models == null)
            {
                return;
            }
            TestModelsByName<T> currentModels = new TestModelsByName<T>(this);
            TestModelsByName<T> newModels = new TestModelsByName<T>(models);

            foreach (T item in this.ToList())
            {
                if (newModels.TryGetValue(item.Name, out T newModel))
                {
                    item.Merge(newModel);
                }
                else
                {
                    Remove(item);
                }
            }

            foreach (T item in models)
            {
                if (!currentModels.TryGetValue(item.Name, out T newModel))
                {
                    int i = IndexOf(item.Name);
                    Insert(i, item);
                }
            }
        }

        public int IndexOf(string name)
        {
            int i = 0;
            foreach (T item in this)
            {
                if (string.Compare(item.Name, name, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    return i;
                }
                i++;
            }
            return 0;
        }
    }

    public class TestModelCollection<TC, TP> : TestModelCollection<TC>
        where TC : TestModel<TP> 
        where TP : TestModel
    {
        private readonly TP _parent;

        public TestModelCollection(TP parent)
        {
            _parent = parent;
        }

        protected override void InsertItem(int index, TC item)
        {
            item.Parent = _parent;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, TC item)
        {
            item.Parent = _parent;
            base.SetItem(index, item);
        }
    }
}