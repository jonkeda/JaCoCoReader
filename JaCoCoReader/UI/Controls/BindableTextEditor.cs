using System;
using System.ComponentModel;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace JaCoCoReader.UI.Controls
{
    public class BindableTextEditor : TextEditor, INotifyPropertyChanged
    {
        /// <summary>
        /// A bindable Text property
        /// </summary>
        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// The bindable text property dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BindableTextEditor), new PropertyMetadata(
                (obj, args) =>
                {
                    BindableTextEditor target = (BindableTextEditor)obj;
                    target.Text = (string)args.NewValue;
                }));

        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged(nameof(Text));
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Raises a property changed event
        /// </summary>
        /// <param name="property">The name of the property that updates</param>
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    // https://stackoverflow.com/questions/22415248/whole-line-highlighting-in-avalonedit
    // 
}