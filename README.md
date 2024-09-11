# Module Management Application

## Version
v2.0.0

## Hardware Specifications
The application can run on any hardware with the following minimum specifications:

- 1 GHz or faster processor
- 512 MB of RAM, or more
- 100 MB of free hard disk space

## Installation Instructions
To install and run the application, follow these steps:
- (SQL Server needs to be installed as this software uses a local database on your system)

- Clone the repository to your local machine or download and extract the ZIP file.
- Unzip the file
- Go to "...\prog6212-part-2-ST10067405\bin\Release"
- Run the ModuleManagementApplicationV2.exe file
- Enjoy :D

## How To Use
- This application is a module management application that allows the user to manage thier semesters, modules, and hours worked.

# NEW!
- The entire application has been overhauled in the background to persist data in a database instead of storing data locally on your machine.
- Added Account functionality:
	- Allowing you to register for an account.
	- Logging in to your account.
	- And only your data on your account should show.

# Semester Functionality
- You will be able to add multiple semesters that can:
  - Hold multiple modules.
  - Hold number of weeks in a semester.
  - hold a start date.
  - And will calculate end date with the number of weeks.
  - Option to edit a semester.
  
# Module Functionality
- You will be able to add multiple modules that can: 
  - It can hold Code, Name, Credits, Class hours per week, Self-Study hours per week.
  - Be able to display in a datagridto your right.
  
# Worked Hours Functionality
- You will be able to add how many hours you worked for each week
- By entering hours worked, selecting the semester and module, and the day you worked.
- It will be displayed in a datagrid to your right.

## FAQs
1. Q) What is this software/application?
- A) This software/application is a Module Management System that can store and display your modules. It can use the data to calculate your self-study hours and has the functionality to display that too.

2. Q) How do I get started with using this software/application?
- A) Simply run the 'ModuleManagementApplicationV2.exe' and start by entering your semester and modules information. Then add your worked hours for your modules.

3. Q) How do I delete a module?
- A) Currently the data is not persistant (meaning it doesn't store any information), therefore you cant delete modules. You can overwrite an existing semester.

4. Q) How do I update the software to the latest version??
- A) Download the lastest tag on GitHub and follow the instructions above.

## Code Attributions
This application was created with the help of the following resources:

- Microsoft .NET documentation: https://docs.microsoft.com/en-us/dotnet/
- C# documentation: https://docs.microsoft.com/en-us/dotnet/csharp/
- 'Pro C# 9 with .NET 5: Foundational Principles and Practices in Programming' - Tenth Edition by Andrew Troelsen & Phillip Japikse.

## Part 1 Changes From Feedback
- Applied changes from my feedback from part 1:
	- Upgraded UML diagram.
	- Used more LINQ.
	- Created Kanban board.

## Dev Info
- The application was created by Jaime Marc Futter.
- Student Number: ST10067405
- If any issues may arise, contact me via my email at: 
ST10067405@vcconnect.edu.za
- GitHub Repository Link: [https://github.com/VCWVL/prog6212-part-2-ST10067405]

## Frameworks and Plugins Used
- .NET Framework delvelopment tools v4.7.2
- Windows Presentation Forms (.NET Framework)
- Entity Framework 6 (EF6)
