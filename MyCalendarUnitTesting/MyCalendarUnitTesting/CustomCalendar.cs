using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Dapper;
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

        //default constructor
        public Note(string id, string name, string date, string title, string description, bool reminder)
        {
            _id = id;
            _name = name;
            _date = date;
            _title = title;
            _description = description;
            _reminder = reminder;
        }
        
        //get note directly from database
        public Note(string name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"SELECT * FROM Notes WHERE Name = '{name}';", new DynamicParameters());
                List<string> values = output.ToList();
                _id = values[0];
                _name = values[1];
                _date = values[2];
                _title = values[3];
                _description = values[4];
                _reminder = values[5] == "1" ? true : false;

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

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO Notes (Name, Date, Title, Description, Reminder) VALUES ({_name}, {_date}, {_title}, {_description}, {reminder});");
            }
        }

        //delete note from database by name
        public void Delete() 
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE * FROM Notes WHERE Name = '{_name}';");
            }
        }

        public static void StaticDelete(string name) 
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE * FROM Notes WHERE Name = '{name}';");
            }
        }

        internal static string LoadConnectionString(string id = "Default")
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
        public CustomMail(string id, string name, string date, string title, string description, bool reminder, string login, string password, string recipent) : base(id, name, date, title, description, reminder)
        {
            _sended = reminder;
            _login = login;
            _password = password;
            _recipent = recipent;
        }

        //constructor from database
        public CustomMail(string name) : base(name)
        {
            //code re-use to make sure all data is readed correctly
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"SELECT * FROM Mails WHERE Name = '{name}';", new DynamicParameters());
                List<string> values = output.ToList();
                _id = values[0];
                _name = values[1];
                _date = values[2];
                _title = values[3];
                _description = values[4];
                _sended = values[5] == "1" ? true : false;
                //load password from db with decryptor
                _login = values[6];
                _password = Encryptor.Decrypt(_key, values[7]);
                _recipent = values[8];

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

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO Mails (Name, Date, Title, Description, Sended, Login, Password, Recipent) VALUES " +
                    $"({_name}, {_date}, {_title}, {_description}, {sended}, {_login}, {Encryptor.Encrypt(_key, _password)}, {_recipent});");
            }
        }

        public void SendMail()
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

                    _sended = true;
                }
                catch (Exception ex)
                {
                    //not send window to display
                }
           
        }

        //send mail directly from database
        public static void StaticSendMail(string name)
        {
            List<string> values = new List<string>();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"SELECT * FROM Mails WHERE Name = '{name}';", new DynamicParameters());
                values = output.ToList();
            }

            try
            {
                var message = new MailMessage(values[6], values[8]);
                message.Subject = values[3];
                message.Body = values[4];

                using (SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587))
                {
                    mailer.Credentials = new NetworkCredential(values[6], Encryptor.Decrypt(_skey, values[7]));
                    mailer.EnableSsl = true;
                    mailer.Send(message);
                }

                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute($"UPDATE Mails SET Sended='1' WHERE Name = {name};");
                }
            }
            catch (Exception ex)
            {
                //not send window to display
            }
        }

        //delete mail from database
        public new void Delete()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE * FROM Mails WHERE Name = '{_name}';");
            }
        }

        public new void StaticDelete(string name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE * FROM Mails WHERE Name = '{name}';");
            }
        }

    }
    
    //define user events
    public class CustomEvent : Note
    {
        //define variables
        private string _startDate;
        private string _endDate;
        private string _location;

        //default constructor
        public CustomEvent(string id, string name, string date, string title, string description, bool reminder, string endDate, string location) : base(id, name, date, title, description, reminder)
        {
            _id = id;
            _name = name;
            _startDate = date;
            _title = title;
            _description = description;
            _reminder = reminder;
            _endDate = endDate;
            _location = location;

        }

        //alternative constructor
        public CustomEvent(string name) : base(name)
        {
            //code re-use to make sure all data is readed correctly
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<string>($"SELECT * FROM Events WHERE Name = '{name}';", new DynamicParameters());
                List<string> values = output.ToList();
                _id = values[0];
                _name = values[1];
                _date = values[2];
                _title = values[3];
                _description = values[4];
                _reminder = values[5] == "1" ? true : false;
                _endDate = values[6];
                _location = values[7];
            }
        }

        //get values defined for event
        public string GetEventValues(string value) =>
            value.ToLower() switch
            {
                "endDate" => _endDate,
                "location" => _location,
                _ => "Invalid value",
            };

        //save event to database
        public new void Save()
        {
            string reminder = _reminder == true ? "1" : "0";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO Events (Name, Date, Title, Description, Reminder, EndDate, Location) VALUES " +
                    $"({_name}, {_date}, {_title}, {_description}, {reminder}, {_endDate}, {_location});");
            }
        }

        public void SendEvent()
        {
            //send event from object   
            if (File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\myID.setting") == false)
            {
                ///OPEN WINDOW WITH CALENDAR CONFIGURATION AND SET STRING myID

                ///OPEN CONFIGURE SCREEN

                return;

            }
            else
            {
                TextReader setid = new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\myID.setting");
                string curID = setid.ReadLine();
                string curSecret = setid.ReadLine();
                string curProID = setid.ReadLine();
                string curMail = setid.ReadLine();
                setid.Close();


                TextWriter credentials = new StreamWriter("credentials.json");
                /// change project id and maybe client secret
                credentials.WriteLine("{\"installed\":{\"client_id\":\"" + curID + "\",\"project_id\":\"" + curProID + "\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"" + curSecret + "\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}");
                credentials.Close();

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
            Email = curMail,
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
        }
            //send event directly from database
            public static void StaticSendEvent(string name)
            {
                //get data from database
                List<string> values = new List<string>();
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<string>($"SELECT * FROM Events WHERE Name = '{name}';", new DynamicParameters());
                    values = output.ToList();
                }

            if (File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\myID.setting") == false)
            {
                ///OPEN WINDOW WITH CALENDAR CONFIGURATION AND SET STRING myID

                ///OPEN CONFIGURE SCREEN

                return;

            }
            else
            {
                TextReader setid = new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\myID.setting");
                string curID = setid.ReadLine();
                string curSecret = setid.ReadLine();
                string curProID = setid.ReadLine();
                string curMail = setid.ReadLine();
                setid.Close();


                TextWriter credentials = new StreamWriter("credentials.json");
                /// change project id and maybe client secret
                credentials.WriteLine("{\"installed\":{\"client_id\":\"" + curID + "\",\"project_id\":\"" + curProID + "\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"" + curSecret + "\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}");
                credentials.Close();

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
                    if (values[5] == "1")
                        RemindTime = 10;
                }

                char[] sDate = values[2].ToCharArray();
                char[] eDate = values[6].ToCharArray();

                //create Event to send for calendar API
                Event newEvent = new Event()
                {
                    Summary = values[3],
                    Location = values[7],
                    Description = values[4],

                    Start = new EventDateTime()
                    {
                        DateTime = DateTime.Parse(sDate[6] + sDate[7] + sDate[8] + sDate[9] + "-" + sDate[3] + sDate[4] + "-" + sDate[0] + sDate[1] + "T" + sDate[11] + sDate[12] + ":" + sDate[14] + sDate[15] + ":00-07:00"),
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
            Email = curMail,
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

        }
        

        //delete event from database
        public new void Delete()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE * FROM Events WHERE Name = '{_name}';");
            }
        }

        public new void StaticDelete(string name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE * FROM Events WHERE Name = '{name}';");
            }
        }

    }
    
    //define user settings
    public class AppSettings
    {
        private static string _skey = "poiuytrewq128954";
        //set google account requirements in config file
        public static void CreareMyID(string clientID, string secret, string projectID, string mail)
        {
            using (var id = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\myID.setting", false))
            {
                id.WriteLine(clientID);
                id.WriteLine(secret);
                id.WriteLine(projectID);
                id.WriteLine(mail);
                id.Close();
            }
        }
        //set default email in file
        public static void CreateDefaultMail(string mail, string password)
        {
            using (var sw = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\personal.setting", false))
            {
                sw.WriteLine(mail);
                sw.WriteLine(Encryptor.Encrypt(_skey, password));
            }
        }
        //get default email
        public static List<string> GetDefaultMail()
        {
            List<string> data = new List<string>();

            TextReader readData = new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\personal.setting");
            data.Add(readData.ReadLine());
            data.Add(Encryptor.Decrypt(_skey, readData.ReadLine()));
            readData.Close();
            return data;


        }
        //method for setting colors from setting file
        public static void saveColors()
        {
            //TO DO
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

