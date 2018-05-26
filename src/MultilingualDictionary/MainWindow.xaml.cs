using MultiDictionaryViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MultilingualDictionary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void trvTranslations_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = ((TreeView)sender);
            //var do1 = tv.ItemContainerGenerator.ContainerFromItem(tv.SelectedItem);
            //TreeViewItem it = (TreeViewItem)(.ContainerFromItem(((TreeView)sender).SelectedItem));
            //do1.BringIntoView();
        }

        int level = 0;
        List<TreeNode> chain = new List<TreeNode>();
        DateTime tm;

        private TreeViewItem FindTreeViewItem(ItemsControl container, TreeNode item, Window warn)
        {
            level = 0;
            TreeNode tmp = item;
            while (tmp != null)
            {
                chain.Insert(0, tmp);
                tmp = tmp.Parent;
            }

            return GetTreeViewItem(container, item, warn);
        }

        /// <summary>
        /// Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// </returns>
        private TreeViewItem GetTreeViewItem(ItemsControl container, TreeNode item, Window warn)
        {
            if(warn.Visibility != Visibility.Visible && (DateTime.Now - tm).Milliseconds > 100)
            {
                warn.Visibility = Visibility.Visible;
                warn.Show();
            }
            if (item == null)
            {
                return null;
            }
            if (container != null)
            {
                if (container.DataContext == item)
                {
                    return container as TreeViewItem;
                }

                // Expand the current container
                if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                {
                    container.SetValue(TreeViewItem.IsExpandedProperty, true);
                }

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the 
                // virtualizing case even if the item is marked 
                // expanded we still need to do this step in order to 
                // regenerate the visuals because they may have been virtualized away.

                container.ApplyTemplate();
                ItemsPresenter itemsPresenter =
                    (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter, 
                    // so walk the descendents and find the child.
                    itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    }
                }

                Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);


                // Ensure that the generator for this panel has been created.
                UIElementCollection children = itemsHostPanel.Children;

                MyVirtualizingStackPanel virtualizingPanel =
                    itemsHostPanel as MyVirtualizingStackPanel;

                for (int i = 0, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (virtualizingPanel != null)
                    {
                        // Bring the item into view so 
                        // that the container will be generated.
                        virtualizingPanel.BringIntoView(i);

                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);

                        // Bring the item into view to maintain the 
                        // same behavior as with a virtualizing panel.
                        subContainer.BringIntoView();
                    }

                    if (subContainer != null)
                    {
                        if (subContainer is TreeViewItem && subContainer.DataContext != chain[level])
                        {
                            continue;
                        }
                        level++;
                        // Search the next level for the object.
                        TreeViewItem resultContainer = GetTreeViewItem(subContainer, item, warn);
                        if (resultContainer != null)
                        {
                            return resultContainer;
                        }
                        else
                        {
                            // The object is not under this TreeViewItem
                            // so collapse it.
                            subContainer.IsExpanded = false;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        private T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }

        public Task<TreeViewItem> Search(MultiDictionaryVM dict, string textToSearch, Window warn)
        {
            MultiDictionaryViewModel.TreeNode tn = dict.SearchTranslation(textToSearch);
            //TreeViewItem item = (TreeViewItem)(trvTranslations.ItemContainerGenerator.ContainerFromItem(trvTranslations.SelectedItem));
            //item..BringIntoView();
            //TreeViewItem tvi = TreeViewitemFinder.BringItemIntoView(trvTranslations, tn);
            Task<TreeViewItem> tvi = Task.Run(() => FindTreeViewItem(trvTranslations, tn, warn));
            return tvi;
        }

        public TreeViewItem Search2(MultiDictionaryVM dict, string textToSearch, Window warn)
        {
            MultiDictionaryViewModel.TreeNode tn = dict.SearchTranslation(textToSearch);
            //TreeViewItem item = (TreeViewItem)(trvTranslations.ItemContainerGenerator.ContainerFromItem(trvTranslations.SelectedItem));
            //item..BringIntoView();
            //TreeViewItem tvi = TreeViewitemFinder.BringItemIntoView(trvTranslations, tn);
            TreeViewItem tvi = FindTreeViewItem(trvTranslations, tn, warn);
            return tvi;
        }

        void ShowWarning()
        {
            var wnd = new SearchWarning();
            wnd.Owner = this;
            wnd.Show();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tm = DateTime.Now;
            var dict = Resources["dictionaryVM"] as MultiDictionaryViewModel.MultiDictionaryVM;

            //Task<TreeViewItem> tt = Search2(dict, SearchTermTextBox.Text);

            SearchWarning wnd = new SearchWarning();
            wnd.Owner = this;

            wnd.Visibility = Visibility.Hidden;
            //wnd.Show();

            TreeViewItem tvi = Search2(dict, SearchTermTextBox.Text, wnd); 
            wnd.Close();

            if (tvi != null)
            {
                tvi.BringIntoView();
                tvi.Focus();
            }
            else
            {
                MessageBox.Show("Заданный текст не найден.");
            }
        }

        private void btnDuplicate_Click(object sender, RoutedEventArgs e)
        {
            var dict = Resources["dictionaryVM"] as MultiDictionaryVM;
            TermVM copy = dict.GetCopySelectedTermVM();
            ShowEditWindow(copy, true);
        }

        private void ShowEditWindow(TermVM copy, bool shouldAddNew)
        {
            EditTerm et = new EditTerm();
            et.DataContext = copy;
            et.Owner = this;
            et.btnAddTerm.Visibility = shouldAddNew ? Visibility.Visible : Visibility.Hidden;
            et.btnUpdateTerm.Visibility = shouldAddNew ? Visibility.Hidden: Visibility.Visible;
            et.ShowDialog();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dict = Resources["dictionaryVM"] as MultiDictionaryVM;
            var termVM = dict.GetNewTermVM();
            ShowEditWindow(termVM, true);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dict = Resources["dictionaryVM"] as MultiDictionaryVM;
            var termVM = dict.GetSelectedTermVM();
            ShowEditWindow(termVM, false);
        }
    }

    public static class TreeViewHelper
    {
        private static T FindVisualChild<T>(System.Windows.Media.Visual visual) where T : System.Windows.Media.Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                System.Windows.Media.Visual child = (System.Windows.Media.Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }

        public static void BringIntoView(TreeViewItem item)
        {
            ItemsControl parent = item.Parent as ItemsControl;

            if (parent != null)
            {
                System.Windows.Controls.VirtualizingStackPanel itemHost = FindVisualChild<System.Windows.Controls.VirtualizingStackPanel>(parent);

                if (itemHost != null)
                {
                    itemHost.BringIndexIntoViewPublic(parent.Items.IndexOf(item));
                    item.Focus();
                }
            }
        }
    }

    public static class TreeViewitemFinder
    {
        //public static TreeViewItem BringTreeViewItemIntoView(this TreeView treeView, TreeNode item)
        //{
        //    if (item == null) return null;
        //    ItemsControl parentContainer = (ItemsControl)treeView.BringTreeViewItemIntoView(item.Parent) ?? treeView;
        //    return parentContainer.BringItemIntoView(item);
        //}

        public static TreeViewItem BringItemIntoView(ItemsControl container, object item)
        {
            var vsp = FindVisualChild<MyVirtualizingStackPanel>(container);
            if (vsp == null)
            {
                var treeViewItem = (TreeViewItem)container.ItemContainerGenerator.ContainerFromItem(item);
                treeViewItem.BringIntoView();
                return treeViewItem;
            }
            //Use exposed BringIntoView method to render each of the items in order
            for (int i = 0; i < container.Items.Count; i++)
            {
                vsp.Dispatcher.Invoke(DispatcherPriority.ContextIdle, (Action<int>)vsp.BringIntoView, i);
                var nextitem = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                if (nextitem.DataContext == item)
                {
                    nextitem.Dispatcher.Invoke(DispatcherPriority.ContextIdle, (Action)nextitem.BringIntoView);
                    return nextitem;
                }
                else
                {
                    // Expand the current container
                    if (nextitem is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                    {
                        nextitem.SetValue(TreeViewItem.IsExpandedProperty, true);
                    }
                    var nextChildItem = BringItemIntoView(nextitem, item);
                    if (nextChildItem != null)
                        return nextChildItem;
                }
            }
            return null;
        }

        private static T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                var child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    var correctlyTyped = child as T;
                    if (correctlyTyped != null)
                        return correctlyTyped;
                    var descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                        return descendent;
                }
            }
            return null;
        }
    }
}
