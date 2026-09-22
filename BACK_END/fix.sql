USE [Amit_Test]
GO
UPDATE tblUsersMaster 
SET PasswordHash = '$2a$11$TZYxofXbph4adhPh2uf2wOlynbR8sLC0rjJC/jeII7DIzdjJto67q' 
WHERE Email = 'admin@company.com';
GO
