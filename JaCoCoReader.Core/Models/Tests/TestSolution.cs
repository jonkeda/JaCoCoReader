namespace JaCoCoReader.Core.Models.Tests
{
    public class TestSolution : TestModel
    {
        public TestProjectCollection Projects { get; } = new TestProjectCollection();

        public override string Type
        {
            get { return "Solution"; }
        }
    }
}