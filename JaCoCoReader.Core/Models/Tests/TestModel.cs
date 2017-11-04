using System.Collections;
using System.Collections.Generic;

namespace JaCoCoReader.Core.Models.Tests
{
    public abstract class TestModel
    {
        public string Name { get; set; }

        public virtual IEnumerable<TestModel> Items
        {
            get
            {
                yield break;
            }
        }
    }
}
