Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class StudentsForm
    Inherits Form

    Private txtName As TextBox
    Private cboCollege As ComboBox
    Private cboYear As ComboBox
    Private cboSection As ComboBox
    Private txtContact As TextBox
    Private btnAdd As Button
    Private btnUpdate As Button
    Private btnDelete As Button
    Private btnClear As Button
    Public dgvStudents As DataGridView

    Private selectedStudentId As Integer = -1

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Students Management"
        Me.Size = New Size(900, 600)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.White
        Dim header As New Label() With {.Text = "Students", .Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold), .Padding = New Padding(15, 0, 0, 0)}
        Me.Controls.Add(header)

        Dim topOffset As Integer = 70
        Dim lblN As New Label() With {.Text = "Full Name", .Left = 20, .Top = topOffset}
        txtName = New TextBox() With {.Left = 140, .Top = topOffset, .Width = 250}
        Dim lblG As New Label() With {.Text = "College", .Left = 20, .Top = topOffset + 40}
        ' Align College width with Full Name textbox
        cboCollege = New ComboBox() With {.Left = 140, .Top = topOffset + 40, .Width = 250, .DropDownStyle = ComboBoxStyle.DropDownList}
        cboCollege.Items.AddRange(New Object() {
            "College of Computing Sciences (CICS)",
            "College of Teacher Education (CTED)",
            "College of Hospitality Management (CHM)",
            "College of Agriculture (CA)",
            "College of Criminal Justice Education (CCJE)",
            "College of Business, Entrepreneurship and Accountancy (CBEA)"
        })
        Dim lblS As New Label() With {.Text = "Year & Section", .Left = 20, .Top = topOffset + 80}
        ' Split the available 250px into Year (120) + gap (10) + Section (120)
        cboYear = New ComboBox() With {.Left = 140, .Top = topOffset + 80, .Width = 120, .DropDownStyle = ComboBoxStyle.DropDownList}
        cboYear.Items.AddRange(New Object() {"1", "2", "3", "4"})
        cboSection = New ComboBox() With {.Left = 270, .Top = topOffset + 80, .Width = 120, .DropDownStyle = ComboBoxStyle.DropDownList}
        cboSection.Items.AddRange(New Object() {"A", "B"})
        Dim lblC As New Label() With {.Text = "Contact", .Left = 20, .Top = topOffset + 120}
        txtContact = New TextBox() With {.Left = 140, .Top = topOffset + 120, .Width = 250}

        btnAdd = New Button() With {.Text = "Add", .Left = 420, .Top = topOffset, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnUpdate = New Button() With {.Text = "Update", .Left = 420, .Top = topOffset + 40, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnDelete = New Button() With {.Text = "Delete", .Left = 420, .Top = topOffset + 80, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(231, 76, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnClear = New Button() With {.Text = "Clear", .Left = 420, .Top = topOffset + 120, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(149, 165, 166), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

        dgvStudents = New DataGridView() With {.Left = 20, .Top = topOffset + 210, .Width = 840, .Height = 310, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom}

        Me.Controls.AddRange(New Control() {lblN, txtName, lblG, cboCollege, lblS, cboYear, cboSection, lblC, txtContact, btnAdd, btnUpdate, btnDelete, btnClear, dgvStudents})
        ' Back button in upper-right
        Functions.AddBackButton(Me)

        ' Round buttons and style grid
        Functions.RoundControl(btnAdd, 8)
        Functions.RoundControl(btnUpdate, 8)
        Functions.RoundControl(btnDelete, 8)
        Functions.RoundControl(btnClear, 8)
        ' Consistent modern button styling
        Functions.StyleButton(btnAdd, Color.FromArgb(52, 152, 219))
        Functions.StyleButton(btnUpdate, Color.FromArgb(52, 152, 219))
        Functions.StyleButton(btnDelete, Color.FromArgb(231, 76, 60))
        Functions.StyleButton(btnClear, Color.FromArgb(149, 165, 166))
        ' Consistent table styling
        Functions.StyleGrid(dgvStudents)
        ' Style inputs
        Functions.StyleTextBox(txtName)
        Functions.StyleTextBox(txtContact)
        Functions.StyleComboBox(cboCollege)
        Functions.StyleComboBox(cboYear)
        Functions.StyleComboBox(cboSection)

        AddHandler Me.Load, AddressOf StudentsForm_Load
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnUpdate.Click, AddressOf BtnUpdate_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click
        AddHandler btnClear.Click, AddressOf BtnClear_Click
        AddHandler dgvStudents.CellClick, AddressOf DgvStudents_CellClick
    End Sub

    Private Sub StudentsForm_Load(sender As Object, e As EventArgs)
        LoadStudents()
    End Sub

    Private Sub LoadStudents()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "SELECT student_id, fullname, grade_level, section, contact FROM tbl_students"
                Using da As New MySqlDataAdapter(sql, con)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvStudents.DataSource = dt
                    If dgvStudents.Columns.Contains("student_id") Then dgvStudents.Columns("student_id").Visible = False
                    If dgvStudents.Columns.Contains("grade_level") Then dgvStudents.Columns("grade_level").HeaderText = "College"
                    If dgvStudents.Columns.Contains("section") Then dgvStudents.Columns("section").HeaderText = "Year & Section"
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading students: " & ex.Message)
        End Try
    End Sub

    Private Sub DgvStudents_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso dgvStudents.Rows.Count > 0 Then
            Dim row = dgvStudents.Rows(e.RowIndex)
            selectedStudentId = Convert.ToInt32(row.Cells("student_id").Value)
            txtName.Text = row.Cells("fullname").Value.ToString()
            Dim college = row.Cells("grade_level").Value.ToString()
            If cboCollege.Items.Contains(college) Then
                cboCollege.SelectedItem = college
            Else
                cboCollege.SelectedIndex = -1
            End If

            Dim ys = row.Cells("section").Value.ToString()
            Dim yr As String = ""
            Dim sec As String = ""
            ' Parse patterns like "1-A", "Year 1 - Section A", or plain "1A"
            Try
                Dim cleaned = ys.Replace("Year", "").Replace("Section", "").Replace(" ", "").Replace("-", "").Replace("/", "").Trim()
                If cleaned.Length >= 2 Then
                    yr = cleaned.Substring(0, 1)
                    sec = cleaned.Substring(1, 1).ToUpperInvariant()
                End If
            Catch
            End Try
            If cboYear.Items.Contains(yr) Then cboYear.SelectedItem = yr Else cboYear.SelectedIndex = -1
            If cboSection.Items.Contains(sec) Then cboSection.SelectedItem = sec Else cboSection.SelectedIndex = -1
            txtContact.Text = row.Cells("contact").Value.ToString()
        End If
    End Sub

    Private Function ComposeYearSection() As String
        Dim y = If(cboYear.SelectedItem, "").ToString()
        Dim s = If(cboSection.SelectedItem, "").ToString()
        If String.IsNullOrWhiteSpace(y) OrElse String.IsNullOrWhiteSpace(s) Then
            Return ""
        End If
        Return y & "-" & s
    End Function

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "INSERT INTO tbl_students(fullname, grade_level, section, contact) VALUES(@n,@g,@s,@c)"
                Using cmd As New MySqlCommand(sql, con)
                    ' Validation
                    If String.IsNullOrWhiteSpace(txtName.Text.Trim()) Then
                        MessageBox.Show("Enter full name.") : Return
                    End If
                    If cboCollege.SelectedIndex < 0 Then
                        MessageBox.Show("Select a college.") : Return
                    End If
                    If cboYear.SelectedIndex < 0 OrElse cboSection.SelectedIndex < 0 Then
                        MessageBox.Show("Select year and section.") : Return
                    End If

                    Dim ys = ComposeYearSection()
                    cmd.Parameters.AddWithValue("@n", txtName.Text.Trim())
                    cmd.Parameters.AddWithValue("@g", cboCollege.SelectedItem.ToString())
                    cmd.Parameters.AddWithValue("@s", ys)
                    cmd.Parameters.AddWithValue("@c", txtContact.Text.Trim())
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Functions.LogActivity("Added student: " & txtName.Text)
            LoadStudents()
            BtnClear_Click(Nothing, Nothing)
            AppEvents.RaiseStatsChanged()
        Catch ex As Exception
            MessageBox.Show("Add error: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
        If selectedStudentId <= 0 Then
            MessageBox.Show("Select a student to update.")
            Return
        End If
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "UPDATE tbl_students SET fullname=@n, grade_level=@g, section=@s, contact=@c WHERE student_id=@id"
                Using cmd As New MySqlCommand(sql, con)
                    ' Validation
                    If String.IsNullOrWhiteSpace(txtName.Text.Trim()) Then
                        MessageBox.Show("Enter full name.") : Return
                    End If
                    If cboCollege.SelectedIndex < 0 Then
                        MessageBox.Show("Select a college.") : Return
                    End If
                    If cboYear.SelectedIndex < 0 OrElse cboSection.SelectedIndex < 0 Then
                        MessageBox.Show("Select year and section.") : Return
                    End If

                    Dim ys = ComposeYearSection()
                    cmd.Parameters.AddWithValue("@n", txtName.Text.Trim())
                    cmd.Parameters.AddWithValue("@g", cboCollege.SelectedItem.ToString())
                    cmd.Parameters.AddWithValue("@s", ys)
                    cmd.Parameters.AddWithValue("@c", txtContact.Text.Trim())
                    cmd.Parameters.AddWithValue("@id", selectedStudentId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Functions.LogActivity("Updated student: " & txtName.Text & " (ID " & selectedStudentId & ")")
            LoadStudents()
            AppEvents.RaiseStatsChanged()
        Catch ex As Exception
            MessageBox.Show("Update error: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If selectedStudentId <= 0 Then
            MessageBox.Show("Select a student to delete.")
            Return
        End If
        If MessageBox.Show("Delete selected student?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                Using con = DbConnection.GetConnection()
                    con.Open()
                    Using tran = con.BeginTransaction()
                        Try
                            ' Check for dependent borrowed records
                            Dim cntSql = "SELECT COUNT(*) FROM tbl_borrowed WHERE student_id=@id"
                            Dim dependentCount As Integer = 0
                            Using cmdCnt As New MySqlCommand(cntSql, con, tran)
                                cmdCnt.Parameters.AddWithValue("@id", selectedStudentId)
                                dependentCount = Convert.ToInt32(cmdCnt.ExecuteScalar())
                            End Using

                            If dependentCount > 0 Then
                                Dim resp = MessageBox.Show("This student has " & dependentCount & " borrow/return records. Delete them as well? (Any active borrow will be returned and stock restored)", "Linked Records Found", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                If resp = DialogResult.Yes Then
                                    ' Restore stock for any active borrows before deletion
                                    Using cmdActive As New MySqlCommand("SELECT book_id FROM tbl_borrowed WHERE student_id=@id AND status='Borrowed'", con, tran)
                                        cmdActive.Parameters.AddWithValue("@id", selectedStudentId)
                                        Using rdr = cmdActive.ExecuteReader()
                                            Dim activeBookIds As New List(Of Integer)()
                                            While rdr.Read()
                                                activeBookIds.Add(Convert.ToInt32(rdr("book_id")))
                                            End While
                                            rdr.Close()
                                            For Each bid In activeBookIds
                                                Using cmdRestore As New MySqlCommand("UPDATE tbl_books SET quantity = quantity + 1 WHERE book_id=@bid", con, tran)
                                                    cmdRestore.Parameters.AddWithValue("@bid", bid)
                                                    cmdRestore.ExecuteNonQuery()
                                                End Using
                                            Next
                                        End Using
                                    End Using

                                    ' Now remove borrow history for this student
                                    Using cmdDelChild As New MySqlCommand("DELETE FROM tbl_borrowed WHERE student_id=@id", con, tran)
                                        cmdDelChild.Parameters.AddWithValue("@id", selectedStudentId)
                                        cmdDelChild.ExecuteNonQuery()
                                    End Using
                                Else
                                    ' Abort to avoid FK violation
                                    tran.Rollback()
                                    MessageBox.Show("Deletion canceled. The student has linked borrow records.")
                                    Return
                                End If
                            End If

                            ' Delete the student
                            Using cmdDel As New MySqlCommand("DELETE FROM tbl_students WHERE student_id=@id", con, tran)
                                cmdDel.Parameters.AddWithValue("@id", selectedStudentId)
                                cmdDel.ExecuteNonQuery()
                            End Using

                            tran.Commit()
                        Catch exTrans As Exception
                            Try
                                tran.Rollback()
                            Catch
                            End Try
                            Throw
                        End Try
                    End Using
                End Using
                Functions.LogActivity("Deleted student ID " & selectedStudentId)
                LoadStudents()
                BtnClear_Click(Nothing, Nothing)
                AppEvents.RaiseStatsChanged()
            Catch ex As Exception
                MessageBox.Show("Delete error: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        txtName.Clear()
        cboCollege.SelectedIndex = -1
        cboYear.SelectedIndex = -1
        cboSection.SelectedIndex = -1
        txtContact.Clear()
        selectedStudentId = -1
    End Sub
End Class
