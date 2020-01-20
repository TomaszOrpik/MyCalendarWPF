using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyCalendar_WPF_App
{
    class AppControl
    {
        mainWindow window = Application.Current.MainWindow as mainWindow;

        public void YearKeyBlock(object sender, KeyEventArgs e)
        {
            if (!char.IsControl(Convert.ToChar(e.Key)) && !char.IsDigit(Convert.ToChar(e.Key)) && (Convert.ToChar(e.Key) != '.'))
                e.Handled = true;
            if ((Convert.ToChar(e.Key) == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                e.Handled = true;
        }


    }
}
