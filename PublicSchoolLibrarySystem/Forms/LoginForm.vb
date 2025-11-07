Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class LoginForm
    Inherits Form

    Private lblTitle As Label
    Private lblFooter As Label
    Private panelCard As Panel
    Public txtUsername As TextBox
    Public txtPassword As TextBox
    Public cboRole As ComboBox
    Public btnLogin As Button
    Public btnExit As Button
    Public lblError As Label

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.DoubleBuffered = True
        Me.Text = "Public School Library System - Login"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(800, 500)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.FromArgb(44, 62, 80)

        lblTitle = New Label() With {
            .Text = "📘 Public School Library System",
            .ForeColor = Color.White,
            .AutoSize = False,
            .Dock = DockStyle.Top,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Height = 60,
            .Font = New Font("Segoe UI", 16, FontStyle.Bold)
        }
        Me.Controls.Add(lblTitle)

        panelCard = New Panel() With {
            .BackColor = Color.White,
            .Width = 380,
            .Height = 300
        }
        panelCard.Location = New Point((Me.ClientSize.Width - panelCard.Width) \ 2, (Me.ClientSize.Height - panelCard.Height) \ 2)
        panelCard.Anchor = AnchorStyles.None
        AddHandler Me.Resize, Sub(sender, e) panelCard.Location = New Point((Me.ClientSize.Width - panelCard.Width) \ 2, (Me.ClientSize.Height - panelCard.Height) \ 2)
        Me.Controls.Add(panelCard)

        ' Footer
        lblFooter = New Label() With {
            .Text = "© Copyrighted by PSLBS 2025 | Developed by Torres",
            .Dock = DockStyle.Bottom,
            .Height = 28,
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Color.White,
            .BackColor = Color.Transparent,
            .Font = New Font("Segoe UI", 9, FontStyle.Regular)
        }
        Me.Controls.Add(lblFooter)
        lblFooter.BringToFront()

        Dim lblU As New Label() With {.Text = "Username", .Top = 30, .Left = 30, .Width = 100}
        txtUsername = New TextBox() With {.Top = 55, .Left = 30, .Width = 320, .Height = 28, .BorderStyle = BorderStyle.None}
        Dim lblP As New Label() With {.Text = "Password", .Top = 95, .Left = 30, .Width = 100}
        txtPassword = New TextBox() With {.Top = 120, .Left = 30, .Width = 320, .Height = 28, .BorderStyle = BorderStyle.None, .UseSystemPasswordChar = True}
        Dim lblR As New Label() With {.Text = "Role", .Top = 160, .Left = 30, .Width = 100}
        cboRole = New ComboBox() With {.Top = 185, .Left = 30, .Width = 320, .DropDownStyle = ComboBoxStyle.DropDownList}
        cboRole.Items.AddRange(New String() {"Admin", "Librarian"})
        btnLogin = New Button() With {
            .Text = "Login",
            .Top = 230,
            .Left = 30,
            .Width = 150,
            .Height = 42,
            .AutoSize = False,
            .BackColor = Color.FromArgb(52, 152, 219),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleCenter,
            .UseCompatibleTextRendering = True
        }
        btnLogin.FlatAppearance.BorderSize = 0
        btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185)
        btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(31, 97, 141)

        btnExit = New Button() With {
            .Text = "Exit",
            .Top = 230,
            .Left = 200,
            .Width = 150,
            .Height = 42,
            .AutoSize = False,
            .BackColor = Color.FromArgb(200, 200, 200),
            .ForeColor = Color.Black,
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleCenter,
            .UseCompatibleTextRendering = True
        }
        btnExit.FlatAppearance.BorderSize = 0
        btnExit.FlatAppearance.MouseOverBackColor = Color.FromArgb(189, 195, 199)
        btnExit.FlatAppearance.MouseDownBackColor = Color.FromArgb(149, 165, 166)

        lblError = New Label() With {.Text = "", .Top = 280, .Left = 30, .Width = 320, .ForeColor = Color.Red}

        ' Underline style for textboxes
        Dim accent As Color = Color.FromArgb(52, 152, 219)
        Dim lineU As New Panel() With {.Left = txtUsername.Left, .Top = txtUsername.Top + txtUsername.Height, .Width = txtUsername.Width, .Height = 2, .BackColor = Color.FromArgb(180, 180, 180)}
        Dim lineP As New Panel() With {.Left = txtPassword.Left, .Top = txtPassword.Top + txtPassword.Height, .Width = txtPassword.Width, .Height = 2, .BackColor = Color.FromArgb(180, 180, 180)}
        AddHandler txtUsername.Enter, Sub() lineU.BackColor = accent
        AddHandler txtUsername.Leave, Sub() lineU.BackColor = Color.FromArgb(180, 180, 180)
        AddHandler txtPassword.Enter, Sub() lineP.BackColor = accent
        AddHandler txtPassword.Leave, Sub() lineP.BackColor = Color.FromArgb(180, 180, 180)

        panelCard.Controls.AddRange(New Control() {lblU, txtUsername, lineU, lblP, txtPassword, lineP, lblR, cboRole, btnLogin, btnExit, lblError})

        ' Rounded input boxes and buttons
        Functions.RoundControl(panelCard, 12)
        ' Remove rounding on TextBox to prevent border artifacts; use underline style instead
        Functions.RoundControl(btnLogin, 8)
        Functions.RoundControl(btnExit, 8)

        ' Keyboard shortcuts for better UX
        Me.AcceptButton = btnLogin
        Me.CancelButton = btnExit

        AddHandler btnLogin.Click, AddressOf Me.BtnLogin_Click
        AddHandler btnExit.Click, Sub() Me.Close()
        AddHandler Me.Paint, AddressOf Me.DrawGradientBackground
    End Sub

    Private Sub DrawGradientBackground(sender As Object, e As PaintEventArgs)
        Dim rect = Me.ClientRectangle
        Using brush As New LinearGradientBrush(rect, Color.FromArgb(44, 62, 80), Color.FromArgb(52, 152, 219), LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(brush, rect)
        End Using
    End Sub

    Private Sub BtnLogin_Click(sender As Object, e As EventArgs)
        lblError.Text = ""
        If String.IsNullOrWhiteSpace(txtUsername.Text) OrElse String.IsNullOrWhiteSpace(txtPassword.Text) OrElse cboRole.SelectedItem Is Nothing Then
            lblError.Text = "Please enter username, password, and role."
            Return
        End If

        Try
            Using con = GetConnection()
                con.Open()
                Dim sql = "SELECT fullname, username, role FROM tbl_users WHERE username=@u AND password=@p AND role=@r"
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@u", txtUsername.Text.Trim())
                    cmd.Parameters.AddWithValue("@p", txtPassword.Text.Trim())
                    cmd.Parameters.AddWithValue("@r", cboRole.SelectedItem.ToString())
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Session.CurrentUser = reader("fullname").ToString()
                            Session.CurrentRole = reader("role").ToString()
                            LogActivity("Login: " & Session.CurrentUser & " (" & Session.CurrentRole & ")")
                            Dim dash As New DashboardForm()
                            dash.Show()
                            Me.Hide()
                        Else
                            lblError.Text = "Invalid credentials."
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            lblError.Text = "Database error: " & ex.Message
        End Try
    End Sub
End Class
