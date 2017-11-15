using JaCoCoReader.Core.UI.Controls;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.UI
{
    public abstract class NodeViewModel<T> : NodeViewModel
    {
        public T Model { get; protected set; }

        protected NodeViewModel()
        { }

        protected NodeViewModel(T model)
        {
            Model = model;
        }

        public abstract FontAwesomeIcon Icon { get; }
    }

    public abstract class NodeViewModel : ViewModel , ITreeViewItem
    {
        private bool _bringIntoView;
        private bool _isSelected;
        private bool _isExpanded;
        private bool _isHighlighted;
        public abstract string Description { get; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public bool BringIntoView
        {
            get { return _bringIntoView; }
            set { SetProperty(ref _bringIntoView, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set { SetProperty(ref _isHighlighted, value); }
        }
    }
}