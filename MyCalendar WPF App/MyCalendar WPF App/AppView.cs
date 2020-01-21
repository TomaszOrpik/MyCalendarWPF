﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomCalendar;

namespace MyCalendar_WPF_App
{
    class AppView
    {
        //define variables
        mainWindow mWindow = Application.Current.MainWindow as mainWindow;
        private AppControl _control;
        private List<Button> _buttons;
        private Dictionary<string, int> _months;
        private (int, int) _currentDayCount;
        private int _b; //start button index

        //main function to setup view
        public void Start()
        {
            _buttons = getButtons();
            _b = 0;
            ResetButtons();
            _months = GetMonths();
            GenMonths(_months);
            GenDayNames();
            _control = new AppControl();
        }

        public void LoadCalendar(string choosenYear, string choosenMonth)
        {
            int year;
            int currentMonthNum;

            Int32.TryParse(choosenYear, out year);
            currentMonthNum = _months[choosenMonth];
            _currentDayCount = CurrentDayCounter(year, currentMonthNum);
            _b = _currentDayCount.Item1;
            PrevMonthDays(currentMonthNum, year, _currentDayCount.Item2);
            CurMonthDays(year, currentMonthNum);
            RedDays();
            NextMonthDays(year, currentMonthNum, _currentDayCount.Item2);
            SetToday(currentMonthNum, year, _currentDayCount.Item2);
            //color days if data exist in database async // get date from control
            //create note window //send data to control
            //create mail window
            //create event window
            //display note/mail/event window //get data from control
            //display messagebox
        }
        //create note window
        public void CreateNoteDisplay()
        {
            AddWindow win = new AddWindow();
            win.TitleLabel.Content = "Title";
            win.LocationLabel.Visibility = Visibility.Hidden;
            win.LocationTextbox.Visibility = Visibility.Hidden;
            win.PasswordLabel.Visibility = Visibility.Hidden;
            win.PasswordTextbox.Visibility = Visibility.Hidden;
            win.StartDateLabel.Content = "Date";
            CreateHours(win.StartDateHourBox);
            CreateMinutes(win.StartDateMinBox);
            win.StartDayYearTextBox.Text = DateTime.Now.ToString("yyyy");
            win.StartDayMonthBox.SelectedIndex = DateTime.Now.Month - 1;
            CreateDayBox(win.StartDateDayBox, Convert.ToInt32(win.StartDayYearTextBox.Text), win.StartDayMonthBox.SelectedIndex + 1);
            win.StartDateDayBox.SelectedIndex = DateTime.Now.Day - 1;
            win.EndDateLabel.Visibility = Visibility.Hidden;
            win.EndDateHourBox.Visibility = Visibility.Hidden;
            win.EndDateMinBox.Visibility = Visibility.Hidden;
            win.EndDateDayBox.Visibility = Visibility.Hidden;
            win.EndDateMonthBox.Visibility = Visibility.Hidden;
            win.EndDateYearTextBox.Visibility = Visibility.Hidden;
            win.DescriptionTextBlock.Text = "Description";
            win.ReminderCheckBox.Content = "Reminder";
            win.StartDayMonthBox.SelectionChanged += (sender, e) => StartDayMonthBox_SelectionChanged(win.StartDateDayBox, Convert.ToInt32(win.StartDayYearTextBox.Text), win.StartDayMonthBox.SelectedIndex + 1);
            win.SaveButton.Click += (sender, e) => SaveNoteButton_Click(win.StartDayYearTextBox + win.StartDayMonthBox.SelectedItem.ToString() + win.StartDateDayBox.SelectedItem.ToString(),
                                                                        $"{win.StartDateDayBox.SelectedItem.ToString()}-{win.StartDayMonthBox.SelectedItem.ToString()}-{win.StartDayYearTextBox.Text} {win.StartDateHourBox.SelectedItem.ToString()}:{win.StartDateMinBox.SelectedItem.ToString()}",
                                                                        win.TitleTextbox.Text,
                                                                        win.DescriptionTextBlock.Text,
                                                                        win.ReminderCheckBox.IsChecked.Value);

            win.Show();
        }
        //send data through changed month for generate correct number of days
        private void StartDayMonthBox_SelectionChanged(ComboBox cb, int year, int month)
        {
            CreateDayBox(cb, year, month);
        }

        //send window values through event
        private void SaveNoteButton_Click(string name, string date, string title, string description, bool reminder)
        {
            Note note = new Note(name, date, title, description, reminder);
            _control.SaveNote(note);
        }

        //create list of buttons
        private List<Button> getButtons()
        {
            Button[] buttonsarr = { mWindow.ButtonCalendar1, mWindow.ButtonCalendar2, mWindow.ButtonCalendar3, mWindow.ButtonCalendar4, mWindow.ButtonCalendar5, mWindow.ButtonCalendar6, mWindow.ButtonCalendar7,
                                    mWindow.ButtonCalendar8, mWindow.ButtonCalendar9, mWindow.ButtonCalendar10, mWindow.ButtonCalendar11, mWindow.ButtonCalendar12, mWindow.ButtonCalendar13, mWindow.ButtonCalendar14,
                                    mWindow.ButtonCalendar15, mWindow.ButtonCalendar16, mWindow.ButtonCalendar17, mWindow.ButtonCalendar18, mWindow.ButtonCalendar19, mWindow.ButtonCalendar20, mWindow.ButtonCalendar21,
                                    mWindow.ButtonCalendar22, mWindow.ButtonCalendar23, mWindow.ButtonCalendar24, mWindow.ButtonCalendar25, mWindow.ButtonCalendar26, mWindow.ButtonCalendar27, mWindow.ButtonCalendar28,
                                    mWindow.ButtonCalendar29, mWindow.ButtonCalendar30, mWindow.ButtonCalendar31, mWindow.ButtonCalendar32, mWindow.ButtonCalendar33, mWindow.ButtonCalendar34, mWindow.ButtonCalendar35,
                                    mWindow.ButtonCalendar36, mWindow.ButtonCalendar37, mWindow.ButtonCalendar38, mWindow.ButtonCalendar39, mWindow.ButtonCalendar40, mWindow.ButtonCalendar41, mWindow.ButtonCalendar42};

            return new List<Button>(buttonsarr);
        }
        //get current month
        private Dictionary<string, int> GetMonths()
        {
            Dictionary<string, int> months = new Dictionary<string, int>();

            months.Add("January", 1);
            months.Add("February", 2);
            months.Add("March", 3);
            months.Add("April", 4);
            months.Add("May", 5);
            months.Add("June", 6);
            months.Add("July", 7);
            months.Add("August", 8);
            months.Add("September", 9);
            months.Add("October", 10);
            months.Add("November", 11);
            months.Add("December", 12);

            return months;
        }
        //get current day number
        private (int, int) CurrentDayCounter(int year, int month)
        {
            DateTime date = new DateTime(year, month, 1);
            string dayofweek = date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));

            (int, int) result = dayofweek switch
            {
                "Mon" => (1, 0),
                "Tue" => (2, 1),
                "Wed" => (3, 2),
                "Thu" => (4, 3),
                "Fri" => (5, 4),
                "Sat" => (6, 5),
                "Sun" => (7, 6),
                _     => (1, 0)
            };

            return result;
        }


        //generate prev month days
        private void PrevMonthDays(int monthNumber, int year, int day)
        {
            int f = 0;
            int prevMonth = monthNumber - 1;
            int tempMonth = monthNumber;
            if (prevMonth == 0)
                tempMonth = 12;
            int LastDay = DateTime.DaysInMonth(year, tempMonth);
            int startLastMonthDay = (LastDay - day) + 1;
            for(int i = startLastMonthDay; i<=LastDay; i++)
            {
                _buttons[f].Content = Convert.ToString(i);
                f++;
            }
        }
        //generate days of current month
        private void CurMonthDays(int year, int monthNumber)
        {
            int days = DateTime.DaysInMonth(year, monthNumber);
            int c = 2;
            _buttons[_b - 1].Content = Convert.ToString(c-1);
            _buttons[_b - 1].Background = Brushes.White;
            _buttons[_b - 1].Foreground = Brushes.Black;
            for(int i = 1; i < days; i++)
            {
                _buttons[_b].Content = Convert.ToString(c);
                _buttons[_b].Background = Brushes.White;
                _buttons[_b].Foreground = Brushes.Black;
                _b++;
                c++;
            }
        }
        //color sundays in red
        private void RedDays()
        {
            mWindow.ButtonCalendar7.Foreground = Brushes.Red;
            mWindow.ButtonCalendar14.Foreground = Brushes.Red;
            mWindow.ButtonCalendar21.Foreground = Brushes.Red;
            mWindow.ButtonCalendar28.Foreground = Brushes.Red;
            mWindow.ButtonCalendar35.Foreground = Brushes.Red;
            mWindow.ButtonCalendar42.Foreground = Brushes.Red;
        }
        //generate next month days
        private void NextMonthDays(int year, int monthNum, int d)
        {
            int days = DateTime.DaysInMonth(year, monthNum);
            int prevDays = 1;
            for(int i = (days + d); i < 42; i++)
            {
                _buttons[i].Content = Convert.ToString(prevDays);
                _buttons[i].Foreground = Brushes.Black;
                prevDays++;
            }
        }
        //select todays date
        private void SetToday(int monthNumber, int year, int d)
        {
            DateTime todayDate = DateTime.Now;
            string todayMonthYear = todayDate.ToString("M yyyy");
            int todayDay = todayDate.Day;

            if (Convert.ToString(monthNumber) + " " + Convert.ToString(year) == todayMonthYear)
            {
                _buttons[(todayDay + d) - 1].Foreground = Brushes.Yellow;
                _buttons[(todayDay + d) - 1].Background = Brushes.Black;
            }
        }
        //set color of buttons to default
        private void ResetButtons()
        {
            for (int i = 0; i < 42; i++)
            {
                _buttons[i].Content = "";
                _buttons[i].Background = Brushes.White;
            }
        }
        //generate months for comboBox
        private void GenMonths(Dictionary<string, int> months)
        {
            for(int i = 1; i <= 12; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "month" + i;
                cbi.Content = months.FirstOrDefault(x => x.Value == i).Key;
                mWindow.MonthCombobox.Items.Add(cbi);
            }
        }
        //generate label names over buttons
        private void GenDayNames()
        {
            mWindow.DayLabel1.Content = "Mon";
            mWindow.DayLabel2.Content = "Tue";
            mWindow.DayLabel3.Content = "Wed";
            mWindow.DayLabel4.Content = "Thu";
            mWindow.DayLabel5.Content = "Fri";
            mWindow.DayLabel6.Content = "Sat";
            mWindow.DayLabel7.Content = "Sun";
        }
        //create hour box items
        private void CreateHours(ComboBox cb)
        {
            for(int i = 0; i<24; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "hour" + i;
                cbi.Content = Convert.ToString(i);
                cb.Items.Add(cbi);
            }
        }
        //create min box items
        private void CreateMinutes(ComboBox cb)
        {
            for (int i = 0; i < 60; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "min" + i;
                cbi.Content = Convert.ToString(i);
                cb.Items.Add(cbi);
            }
        }

        private void CreateDayBox(ComboBox cb,int year, int month)
        {
            int daysCount = DateTime.DaysInMonth(year, month);
            
            for(int i = 1; i <= daysCount; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "day" + i;
                cbi.Content = Convert.ToString(i);
                cb.Items.Add(cbi);
            }
        }

        //get current month
        public static int GetCurMonthIndex()
        {
            string todayMonth = DateTime.Now.ToString("MM");
             return Convert.ToInt32(todayMonth) - 1;
        }
        //get current year
        public static string GetCurrentYear() => DateTime.Now.ToString("yyyy");


    }
}
