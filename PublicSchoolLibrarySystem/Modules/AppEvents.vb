Imports System
Imports System.Windows.Forms

Public NotInheritable Class AppEvents
    ' Central event hub for broadcast-style notifications.
    Public Shared Event StatsChanged As EventHandler

    Public Shared Sub RaiseStatsChanged()
        RaiseEvent StatsChanged(Nothing, EventArgs.Empty)
    End Sub
End Class

