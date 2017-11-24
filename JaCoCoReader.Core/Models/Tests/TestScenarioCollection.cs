namespace JaCoCoReader.Core.Models.Tests
{
    public class TestScenarioCollection : TestModelCollection<TestScenario, TestFeature>
    {
        public TestScenarioCollection(TestFeature parent) : base(parent)
        {
        }
    }
}