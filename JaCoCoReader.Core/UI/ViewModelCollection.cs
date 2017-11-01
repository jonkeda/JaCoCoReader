using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace JaCoCoReader.Core.UI
{
    public class ViewModelCollection<TVm, TM> : Collection<TVm>
        where TVm : ViewModel<TM>
        where TM : class, new()
    {
        private readonly Func<TM,TVm> _createViewModel;

        public ViewModelCollection(Func<TM, TVm> createViewModel)
        {
            _createViewModel = createViewModel;
        }

        public TVm Get(TM model)
        {
            if (model == null)
            {
                return null;
            }

            TVm viewModel =
                this.FirstOrDefault(i => ReferenceEquals(i.Model, model));
            if (viewModel == null)
            {
                viewModel = _createViewModel(model);
                Add(viewModel);
            }
            return viewModel;
        }
    }
}