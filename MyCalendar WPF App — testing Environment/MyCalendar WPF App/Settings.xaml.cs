using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyCalendar_WPF_App
{
    /// <summary>
    /// Logika interakcji dla klasy Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        AppControl _control;

        public Settings()
        {
            InitializeComponent();
            _control = new AppControl();

            Link.MouseLeftButtonDown += (o, e) => LinkClicked();
        }

        private void LinkClicked()
        {
            var ps = new ProcessStartInfo("https://developers.google.com/adwords/api/docs/guides/authentication")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void SetDefaultEventBtn_Click(object sender, RoutedEventArgs e)
        {
            _control.SaveDefaultEvent(ProjectIdTextBox.Text, ClientIdTextBox.Text, ClientSecretTextBox.Text, EventMailTextBox.Text);
        }

        private void SetDefaultMailBtn_Click(object sender, RoutedEventArgs e)
        {
            _control.SaveDefaultMail(LoginTextBox.Text, PasswordTextBox.Password);
        }
    }
}
