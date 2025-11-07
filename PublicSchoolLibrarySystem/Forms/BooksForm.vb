Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class BooksForm
    Inherits Form

    Private txtTitle As TextBox
    Private txtAuthor As TextBox
    Private txtCategory As TextBox
    Private txtISBN As TextBox
    Private txtQuantity As TextBox
    Private btnAdd As Button
    Private btnUpdate As Button
    Private btnDelete As Button
    Private btnClear As Button
    Public dgvBooks As DataGridView

    Private selectedBookId As Integer = -1

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Books Management"
        Me.Size = New Size(900, 600)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.White
        Dim header As New Label() With {.Text = "Books", .Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold), .Padding = New Padding(15, 0, 0, 0)}
        Me.Controls.Add(header)

        Dim topOffset As Integer = 70
        Dim lblT As New Label() With {.Text = "Title", .Left = 20, .Top = topOffset}
        txtTitle = New TextBox() With {.Left = 120, .Top = topOffset, .Width = 250}
        Dim lblA As New Label() With {.Text = "Author", .Left = 20, .Top = topOffset + 40}
        txtAuthor = New TextBox() With {.Left = 120, .Top = topOffset + 40, .Width = 250}
        Dim lblC As New Label() With {.Text = "Category", .Left = 20, .Top = topOffset + 80}
        txtCategory = New TextBox() With {.Left = 120, .Top = topOffset + 80, .Width = 250}
        Dim lblI As New Label() With {.Text = "ISBN", .Left = 20, .Top = topOffset + 120}
        txtISBN = New TextBox() With {.Left = 120, .Top = topOffset + 120, .Width = 250}
        Dim lblQ As New Label() With {.Text = "Quantity", .Left = 20, .Top = topOffset + 160}
        txtQuantity = New TextBox() With {.Left = 120, .Top = topOffset + 160, .Width = 250}

        btnAdd = New Button() With {.Text = "Add", .Left = 400, .Top = topOffset, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .TextAlign = ContentAlignment.MiddleCenter}
        btnUpdate = New Button() With {.Text = "Update", .Left = 400, .Top = topOffset + 40, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .TextAlign = ContentAlignment.MiddleCenter}
        btnDelete = New Button() With {.Text = "Delete", .Left = 400, .Top = topOffset + 80, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(231, 76, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .TextAlign = ContentAlignment.MiddleCenter}
        btnClear = New Button() With {.Text = "Clear", .Left = 400, .Top = topOffset + 120, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(149, 165, 166), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .TextAlign = ContentAlignment.MiddleCenter}

        dgvBooks = New DataGridView() With {.Left = 20, .Top = topOffset + 210, .Width = 840, .Height = 310, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom}

        Me.Controls.AddRange(New Control() {lblT, txtTitle, lblA, txtAuthor, lblC, txtCategory, lblI, txtISBN, lblQ, txtQuantity, btnAdd, btnUpdate, btnDelete, btnClear, dgvBooks})
        ' Back button in upper-right
        Functions.AddBackButton(Me)

        ' Round buttons
        Functions.RoundControl(btnAdd, 8)
        Functions.RoundControl(btnUpdate, 8)
        Functions.RoundControl(btnDelete, 8)
        Functions.RoundControl(btnClear, 8)

        ' Enhance button look and feel
        StyleButton(btnAdd, Color.FromArgb(52, 152, 219))
        StyleButton(btnUpdate, Color.FromArgb(52, 152, 219))
        StyleButton(btnDelete, Color.FromArgb(231, 76, 60))
        StyleButton(btnClear, Color.FromArgb(149, 165, 166))
        ' Style grid
        Functions.StyleGrid(dgvBooks)
        dgvBooks.BackgroundColor = Color.White
        dgvBooks.GridColor = Color.FromArgb(230, 235, 240)
        dgvBooks.ColumnHeadersHeight = 38
        dgvBooks.ColumnHeadersDefaultCellStyle.Padding = New Padding(10, 6, 10, 6)
        dgvBooks.DefaultCellStyle.Padding = New Padding(8, 4, 8, 4)
        dgvBooks.RowTemplate.Height = 34
        dgvBooks.AllowUserToResizeRows = False

        AddHandler Me.Load, AddressOf BooksForm_Load
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnUpdate.Click, AddressOf BtnUpdate_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click
        AddHandler btnClear.Click, AddressOf BtnClear_Click
        AddHandler dgvBooks.CellClick, AddressOf DgvBooks_CellClick
    End Sub

    Private Sub BooksForm_Load(sender As Object, e As EventArgs)
        LoadBooks()
        If Session.CurrentRole <> "Admin" AndAlso Session.CurrentRole <> "Librarian" Then
            btnAdd.Enabled = False : btnUpdate.Enabled = False : btnDelete.Enabled = False
        End If
    End Sub

    Private Sub LoadBooks()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "SELECT book_id, title, author, category, isbn, quantity FROM tbl_books"
                Using da As New MySqlDataAdapter(sql, con)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvBooks.DataSource = dt
                    If dgvBooks.Columns.Contains("book_id") Then dgvBooks.Columns("book_id").Visible = False
                    ' Friendly headers and proportional widths
                    If dgvBooks.Columns.Contains("title") Then dgvBooks.Columns("title").HeaderText = "Title" : dgvBooks.Columns("title").FillWeight = 35
                    If dgvBooks.Columns.Contains("author") Then dgvBooks.Columns("author").HeaderText = "Author" : dgvBooks.Columns("author").FillWeight = 25
                    If dgvBooks.Columns.Contains("category") Then dgvBooks.Columns("category").HeaderText = "Category" : dgvBooks.Columns("category").FillWeight = 15
                    If dgvBooks.Columns.Contains("isbn") Then dgvBooks.Columns("isbn").HeaderText = "ISBN" : dgvBooks.Columns("isbn").FillWeight = 15
                    If dgvBooks.Columns.Contains("quantity") Then
                        dgvBooks.Columns("quantity").HeaderText = "Quantity"
                        dgvBooks.Columns("quantity").FillWeight = 10
                        dgvBooks.Columns("quantity").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading books: " & ex.Message)
        End Try
    End Sub

    Private Sub DgvBooks_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso dgvBooks.Rows.Count > 0 Then
            Dim row = dgvBooks.Rows(e.RowIndex)
            selectedBookId = Convert.ToInt32(row.Cells("book_id").Value)
            txtTitle.Text = row.Cells("title").Value.ToString()
            txtAuthor.Text = row.Cells("author").Value.ToString()
            txtCategory.Text = row.Cells("category").Value.ToString()
            txtISBN.Text = row.Cells("isbn").Value.ToString()
            txtQuantity.Text = row.Cells("quantity").Value.ToString()
        End If
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "INSERT INTO tbl_books(title, author, category, isbn, quantity) VALUES(@t,@a,@c,@i,@q)"
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@t", txtTitle.Text.Trim())
                    cmd.Parameters.AddWithValue("@a", txtAuthor.Text.Trim())
                    cmd.Parameters.AddWithValue("@c", txtCategory.Text.Trim())
                    cmd.Parameters.AddWithValue("@i", txtISBN.Text.Trim())
                    Dim qty = Integer.Parse(If(String.IsNullOrWhiteSpace(txtQuantity.Text), "0", txtQuantity.Text))
                    cmd.Parameters.AddWithValue("@q", qty)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Functions.LogActivity("Added book: " & txtTitle.Text)
            LoadBooks()
            BtnClear_Click(Nothing, Nothing)
            AppEvents.RaiseStatsChanged()
        Catch ex As Exception
            MessageBox.Show("Add error: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
        If selectedBookId <= 0 Then
            MessageBox.Show("Select a book to update.")
            Return
        End If
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "UPDATE tbl_books SET title=@t, author=@a, category=@c, isbn=@i, quantity=@q WHERE book_id=@id"
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@t", txtTitle.Text.Trim())
                    cmd.Parameters.AddWithValue("@a", txtAuthor.Text.Trim())
                    cmd.Parameters.AddWithValue("@c", txtCategory.Text.Trim())
                    cmd.Parameters.AddWithValue("@i", txtISBN.Text.Trim())
                    Dim qty = Integer.Parse(If(String.IsNullOrWhiteSpace(txtQuantity.Text), "0", txtQuantity.Text))
                    cmd.Parameters.AddWithValue("@q", qty)
                    cmd.Parameters.AddWithValue("@id", selectedBookId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Functions.LogActivity("Updated book: " & txtTitle.Text & " (ID " & selectedBookId & ")")
            LoadBooks()
            AppEvents.RaiseStatsChanged()
        Catch ex As Exception
            MessageBox.Show("Update error: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If selectedBookId <= 0 Then
            MessageBox.Show("Select a book to delete.")
            Return
        End If
        If MessageBox.Show("Delete selected book?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                Using con = DbConnection.GetConnection()
                    con.Open()
                    Dim sql = "DELETE FROM tbl_books WHERE book_id=@id"
                    Using cmd As New MySqlCommand(sql, con)
                        cmd.Parameters.AddWithValue("@id", selectedBookId)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                Functions.LogActivity("Deleted book ID " & selectedBookId)
                LoadBooks()
                BtnClear_Click(Nothing, Nothing)
                AppEvents.RaiseStatsChanged()
            Catch ex As Exception
                MessageBox.Show("Delete error: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        txtTitle.Clear()
        txtAuthor.Clear()
        txtCategory.Clear()
        txtISBN.Clear()
        txtQuantity.Clear()
        selectedBookId = -1
    End Sub
End Class

' Helpers for UI polish
Partial Class BooksForm
    Private Sub StyleButton(btn As Button, baseColor As Color)
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(baseColor, 0.15F)
        btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(baseColor)
        btn.Cursor = Cursors.Hand
        btn.UseCompatibleTextRendering = True
        btn.Font = New Font("Segoe UI", 10, FontStyle.Bold)
    End Sub
End Class
