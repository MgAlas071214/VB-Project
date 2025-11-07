Imports MySql.Data.MySqlClient

Module BookImporter
    Public Sub ImportProvidedBooks()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim items = New List(Of (String, String, String, String, Integer)) From {
                    ("The Silent Library", "John Matthews", "Mystery", "978-1-23456-001", 10),
                    ("Dreams of Tomorrow", "Emily Carter", "Science Fiction", "978-1-23456-002", 5),
                    ("The Lost Kingdom", "Arthur Bennett", "Fantasy", "978-1-23456-003", 7),
                    ("Whispers in the Dark", "Samantha Lee", "Thriller", "978-1-23456-004", 8),
                    ("Echoes of Time", "Michael Torres", "Historical Fiction", "978-1-23456-005", 9),
                    ("The Art of Peace", "Hannah Johnson", "Philosophy", "978-1-23456-006", 6),
                    ("Digital Fortress", "Dan Brown", "Tech Thriller", "978-1-23456-007", 12),
                    ("Infinite Horizons", "Rachel Kim", "Adventure", "978-1-23456-008", 10),
                    ("Mind Over Matter", "Sarah Cruz", "Self-Help", "978-1-23456-009", 8),
                    ("Codebreakers", "Liam Garcia", "Technology", "978-1-23456-010", 11),
                    ("Beneath the Stars", "Olivia Scott", "Romance", "978-1-23456-011", 9),
                    ("A Study in Scarlet", "Arthur Conan Doyle", "Detective", "978-1-23456-012", 5),
                    ("The Ocean Between Us", "James Collins", "Drama", "978-1-23456-013", 7),
                    ("Forgotten Memories", "Laura Hayes", "Psychological", "978-1-23456-014", 10),
                    ("The Hidden Path", "William Clark", "Adventure", "978-1-23456-015", 9),
                    ("Rise of the Titans", "Alexandra Stone", "Fantasy", "978-1-23456-016", 8),
                    ("Parallel Realities", "Nathan Green", "Science Fiction", "978-1-23456-017", 12),
                    ("The Light Within", "Sophia Brooks", "Inspirational", "978-1-23456-018", 6),
                    ("Broken Promises", "Ethan Morales", "Drama", "978-1-23456-019", 9),
                    ("Winds of Change", "Victoria Adams", "Romance", "978-1-23456-020", 7),
                    ("Quantum Mind", "Daniel Foster", "Science", "978-1-23456-021", 10),
                    ("Legacy of the Fallen", "Marcus Hill", "Fantasy", "978-1-23456-022", 8),
                    ("Voices from the Deep", "Caroline Reed", "Mystery", "978-1-23456-023", 11),
                    ("Shattered Reality", "Patrick Owens", "Thriller", "978-1-23456-024", 9),
                    ("A World Apart", "Chloe Turner", "Science Fiction", "978-1-23456-025", 10)
                }

                For Each b In items
                    Dim exists As Boolean
                    Using checkCmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_books WHERE isbn=@i", con)
                        checkCmd.Parameters.AddWithValue("@i", b.Item4)
                        exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0
                    End Using
                    If Not exists Then
                        Using insertCmd As New MySqlCommand("INSERT INTO tbl_books(title, author, category, isbn, quantity) VALUES(@t,@a,@c,@i,@q)", con)
                            insertCmd.Parameters.AddWithValue("@t", b.Item1)
                            insertCmd.Parameters.AddWithValue("@a", b.Item2)
                            insertCmd.Parameters.AddWithValue("@c", b.Item3)
                            insertCmd.Parameters.AddWithValue("@i", b.Item4)
                            insertCmd.Parameters.AddWithValue("@q", b.Item5)
                            insertCmd.ExecuteNonQuery()
                        End Using
                    End If
                Next

                Functions.LogActivity("Imported additional books list")
            End Using
        Catch ex As Exception
        End Try
    End Sub
End Module

