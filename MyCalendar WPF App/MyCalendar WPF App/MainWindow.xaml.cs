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
using CustomCalendar;

namespace MyCalendar_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class mainWindow : Window
    {
        AppView _view = new AppView();
        internal static mainWindow main;
        internal string Status
        {
            get { return TimeLabel.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { TimeLabel.Content = value; })); }
        }
        
        public mainWindow()
        {
            main = this;
            InitializeComponent();
            MonthCombobox.SelectedIndex = AppView.GetCurMonthIndex();
            YearCombobox.Text = AppView.GetCurrentYear();
            _view = new AppView();
            //MessageBox.Show(MonthCombobox.SelectionBoxItem.ToString()); YearTextbox.Text, MonthCombobox.SelectionBoxItem.ToString()
            _view.Start();
            _view.LoadCalendar(YearCombobox.Text, MonthCombobox.SelectionBoxItem.ToString());
            AppView.SetCurrentMonth(_view.GetMonths(), MonthCombobox);



            MonthCombobox.SelectionChanged += (o, e) => RefreshCalendar();
            YearCombobox.SelectionChanged += (o, e) => RefreshCalendar();
        }

        private void RefreshCalendar()
        {
            if (MonthCombobox.SelectedItem == null) return;
            if (YearCombobox.SelectedItem == null) return;

            string month = MonthCombobox.SelectionBoxItem.ToString();
            string year = YearCombobox.SelectionBoxItem.ToString();

            _view.LoadCalendar(year, month);
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            _view.CreateNoteDisplay("note");
        }

        private void AddMailButton_Click(object sender, RoutedEventArgs e)
        {
            _view.CreateNoteDisplay("mail");
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            _view.CreateNoteDisplay("event");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (MonthCombobox.SelectedIndex == 12) return;
            MonthCombobox.SelectedIndex = MonthCombobox.SelectedIndex + 1;
            RefreshCalendar();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (MonthCombobox.SelectedIndex == 0) return;            
            MonthCombobox.SelectedIndex = MonthCombobox.SelectedIndex - 1;
            RefreshCalendar();
        }
    }

}
