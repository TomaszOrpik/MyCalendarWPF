using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CustomCalendar;

namespace MyCalendar_WPF_App
{
    class AppControl
    {
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
        
        
        //methoda przesyłająca zajęte terminy do view a następnie do kalendarza
        //metoda przesyłająca datę z kliknięcia do control //generująca współprzedne (if exist) //zwracająca obiek
        //metoda metoda generująca obiekt i przesyłająca go do bazy 

        //metoda dodająca pass i login do pliku config
        //metoda szczytująca dane z pliku config

        //metoda sprawdzająca czy istnieje today w bazie i wysyłająca maila
        //metoda odpowiedzialna za konfigurację tokena google (wysyłanie danych do pliku config)
        //metoda szczytująca również dane z pliku config

        //coś o czym kurwa zapomniałem

    }
}
