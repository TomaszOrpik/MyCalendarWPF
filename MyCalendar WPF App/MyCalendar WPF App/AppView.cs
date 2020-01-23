using System;
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
            GenerateMonths(_months, mWindow.MonthCombobox);
            GenerateYears(mWindow.YearCombobox);
            SetCurrentMonth(_months, mWindow.MonthCombobox);
            SetCurrentYear(mWindow.YearCombobox);
            GenerateDayNames();
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
            NextMonthDays(year, currentMonthNum, _currentDayCount.Item2);
            RedDays();
            SetToday(currentMonthNum, year, _currentDayCount.Item2);
            ButtonsViewMethod();


<<<<<<< HEAD
=======
            //function for adding to config file mail and password / event token

>>>>>>> a69af6aa7ed569b1ae5520340469f8106972898a
            //main window - slider to change language (english <> polish)

            //display messagebox
        }

        //create note window
        public void CreateNoteDisplay(string type)
        {
            AddWindow win = new AddWindow();
            Random random = new Random();

            win.TitleLabel.Content = "Title";
            if(type == "mail")
            {
                win.LocationLabel.Visibility = Visibility.Visible;
                win.LocationLabel.Content = "Login";
                win.LocationTextbox.Visibility = Visibility.Visible;
                win.PasswordLabel.Visibility = Visibility.Visible;
                win.PasswordLabel.Content = "Password";
                win.PasswordTextbox.Visibility = Visibility.Visible;
                win.RecipentLabel.Visibility = Visibility.Visible;
                win.RecipentLabel.Content = "Recipent";
                win.RecipentTextBox.Visibility = Visibility.Visible;
            }
            else if(type == "event")
            {
                win.LocationLabel.Visibility = Visibility.Visible;
                win.LocationLabel.Content = "Location";
                win.LocationTextbox.Visibility = Visibility.Visible;
            }
            else
            {
                win.LocationLabel.Visibility = Visibility.Hidden;
                win.LocationTextbox.Visibility = Visibility.Hidden;
                win.PasswordLabel.Visibility = Visibility.Hidden;
                win.PasswordTextbox.Visibility = Visibility.Hidden;
                win.RecipentLabel.Visibility = Visibility.Hidden;
                win.RecipentTextBox.Visibility = Visibility.Hidden;
            }
            if (type == "event")
                win.StartDateLabel.Content = "Start Date";
            else
                win.StartDateLabel.Content = "Date";
            CreateHours(win.StartDateHourBox);
            CreateMinutes(win.StartDateMinBox);
            GenerateMonths(_months, win.StartDayMonthBox);
            win.StartDayYearTextBox.Text = DateTime.Now.ToString("yyyy");
            win.StartDayMonthBox.SelectedIndex = DateTime.Now.Month - 1;
            CreateDayBox(win.StartDateDayBox, Convert.ToInt32(win.StartDayYearTextBox.Text), win.StartDayMonthBox.SelectedIndex + 1);
            win.StartDateDayBox.SelectedIndex = DateTime.Now.Day - 1;
            if(type == "event")
            {
                win.EndDateLabel.Content = "End Date";
                CreateHours(win.EndDateHourBox);
                CreateMinutes(win.EndDateMinBox);
                GenerateMonths(_months, win.EndDateDayBox);
                win.EndDateYearTextBox.Text = DateTime.Now.ToString("yyyy");
                win.EndDateMonthBox.SelectedIndex = DateTime.Now.Month - 1;
                CreateDayBox(win.EndDateDayBox, Convert.ToInt32(win.EndDateYearTextBox.Text), win.EndDateMonthBox.SelectedIndex + 1);
                win.EndDateDayBox.SelectedIndex = DateTime.Now.Day - 1;
            }
            else
            {
                //can be blocked in xml for shorter code
                win.EndDateLabel.Visibility = Visibility.Hidden;
                win.EndDateHourBox.Visibility = Visibility.Hidden;
                win.EndDateMinBox.Visibility = Visibility.Hidden;
                win.EndDateDayBox.Visibility = Visibility.Hidden;
                win.EndDateMonthBox.Visibility = Visibility.Hidden;
                win.EndDateYearTextBox.Visibility = Visibility.Hidden;
            }
            win.DescriptionTextBlock.Text = "Description";
            if (type == "mail")
                win.ReminderCheckBox.Content = "Send now?";
            else
                win.ReminderCheckBox.Content = "Reminder";
            win.StartDayMonthBox.SelectionChanged += (sender, e) => DayMonthBox_SelectionChanged(win.StartDateDayBox, Convert.ToInt32(win.StartDayYearTextBox.Text), win.StartDayMonthBox.SelectedIndex + 1);
           
            if (type == "note")
                win.SaveButton.Click += (sender, e) => SaveNoteButton_Click(win.StartDayYearTextBox + win.StartDayMonthBox.SelectedItem.ToString() + win.StartDateDayBox.SelectedItem.ToString()+ win.StartDateHourBox.SelectedItem.ToString()+win.StartDateMinBox.SelectedItem.ToString()+random.Next(0, 1000).ToString(),
                                                                        $"{win.StartDateDayBox.SelectedItem.ToString()}-{win.StartDayMonthBox.SelectedItem.ToString()}-{win.StartDayYearTextBox.Text} {win.StartDateHourBox.SelectedItem.ToString()}:{win.StartDateMinBox.SelectedItem.ToString()}",
                                                                        win.TitleTextbox.Text,
                                                                        win.DescriptionRichTextBox.Selection.Text,
                                                                        win.ReminderCheckBox.IsChecked.Value);
            if(type == "mail")
                win.SaveButton.Click += (sender, e) => SaveMailButton_Click(win.StartDayYearTextBox + win.StartDayMonthBox.SelectedItem.ToString() + win.StartDateDayBox.SelectedItem.ToString() + win.StartDateHourBox.SelectedItem.ToString() + win.StartDateMinBox.SelectedItem.ToString()+random.Next(0, 1000).ToString(),
                                                                       $"{win.StartDateDayBox.SelectedItem.ToString()}-{win.StartDayMonthBox.SelectedItem.ToString()}-{win.StartDayYearTextBox.Text} {win.StartDateHourBox.SelectedItem.ToString()}:{win.StartDateMinBox.SelectedItem.ToString()}",
                                                                       win.TitleTextbox.Text,
                                                                       win.DescriptionRichTextBox.Selection.Text,
                                                                       win.ReminderCheckBox.IsChecked.Value,
                                                                       win.LocationTextbox.Text,
                                                                       win.PasswordTextbox.Text,
                                                                       win.RecipentTextBox.Text);
            if (type == "event")
                win.SaveButton.Click += (sender, e) => SaveEventButton_Click(win.StartDayYearTextBox + win.StartDayMonthBox.SelectedItem.ToString() + win.StartDateDayBox.SelectedItem.ToString() + win.StartDateHourBox.SelectedItem.ToString() + win.StartDateMinBox.SelectedItem.ToString()+random.Next(0, 1000).ToString(),
                                                                       $"{win.StartDateDayBox.SelectedItem.ToString()}-{win.StartDayMonthBox.SelectedItem.ToString()}-{win.StartDayYearTextBox.Text} {win.StartDateHourBox.SelectedItem.ToString()}:{win.StartDateMinBox.SelectedItem.ToString()}",
                                                                       win.TitleTextbox.Text,
                                                                       win.DescriptionRichTextBox.Selection.Text,
                                                                       win.ReminderCheckBox.IsChecked.Value,
                                                                       $"{win.EndDateDayBox.SelectedItem.ToString()}-{win.EndDateMonthBox.SelectedItem.ToString()}-{win.EndDateYearTextBox.Text} {win.EndDateHourBox.SelectedItem.ToString()}:{win.EndDateMinBox.SelectedItem.ToString()}",
                                                                       win.LocationTextbox.Text);

            win.Show();
        }
        //send data through changed month for generate correct number of days
        private void DayMonthBox_SelectionChanged(ComboBox cb, int year, int month)
        {
            CreateDayBox(cb, year, month);
        }

        //send window values through event
        private void SaveNoteButton_Click(string name, string date, string title, string description, bool reminder)
        {
            Note note = new Note(name, date, title, description, reminder);
            _control.SaveNote(note);
        }
        private void SaveMailButton_Click(string name, string date, string title, string description, bool reminder, string login, string password, string recipent)
        {
            CustomMail mail = new CustomMail(name, date, title, description, reminder, login, password, recipent);
            _control.SaveMail(mail);
        }
        private void SaveEventButton_Click(string name, string date, string title, string description, bool reminder, string endDate, string location)
        {
            MyEvent mevent = new MyEvent(name, date, title, description, reminder, endDate, location);
            _control.SaveEvent(mevent);
        }

        public void CreateSettingsWindow()
        {
            Settings set = new Settings();

            set.MailTitle.Content = "Set Default Mail";
            set.LoginLabel.Content = "Login";
            set.PasswordLabel.Content = "Password";
            set.EventTitle.Content = "Set Data for Event";
            set.ProjectIdLabel.Content = "Project ID";
            set.ClientIdLabel.Content = "Client ID";
            set.ClientSecretLabel.Content = "Client Secret";
            set.EventMailLabel.Content = "Google Account (e-mail)";

            set.SetDefaultMailBtn.Click += (sender, e) => SetDefaultMail_Click(set.LoginTextBox.Text, set.PasswordTextBox.Text);
            set.SetDefaultEventBtn.Click += (sender, e) => SetDefaultEvent_Click(set.ProjectIdTextBox.Text, set.ClientIdTextBox.Text, set.ClientSecretTextBox.Text, set.EventMailTextBox.Text);
        }

        private void SetDefaultMail_Click(string login, string password)
        {
            _control.SaveDefaultMail(login, password);
        }

        private void SetDefaultEvent_Click(string projectID, string clientID, string secret, string account)
        {
            _control.SaveDefaultEvent(projectID, clientID, secret, account);
        }

        //show note window
        public void ShowNoteDisplay(Note note)
        {
            ShowWindow sw = new ShowWindow();

            sw.TitleLabel.Content = note.GetValue("title");
            sw.LocationLabel.Visibility = Visibility.Hidden;
            sw.RecipentLabel.Visibility = Visibility.Hidden;
            sw.EndDateLabel.Visibility = Visibility.Hidden;
            sw.StartDateLabel.Content = note.GetValue("date");
            sw.DescriptionLabel.Content = note.GetValue("description");
            sw.ReminderLabel.Content = note.GetReminder() ? "Reminder Active" : "Reminder Not Active";

            sw.DeleteButton.Click += (sender, e) => DeleteNoteButton_Click(note.GetValue("name"));
        }

        private void DeleteNoteButton_Click(string name) { _control.DeleteNote(name); }

        //show mail window
        public void ShowMailDisplay(CustomMail mail)
        {
            ShowWindow sw = new ShowWindow();

            sw.TitleLabel.Content = mail.GetValue("title");
            sw.LocationLabel.Content = mail.GetMailValues("login");
            sw.RecipentLabel.Content = mail.GetMailValues("recipent");
            sw.EndDateLabel.Visibility = Visibility.Hidden;
            sw.StartDateLabel.Content = mail.GetValue("date");
            sw.DescriptionLabel.Content = mail.GetValue("description");
            sw.ReminderLabel.Content = mail.GetSended() ? "Message Sended" : "Message Not Sended";

            sw.DeleteButton.Click += (sender, e) => DeleteMailButton_Click(mail.GetValue("name"));
        }

        private void DeleteMailButton_Click(string name) { _control.DeleteMail(name); }

        //show event window
        public void ShowEventDisplay(MyEvent mevent)
        {
            ShowWindow sw = new ShowWindow();

            sw.TitleLabel.Content = mevent.GetValue("title");
            sw.LocationLabel.Content = mevent.GetEventValues("location");
            sw.RecipentLabel.Visibility = Visibility.Hidden;
            sw.StartDateLabel.Content = mevent.GetValue("date");
            sw.EndDateLabel.Content = mevent.GetEventValues("endDate");
            sw.DescriptionLabel.Content = mevent.GetValue("description");
            sw.ReminderLabel.Content = mevent.GetReminder() ? "You set SMS Reminder" : "You didn't set SMS Reminder";

            sw.DeleteButton.Click += (sender, e) => DeleteEventButton_Click(mevent.GetValue("name"));
        }

        private void DeleteEventButton_Click(string name) { _control.DeleteEvent(name); }
        
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
        public Dictionary<string, int> GetMonths()
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
                _buttons[(todayDay + d) - 1].Foreground = Brushes.White;
                _buttons[(todayDay + d) - 1].Background = Brushes.DeepSkyBlue;
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
        private void GenerateMonths(Dictionary<string, int> months, ComboBox cb)
        {
            for(int i = 1; i <= 12; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "month" + i;
                cbi.Content = months.FirstOrDefault(x => x.Value == i).Key;
                cb.Items.Add(cbi);
            }
        }

        private void GenerateYears(ComboBox cb)
        {
            for (int i = -50; i < 50; i++)
            {
                cb.Items.Add(DateTime.Today.AddYears(i).Year);
            }
        }

        internal static void SetCurrentMonth(Dictionary<string, int> months, ComboBox cb)
        {
            if (months.ContainsKey(DateTime.Today.ToString("MMMM")))
                cb.SelectedIndex = months.FirstOrDefault(x => x.Value == Int32.Parse(DateTime.Now.Month.ToString())).Value;
        }

        internal static void SetCurrentYear(ComboBox cb)
        {
            cb.SelectedItem = DateTime.Today.Year;
        }

        //generate label names over buttons
        private void GenerateDayNames()
        {
            mWindow.DayLabel1.Content = "Mon";
            mWindow.DayLabel2.Content = "Tue";
            mWindow.DayLabel3.Content = "Wed";
            mWindow.DayLabel4.Content = "Thu";
            mWindow.DayLabel5.Content = "Fri";
            mWindow.DayLabel6.Content = "Sat";
            mWindow.DayLabel7.Content = "Sun";
        }
        //add function to buttons for display note view
        private void ButtonsViewMethod()
        {
            foreach (Button button in _buttons)
                button.Click += (sender, e) => calendarButton_Click(mWindow.YearTextbox.Text+mWindow.MonthCombobox.Text+button.Content.ToString());
        }

        private void calendarButton_Click(string nameStart)
        {
            _control.DayEvent(nameStart);
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
