using System;
using System.Collections.Generic;
using System.IO;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFileModel : TestModel
    {
        public string Path { get; set; }

        private string _text;
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(_text))
                {
                    try
                    {
                        _text = File.ReadAllText(Path);
                    }
                    catch (Exception ex)
                    {
                        _text = ex.Message;
                    }
                }
                return _text;
            }
        }
    }
    public class TestFile : TestFileModel
    {
        public TestDescribeCollection Describes { get; } = new TestDescribeCollection();

        public override IEnumerable<TestModel> Items
        {
            get { return Describes; }
        }


    }
}