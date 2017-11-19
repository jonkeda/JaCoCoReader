using System.Collections.Generic;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestModelsByName<T> : Dictionary<string, T>
        where T : TestModel
    {
        public TestModelsByName(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (!ContainsKey(item.Name))
                {
                    Add(item.Name, item);
                }
            }
        }
    }
}