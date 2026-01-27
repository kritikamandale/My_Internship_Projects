IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[KYCSubmissions]') AND name = 'Status')
BEGIN
    ALTER TABLE KYCSubmissions ADD Status NVARCHAR(20) DEFAULT 'Pending' WITH VALUES;
    PRINT 'Status column added successfully.';
END
ELSE
BEGIN
    PRINT 'Status column already exists.';
END
