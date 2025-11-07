Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D

Module Functions
    ' Utility functions shared across forms for logging, data access,
    ' and common UI styling helpers (rounded controls, DataGridView styling).
    Public Sub LogActivity(action As String)
        Try
            Using con = GetConnection()
                con.Open()
                Dim cmd As New MySqlCommand("INSERT INTO tbl_activity_logs (action, user) VALUES (@action, @user)", con)
                cmd.Parameters.AddWithValue("@action", action)
                cmd.Parameters.AddWithValue("@user", If(Session.CurrentUser, "Unknown"))
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Logging error: " & ex.Message)
        End Try
    End Sub

    ' Fill a DataTable from SQL with optional parameters
    Public Function FillDataTable(sql As String, Optional params As Dictionary(Of String, Object) = Nothing) As DataTable
        Dim dt As New DataTable()
        Using con = GetConnection()
            con.Open()
            Using cmd As New MySqlCommand(sql, con)
                If params IsNot Nothing Then
                    For Each kv In params
                        cmd.Parameters.AddWithValue(kv.Key, kv.Value)
                    Next
                End If
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ' Execute non-query SQL (INSERT/UPDATE/DELETE)
    Public Function ExecNonQuery(sql As String, Optional params As Dictionary(Of String, Object) = Nothing) As Integer
        Using con = GetConnection()
            con.Open()
            Using cmd As New MySqlCommand(sql, con)
                If params IsNot Nothing Then
                    For Each kv In params
                        cmd.Parameters.AddWithValue(kv.Key, kv.Value)
                    Next
                End If
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ' Round control by setting its Region to a rounded rectangle
    Public Sub RoundControl(ctrl As Control, radius As Integer)
        Dim gp As New GraphicsPath()
        Dim rect As Rectangle = New Rectangle(0, 0, ctrl.Width, ctrl.Height)
        Dim d = radius * 2
        gp.AddArc(rect.X, rect.Y, d, d, 180, 90)
        gp.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        gp.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        gp.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        gp.CloseFigure()
        ctrl.Region = New Region(gp)
        AddHandler ctrl.Resize, Sub(s, e)
                                     ' Re-apply on resize
                                     RoundControl(ctrl, radius)
                                 End Sub
    End Sub

    ' Apply consistent styling to DataGridViews (headers, borders, alternating rows, hover)
    Public Sub StyleGrid(dgv As DataGridView)
        dgv.RowHeadersVisible = False
        dgv.EnableHeadersVisualStyles = False
        dgv.BackgroundColor = Color.White
        dgv.GridColor = Color.FromArgb(230, 235, 240)
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgv.ColumnHeadersDefaultCellStyle.Padding = New Padding(10, 6, 10, 6)
        dgv.ColumnHeadersHeight = 38

        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Regular)
        dgv.DefaultCellStyle.Padding = New Padding(8, 4, 8, 4)
        dgv.RowTemplate.Height = 34
        dgv.AllowUserToResizeRows = False

        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250)
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219)
        dgv.DefaultCellStyle.SelectionForeColor = Color.White

        dgv.BorderStyle = BorderStyle.None
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal

        ' Lightweight row hover effect
        Dim alreadyWired As Boolean = False
        If dgv.Tag IsNot Nothing AndAlso TypeOf dgv.Tag Is String Then
            alreadyWired = DirectCast(dgv.Tag, String).Contains("StyledGridWired")
        End If
        If Not alreadyWired Then
            AddHandler dgv.CellMouseEnter, Sub(s As Object, e As DataGridViewCellEventArgs)
                                               If e.RowIndex >= 0 Then
                                                   Dim r = dgv.Rows(e.RowIndex)
                                                   If Not r.Selected Then
                                                       r.DefaultCellStyle.BackColor = Color.FromArgb(237, 244, 255)
                                                   End If
                                               End If
                                           End Sub
            AddHandler dgv.CellMouseLeave, Sub(s As Object, e As DataGridViewCellEventArgs)
                                               If e.RowIndex >= 0 Then
                                                   Dim r = dgv.Rows(e.RowIndex)
                                                   If Not r.Selected Then
                                                       r.DefaultCellStyle.BackColor = Color.Empty ' revert to grid default
                                                   End If
                                               End If
                                           End Sub
            dgv.Tag = If(dgv.Tag Is Nothing, "StyledGridWired", dgv.Tag.ToString() & ";StyledGridWired")
        End If
    End Sub

    ' Apply consistent modern styling to buttons
    Public Sub StyleButton(btn As Button, baseColor As Color)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(baseColor, 0.15F)
        btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(baseColor)
        btn.BackColor = baseColor
        btn.ForeColor = Color.White
        btn.Cursor = Cursors.Hand
        btn.UseCompatibleTextRendering = True
        btn.Font = New Font("Segoe UI", 10, FontStyle.Bold)
    End Sub

    ' Add a consistent Back button to the top-right of a form
    Public Sub AddBackButton(targetForm As Form)
        Dim btnBack As New Button()
        btnBack.Text = "Back"
        btnBack.Width = 100
        btnBack.Height = 36
        btnBack.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnBack.Top = 8
        btnBack.Left = targetForm.ClientSize.Width - btnBack.Width - 20
        StyleButton(btnBack, Color.FromArgb(52, 152, 219))
        RoundControl(btnBack, 8)
        targetForm.Controls.Add(btnBack)
        btnBack.BringToFront()

        AddHandler targetForm.Resize, Sub(s, e)
                                           btnBack.Left = targetForm.ClientSize.Width - btnBack.Width - 20
                                       End Sub
        AddHandler btnBack.Click, Sub(s, e)
                                       Try
                                           LogActivity("Back: " & targetForm.Text)
                                       Catch
                                           ' best-effort logging
                                       End Try
                                       targetForm.Close()
                                   End Sub
    End Sub

    ' Ensure TextBox text is not vertically clipped and uses consistent look
    Public Sub StyleTextBox(tb As TextBox)
        tb.AutoSize = False
        tb.Height = 34
        tb.BorderStyle = BorderStyle.FixedSingle
        tb.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        tb.Margin = New Padding(0)
    End Sub

    ' Modern ComboBox rendering that avoids left-side clipping and adds padding
    Public Sub StyleComboBox(cb As ComboBox)
        cb.DropDownStyle = ComboBoxStyle.DropDownList
        cb.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        cb.RightToLeft = RightToLeft.No
        cb.DrawMode = DrawMode.OwnerDrawFixed
        cb.ItemHeight = 32
        cb.Height = 36
        cb.IntegralHeight = False
        cb.MaxDropDownItems = 10
        cb.FlatStyle = FlatStyle.Standard

        ' Wire once
        Dim t = If(cb.Tag, "")
        If Not t.ToString().Contains("StyledComboWired") Then
            AddHandler cb.DrawItem, AddressOf Combo_DrawItem
            cb.Tag = t & ";StyledComboWired"
        End If
    End Sub

    Private Sub Combo_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim cb = CType(sender, ComboBox)
        e.DrawBackground()
        If e.Index >= 0 AndAlso e.Index < cb.Items.Count Then
            Dim text = cb.GetItemText(cb.Items(e.Index))
            Dim isPlaceholder As Boolean = (e.Index = 0 AndAlso text.ToLower().StartsWith("select "))
            Dim fore = If(isPlaceholder, Color.Gray, If((e.State And DrawItemState.Selected) = DrawItemState.Selected, Color.White, cb.ForeColor))
            Dim back = If((e.State And DrawItemState.Selected) = DrawItemState.Selected, Color.FromArgb(52, 152, 219), cb.BackColor)
            Using backBrush As New SolidBrush(back)
                e.Graphics.FillRectangle(backBrush, e.Bounds)
            End Using
            Dim rect As New Rectangle(e.Bounds.X + 10, e.Bounds.Y, e.Bounds.Width - 20, e.Bounds.Height)
            Dim fontToUse As Font = If(isPlaceholder, New Font(cb.Font, FontStyle.Italic), cb.Font)
            TextRenderer.DrawText(e.Graphics, text, fontToUse, rect, fore, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        End If
        e.DrawFocusRectangle()
    End Sub
End Module
