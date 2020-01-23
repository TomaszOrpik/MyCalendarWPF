using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CustomCalendar;

namespace MyCalendar_WPF_App
{
    class AppControl
    {
        private string _key = "poiuytrewq128954";

        public void SaveNote(Note note) { note.Save(); }

        public void SaveMail(CustomMail mail)
        {
            if (mail.GetReminder())
            {
                MessageWindow mw = new MessageWindow();
                mw.TextLabel.Content = mail.SendMail();
                mw.Show();
            }
            mail.Save();
        }

        public void SaveEvent(MyEvent mevent)
        {
            mevent.SendEvent();
            mevent.Save();
        }

        public void DeleteNote(string name) { Note.StaticDelete(name); }

        public void DeleteMail(string name) { CustomMail.StaticDelete(name); }

        public void DeleteEvent(string name) { MyEvent.StaticDelete(name); }

        //method to save def mail
        public void SaveDefaultMail(string login, string password)
        {
            SettingsSave.SetSetting("login", login);
            SettingsSave.SetSetting("password", Encryptor.Encrypt(_key, password));
        }

        //method to save def event
        public void SaveDefaultEvent(string projectID, string clientID, string secret, string account)
        {
            SettingsSave.SetSetting("clientID", clientID);
            SettingsSave.SetSetting("secret", secret);
            SettingsSave.SetSetting("projectID", projectID);
            SettingsSave.SetSetting("account", account);
        }

        public void CustomJson()
        {
            if (SettingsSave.GetSetting("clientID").Length == 0 || SettingsSave.GetSetting("secret").Length == 0 || SettingsSave.GetSetting("projectID").Length == 0 || SettingsSave.GetSetting("account").Length == 0)
            {
                //message box
            }
            else
            {
                if (File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\credentials.json") == false) //json exist
                {
                    MyEvent.CreateJSon(SettingsSave.GetSetting("clientID"), SettingsSave.GetSetting("projectID"), SettingsSave.GetSetting("secret"));
                }
                else
                    return;
            }
        }
        //add main window buttons functionality
        public void DayEvent(string nameStart)
        {
            AppView view = new AppView();

            List<string> notesNames = Note.GetSearch(nameStart);
            List<string> mailsNames = CustomMail.GetSearch(nameStart);
            List<string> eventsNames = MyEvent.GetSearch(nameStart);

            if(notesNames.Count != 0)
            {
                foreach(string name in notesNames)
                {
                    Note note = new Note(name);
                    view.ShowNoteDisplay(note);
                }
            }

            if (mailsNames.Count != 0)
            {
                foreach (string name in mailsNames)
                {
                    CustomMail mail = new CustomMail(name);
                    view.ShowMailDisplay(mail);
                }
            }

            if (eventsNames.Count != 0)
            {
                foreach (string name in eventsNames)
                {
                    MyEvent mevent = new MyEvent(name);
                    view.ShowEventDisplay(mevent);
                }
            }
        }
        //async method for execute send mail method
        public async void SendMailManager()
        {
            Task<string> task = new Task<string>(SendEmail);           
            task.Start();
            MessageWindow mw = new MessageWindow();
            mw.TextLabel.Content = await task;
            mw.Show();
        }
        //method to send mail every 1 hour
        private string SendEmail()
        {
            List<string> mails = CustomMail.GetSearch(DateTime.Today.Year.ToString()+DateTime.Today.Month.ToString("D2")+DateTime.Today.Day.ToString());
            foreach (string mail in mails)
            {
                CustomMail.StaticSendMail(mail);
                Thread.Sleep(3600000); 
            }

            return $"Planned mails sended up to:{DateTime.Today.Year.ToString()}-{DateTime.Today.Month.ToString("D2")}-{DateTime.Today.Day.ToString()} {DateTime.Today.Hour.ToString()}:{DateTime.Today.Minute.ToString()}"
        }

    }
}
