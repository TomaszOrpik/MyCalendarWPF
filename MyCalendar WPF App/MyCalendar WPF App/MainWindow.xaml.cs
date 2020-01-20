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
            _view = new AppView();
            MonthCombobox.SelectedIndex = AppView.GetCurMonthIndex();
            YearTextbox.Text = AppView.GetCurrentYear();
            _view.Start(YearTextbox.Text, MonthCombobox.Text);
        }
    }

}
