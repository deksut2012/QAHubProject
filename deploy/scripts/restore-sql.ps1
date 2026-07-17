param([Parameter(Mandatory)][string]$BackupName, [string]$Container = "qahub-sqlserver", [string]$Database = "QAHub_RestoreTest")
$ErrorActionPreference = "Stop"
if ($BackupName -notmatch '^[A-Za-z0-9_.-]+\.bak$') { throw "Invalid backup filename." }
docker exec $Container bash -c "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P \"`$MSSQL_SA_PASSWORD\" -C -Q \"RESTORE VERIFYONLY FROM DISK='/var/opt/mssql/backup/$BackupName' WITH CHECKSUM\""
if ($LASTEXITCODE) { throw "Backup verification failed." }
Write-Output "Backup verified. Restore to $Database requires an approved maintenance window and explicit file mapping."
