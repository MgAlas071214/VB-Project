Imports MySql.Data.MySqlClient

Module DatabaseInitializer
    ' Ensures required tables exist and seeds default users if absent.
    ' This is safe to run at startup and only creates what is missing.
    Public Sub EnsureSchema()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()

                ' tbl_users
                Using cmd As New MySqlCommand("" & _
                    "CREATE TABLE IF NOT EXISTS tbl_users (" & _
                    "  user_id INT AUTO_INCREMENT PRIMARY KEY," & _
                    "  fullname VARCHAR(100)," & _
                    "  username VARCHAR(50) UNIQUE," & _
                    "  password VARCHAR(255)," & _
                    "  role ENUM('Admin','Librarian')" & _
                    ") ENGINE=InnoDB;", con)
                    cmd.ExecuteNonQuery()
                End Using

                ' tbl_books
                Using cmd As New MySqlCommand("" & _
                    "CREATE TABLE IF NOT EXISTS tbl_books (" & _
                    "  book_id INT AUTO_INCREMENT PRIMARY KEY," & _
                    "  title VARCHAR(100)," & _
                    "  author VARCHAR(100)," & _
                    "  category VARCHAR(50)," & _
                    "  isbn VARCHAR(20)," & _
                    "  quantity INT DEFAULT 0" & _
                    ") ENGINE=InnoDB;", con)
                    cmd.ExecuteNonQuery()
                End Using

                ' tbl_students
                Using cmd As New MySqlCommand("" & _
                    "CREATE TABLE IF NOT EXISTS tbl_students (" & _
                    "  student_id INT AUTO_INCREMENT PRIMARY KEY," & _
                    "  fullname VARCHAR(100)," & _
                    "  grade_level VARCHAR(50)," & _
                    "  section VARCHAR(50)," & _
                    "  contact VARCHAR(50)" & _
                    ") ENGINE=InnoDB;", con)
                    cmd.ExecuteNonQuery()
                End Using

                ' tbl_borrowed (FKs will be applied when table is created)
                Using cmd As New MySqlCommand("" & _
                    "CREATE TABLE IF NOT EXISTS tbl_borrowed (" & _
                    "  borrow_id INT AUTO_INCREMENT PRIMARY KEY," & _
                    "  student_id INT," & _
                    "  book_id INT," & _
                    "  borrow_date DATE," & _
                    "  return_date DATE," & _
                    "  status VARCHAR(20)," & _
                    "  INDEX idx_borrow_student(student_id)," & _
                    "  INDEX idx_borrow_book(book_id)," & _
                    "  CONSTRAINT fk_borrow_student FOREIGN KEY (student_id) REFERENCES tbl_students(student_id) ON DELETE CASCADE ON UPDATE CASCADE," & _
                    "  CONSTRAINT fk_borrow_book FOREIGN KEY (book_id) REFERENCES tbl_books(book_id)" & _
                    ") ENGINE=InnoDB;", con)
                    cmd.ExecuteNonQuery()
                End Using

                ' tbl_activity_logs
                Using cmd As New MySqlCommand("" & _
                    "CREATE TABLE IF NOT EXISTS tbl_activity_logs (" & _
                    "  log_id INT AUTO_INCREMENT PRIMARY KEY," & _
                    "  action VARCHAR(255)," & _
                    "  user VARCHAR(100)," & _
                    "  log_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP" & _
                    ") ENGINE=InnoDB;", con)
                    cmd.ExecuteNonQuery()
                End Using

                ' Seed default users if missing
                Dim userCount As Integer = 0
                Try
                    Using cmdCount As New MySqlCommand("SELECT COUNT(*) FROM tbl_users", con)
                        userCount = Convert.ToInt32(cmdCount.ExecuteScalar())
                    End Using
                Catch
                    userCount = 0
                End Try

                If userCount = 0 Then
                    Using cmdSeed As New MySqlCommand("INSERT INTO tbl_users (fullname, username, password, role) VALUES (" & _
                        "'System Admin','admin','admin123','Admin')," & _
                        "('School Librarian','librarian','lib123','Librarian')", con)
                        cmdSeed.ExecuteNonQuery()
                    End Using
                End If
            End Using
        Catch
            ' Do not block app start; schema creation will be attempted again next run.
        End Try
    End Sub
End Module

