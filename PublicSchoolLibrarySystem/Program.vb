Imports System
Imports System.Windows.Forms

Module Program
    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        ' Ensure schema and seed data on startup (handles WAMP/XAMPP imports that missed tables)
        DatabaseInitializer.EnsureSchema()
        ' Ensure database has initial data (books)
        DatabaseSeeder.EnsureSeed()
        BookImporter.ImportProvidedBooks()
        Application.Run(New LoginForm())
    End Sub
End Module
