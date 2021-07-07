Task Description:
Build a system that will provide user with insights of real time unique visitors for their website.
System should be able to consume apache logs and present user with information on how many unique visits occurred during past 5 seconds, 1 minute, 5 minutes, 30 minutes, 
1 hour and 1 day.
You can use https://github.com/mingrammer/flog for generating apache log files.
Logs should be in apache_combined format.
System should be able to support situation when logs are late. Output information should be stored in table in MySQL.

This application is build using .Net Core 5.0

Before starting the application, need to generate apache log file and specify the path to it in appsettings.json
The application used the MySql database for storing the data. In appsetting.json needs to be declared connection string to the MySql database.

