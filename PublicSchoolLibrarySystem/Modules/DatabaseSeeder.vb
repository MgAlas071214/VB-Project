Imports MySql.Data.MySqlClient

Module DatabaseSeeder
    Public Sub EnsureSeed()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                ' Check if books table has any rows
                Using cmdCount As New MySqlCommand("SELECT COUNT(*) FROM tbl_books", con)
                    Dim count = Convert.ToInt32(cmdCount.ExecuteScalar())
                    If count > 0 Then Return
                End Using

                ' Seed initial book records
                Dim sql = "INSERT INTO tbl_books(title, author, category, isbn, quantity) VALUES (@t,@a,@c,@i,@q)"
                Dim books = New List(Of (String, String, String, String, Integer)) From {
                    ("To Kill a Mockingbird", "Harper Lee", "Literature", "9780061120084", 5),
                    ("1984", "George Orwell", "Literature", "9780451524935", 4),
                    ("The Great Gatsby", "F. Scott Fitzgerald", "Literature", "9780743273565", 3),
                    ("Introduction to Algorithms", "Thomas H. Cormen", "Computer Science", "9780262033848", 2),
                    ("Clean Code", "Robert C. Martin", "Computer Science", "9780132350884", 3),
                    ("Thinking, Fast and Slow", "Daniel Kahneman", "Psychology", "9780374533557", 2),
                    ("A Brief History of Time", "Stephen Hawking", "Science", "9780553380163", 3),
                    ("Sapiens: A Brief History of Humankind", "Yuval Noah Harari", "History", "9780062316097", 2),
                    ("The Art of War", "Sun Tzu", "Philosophy", "9781590302255", 5),
                    ("The Pragmatic Programmer", "Andrew Hunt; David Thomas", "Computer Science", "9780201616224", 2),
                    ("Calculus Made Easy", "Silvanus P. Thompson", "Mathematics", "9780312185480", 4),
                    ("The Immortal Life of Henrietta Lacks", "Rebecca Skloot", "Biography", "9781400052189", 2)
                }

                For Each b In books
                    Using cmd As New MySqlCommand(sql, con)
                        cmd.Parameters.AddWithValue("@t", b.Item1)
                        cmd.Parameters.AddWithValue("@a", b.Item2)
                        cmd.Parameters.AddWithValue("@c", b.Item3)
                        cmd.Parameters.AddWithValue("@i", b.Item4)
                        cmd.Parameters.AddWithValue("@q", b.Item5)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                Functions.LogActivity("Seeded initial books")
            End Using
        Catch ex As Exception
            ' Swallow errors silently to avoid blocking app start
        End Try
    End Sub
End Module
