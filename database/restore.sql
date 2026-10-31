IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DbQLBH_65132292')
BEGIN
    PRINT 'Restoring database DbQLBH_65132292 from backup...';
    BEGIN TRY
        RESTORE DATABASE [DbQLBH_65132292]
        FROM DISK = '/database/DbQLBH_65132292.bak'
        WITH MOVE 'DbQLBH_65132292' TO '/var/opt/mssql/data/DbQLBH_65132292.mdf',
             MOVE 'DbQLBH_65132292_log' TO '/var/opt/mssql/data/DbQLBH_65132292_log.ldf',
             REPLACE;
        PRINT 'Database DbQLBH_65132292 restored successfully from .bak.';
    END TRY
    BEGIN CATCH
        PRINT 'Restore from backup failed: ' + ERROR_MESSAGE();
        PRINT 'Falling back to creating schema from init.sql...';
    END CATCH
END
ELSE
BEGIN
    PRINT 'Database DbQLBH_65132292 already exists. Skipping restore.';
END
GO
