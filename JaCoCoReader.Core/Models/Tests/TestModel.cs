using System.Collections.Generic;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.Models.Tests
{
    public abstract class TestModel : PropertyNotifier
    {
        private TestOutcome _outcome;
        public string Name { get; set; }

        public TestOutcome Outcome
        {
            get { return _outcome; }
            set { SetProperty(ref _outcome, value); }
        }

        public void SetOutcome(TestOutcome outcome)
        {
            Outcome = outcome;
            foreach (TestModel model in Items)
            {
                model.SetOutcome(outcome);
            }
        }

        public virtual IEnumerable<TestModel> Items
        {
            get
            {
                yield break;
            }
        }
    }
}
