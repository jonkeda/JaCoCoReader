using System.Collections.Generic;
using System.Windows.Media;
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
            set
            {
                if (SetProperty(ref _outcome, value))
                {
                    NotifyPropertyChanged(nameof(Background));
                }
            }
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

        public Brush Background
        {
            get
            {
                switch (Outcome)
                {
                    case TestOutcome.Failed:
                        return Brushes.DarkRed;
                    case TestOutcome.Passed:
                        return Brushes.DarkGreen;
                    default:
                        return ViewModels.Colors.DefaultBackground;
                }
            }
        }
    }
}
