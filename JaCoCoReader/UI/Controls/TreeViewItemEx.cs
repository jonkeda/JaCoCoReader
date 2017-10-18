using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JaCoCoReader.UI.Controls
{
    public class TreeViewItemEx : TreeViewItem
    {
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            RequestBringIntoView += TreeViewItemEx_RequestBringIntoView;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                ItemsControl items = ParentItemsControl;

                bool hasMultipleSelected = false;
                foreach (object item in items.Items)
                {
                    TreeViewItemEx ex = items.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemEx;
                    if (ex != null
                        && ex.IsMultipleSelected)
                    {
                        hasMultipleSelected = true;
                        break;
                    }
                }
                if (hasMultipleSelected)
                {
                    ParentTreeView.HandlingMultipleSelected = true;
                    bool select = false;
                    foreach (object item in items.Items)
                    {
                        TreeViewItemEx ex = items.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemEx;
                        if (ex == null)
                        {
                            continue;
                        }
                        if (select)
                        {

                            if (ReferenceEquals(ex, this)
                                || ex.IsMultipleSelected)
                            {
                                ex.IsMultipleSelected = true;
                                break;
                            }
                            ex.IsMultipleSelected = true;
                        }
                        else
                        {
                            if (ReferenceEquals(ex, this)
                                || ex.IsMultipleSelected)
                            {
                                ex.IsMultipleSelected = true;
                                select = true;
                            }
                        }
                    }
                }

            }
            else
            {
                ParentTreeView?.ClearMultipleSelection();
                IsMultipleSelected = true;
                if (ParentTreeView != null)
                {
                    ParentTreeView.HandlingMultipleSelected = true;
                }
            }
        }

        private void TreeViewItemEx_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = ParentTreeView?.BringIntoViewHandled ?? true;
        }

        public static readonly DependencyProperty IsMultipleSelectedProperty = DependencyProperty.Register(
            nameof(IsMultipleSelected), typeof(bool), typeof(TreeViewItemEx), new PropertyMetadata(default(bool)));

        public bool IsMultipleSelected
        {
            get { return (bool)GetValue(IsMultipleSelectedProperty); }
            set { SetValue(IsMultipleSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            nameof(IsHighlighted), typeof(bool), typeof(TreeViewItemEx), new PropertyMetadata(default(bool)));

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public static readonly DependencyProperty DoBringIntoViewProperty = DependencyProperty.Register(
            nameof(DoBringIntoView), typeof(bool), typeof(TreeViewItemEx), new PropertyMetadata(DoBringIntoViewChanged));

        private static void DoBringIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemEx ex = d as TreeViewItemEx;
            if (ex?.ParentTreeView != null
                && (bool)e.NewValue)
            {
                ex.ParentTreeView.BringIntoViewHandled = false;
                ex.BringIntoView();
                ex.ParentTreeView.BringIntoViewHandled = true;
                ex.DoBringIntoView = false;
            }
        }

        public bool DoBringIntoView
        {
            get { return (bool)GetValue(DoBringIntoViewProperty); }
            set { SetValue(DoBringIntoViewProperty, value); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled
                && IsEnabled
                && !ParentTreeView.LeftMouseDown)
            {
                ParentTreeView.LeftMouseDown = true;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ParentTreeView.LeftMouseDown = false;

            base.OnMouseLeftButtonUp(e);
        }


        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled
                && IsEnabled
                && !ParentTreeView.HandlingSelected)
            {
                bool isFocused = IsFocused;
                if (Focus()
                    && isFocused
                    && !IsSelected)
                {
                    IsSelected = true;
                }
                ParentTreeView.HandlingSelected = true;
            }
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            ParentTreeView.HandlingSelected = false;

            base.OnMouseRightButtonUp(e);
        }

        internal ItemsControl ParentItemsControl
        {
            get
            {
                return ItemsControlFromItemContainer(this);
            }
        }

        internal TreeViewEx ParentTreeView
        {
            get
            {
                for (ItemsControl i = ParentItemsControl; i != null; i = ItemsControlFromItemContainer(i))
                {
                    TreeViewEx treeView = i as TreeViewEx;
                    if (treeView != null)
                    {
                        return treeView;
                    }
                }
                return null;
            }
        }



        #region Items

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItemEx;
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemEx();
        }

        public void ClearMultipleSelection()
        {
            if (IsMultipleSelected)
            {
                IsMultipleSelected = false;
            }
            foreach (object item in Items)
            {
                TreeViewItemEx ex = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemEx;
                ex?.ClearMultipleSelection();
            }
        }

        #endregion

        

    }
}