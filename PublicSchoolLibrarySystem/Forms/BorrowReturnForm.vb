Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class BorrowReturnForm
    Inherits Form

    Private cboStudents As ComboBox
    Private cboBooks As ComboBox
    Private btnBorrow As Button
    Private btnReturn As Button
    Public dgvBorrowed As DataGridView

    Private selectedBorrowId As Integer = -1
    Private selectedBorrowBookId As Integer = -1

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Borrow / Return"
        Me.Size = New Size(1000, 600)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.White
        Dim header As New Label() With {.Text = "Borrow / Return", .Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold), .Padding = New Padding(15, 0, 0, 0)}
        Me.Controls.Add(header)

        Dim topOffset As Integer = 80
        Dim lblS As New Label() With {.Text = "Student", .Left = 20, .Top = topOffset}
        cboStudents = New ComboBox() With {.Left = 120, .Top = topOffset - 2, .Width = 300, .DropDownStyle = ComboBoxStyle.DropDownList}
        Dim lblB As New Label() With {.Text = "Book", .Left = 440, .Top = topOffset}
        cboBooks = New ComboBox() With {.Left = 510, .Top = topOffset - 2, .Width = 300, .DropDownStyle = ComboBoxStyle.DropDownList}

        btnBorrow = New Button() With {.Text = "Borrow", .Left = 820, .Top = topOffset - 4, .Width = 100, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnReturn = New Button() With {.Text = "Return", .Left = 930, .Top = topOffset - 4, .Width = 100, .Height = 40, .BackColor = Color.FromArgb(46, 204, 113), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

        dgvBorrowed = New DataGridView() With {.Left = 20, .Top = topOffset + 40, .Width = 950, .Height = 500, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom}

        Me.Controls.AddRange(New Control() {lblS, cboStudents, lblB, cboBooks, btnBorrow, btnReturn, dgvBorrowed})
        ' Back button in upper-right
        Functions.AddBackButton(Me)

        ' Round buttons and style grid
        Functions.RoundControl(btnBorrow, 8)
        Functions.RoundControl(btnReturn, 8)
        ' Consistent modern button styling
        Functions.StyleButton(btnBorrow, Color.FromArgb(52, 152, 219))
        Functions.StyleButton(btnReturn, Color.FromArgb(46, 204, 113))
        ' Consistent table styling
        Functions.StyleGrid(dgvBorrowed)

        ' Prevent clipped text and unify input styling
        Functions.StyleComboBox(cboStudents)
        Functions.StyleComboBox(cboBooks)
        cboStudents.BringToFront()
        cboBooks.BringToFront()
    
        AddHandler Me.Load, AddressOf BorrowReturnForm_Load
        AddHandler btnBorrow.Click, AddressOf BtnBorrow_Click
        AddHandler btnReturn.Click, AddressOf BtnReturn_Click
        AddHandler dgvBorrowed.CellClick, AddressOf DgvBorrowed_CellClick
    End Sub

    Private Sub BorrowReturnForm_Load(sender As Object, e As EventArgs)
        LoadStudents()
        LoadBooks()
        LoadBorrowed()
    End Sub

    Private Sub LoadStudents()
        Try
            Dim dt = Functions.FillDataTable("SELECT student_id, fullname FROM tbl_students ORDER BY fullname")
            Dim ph = dt.NewRow()
            ph("student_id") = -1
            ph("fullname") = "Select student"
            dt.Rows.InsertAt(ph, 0)
            cboStudents.DataSource = dt
            cboStudents.DisplayMember = "fullname"
            cboStudents.ValueMember = "student_id"
            cboStudents.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Error loading students: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadBooks()
        Try
            Dim dt = Functions.FillDataTable("SELECT book_id, title, quantity FROM tbl_books WHERE quantity > 0 ORDER BY title")
            Dim ph = dt.NewRow()
            ph("book_id") = -1
            ph("title") = "Select book"
            ph("quantity") = DBNull.Value
            dt.Rows.InsertAt(ph, 0)
            cboBooks.DataSource = dt
            cboBooks.DisplayMember = "title"
            cboBooks.ValueMember = "book_id"
            cboBooks.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Error loading books: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadBorrowed()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "SELECT b.borrow_id, s.fullname AS student, bk.title AS book, b.borrow_date, b.return_date, b.status, b.book_id FROM tbl_borrowed b JOIN tbl_students s ON b.student_id=s.student_id JOIN tbl_books bk ON b.book_id=bk.book_id ORDER BY b.borrow_id DESC"
                Using da As New MySqlDataAdapter(sql, con)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvBorrowed.DataSource = dt
                    If dgvBorrowed.Columns.Contains("borrow_id") Then dgvBorrowed.Columns("borrow_id").Visible = False
                    If dgvBorrowed.Columns.Contains("book_id") Then dgvBorrowed.Columns("book_id").Visible = False
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading borrowed: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnBorrow_Click(sender As Object, e As EventArgs)
        If cboStudents.SelectedItem Is Nothing OrElse cboBooks.SelectedItem Is Nothing OrElse CInt(cboStudents.SelectedValue) = -1 OrElse CInt(cboBooks.SelectedValue) = -1 Then
            MessageBox.Show("Select a student and a book.")
            Return
        End If
        Dim studentId As Integer = CInt(cboStudents.SelectedValue)
        Dim bookId As Integer = CInt(cboBooks.SelectedValue)

        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Using tran = con.BeginTransaction()
                    Try
                        Dim insertSql = "INSERT INTO tbl_borrowed(student_id, book_id, borrow_date, status) VALUES(@sid,@bid,@bd,'Borrowed')"
                        Using cmd As New MySqlCommand(insertSql, con, tran)
                            cmd.Parameters.AddWithValue("@sid", studentId)
                            cmd.Parameters.AddWithValue("@bid", bookId)
                            cmd.Parameters.AddWithValue("@bd", Date.Now.Date)
                            cmd.ExecuteNonQuery()
                        End Using

                        Dim updateSql = "UPDATE tbl_books SET quantity = quantity - 1 WHERE book_id=@bid AND quantity > 0"
                        Using cmd2 As New MySqlCommand(updateSql, con, tran)
                            cmd2.Parameters.AddWithValue("@bid", bookId)
                            Dim affected = cmd2.ExecuteNonQuery()
                            If affected = 0 Then
                                Throw New Exception("No stock available.")
                            End If
                        End Using

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
            Functions.LogActivity("Borrow: Student ID " & studentId & " borrowed Book ID " & bookId)
            LoadBorrowed()
            LoadBooks()
            AppEvents.RaiseStatsChanged()
        Catch ex As Exception
            MessageBox.Show("Borrow error: " & ex.Message)
        End Try
    End Sub

    Private Sub DgvBorrowed_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso dgvBorrowed.Rows.Count > 0 Then
            Dim row = dgvBorrowed.Rows(e.RowIndex)
            selectedBorrowId = Convert.ToInt32(row.Cells("borrow_id").Value)
            selectedBorrowBookId = Convert.ToInt32(row.Cells("book_id").Value)
        End If
    End Sub

    Private Sub BtnReturn_Click(sender As Object, e As EventArgs)
        If selectedBorrowId <= 0 OrElse selectedBorrowBookId <= 0 Then
            MessageBox.Show("Select a borrowed record to return.")
            Return
        End If
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Using tran = con.BeginTransaction()
                    Try
                        Dim updateBorrowSql = "UPDATE tbl_borrowed SET status='Returned', return_date=@rd WHERE borrow_id=@id AND status='Borrowed'"
                        Using cmd As New MySqlCommand(updateBorrowSql, con, tran)
                            cmd.Parameters.AddWithValue("@rd", Date.Now.Date)
                            cmd.Parameters.AddWithValue("@id", selectedBorrowId)
                            Dim affected = cmd.ExecuteNonQuery()
                            If affected = 0 Then
                                Throw New Exception("Record already returned or invalid.")
                            End If
                        End Using

                        Dim updateBookSql = "UPDATE tbl_books SET quantity = quantity + 1 WHERE book_id=@bid"
                        Using cmd2 As New MySqlCommand(updateBookSql, con, tran)
                            cmd2.Parameters.AddWithValue("@bid", selectedBorrowBookId)
                            cmd2.ExecuteNonQuery()
                        End Using

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
            Functions.LogActivity("Return: Borrow ID " & selectedBorrowId & " returned Book ID " & selectedBorrowBookId)
            LoadBorrowed()
            LoadBooks()
            AppEvents.RaiseStatsChanged()
        Catch ex As Exception
            MessageBox.Show("Return error: " & ex.Message)
        End Try
    End Sub
End Class
