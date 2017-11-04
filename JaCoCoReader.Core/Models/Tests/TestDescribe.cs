using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation.Language;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestDescribe : TestFileModel
    {
        public TestContextCollection Contexts { get; } = new TestContextCollection();

       // public TestItCollection Its { get; } = new TestItCollection();
        public Ast Ast { get; set; }

        public override IEnumerable<TestModel> Items
        {
            get { return Contexts; }
        }

        public IEnumerable TestResults { get; set; }

        public void ProcessTestResults(Array results)
        {
           // throw new NotImplementedException();
        }
    }
}