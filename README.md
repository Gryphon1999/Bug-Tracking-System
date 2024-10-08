# Bug-Tracking-System
Setup Migration: 
This project uses Entity Framework Core for database migrations. You can migrate the database using the console by running the following command:
dotnet ef database update or update-database
For Database: MySql is used

Project Setup:
Set both the API and Web projects as startup projects in your solution.
To do this, right-click on the solution in Solution Explorer, select "Set Startup Projects," and configure both projects to start together.

During registration, users can choose their role:
Developer: Default role for users without the "Is User" checkbox checked.
User: Registered by checking the "Is User" checkbox.

Searching Bugs
The system employs Sieve for searching bug reports.
Users and Developers can search the bug list by:
Bug Number: Enter the specific bug number to find.
Assignee: Filter bugs based on the assigned Developer.
