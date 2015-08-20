using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPrivateNotepad
{
    /// <summary>
    ///     Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        public enum Option
        {
            Open,
            Cancel
        }

        public delegate bool PasswordChecker(string name, string password);

        private readonly PasswordChecker _checkPass;

        public Option Selection { get; private set; }

        public string NoteName
        {
            get { return NoteBox.Text; }
        }

        public string Password
        {
            get { return PassBox.Text; }
        }

        public OpenWindow(IEnumerable<TabModel> closedTabs, PasswordChecker checkPass)
        {
            InitializeComponent();
            Selection = Option.Cancel;
            foreach (TabModel closedTab in closedTabs)
                NoteBox.Items.Add(closedTab.Name);
            _checkPass = checkPass;
            CheckForm();
        }

        #region Events

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            switch (button.Name)
            {
                case "OpenButton":
                    if (!CheckForm(true))
                        break;
                    Selection = Option.Open;
                    Close();
                    break;

                case "CancelButton":
                    Close();
                    break;

                default:
                    throw new ArgumentException("No such button : " + button.Name);
            }
        }

        private void TextBoxChange(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox) sender;
            switch (box.Name)
            {
                case "PassBox":
                    // Real-time checking for password validity, youhouu!
                    CheckPassword();
                    break;

                default:
                    throw new ArgumentException("No such TextBox : " + box.Name);
            }
        }

        private void BoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NoteBox.Text = e.AddedItems[0].ToString();
            // Check the password too, it may have become valid.
            CheckForm();
        }

        #endregion Events

        #region Form Validation

        /// <summary>
        ///     Check for form completeness.
        /// </summary>
        /// <returns>true if form is valid</returns>
        private bool CheckForm(bool aggressive = false)
        {
            return (CheckName(aggressive) & CheckPassword(aggressive));
        }

        /// <summary>
        ///     Verifies the name input.
        /// </summary>
        /// <returns>true if the name is valid</returns>
        private bool CheckName(bool aggressive = false)
        {
            ComboBox box = NoteBox;
            bool valid = !string.IsNullOrEmpty(box.Text);
            if (aggressive)
                box.BorderBrush = valid
                    ? new SolidColorBrush(Colors.Transparent)
                    : new SolidColorBrush(Colors.DarkRed);
            NameValidation.Source = ValidIcon(valid
                ? IconType.Green
                : (!aggressive && string.IsNullOrEmpty(box.Text)
                    ? IconType.Gray
                    : IconType.Red));
            return valid;
        }

        /// <summary>
        ///     Verifies the password input.
        /// </summary>
        /// <returns>true if the password is valid</returns>
        private bool CheckPassword(bool aggressive = false)
        {
            TextBox box = PassBox;
            bool valid = !string.IsNullOrEmpty(box.Text) && _checkPass(NoteBox.Text, box.Text);
            if (aggressive)
                box.BorderBrush = valid
                    ? new SolidColorBrush(Colors.Transparent)
                    : new SolidColorBrush(Colors.DarkRed);
            PassValidation.Source = ValidIcon(valid
                ? IconType.Green
                : (!aggressive && string.IsNullOrEmpty(box.Text)
                    ? IconType.Gray
                    : IconType.Red));
            return valid;
        }

        private enum IconType
        {
            Gray,
            Green,
            Red
        }

        /// <summary>
        ///     Gets the icon corresponding to specified validity.
        /// </summary>
        /// <param name="valid">validation</param>
        /// <returns>the correct icon</returns>
        private static BitmapImage ValidIcon(IconType icon)
        {
            string path;
            switch (icon)
            {
                case IconType.Gray:
                    path = "Resources/flag_gray.png";
                    break;
                case IconType.Green:
                    path = "Resources/flag_green.png";
                    break;
                case IconType.Red:
                    path = "Resources/flag_red.png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("icon", icon, null);
            }
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

        #endregion Form Validation
    }
}
