using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JaCoCoReader.UI.Controls
{
    [SuppressMessage("Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class TreeViewEx : TreeView
    {
        #region SelectedNode

        public static readonly DependencyProperty SelectedNodeItemProperty =
            DependencyProperty.Register(nameof(SelectedNodeItem), typeof(object), typeof(TreeViewEx),
                new PropertyMetadata(SelectedNodeItemChanged));

        private static void SelectedNodeItemChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            TreeViewEx tv = dependencyObject as TreeViewEx;
            if (tv == null)
            {
                return;
            }

            ITreeViewItem item;
            if ((item = e.OldValue as ITreeViewItem) != null)
            {
                item.IsSelected = false;
            }

            if ((item = e.NewValue as ITreeViewItem) != null)
            {
                item.IsSelected = true;

            }
            if (!tv.HandlingMultipleSelected)
            {
                tv.ClearMultipleSelection();
            }
            tv.HandlingMultipleSelected = false;
            if (tv.LeftMouseDown)
            {
                ICommand command = tv.NodeItemSelectedCommand;
                if (command != null
                    && command.CanExecute(tv.NodeItemSelectedCommandParameter))
                {
                    command.Execute(tv.NodeItemSelectedCommandParameter);
                }
            }
        }

        public object SelectedNodeItem
        {
            get { return GetValue(SelectedNodeItemProperty); }
            set { SetValue(SelectedNodeItemProperty, value); }
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            base.OnSelectedItemChanged(e);

            if (SelectedNodeItem != e.NewValue)
            {
                SelectedNodeItem = e.NewValue;
            }
        }

        #endregion

        #region Command

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TreeViewEx), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TreeViewEx),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            ICommand command = Command;
            if (command != null
                && command.CanExecute(CommandParameter))
            {
                e.Handled = true;
                command.Execute(CommandParameter);
            }
        }

        #endregion

        #region NodeItemSelectedCommand

        public static readonly DependencyProperty NodeItemSelectedCommandProperty =
            DependencyProperty.Register(nameof(NodeItemSelectedCommand), typeof(ICommand), typeof(TreeViewEx), new PropertyMetadata(null));

        public static readonly DependencyProperty NodeItemSelectedCommandParameterProperty =
            DependencyProperty.Register(nameof(NodeItemSelectedCommandParameter), typeof(object), typeof(TreeViewEx),
                new PropertyMetadata(null));

        public ICommand NodeItemSelectedCommand
        {
            get { return (ICommand)GetValue(NodeItemSelectedCommandProperty); }
            set { SetValue(NodeItemSelectedCommandProperty, value); }
        }

        public object NodeItemSelectedCommandParameter
        {
            get { return GetValue(NodeItemSelectedCommandParameterProperty); }
            set { SetValue(NodeItemSelectedCommandParameterProperty, value); }
        }

        #endregion

        #region Drag and Drop

        public bool DragDropEnabled { get; set; }
        internal bool HandlingSelected { get; set; }
        internal bool LeftMouseDown { get; set; }
        internal bool BringIntoViewHandled { get; set; }
        internal bool HandlingMultipleSelected { get; set; }

        #endregion

        #region Items

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItemEx;
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemEx();
        }

        #endregion

        public void ClearMultipleSelection()
        {
            foreach (object item in Items)
            {
                TreeViewItemEx ex = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemEx;
                ex?.ClearMultipleSelection();
            }
        }
    }
}