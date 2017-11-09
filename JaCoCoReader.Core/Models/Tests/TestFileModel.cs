using System;
using System.IO;

namespace JaCoCoReader.Core.Models.Tests
{
    public interface ITestFileModel
    {
        string Path { get; set; }
        string Text { get; }
    }

    public abstract class TestFileModel<TC> : TestModel<TC>, ITestFileModel
        where TC : TestModel
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
}