-- Diagnostic Script
SELECT 'Current Database: ' + DB_NAME();
SELECT 'Columns in KYCSubmissions:';
SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('KYCSubmissions');
