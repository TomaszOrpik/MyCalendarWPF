﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace CustomCalendar
{

    //define user notes (base)
    public class Note
    {
        //required variables
        internal string _id;
        internal string _name;
        internal string _date;
        internal string _title;
        internal string _description;
        internal bool _reminder;

        internal string _database = @"URI=file:"+Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\CalendarDB.db";
        internal static string _sdatabase = @"URI=file:" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\CalendarDB.db";



        //default constructor
        public Note(string name, string date, string title, string description, bool reminder)
        {
            CreateTable();
            _name = name;
            _date = date;
            _title = title;
            _description = description;
            _reminder = reminder;
        }

        //get note directly from database
        public Note(string name)
        {
            using var con = new SQLiteConnection(_database);
            con.Open();

            string stm = $"SELECT Id, Name, Date, Title, Description, Reminder FROM Notes WHERE Name = \"{name}\" LIMIT 1;";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                _id = rdr.GetInt32(0).ToString();
                _name = rdr.GetString(1);
                _date = rdr.GetString(2);
                _title = rdr.GetString(3);
                _description = rdr.GetString(4);
                _reminder = rdr.GetString(5) == "1" ? true : false;
            }
            con.Close();
        }
        //check if table exists
        public static bool CheckForTable(string tableName)
        {
            try
            {
                using var con = new SQLiteConnection(_sdatabase);
                con.Open();
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = "SELECT COUNT(*) FROM " + tableName;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //create table if exists
        internal void CreateTable()
        {
            if(CheckForTable("Notes") == false)
            {
                using var con = new SQLiteConnection(_database);
                con.Open();
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE Notes(Id INTEGER PRIMARY KEY, Name TEXT,
                Date TEXT, Title TEXT, Description TEXT, Reminder TEXT);";
                cmd.ExecuteNonQuery();
            }
        }
        
        //get private values
        public string GetValue(string value) =>
         value.ToLower() switch
         {
             "id"           => _id,
             "name"         => _name,
             "date"         => _date,
             "title"        => _title,
             "description"  => _description,
             _              => "Invalid Input",
         };
        public bool GetReminder() => _reminder;
        
        //Save note object to database
        public void Save() 
        {
            string reminder = _reminder == true ? "1" : "0";

            using var con = new SQLiteConnection(_database);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            
            cmd.CommandText = "INSERT INTO Notes(Name, Date, Title, Description, Reminder) VALUES('"+_name+"', '"+_date+"', '"+_title+"', '"+_description+"', '"+reminder+"');";
            cmd.ExecuteNonQuery();
        }

        //delete note from database by name
        public void Delete() 
        {
            using var con = new SQLiteConnection(_database);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DELETE FROM Notes WHERE Name = \"{_name}\";";
            cmd.ExecuteNonQuery();
        }

        public static void StaticDelete(string name) 
        {
            using var con = new SQLiteConnection(_sdatabase);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DELETE FROM Notes WHERE Name = \"{name}\" LIMIT 1;";
            cmd.ExecuteNonQuery();
        }
        //get static list of elements with name
        public static List<string> GetSearch(string value) => SearchDataBase("Notes", value);

        internal static List<string> SearchDataBase(string type, string nameStart)
        {
            List<string> tempList = new List<string>();
            
            if(CheckForTable(type))
            {
                string query = $"Select Name FROM {type} WHERE Name LIKE '{nameStart}%';";

                DataTable table = GetDataTable(query);

                foreach (DataRow row in table.Rows)
                    tempList.Add(row.ItemArray[0].ToString());
            }
                return tempList;

        }

        private static DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (var c = new SQLiteConnection(_sdatabase))
            {
                c.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, c))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                        return dt;
                    }
                }
            }
        }

        internal static string LoadConnectionString(string id="Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }
 
    //define user mails
    public class CustomMail : Note
    {
        //define variables
        private string _login;
        private string _password;
        private string _recipent;
        private bool _sended;

        //debugger key
        private string _key = "poiuytrewq128954";
        private static string _skey = "poiuytrewq128954";

        //default constructor
        public CustomMail(string name, string date, string title, string description, bool reminder, string login, string password, string recipent) : base(name, date, title, description, reminder)
        {
            CreateTable();
            _sended = reminder;
            _login = login;
            _password = password;
            _recipent = recipent;
        }

        //constructor from database
        public CustomMail(string name) : base(name)
        {
            //code re-use to make sure all data is readed correctly
            using var con = new SQLiteConnection(_database);
            con.Open();

            string stm = $"SELECT Id, Name, Date, Title, Description, Sended, Login, Password, Recipent FROM Mails WHERE Name = \"{name}\" LIMIT 1;";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                _id = rdr.GetInt32(0).ToString();
                _name = rdr.GetString(1);
                _date = rdr.GetString(2);
                _title = rdr.GetString(3);
                _description = rdr.GetString(4);
                _sended = rdr.GetString(5) == "1" ? true : false;
                _login = rdr.GetString(6);
                _password = Encryptor.Decrypt(_key, rdr.GetString(7));
                _recipent = rdr.GetString(8);
            }
            con.Close();
        }

        //create table if no exists
        internal new void CreateTable()
        {
            if (CheckForTable("Mails") == false)
            {
                using var con = new SQLiteConnection(_database);
                con.Open();
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE Mails(Id INTEGER PRIMARY KEY, Name TEXT,
                Date TEXT, Title TEXT, Description TEXT, Sended TEXT, Login TEXT, Password TEXT, Recipent TEXT);";
                cmd.ExecuteNonQuery();
            }
        }

        //get values defined for mail
        public string GetMailValues(string value) =>
            value.ToLower() switch
            {
                "login"    => _login,
                "password" => _password,
                "recipent" => _recipent,
                _          => "Invalid value",
            };

        public bool GetSended() => _sended;


        //save mail to database
        public new void Save()
        {
            string sended = _sended == true ? "1" : "0";

            using var con = new SQLiteConnection(_database);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = "INSERT INTO Mails(Name, Date, Title, Description, Sended, Login, Password, Recipent) VALUES('" + _name + "', '" + _date + "', '" + _title 
                + "', '" + _description + "', '" + sended + "', '"+_login+"', '"+Encryptor.Encrypt(_key, _password)+"', '"+_recipent+"');";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public string SendMail()
        {
            //send mail from object   
                try
                {
                    var message = new MailMessage(_login, _recipent);
                    message.Subject = _title;
                    message.Body = _description;

                    using (SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587))
                    {
                        mailer.Credentials = new NetworkCredential(_login, _password);
                        mailer.EnableSsl = true;
                        mailer.Send(message);
                    }

                //update sended to true
                using var con = new SQLiteConnection(_database);
                con.Open();
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = $"UPDATE Mails SET Sended = '1' WHERE Name = '{_name}'";
                cmd.ExecuteNonQuery();
                con.Close();

                return $"Sended message to {_recipent}";
            }
            catch (Exception)
                {
                return "Mail couldn't be send";
                }
           
        }

        //send mail directly from database
        public static string StaticSendMail(string name) //check if table mail exist
        {
            using var con = new SQLiteConnection(_sdatabase);
            con.Open();

            string stm = $"SELECT Id, Name, Date, Title, Description, Sended, Login, Password, Recipent FROM Mails WHERE Name = \"{name}\" LIMIT 1;";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            string subject = "";
            string body = "";
            string mail = "";
            string password = "";
            string recipent = "";

            try
            {
                if (rdr.Read())
                {
                    subject = rdr.GetString(3);
                    body = rdr.GetString(4);
                    mail = rdr.GetString(6);
                    password = Encryptor.Decrypt(_skey, rdr.GetString(7));
                    recipent = rdr.GetString(8);
                }

                var message = new MailMessage(mail,recipent);
                message.Subject = subject;
                message.Body = body;

                using (SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587))
                {
                    mailer.Credentials = new NetworkCredential(mail, password);
                    mailer.EnableSsl = true;
                    mailer.Send(message);
                }

                //update sended to true
                cmd.CommandText = $"UPDATE Mails SET Sended = '1' WHERE Name = '{name}'";
                cmd.ExecuteNonQuery();
                con.Close();

                return $"Sended message to {recipent}";
            }
            catch (Exception)
            {
                return "Mail couldn't be send";
            }
        }

        //get static list of elements with name
        public static new List<string> GetSearch(string value) => SearchDataBase("Mails", value);

        //delete mail from database
        public new void Delete()
        {
            using var con = new SQLiteConnection(_database);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DELETE FROM Mails WHERE Name = \"{_name}\" LIMIT 1;";
            cmd.ExecuteNonQuery();
        }

        public new static void StaticDelete(string name)
        {
            using var con = new SQLiteConnection(_sdatabase);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DELETE FROM Mails WHERE Name = \"{name}\" LIMIT 1;";
            cmd.ExecuteNonQuery();
        }

    }
    
    //define user events
    public class MyEvent : Note
    {
        //define variables
        private string _startDate;
        private string _endDate;
        private string _location;

        //default constructor
        public MyEvent(string name, string date, string title, string description, bool reminder, string endDate, string location) : base(name, date, title, description, reminder)
        {
            CreateTable();
            _name = name;
            _startDate = date;
            _title = title;
            _description = description;
            _reminder = reminder;
            _endDate = endDate;
            _location = location;

        }

        //alternative constructor
        public MyEvent(string name) : base(name)
        {
            //code re-use to make sure all data is readed correctly
            using var con = new SQLiteConnection(_database);
            con.Open();

            string stm = $"SELECT Id, Name, Date, Title, Description, Reminder, EndDate, Location FROM Events WHERE Name = \"{name}\" LIMIT 1;";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                _id = rdr.GetInt32(0).ToString();
                _name = rdr.GetString(1);
                _date = rdr.GetString(2);
                _title = rdr.GetString(3);
                _description = rdr.GetString(4);
                _reminder = rdr.GetString(5) == "1" ? true : false;
                _endDate = rdr.GetString(6);
                _location = rdr.GetString(7);
            }
            con.Close();
        }

        //get values defined for event
        public string GetEventValues(string value) =>
            value.ToLower() switch
            {
                "endDate"  => _endDate,
                "location" => _location,
                _          => "Invalid value",
            };

        //create table if no exists
        internal new void CreateTable()
        {
            if (CheckForTable("Events") == false)
            {
                using var con = new SQLiteConnection(_database);
                con.Open();
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE Events(Id INTEGER PRIMARY KEY, Name TEXT,
                Date TEXT, Title TEXT, Description TEXT, Reminder TEXT, EndDate TEXT, Location TEXT);";
                cmd.ExecuteNonQuery();
            }
        }

        //save event to database
        public new void Save()
        {
            string reminder = _reminder == true ? "1" : "0";

            using var con = new SQLiteConnection(_database);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = "INSERT INTO Events(Name, Date, Title, Description, Reminder, EndDate, Location) VALUES('" + _name + "', '" + _date + "', '" + _title
                + "', '" + _description + "', '" + reminder + "', '" + _endDate + "', '" + _location + "');";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void SendEvent()
        {
            //send event from object   
                string mail = SettingsSave.GetSetting("account");

                string[] Scopes = { CalendarService.Scope.Calendar };
                string ApplicationName = "MyCalendar Google Calendar API";


                UserCredential credential;

                using (var stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Create Google Calendar API service.
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });


                int RemindTime = 0;

                ChangeReminder();

                void ChangeReminder()
                {
                    if (_reminder)
                        RemindTime = 10;
                }

                char[] sDate = _startDate.ToCharArray();
                char[] eDate = _endDate.ToCharArray();

                //create Event to send for calendar API
                Event newEvent = new Event()
                {
                    Summary = _title,
                    Location = _location,
                    Description = _description,

                    Start = new EventDateTime()
                    {
                        DateTime = DateTime.Parse(sDate[6]+sDate[7]+sDate[8]+sDate[9] + "-" + sDate[3]+sDate[4] + "-" + sDate[0]+sDate[1] + "T" + sDate[11]+sDate[12] + ":" + sDate[14]+sDate[15] + ":00-07:00"),
                        TimeZone = "Europe/Warsaw",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = DateTime.Parse(eDate[6] + eDate[7] + eDate[8] + eDate[9] + "-" + eDate[3] + eDate[4] + "-" + eDate[0] + eDate[1] + "T" + eDate[11] + eDate[12] + ":" + eDate[14] + eDate[15] + ":00-07:00"),
                        TimeZone = "Europe/Warsaw",
                    },

                    Attendees = new EventAttendee[] {
        new EventAttendee() {
            Organizer = true,
            Email = mail,
        ResponseStatus = "accepted" }, /// automaticly confirmed
                },

                    Reminders = new Event.RemindersData()
                    {
                        UseDefault = false,
                        Overrides = new EventReminder[] {
            new EventReminder() { Method = "sms", Minutes = RemindTime },
        }
                    }

                };


                //Add event to calendar by google calendar API
                String calendarId = "primary";
                EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
                Event createdEvent = request.Execute();
            

        }
        
        public static void CreateJSon(string curID, string curProID, string curSecret)
        {
            TextWriter credentials = new StreamWriter("credentials.json");
            credentials.WriteLine("{\"installed\":{\"client_id\":\"" + curID + "\",\"project_id\":\"" + curProID + "\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"" + curSecret + "\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}");
            credentials.Close();
        }
        //delete event from database
        public new void Delete()
        {
            using var con = new SQLiteConnection(_database);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DELETE FROM Events WHERE Name = \"{_name}\" LIMIT 1;";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        //get static list of elements with name
        public static new List<string> GetSearch(string value) => SearchDataBase("Events", value);

        public new static void StaticDelete(string name)
        {
            using var con = new SQLiteConnection(_sdatabase);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = $"DELETE FROM Events WHERE Name = \"{name}\" LIMIT 1;";
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }

    public class SettingsSave
    {
        //method to save settings to App.Config file
        public static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    public class Encryptor
    {
        //method for encrypt password
        public static string Encrypt(string key, string text)
        {
            ///create arrays for holding bytes
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);  ///convert key to bytes
                aes.IV = iv;
                ///transformation interface gets key and iv values
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream()) ///write stream to memory
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) ///connect stream
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) ///saves stream
                        {
                            streamWriter.Write(text);
                        }
                        ///put bytes into array
                        array = memoryStream.ToArray();
                    }
                }

            }
            ///convert array into string
            return Convert.ToBase64String(array);
        }
        ///method for decrypt password
        public static string Decrypt(string key, string text)
        {
            ///create array for bytes
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(text);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key); ///convert bytes to stream
                aes.IV = iv;

                ///transformation interface gets key and iv values
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer)) ///write stream to memory
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) ///connect stream
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream)) ///reads stream
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

