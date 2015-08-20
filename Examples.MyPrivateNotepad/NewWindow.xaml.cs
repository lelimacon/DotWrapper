using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPrivateNotepad
{
    /// <summary>
    ///     Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class NewWindow : Window
    {
        public enum Option
        {
            Create,
            Cancel
        }

        private readonly Func<string, bool> _nameChecker;

        public Option Selection { get; private set; }

        public string NoteName
        {
            get { return NameBox.Text; }
        }

        public string Password
        {
            get { return PassBox.Text; }
        }

        public NewWindow(Func<string, bool> nameChecker)
        {
            InitializeComponent();
            Selection = Option.Cancel;
            _nameChecker = nameChecker;
            CheckForm();
        }

        #region Events

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            switch (button.Name)
            {
                case "CreateButton":
                    if (!CheckForm(true))
                        break;
                    Selection = Option.Create;
                    Close();
                    break;

                case "CancelButton":
                    Close();
                    break;

                default:
                    throw new ArgumentException("No such Button : " + button.Name);
            }
        }

        private void TextBoxChange(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox) sender;
            switch (box.Name)
            {
                case "NameBox":
                    CheckName();
                    break;

                case "PassBox":
                    CheckPassword();
                    break;

                default:
                    throw new ArgumentException("No such TextBox : " + box.Name);
            }
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
            TextBox box = NameBox;
            bool valid = !string.IsNullOrEmpty(box.Text) && _nameChecker(box.Text);
            if (aggressive)
                box.BorderBrush = valid
                    ? new SolidColorBrush(Colors.Transparent)
                    : new SolidColorBrush(Colors.DarkRed);
            NameValidation.Source = ValidIcon(valid);
            return valid;
        }

        /// <summary>
        ///     Verifies the password input.
        /// </summary>
        /// <returns>true if the password is valid</returns>
        private bool CheckPassword(bool aggressive = false)
        {
            TextBox box = PassBox;
            bool valid = !string.IsNullOrEmpty(box.Text);
            if (aggressive)
                box.BorderBrush = valid
                    ? new SolidColorBrush(Colors.Transparent)
                    : new SolidColorBrush(Colors.DarkRed);
            PassValidation.Source = ValidIcon(valid);
            return valid;
        }

        /// <summary>
        ///     Gets the icon corresponding to specified validity.
        /// </summary>
        /// <param name="valid">validation</param>
        /// <returns>the correct icon</returns>
        private static BitmapImage ValidIcon(bool valid)
        {
            string path = valid ? "Resources/valid.ico" : "Resources/invalid.ico";
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

        #endregion Form Validation
    }
}
