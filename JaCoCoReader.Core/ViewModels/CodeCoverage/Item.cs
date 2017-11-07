namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public class Item<T>
    {
        public Item(T value, string description)
        {
            Value = value;
            Description = description;
        }

        public T Value { get; }
        public string Description { get; }
        public override string ToString()
        {
            return Description;
        }
    }
}