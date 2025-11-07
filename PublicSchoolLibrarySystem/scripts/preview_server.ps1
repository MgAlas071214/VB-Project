param(
    [string]$Port = "8765"
)

$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("http://localhost:$Port/")
$listener.Start()
Write-Host "Preview server listening at http://localhost:$Port/"

while ($true) {
    $context = $listener.GetContext()
    $response = $context.Response
    $html = @"
<html>
  <head>
    <title>PSLBS UI Preview</title>
    <style>
      body{font-family:Segoe UI,Arial,sans-serif;margin:40px;}
      .card{padding:20px;border:1px solid #e6eaef;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,0.05);} 
      h1{color:#2c3e50;} p{color:#34495e;} code{background:#f6f8fa;padding:2px 6px;border-radius:4px;}
    </style>
  </head>
  <body>
    <div class='card'>
      <h1>PSLBS UI Preview</h1>
      <p>The app is launched. Open the Books tab to verify seeded records are present.</p>
      <p>Seeding runs once when the table is empty.</p>
      <p>Process: <code>PublicSchoolLibrarySystem.exe</code></p>
    </div>
  </body>
</html>
"@

    $buffer = [Text.Encoding]::UTF8.GetBytes($html)
    $response.ContentLength64 = $buffer.Length
    $response.OutputStream.Write($buffer, 0, $buffer.Length)
    $response.OutputStream.Close()
}
