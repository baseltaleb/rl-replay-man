using System.Windows;

namespace RLReplayMan
{
    /// <summary>
    /// Interaction logic for FolderTreeView.xaml
    /// </summary>
    public partial class FolderTreeView : StretchingTreeView
    {


        public FolderTreeView()
        {
            InitializeComponent();
        }

        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "ItemClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FolderTreeView));

        // Provide CLR accessors for the event
        public event RoutedEventHandler ItemClicked
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        // This method raises the Tap event
        public void RaiseClickEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(FolderTreeView.ClickEvent);
            RaiseEvent(newEventArgs);
        }


        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            RaiseClickEvent();
        }
    }
}
