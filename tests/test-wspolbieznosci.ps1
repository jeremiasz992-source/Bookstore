# Test wspolbieznego skladania zamowien na ostatni egzemplarz ksiazki.
# Wymaga dzialajacego srodowiska kontenerowego (docker compose up -d).

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Add-Type -AssemblyName System.Net.Http

$base    = "http://localhost:5172"
$bookId  = 9           # Zaginiony Manuskrypt
$klienci = 12          # liczba jednoczesnych zadan
$haslo   = "Test1234!"

# Haslo administratora jest podawane przy uruchomieniu i nie trafia do pliku.
$adminEmail = Read-Host "Adres e-mail administratora"
$adminPassword = Read-Host "Haslo administratora" -AsSecureString
$adminCredentials = [System.Net.NetworkCredential]::new($adminEmail, $adminPassword)
try {
    $adminBody = @{ email = $adminEmail; password = $adminCredentials.Password } | ConvertTo-Json
    $adminToken = (Invoke-RestMethod "$base/api/Auth/login" -Method Post -ContentType "application/json" -Body $adminBody -ErrorAction Stop).token
    Invoke-RestMethod "$base/api/Orders/all" -Headers @{ Authorization = "Bearer $adminToken" } -ErrorAction Stop | Out-Null
} catch {
    throw "Nie udalo sie zalogowac na konto administratora lub potwierdzic jego uprawnien."
} finally {
    $adminBody = $null
    $adminCredentials = $null
    $adminPassword.Dispose()
}

function Get-Token($email, $password) {
    $body = @{ email = $email; password = $password } | ConvertTo-Json
    try {
        return (Invoke-RestMethod "$base/api/Auth/register" -Method Post -ContentType "application/json" -Body $body).token
    } catch {
        return (Invoke-RestMethod "$base/api/Auth/login" -Method Post -ContentType "application/json" -Body $body).token
    }
}

function Get-Stock($id) {
    $lista = Invoke-RestMethod "$base/api/Books?page=1&pageSize=100"
    if ($lista.items) { $lista = $lista.items }
    return ($lista | Where-Object { $_.bookId -eq $id }).stock
}

Write-Host ""
Write-Host "1. Przygotowanie kont klientow i koszykow" -ForegroundColor Cyan

$tokeny = @()
for ($i = 1; $i -le $klienci; $i++) {
    $t = Get-Token "klient$i@example.com" $haslo
    $naglowki = @{ Authorization = "Bearer $t" }
    Invoke-RestMethod "$base/api/Cart" -Method Delete -Headers $naglowki | Out-Null
    Invoke-RestMethod "$base/api/Cart/items" -Method Post -ContentType "application/json" `
        -Headers $naglowki -Body (@{ bookId = $bookId; quantity = 1 } | ConvertTo-Json) | Out-Null
    $tokeny += $t
}

Write-Host "   Przygotowano $klienci koszykow zawierajacych ksiazke nr $bookId."
Write-Host "   Biezacy stan magazynowy: $(Get-Stock $bookId)"
Write-Host ""
Read-Host "   Ustaw teraz stan magazynowy tej ksiazki na 1 w panelu administratora, potem nacisnij Enter"

$stanPrzed = Get-Stock $bookId

Write-Host ""
Write-Host "2. Wysylanie $klienci jednoczesnych zadan zlozenia zamowienia" -ForegroundColor Cyan
Write-Host "   Stan magazynowy przed testem: $stanPrzed"

$polaczenia = @()
$zadania    = @()
foreach ($t in $tokeny) {
    $c = New-Object System.Net.Http.HttpClient
    $c.DefaultRequestHeaders.Authorization =
        New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $t)
    $polaczenia += $c
    $tresc = New-Object System.Net.Http.StringContent("", [System.Text.Encoding]::UTF8, "application/json")
    $zadania += $c.PostAsync("$base/api/Orders/checkout", $tresc)
}

[System.Threading.Tasks.Task]::WaitAll($zadania)

$wyniki = foreach ($z in $zadania) {
    $o = $z.Result
    [pscustomobject]@{
        Kod   = [int]$o.StatusCode
        Tresc = ($o.Content.ReadAsStringAsync().Result -replace '\s+', ' ')
    }
}
$polaczenia | ForEach-Object { $_.Dispose() }

Write-Host ""
Write-Host "3. Wyniki" -ForegroundColor Cyan
$wyniki | Group-Object Kod | Sort-Object Name |
    Select-Object @{ n = 'Kod odpowiedzi'; e = { $_.Name } }, @{ n = 'Liczba żądań'; e = { $_.Count } } |
    Format-Table -AutoSize

foreach ($g in ($wyniki | Group-Object Kod | Sort-Object Name)) {
    $p = $g.Group[0].Tresc
    if ($p.Length -gt 110) { $p = $p.Substring(0, 110) + "..." }
    Write-Host ("   {0}: {1}" -f $g.Name, $p)
}

$wszystkie  = Invoke-RestMethod "$base/api/Orders/all" -Headers @{ Authorization = "Bearer $adminToken" }
$powstale   = @($wszystkie | Where-Object { @($_.items.bookId) -contains $bookId }).Count

Write-Host ""
Write-Host ("   Stan magazynowy przed testem:          {0}" -f $stanPrzed)
Write-Host ("   Stan magazynowy po teście:             {0}" -f (Get-Stock $bookId))
Write-Host ("   Zamówień zawierających tę książkę:     {0}" -f $powstale)
Write-Host ""
