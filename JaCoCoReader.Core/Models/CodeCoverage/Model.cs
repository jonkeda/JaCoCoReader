namespace JaCoCoReader.Core.Models.CodeCoverage
{
    public abstract class Model<T, TK>
    {
        public abstract T Merge(T model);

        public abstract TK Key { get; }
    }
}