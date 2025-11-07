Imports MySql.Data.MySqlClient

Module DbConnection
    Private Function BuildConnString(port As Integer) As String
        Dim pwd As String = Environment.GetEnvironmentVariable("MYSQL_PASSWORD")
        If pwd Is Nothing Then pwd = ""
        Return $"server=localhost;user=root;password={pwd};database=library_db;port={port};"
    End Function

    Public Function GetConnection() As MySqlConnection
        ' If user provides MYSQL_PORT via environment, prefer it
        Dim envPortStr As String = Environment.GetEnvironmentVariable("MYSQL_PORT")
        Dim envPort As Integer
        If Integer.TryParse(envPortStr, envPort) AndAlso envPort > 0 Then
            Return New MySqlConnection(BuildConnString(envPort))
        End If

        ' Probe common WAMP/WAMPP ports: 3306 then 3307
        Dim primary = BuildConnString(3306)
        Try
            Using probe As New MySqlConnection(primary)
                probe.Open()
            End Using
            Return New MySqlConnection(primary)
        Catch
            ' Fall back to 3307 without probing again
            Dim fallback = BuildConnString(3307)
            Return New MySqlConnection(fallback)
        End Try
    End Function
End Module
