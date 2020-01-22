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
        
        public mainWindow()
        {
            InitializeComponent();
            MonthCombobox.SelectedIndex = AppView.GetCurMonthIndex();
            YearTextbox.Text = AppView.GetCurrentYear();
            _view = new AppView();
            //MessageBox.Show(MonthCombobox.SelectionBoxItem.ToString()); YearTextbox.Text, MonthCombobox.SelectionBoxItem.ToString()
            _view.Start();
            MonthCombobox.SelectedItem = AppView.SetCurrentMonth(_view.GetMonths());
            _view.LoadCalendar(YearTextbox.Text, MonthCombobox.SelectionBoxItem.ToString());
            

            MonthCombobox.SelectionChanged += (o, e) => RefreshCalendar();
            //YearTextbox.Text += (o, e) => RefreshCalendar();
        }

        private void RefreshCalendar()
        {
            if (MonthCombobox.SelectedItem == null) return;
            if (String.IsNullOrEmpty(YearTextbox.Text)) return;

            string month = MonthCombobox.SelectionBoxItem.ToString();
            string year = YearTextbox.Text;

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

        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
