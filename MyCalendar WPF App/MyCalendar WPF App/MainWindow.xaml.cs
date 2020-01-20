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
        AppView _view;
        
        public mainWindow()
        {
            InitializeComponent();
            MonthCombobox.SelectedIndex = AppView.GetCurMonthIndex();
            YearTextbox.Text = AppView.GetCurrentYear();
            _view = new AppView();
            //MessageBox.Show(MonthCombobox.SelectionBoxItem.ToString()); YearTextbox.Text, MonthCombobox.SelectionBoxItem.ToString()
            _view.Start();
            _view.LoadCalendar(YearTextbox.Text, MonthCombobox.SelectionBoxItem.ToString());
        }
    }

}
