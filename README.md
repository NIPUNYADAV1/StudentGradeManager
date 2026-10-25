# Gradebook

A student grade manager built with ASP.NET Core MVC.

## Run

```powershell
dotnet run
```

Open the local URL printed by `dotnet run`. The default page shows the grade overview.

## Features

- Review grade records and search by student name, student ID, or course.
- Add, edit, and delete student grade records.
- View summary metrics, pass status, and computed letter grades.
- Validate scores from 0 to 100; scores of 60 or higher are passing.
- Register and sign in with an email address and password. Each account can only access its own grade records.

Identity accounts and grade records are stored in the SQLite database at `App_Data/gradebook.db`, which is excluded from source control. Existing records in `App_Data/grades.json` are imported at startup and assigned to the first account registered; future records belong to the account that creates them.

Passwords must be at least eight characters and include an uppercase letter, a lowercase letter, and a number. Five consecutive failed sign-in attempts temporarily lock the account.
