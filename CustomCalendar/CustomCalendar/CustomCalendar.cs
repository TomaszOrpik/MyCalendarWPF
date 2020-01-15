using System;
using System.IO;

namespace CustomCalendar
{

    //define user files
    public class CustomFile
    {
        //required variables
        internal string _path;
        internal string _type;
        internal string _year;
        internal string _month;
        internal string _day;
        internal string _hour;
        internal string _min;
        internal string _title;
        internal string _description;
        internal bool _reminder;

        //default constructor
        public CustomFile(string type, string date, string title, string description, bool reminder)
        {
            _type = type;
            char[] dateArr = date.ToCharArray();
            _year = (dateArr[6] + dateArr[7] + dateArr[8] + dateArr[9]).ToString();
            _month = (dateArr[3] + dateArr[4]).ToString();
            _day = (dateArr[0] + dateArr[1]).ToString();
            _hour = (dateArr[11] + dateArr[12]).ToString();
            _min = (dateArr[14] + dateArr[15]).ToString();

            _title = title;
            _description = description;
            _reminder = reminder;
        }
        
        //constructor of object from file
        public CustomFile(string path)
        {
            _path = path;
            string[] lines = File.ReadAllLines(@path);

            _type = lines[0];
            _year = lines[1];
            _month = lines[2];
            _day = lines[3];
            _hour = lines[4];
            _min = lines[5];
            _title = lines[6];
            if (lines[7] == "1") _reminder = true;
            else _reminder = false;
            _description = lines[8];
        }
        
        //get private values
        public string GetValue(string value) =>
         _ = (value.ToLower() switch
         {
             "type"         => _type,
             "year"         => _year,
             "month"        => _month,
             "day"          => _day,
             "hour"         => _hour,
             "min"          => _min,
             "title"        => _title,
             "description"  => _description,
             _              => "Invalid Input",
         });
        public bool GetReminder() => _reminder;
        
        //Save note object to file
        public string SaveToFile()
        {
            TextWriter Save = new StreamWriter(_year + _month + _day + _hour + _min + ".userfile");
            Save.WriteLine("N");
            Save.WriteLine(_year);
            Save.WriteLine(_month);
            Save.WriteLine(_day);
            Save.WriteLine(_hour);
            Save.WriteLine(_min);
            Save.WriteLine(_title);
            //checking reminder
            if (_reminder) Save.WriteLine("1");
            else Save.WriteLine("0");
            Save.WriteLine(_description);
            Save.Close();

            return "Note Added";
        }

        //delete file
        public string DeleteFile()
        {
            File.Delete(@_path);
            return "File deleted!";
        }

        public static string DeleteFile(string path)
        {
            File.Delete(@path);
            return "File deleted!";
        }

    }

    //define user notes
    public class CustomNote : CustomFile
    {
        //userNotes constructors
        public CustomNote(string type, string date, string title, string description, bool reminder) : base(type, date, title, description, reminder) { }

        public CustomNote(string path) : base(path) { }
    }
    
    //define user mails
    public class CustomMail
    {

    }
    
    //define user events
    public class CustomEvent : CustomFile
    {
        public CustomEvent(string type,string date, string title, string description, bool reminder) : base(type, date, title, description, reminder)
        {
            //add text
        }

    }
    
    //define user settings
    public class AppSettings
    {

    }
}

