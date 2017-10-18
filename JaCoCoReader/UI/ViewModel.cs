using System;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace JaCoCoReader.UI
{
    public abstract class ViewModel : PropertyNotifier
    { }

    public abstract class ViewModel<T> 
        : ViewModel where T : class
    {
        protected ViewModel()
        {
        }

        protected ViewModel(T model)
        {
            Model = model;
        }

        protected string FileName { get; private set; }

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

        protected void LoadModel(string fileName)
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