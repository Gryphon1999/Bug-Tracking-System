# Bug-Tracking-System
Setup Migration: 
This project uses Entity Framework Core for database migrations. You can migrate the database using the console by running the following command:
dotnet ef database update or update-database

Project Setup:
Set both the API and Web projects as startup projects in your solution.
To do this, right-click on the solution in Solution Explorer, select "Set Startup Projects," and configure both projects to start together.
