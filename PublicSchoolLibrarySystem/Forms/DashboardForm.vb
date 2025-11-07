Imports System.Drawing
Imports System.Windows.Forms

Public Class DashboardForm
    Inherits Form

    Private sidebar As Panel
    Private topbar As Panel
    Private lblUserInfo As Label
    Private btnBooks, btnStudents, btnBorrowReturn, btnLogs, btnReports, btnUsers, btnLogout As Button

    Private panelBooksStat, panelStudentsStat, panelBorrowedStat, panelReturnedStat As Panel
    Private lblBooksStat, lblStudentsStat, lblBorrowedStat, lblReturnedStat As Label

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Dashboard"
        Me.WindowState = FormWindowState.Maximized
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.FromArgb(236, 240, 241)

        sidebar = New Panel() With {.Dock = DockStyle.Left, .Width = 220, .BackColor = Color.FromArgb(44, 62, 80)}
        Me.Controls.Add(sidebar)

        topbar = New Panel() With {.Dock = DockStyle.Top, .Height = 60, .BackColor = Color.FromArgb(52, 152, 219)}
        Me.Controls.Add(topbar)

        lblUserInfo = New Label() With {.Text = "", .ForeColor = Color.White, .AutoSize = False, .Dock = DockStyle.Right, .Width = 350, .TextAlign = ContentAlignment.MiddleRight, .Font = New Font("Segoe UI", 11, FontStyle.Bold)}
        Dim lblTitle As New Label() With {.Text = "Public School Library System", .ForeColor = Color.White, .AutoSize = False, .Dock = DockStyle.Left, .Width = 400, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold)}
        topbar.Controls.Add(lblUserInfo)
        topbar.Controls.Add(lblTitle)

        btnBooks = SidebarButton("📚 Books")
        btnStudents = SidebarButton("👥 Students")
        btnBorrowReturn = SidebarButton("🔁 Borrow / Return")
        btnLogs = SidebarButton("🧾 Activity Logs")
        btnReports = SidebarButton("📊 Reports")
        btnUsers = SidebarButton("👤 Users")
        btnLogout = SidebarButton("🔓 Logout")

        Dim y As Integer = 20
        For Each b In New Button() {btnBooks, btnStudents, btnBorrowReturn, btnLogs, btnReports, btnUsers, btnLogout}
            b.Top = y : b.Left = 10 : b.Width = sidebar.Width - 20 : b.Height = 45
            y += 55
            sidebar.Controls.Add(b)
        Next

        AddHandler btnBooks.Click, Sub() OpenForm(New BooksForm())
        AddHandler btnStudents.Click, Sub() OpenForm(New StudentsForm())
        AddHandler btnBorrowReturn.Click, Sub() OpenForm(New BorrowReturnForm())
        AddHandler btnLogs.Click, Sub() OpenForm(New ActivityLogsForm())
        AddHandler btnReports.Click, Sub() OpenForm(New ReportsForm())
        AddHandler btnUsers.Click, Sub() OpenForm(New UserManagementForm())
        AddHandler btnLogout.Click, AddressOf Logout

        Dim statsPanel As New Panel() With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .BackColor = Me.BackColor}
        Me.Controls.Add(statsPanel)
        statsPanel.BringToFront()
        topbar.BringToFront()

        panelBooksStat = StatCard("Books", lblBooksStat)
        panelStudentsStat = StatCard("Students", lblStudentsStat)
        panelBorrowedStat = StatCard("Borrowed", lblBorrowedStat)
        panelReturnedStat = StatCard("Returned", lblReturnedStat)

        panelBooksStat.Left = 260 : panelBooksStat.Top = 100
        panelStudentsStat.Left = 260 + 280 : panelStudentsStat.Top = 100
        panelBorrowedStat.Left = 260 : panelBorrowedStat.Top = 220
        panelReturnedStat.Left = 260 + 280 : panelReturnedStat.Top = 220

        statsPanel.Controls.Add(panelBooksStat)
        statsPanel.Controls.Add(panelStudentsStat)
        statsPanel.Controls.Add(panelBorrowedStat)
        statsPanel.Controls.Add(panelReturnedStat)

        AddHandler Me.Load, AddressOf DashboardForm_Load
        ' Subscribe to global stats change notifications
        AddHandler AppEvents.StatsChanged, AddressOf OnStatsChanged
    End Sub

    Private Function SidebarButton(text As String) As Button
        Dim b As New Button()
        b.Text = text
        b.FlatStyle = FlatStyle.Flat
        b.ForeColor = Color.White
        b.BackColor = Color.FromArgb(52, 73, 94)
        b.FlatAppearance.BorderSize = 0
        b.TextAlign = ContentAlignment.MiddleLeft
        b.Padding = New Padding(12, 0, 0, 0)
        Return b
    End Function

    Private Function StatCard(title As String, ByRef valueLabel As Label) As Panel
        Dim p As New Panel()
        p.Width = 260 : p.Height = 110 : p.BackColor = Color.White
        Dim lblT As New Label() With {.Text = title, .Left = 15, .Top = 12, .Width = 220, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}
        valueLabel = New Label() With {
            .Text = "0",
            .Left = 15,
            .Top = 46,
            .Width = 220,
            .Height = 50,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 28, FontStyle.Bold),
            .ForeColor = Color.FromArgb(52, 152, 219)
        }
        ' Improve rendering to avoid glyph clipping on some DPI settings
        valueLabel.UseCompatibleTextRendering = True
        p.Controls.Add(lblT) : p.Controls.Add(valueLabel)
        Functions.RoundControl(p, 10)
        Return p
    End Function

    Private Sub DashboardForm_Load(sender As Object, e As EventArgs)
        lblUserInfo.Text = "User: " & Session.CurrentUser & " | Role: " & Session.CurrentRole
        Dim isAdmin = (Session.CurrentRole = "Admin")
        btnLogs.Enabled = isAdmin
        btnUsers.Enabled = isAdmin
        RefreshStats()
    End Sub

    Private Sub RefreshStats()
        Try
            lblBooksStat.Text = ExecuteScalarCount("SELECT COUNT(*) FROM tbl_books").ToString()
            lblStudentsStat.Text = ExecuteScalarCount("SELECT COUNT(*) FROM tbl_students").ToString()
            lblBorrowedStat.Text = ExecuteScalarCount("SELECT COUNT(*) FROM tbl_borrowed WHERE status='Borrowed'").ToString()
            lblReturnedStat.Text = ExecuteScalarCount("SELECT COUNT(*) FROM tbl_borrowed WHERE status='Returned'").ToString()
        Catch ex As Exception
            MessageBox.Show("Error loading stats: " & ex.Message)
        End Try
    End Sub

    Private Function ExecuteScalarCount(sql As String) As Integer
        Using con = DbConnection.GetConnection()
            con.Open()
            Using cmd As New MySql.Data.MySqlClient.MySqlCommand(sql, con)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    Private Sub OpenForm(f As Form)
        f.StartPosition = FormStartPosition.CenterScreen
        f.ShowDialog()
        ' After closing a modal form, refresh stats in case of changes
        RefreshStats()
    End Sub

    Private Sub OnStatsChanged(sender As Object, e As EventArgs)
        ' Ensure refresh runs on the UI thread
        If Me.IsHandleCreated Then
            Try
                Me.BeginInvoke(New Action(AddressOf RefreshStats))
            Catch
                ' If invoke fails (e.g., during closing), fallback
                RefreshStats()
            End Try
        Else
            RefreshStats()
        End If
    End Sub

    Private Sub Logout(sender As Object, e As EventArgs)
        Functions.LogActivity("Logout: " & Session.CurrentUser)
        Session.CurrentUser = Nothing
        Session.CurrentRole = Nothing
        Dim login As New LoginForm()
        login.Show()
        Me.Close()
    End Sub
End Class
