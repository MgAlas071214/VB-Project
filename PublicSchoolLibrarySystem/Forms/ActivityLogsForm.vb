Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class ActivityLogsForm
    Inherits Form

    Public dgvLogs As DataGridView

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Activity Logs"
        Me.Size = New Size(900, 600)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.White

        Dim header As New Label() With {.Text = "Activity Logs", .Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold), .Padding = New Padding(15, 0, 0, 0)}
        Me.Controls.Add(header)

        dgvLogs = New DataGridView() With {.Left = 20, .Top = 80, .Width = 840, .Height = 480, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom}
        Me.Controls.Add(dgvLogs)
        ' Back button in upper-right
        Functions.AddBackButton(Me)

        AddHandler Me.Load, AddressOf ActivityLogsForm_Load
    End Sub

    Private Sub ActivityLogsForm_Load(sender As Object, e As EventArgs)
        If Session.CurrentRole <> "Admin" Then
            MessageBox.Show("Access denied. Admins only.")
            Me.Close()
            Return
        End If
        Try
            Using con = DbConnection.GetConnection()
                con.Open()
                Dim sql = "SELECT log_id, action, user, log_date FROM tbl_activity_logs ORDER BY log_date DESC"
                Using da As New MySqlDataAdapter(sql, con)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvLogs.DataSource = dt
                    If dgvLogs.Columns.Contains("log_id") Then dgvLogs.Columns("log_id").Visible = False
                End Using
            End Using
            Functions.StyleGrid(dgvLogs)
        Catch ex As Exception
            MessageBox.Show("Error loading logs: " & ex.Message)
        End Try
    End Sub
End Class
