Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class ReportsForm
    Inherits Form

    Private lblBooks As Label
    Private lblStudents As Label
    Private lblBorrowed As Label
    Private lblReturned As Label
    Private btnExportCsv As Button

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Reports"
        Me.Size = New Size(600, 400)
        Me.Font = New Font("Segoe UI", 11)
        Me.BackColor = Color.White

        Dim header As New Label() With {.Text = "Reports", .Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .TextAlign = ContentAlignment.MiddleLeft, .Font = New Font("Segoe UI", 14, FontStyle.Bold), .Padding = New Padding(15, 0, 0, 0)}
        Me.Controls.Add(header)
        ' Back button in upper-right
        Functions.AddBackButton(Me)

        lblBooks = New Label() With {.Text = "Books: 0", .Left = 30, .Top = 90, .Width = 200, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}
        lblStudents = New Label() With {.Text = "Students: 0", .Left = 30, .Top = 130, .Width = 200, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}
        lblBorrowed = New Label() With {.Text = "Borrowed: 0", .Left = 30, .Top = 170, .Width = 200, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}
        lblReturned = New Label() With {.Text = "Returned: 0", .Left = 30, .Top = 210, .Width = 200, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}
        btnExportCsv = New Button() With {.Text = "Export CSV", .Left = 30, .Top = 260, .Width = 140, .Height = 40, .BackColor = Color.FromArgb(52, 152, 219), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left}

        Me.Controls.AddRange(New Control() {lblBooks, lblStudents, lblBorrowed, lblReturned, btnExportCsv})

        Functions.RoundControl(btnExportCsv, 8)
        Functions.StyleButton(btnExportCsv, Color.FromArgb(52, 152, 219))

        AddHandler Me.Load, AddressOf ReportsForm_Load
        AddHandler btnExportCsv.Click, AddressOf BtnExportCsv_Click
    End Sub

    Private Sub ReportsForm_Load(sender As Object, e As EventArgs)
        Try
            lblBooks.Text = "Books: " & Count("SELECT COUNT(*) FROM tbl_books")
            lblStudents.Text = "Students: " & Count("SELECT COUNT(*) FROM tbl_students")
            lblBorrowed.Text = "Borrowed: " & Count("SELECT COUNT(*) FROM tbl_borrowed WHERE status='Borrowed'")
            lblReturned.Text = "Returned: " & Count("SELECT COUNT(*) FROM tbl_borrowed WHERE status='Returned'")
        Catch ex As Exception
            MessageBox.Show("Error loading reports: " & ex.Message)
        End Try
    End Sub

    Private Function Count(sql As String) As Integer
        Using con = DbConnection.GetConnection()
            con.Open()
            Using cmd As New MySqlCommand(sql, con)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    Private Sub BtnExportCsv_Click(sender As Object, e As EventArgs)
        Try
            Dim sfd As New SaveFileDialog()
            sfd.Filter = "CSV Files (*.csv)|*.csv"
            sfd.FileName = "library_reports.csv"
            If sfd.ShowDialog() = DialogResult.OK Then
                Using sw As New StreamWriter(sfd.FileName)
                    sw.WriteLine("Metric,Count")
                    sw.WriteLine("Books," & Count("SELECT COUNT(*) FROM tbl_books"))
                    sw.WriteLine("Students," & Count("SELECT COUNT(*) FROM tbl_students"))
                    sw.WriteLine("Borrowed," & Count("SELECT COUNT(*) FROM tbl_borrowed WHERE status='Borrowed'"))
                    sw.WriteLine("Returned," & Count("SELECT COUNT(*) FROM tbl_borrowed WHERE status='Returned'"))
                End Using
                MessageBox.Show("Exported: " & sfd.FileName)
                Functions.LogActivity("Exported reports to CSV")
            End If
        Catch ex As Exception
            MessageBox.Show("Export error: " & ex.Message)
        End Try
    End Sub
End Class
