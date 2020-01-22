using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
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
                mail.SendMail();
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

        //metoda sprawdzająca czy istnieje today w bazie i wysyłająca maila

    }
}
