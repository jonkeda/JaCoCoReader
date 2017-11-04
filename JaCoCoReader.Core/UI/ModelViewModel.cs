namespace JaCoCoReader.Core.UI
{
    public abstract class ModelViewModel<T> 
        : ViewModel where T : class, new()
    {
        protected ModelViewModel()
        {
            Model = new T();
        }

        protected ModelViewModel(T model)
        {
            Model = model;
        }

        private T _model;
        public T Model
        {
            get { return _model; }
            set
            {
                if (SetProperty(ref _model, value))
                {
                    OnModelChanged();

                }
            }
        }

        protected virtual void OnModelChanged()
        {
            
        }

    }
}