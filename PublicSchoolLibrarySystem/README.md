Public School Library Borrowing System (VB.NET WinForms + MySQL)

Overview
- A complete Windows Forms application with Admin and Librarian roles.
- Uses XAMPP MySQL for data storage.
- Includes solution file, project, modules, forms, and database SQL.

Prerequisites
- Windows with Visual Studio 2022/2025 and .NET SDK 8+
- XAMPP installed and MySQL service running
- MySQL root access on `localhost:3306`

Database Setup
1) Start XAMPP and ensure `MySQL` is running.
2) Import the database:
   - Using command line:
     `C:\xampp\mysql\bin\mysql.exe -u root < "PublicSchoolLibrarySystem\Database\library_db.sql"`
   - Or via phpMyAdmin: create `library_db`, then Import the SQL file.
3) Default users:
   - Admin: `admin / admin123`
   - Librarian: `librarian / lib123`

Project Structure
- `PublicSchoolLibrarySystem.sln` – Visual Studio solution
- `PublicSchoolLibrarySystem/` – VB.NET WinForms project
  - `Database/library_db.sql` – schema and seed data
  - `Modules/DbConnection.vb` – MySQL connection provider
  - `Modules/Session.vb` – current user session info
  - `Modules/Functions.vb` – activity logging, UI helpers
  - `Forms/*.vb` – all UI forms and logic
  - `Program.vb` – app entry, starts at LoginForm

Build & Run (Visual Studio)
1) Open `PublicSchoolLibrarySystem.sln` in Visual Studio.
2) Press F5 to run.
3) Login as Admin or Librarian.

Build & Run (CLI)
1) Build: `dotnet build PublicSchoolLibrarySystem.sln`
2) Run: `dotnet run --project PublicSchoolLibrarySystem`
3) Publish Release (optional):
   `dotnet publish PublicSchoolLibrarySystem\PublicSchoolLibrarySystem.vbproj -c Release -r win-x64 --self-contained false`
   Output: `PublicSchoolLibrarySystem\bin\Release\net8.0-windows\win-x64\publish\PublicSchoolLibrarySystem.exe`

HTML Presentation
- For a quick visual walkthrough of the usage flow (setup → login → dashboard → students/books → borrow/return → logs/reports → users), open:
  `PublicSchoolLibrarySystem/docs/flow_presentation.html`

Connection String
- Edit `Modules/DbConnection.vb` if needed:
  `server=localhost;user=root;password=;database=library_db;`
- If you set a root password, update `password=YOURPASS`.
- If MySQL uses another port, append `port=3306`.

Features
- Login with role selection and activity logging
- Dashboard with role-based navigation
- Books and Students management (CRUD)
- Borrow/Return with availability tracking
- Activity Logs (Admin only)
- Reports with CSV export
- User Management (Admin only)

Troubleshooting
- MySQL not running: start it in XAMPP Control Panel.
- Cannot connect: verify credentials in `DbConnection.vb`.
- Missing driver: ensure `MySql.Data` NuGet package is restored.

Security Note
- Passwords are plaintext per schema; for production, use hashed passwords.

