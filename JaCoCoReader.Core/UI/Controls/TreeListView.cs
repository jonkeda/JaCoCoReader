using System;
using System.Windows;
using System.Windows.Controls;

namespace JaCoCoReader.Core.UI.Controls
{
    public class TreeListView : TreeViewEx
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        #region Public Properties

        public GridViewColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new GridViewColumnCollection();
                }

                return _columns;
            }
        }

        private GridViewColumnCollection _columns;

        #endregion
    }

    //public class GridViewColumnCollectionEx : GridViewColumnCollection
    //{
    //    protected override void InsertItem(int index, GridViewColumn column)
    //    {
    //        GridViewColumnHeader header = new GridViewColumnHeader
    //        {
    //            Content = column.Header
    //        };
    //        header.Click += HeaderOnClick;


    //        column.Header = header;

    //        base.InsertItem(index, column);
    //    }

    //    private void HeaderOnClick(object sender, RoutedEventArgs routedEventArgs)
    //    {

    //    }
    //}
}