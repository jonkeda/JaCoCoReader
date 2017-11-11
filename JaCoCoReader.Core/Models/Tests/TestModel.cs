using System;
using System.Collections.Generic;
using System.Windows.Media;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.Models.Tests
{
    public abstract class TestModel : PropertyNotifier
    {
        public string Name { get; set; }
        public string ErrorStackTrace { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan? Time { get; set; }
        public abstract string Type { get; }

        private string _output;
        public string Output
        {
            get { return _output; }
            set { SetProperty(ref _output, value); }
        }

        private TestOutcome _outcome;
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

    public abstract class TestModel<T> : TestModel
    {
        public T Parent { get; set; }
    }
}
