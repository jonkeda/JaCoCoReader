namespace JaCoCoReader.Core.Models.Tests
{
    public class TestStepCollection : TestModelCollection<TestStep, TestScenario>
    {
        public TestStepCollection(TestScenario parent) : base(parent)
        {
        }
    }
}