using System;
using System.Collections.Generic;
using System.Windows.Media;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI;
using JaCoCoReader.Core.UI.Icons;

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

        public abstract FontAwesomeIcon Icon { get; }

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

        public int LineNr { get; set; }

        public virtual void CalculateOutcome()
        {
            TestOutcome outcome = TestOutcome.None;
            foreach (TestModel model in Items)
            {
                model.CalculateOutcome();
                if (model.Outcome > outcome)
                {
                    outcome = model.Outcome;
                }
            }
            Outcome = outcome;
        }


        public void SetOutcome(TestOutcome outcome)
        {
            Outcome = outcome;
            foreach (TestModel model in Items)
            {
                model.SetOutcome(outcome);
            }
        }

        public TestModel FindModelByLineNumber(int lineNumber)
        {
            if (lineNumber == LineNr)
            {
                return this;
            }
            foreach (TestModel model in Items)
            {
                TestModel found = model.FindModelByLineNumber(lineNumber);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
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
                        return Brushes.Black;
                }
            }
        }


        public void NotifyItemsChanged()
        {
            NotifyPropertyChanged(nameof(Items));
        }

        public void Merge(TestModel model)
        {
            LineNr = model.LineNr;
            DoMerge(model);
        }

        protected abstract void DoMerge(TestModel model);

    }

    public abstract class TestModel<T> : TestModel
        where T : TestModel
    {
        public T Parent { get; set; }
    }
}
