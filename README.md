# MyCalendar
 WPF calendar for Windows
 
# Introduction

MyCalendar is created in C# + Visual Studio application, created for better time management. Main focus of app is to centralize users mails, meetings and notes for fast access. The calendar renders all terms in the main window, on chosen day and makes it easy to access by just click on chosen day. Thanks to custom controls, for adding new terms user doesn't need to open a new window. Application is fully customizable, works in the background and user can set default e-mail, making working with calendar even faster.

# Technologies

App is programmed in C# language, based on .Net Core in version 3.1, for correct working I recommend update/ install minimum to this version. Libraries used in project:
* Globalization for setting default dates to optimize application
* IO for managing files and directories in application folder
* Linq for operating on strings and file names
* RegularExpressions for avoid bad format bugs
* Threadening for async mail and timer access
* Net.Mail for connecting to mail server and automate the message sending
* Google.Apis.Calendar for synchronizing app with Google Calendar events
* Google.Apis.Auth.OAuth2 for user authentication

# Setup

* Download project solution and generate app/install from file
* Go to Settings to set default data for Google Oauth 2.0 and mail account
* set access for app in your mail settings in gmail settings and for Calendar in [here](https://console.developers.google.com/apis/api/calendar-json.googleapis.com/overview?project=866383696138)

# Features

Calendar main features:

* reminders - user can set alert to remind him for incoming notes 
* e-mail sending - easy way to send e-mail from users mailbox to chosen receiver with option to set automatic sending at a certain time
* Google Calendar synchronization - adding meetings from app level direct to users Google Account makes it easy to set phone and e-mail reminders
* Fully customizable - you can set default e-mail, reset authentication and delete notes
* App saves user configurations in App.config file

# Motivation

Starting point of development was to make Windows Calendar. After reading about all Apis to use We realize that they to hard to implement into other projects (problem with sending input to calendar) so we decided to write our own Calendar Api. After that we started to improve functionality by adding custom notes, than implement mail sending and finally Google Calendar Apis. Next main goal was to implement async controls to project for making app more intuitive. Finally, the project was ready to use after some code optimization. Hope you'll enjoy using it!

# Contributors
- [Maciej Bandyk](https://github.com/maciejbandyk)
- [Tomasz Orpik](https://github.com/TomaszOrpik)

# Others

### Report Bug and improves

You can report encountered bugs or send ideas for improvement [here](hhttps://github.com/maciejbandyk/MyCalendar/issues/new)


### License

Application was uploaded under GENERAL PUBLIC LICENSE for more information [check license file](https://github.com/TomaszOrpik/Music-Player/blob/master/LICENSE) link to license

### Contact

Feel free to [Contact me!](https://github.com/TomaszOrpik) or [Maciek!](https://github.com/maciejbandyk)

### Preview
![Image of MyCalendar app](https://i.gyazo.com/86a6d7bd33a497b9f6427c129b735373.png)
