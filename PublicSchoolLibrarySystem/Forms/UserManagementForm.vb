Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class UserManagementForm
    Inherits Form

    Private txtFullName As TextBox
    Private txtUsername As TextBox
    Private txtPassword As TextBox
    Private cboRole As ComboBox
    Private btnAdd As Button
    Private btnUpdate As Button
    Private btnDelete As Button
    Private btnClear As Button
    Public dgvUsers As DataGridView

    Private selectedUserId As Integer = -1

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "User Management"
        Me.Size = New Size(900, 600)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.White
        Dim header As New Label() With {.Text = "User Management", .Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold), .Padding = New Padding(15, 0, 0, 0)}
        Me.Controls.Add(header)

        Dim topOffset As Integer = 70
        Dim lblFN As New Label() With {.Text = "Full Name", .Left = 20, .Top = topOffset}
        txtFullName = New TextBox() With {.Left = 140, .Top = topOffset, .Width = 250}
        Dim lblUN As New Label() With {.Text = "Username", .Left = 20, .Top = topOffset + 40}
        txtUsername = New TextBox() With {.Left = 140, .Top = topOffset + 40, .Width = 250}
        Dim lblPW As New Label() With {.Text = "Password", .Left = 20, .Top = topOffset + 80}
        txtPassword = New TextBox() With {.Left = 140, .Top = topOffset + 80, .Width = 250}
        Dim lblRL As New Label() With {.Text = "Role", .Left = 20, .Top = topOffset + 120}
        cboRole = New ComboBox() With {.Left = 140, .Top = topOffset + 120, .Width = 250, .DropDownStyle = ComboBoxStyle.DropDownList}
        cboRole.Items.AddRange(New String() {"Admin", "Librarian"})

        btnAdd = New Button() With {.Text = "Add", .Left = 420, .Top = topOffset, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnUpdate = New Button() With {.Text = "Update", .Left = 420, .Top = topOffset + 40, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnDelete = New Button() With {.Text = "Delete", .Left = 420, .Top = topOffset + 80, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(231, 76, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}
        btnClear = New Button() With {.Text = "Clear", .Left = 420, .Top = topOffset + 120, .Width = 120, .Height = 40, .BackColor = Color.FromArgb(149, 165, 166), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat}

        dgvUsers = New DataGridView() With {.Left = 20, .Top = topOffset + 210, .Width = 840, .Height = 310, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom}

        Me.Controls.AddRange(New Control() {lblFN, txtFullName, lblUN, txtUsername, lblPW, txtPassword, lblRL, cboRole, btnAdd, btnUpdate, btnDelete, btnClear, dgvUsers})
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
        Functions.StyleGrid(dgvUsers)

        AddHandler Me.Load, AddressOf UserManagementForm_Load
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnUpdate.Click, AddressOf BtnUpdate_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click
        AddHandler btnClear.Click, AddressOf BtnClear_Click
        AddHandler dgvUsers.CellClick, AddressOf DgvUsers_CellClick
    End Sub

    Private Sub UserManagementForm_Load(sender As Object, e As EventArgs)
        If Session.CurrentRole <> "Admin" Then
            MessageBox.Show("Access denied. Admins only.")
            Me.Close()
            Return
        End If
        LoadUsers()
    End Sub

    Private Sub LoadUsers()
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "SELECT user_id, fullname, username, role FROM tbl_users"
                Using da As New MySqlDataAdapter(sql, con)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvUsers.DataSource = dt
                    If dgvUsers.Columns.Contains("user_id") Then dgvUsers.Columns("user_id").Visible = False
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        End Try
    End Sub

    Private Sub DgvUsers_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso dgvUsers.Rows.Count > 0 Then
            Dim row = dgvUsers.Rows(e.RowIndex)
            selectedUserId = Convert.ToInt32(row.Cells("user_id").Value)
            txtFullName.Text = row.Cells("fullname").Value.ToString()
            txtUsername.Text = row.Cells("username").Value.ToString()
            cboRole.SelectedItem = row.Cells("role").Value.ToString()
        End If
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "INSERT INTO tbl_users(fullname, username, password, role) VALUES(@fn,@un,@pw,@rl)"
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@fn", txtFullName.Text.Trim())
                    cmd.Parameters.AddWithValue("@un", txtUsername.Text.Trim())
                    cmd.Parameters.AddWithValue("@pw", txtPassword.Text.Trim())
                    cmd.Parameters.AddWithValue("@rl", cboRole.SelectedItem?.ToString())
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Functions.LogActivity("Added user: " & txtUsername.Text)
            LoadUsers()
            BtnClear_Click(Nothing, Nothing)
        Catch ex As Exception
            MessageBox.Show("Add error: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
        If selectedUserId <= 0 Then
            MessageBox.Show("Select a user to update.")
            Return
        End If
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "UPDATE tbl_users SET fullname=@fn, username=@un, password=@pw, role=@rl WHERE user_id=@id"
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@fn", txtFullName.Text.Trim())
                    cmd.Parameters.AddWithValue("@un", txtUsername.Text.Trim())
                    cmd.Parameters.AddWithValue("@pw", txtPassword.Text.Trim())
                    cmd.Parameters.AddWithValue("@rl", cboRole.SelectedItem?.ToString())
                    cmd.Parameters.AddWithValue("@id", selectedUserId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Functions.LogActivity("Updated user: " & txtUsername.Text & " (ID " & selectedUserId & ")")
            LoadUsers()
        Catch ex As Exception
            MessageBox.Show("Update error: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If selectedUserId <= 0 Then
            MessageBox.Show("Select a user to delete.")
            Return
        End If
        If MessageBox.Show("Delete selected user?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                Using con = DbConnection.GetConnection()
                    con.Open()
                    Dim sql = "DELETE FROM tbl_users WHERE user_id=@id"
                    Using cmd As New MySqlCommand(sql, con)
                        cmd.Parameters.AddWithValue("@id", selectedUserId)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                Functions.LogActivity("Deleted user ID " & selectedUserId)
                LoadUsers()
                BtnClear_Click(Nothing, Nothing)
            Catch ex As Exception
                MessageBox.Show("Delete error: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        txtFullName.Clear()
        txtUsername.Clear()
        txtPassword.Clear()
        cboRole.SelectedIndex = -1
        selectedUserId = -1
    End Sub
End Class
