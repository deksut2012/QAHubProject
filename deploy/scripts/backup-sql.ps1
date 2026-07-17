param([string]$Container = "qahub-sqlserver", [string]$Database = "QAHub")
$ErrorActionPreference = "Stop"
$backupName = "${Database}_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"
docker exec $Container bash -c "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P \"`$MSSQL_SA_PASSWORD\" -C -Q \"BACKUP DATABASE [$Database] TO DISK='/var/opt/mssql/backup/$backupName' WITH COPY_ONLY, CHECKSUM\""
if ($LASTEXITCODE) { throw "Backup failed." }
Write-Output "Backup created inside persistent SQL volume: $backupName"
