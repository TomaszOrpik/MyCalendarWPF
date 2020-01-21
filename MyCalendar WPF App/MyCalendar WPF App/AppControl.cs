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
        mainWindow window = Application.Current.MainWindow as mainWindow;
        private AppControl _control = new AppControl();

        public void SaveNote(Note note)
        {
            note.Save();
        }
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
