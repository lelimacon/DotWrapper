using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyPrivateNotepad
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PrivatePad _pad;

        private bool _saved;

        public static int NewAcc = 1;

        public MainWindow()
        {
            InitializeComponent();
            _pad = new PrivatePad("data.mpn");
            TabControl.ItemsSource = _pad.OpenTabs;
            _saved = true;
        }

        #region Events

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTitle();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem) sender;
            switch (item.Name)
            {
                case "file_new":
                    if (!_pad.CheckNames())
                        break;
                    NewWindow newW = new NewWindow(n => _pad.CheckTabName(n) == 0);
                    newW.ShowDialog();
                    if (newW.Selection == NewWindow.Option.Create)
                    {
                        _pad.OpenTab(newW.NoteName, newW.Password);
                        TabControl.SelectedIndex = TabControl.Items.Count - 1;
                    }
                    break;

                case "file_open":
                    if (!_pad.CheckNames())
                        break;
                    OpenWindow open = new OpenWindow(_pad.ClosedTabs, _pad.CheckPassword);
                    open.ShowDialog();
                    if (open.Selection == OpenWindow.Option.Open)
                    {
                        _pad.OpenTab(open.NoteName, open.Password);
                        TabControl.SelectedIndex = TabControl.Items.Count - 1;
                    }
                    break;

                case "file_save":
                    if (_pad.Save())
                        _saved = true;
                    break;

                case "file_close":
                    CloseCurrentTab();
                    break;

                case "file_closeAll":
                    _pad.CloseAllTabs();
                    break;

                case "file_exit":
                    if (!_pad.CheckNames())
                        break;
                    if (_saved)
                        Environment.Exit(0);
                    const string content = "Do you want to save your pads?";
                    const string title = "Confirmation";
                    MessageBoxResult res = MessageBox.Show(content, title, MessageBoxButton.YesNoCancel);
                    if (res == MessageBoxResult.Yes)
                        if (_pad.Save())
                            Environment.Exit(0);
                        else if (res == MessageBoxResult.No)
                            Environment.Exit(0);
                    break;

                case "about":
                    new AboutWindow().ShowDialog();
                    break;
            }
        }

        private void TabTitleChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            if (textBox.Tag == null)
                return;
            int pos = (int) textBox.Tag;
            bool res = _pad.TabTitleChanged(pos, textBox.Text);
            textBox.Foreground = !res
                ? new SolidColorBrush(Colors.DarkRed)
                : new SolidColorBrush(Colors.Black);
            var selected = (TabModel) TabControl.SelectedItem;
            selected.Name = textBox.Text;
            UpdateTitle();
        }

        private void TabContentChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            if (textBox.Tag == null)
                return;
            int pos = (int) textBox.Tag;
            _pad.TabContentChanged(pos, textBox.Text);
            _saved = false;
        }

        private void CloseTabClick(object sender, RoutedEventArgs e)
        {
            CloseCurrentTab();
        }

        #endregion Events

        private void CloseCurrentTab()
        {
            if (!TabControl.HasItems)
                return;
            TabModel tab = (TabModel) TabControl.SelectedContent;
            _pad.CloseTab(tab.Name);
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            var selected = (TabModel) TabControl.SelectedItem;
            if (selected == null)
                Title = "MyPrivateNotepad";
            else
                Title = selected.Name + " - MyPrivateNotepad";
        }
    }
}
