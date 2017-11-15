using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace JaCoCoReader.Core.Models.CodeCoverage
{
    public abstract class ModelCollection<T, TK> : Collection<T>
        where T : Model<T, TK>
    {
        protected override void ClearItems()
        {
            base.ClearItems();
            ModelsByKey = null;
        }

        private class ModelDictionary : ConcurrentDictionary<TK, T>
        { }

        private readonly object _lock = new object();
        private ModelDictionary ModelsByKey { get; set; }
        public void Merge(ModelCollection<T, TK> models)
        {
            if (ModelsByKey == null)
            {
                lock (_lock)
                {
                    if (ModelsByKey == null)
                    {
                        ModelDictionary modelsByKey = new ModelDictionary();

                        foreach (T model in this)
                        {
                            modelsByKey.TryAdd(model.Key, model);
                        }

                        ModelsByKey = modelsByKey;
                    }
                }
            }
            foreach (T model in models)
            {
                ModelsByKey.AddOrUpdate(model.Key,
                    k =>
                    {
                        Add(model);
                        return model;
                    },
                    (k, m) => m.Merge(model));
            }
        }
    }
}