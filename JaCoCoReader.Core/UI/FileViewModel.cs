using System;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace JaCoCoReader.Core.UI
{
    public abstract class FileViewModel<T> 
        : ModelViewModel<T> where T : class, new()
    {
        protected FileViewModel()
        {
        }

        protected FileViewModel(T model) : base(model)
        {
        }

        protected string FileName { get; private set; }

        public ICommand LoadCommand
        {
            get
            {
                return new TargetCommand(DoLoadCommand);
            }
        }

        private void DoLoadCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                LoadModel(ofd.FileName);
            }
        }

        protected virtual void LoadModel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            XmlSerializer xml = new XmlSerializer(typeof(T));

            FileName = fileName;

            using (var s = File.OpenRead(fileName))
            {
                try
                {
                    Model = xml.Deserialize(s) as T;
                }
                catch (Exception e)
                {
                    AddError(e);
                }
            }
        }

        protected virtual void AddError(Exception exception)
        {
          
        }

        public ICommand SaveCommand
        {
            get
            {
                return new TargetCommand(DoSaveCommand);
            }
        }

        private void DoSaveCommand()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                DoSaveAsCommand();
            }
            else
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));

                using (var s = File.CreateText(FileName))
                {
                    xml.Serialize(s, Model);
                }

            }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return new TargetCommand(DoSaveAsCommand);
            }
        }

        private void DoSaveAsCommand()
        {
            SaveFileDialog ofd = new SaveFileDialog();
            if (ofd.ShowDialog() == true)
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                FileName = ofd.FileName;
                using (var s = File.CreateText(ofd.FileName))
                {
                    xml.Serialize(s, Model);
                }
            }
        }
    }
}