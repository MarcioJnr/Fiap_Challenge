-- =============================================
-- Script de criação do banco de dados
-- FIAP Challenge - Sistema de Gestão Acadêmica
-- =============================================

USE master;
GO

-- Criar banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FiapChallenge')
BEGIN
    CREATE DATABASE FiapChallenge;
END
GO

USE FiapChallenge;
GO

-- =============================================
-- Tabela: Usuarios
-- Descrição: Armazena usuários administradores do sistema
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Email NVARCHAR(100) NOT NULL UNIQUE,
        SenhaHash NVARCHAR(MAX) NOT NULL,
        Role NVARCHAR(50) NOT NULL DEFAULT 'Admin',
        DataCadastro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT CK_Usuarios_Email CHECK (Email LIKE '%@%.%')
    );

    CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
END
GO

-- =============================================
-- Tabela: Alunos
-- Descrição: Armazena informações dos alunos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Alunos')
BEGIN
    CREATE TABLE Alunos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(100) NOT NULL,
        DataNascimento DATETIME2 NOT NULL,
        Cpf NVARCHAR(11) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        SenhaHash NVARCHAR(MAX) NOT NULL,
        DataCadastro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT CK_Alunos_Cpf CHECK (LEN(Cpf) = 11),
        CONSTRAINT CK_Alunos_Email CHECK (Email LIKE '%@%.%'),
        CONSTRAINT CK_Alunos_DataNascimento CHECK (DataNascimento < GETUTCDATE())
    );

    CREATE INDEX IX_Alunos_Cpf ON Alunos(Cpf);
    CREATE INDEX IX_Alunos_Email ON Alunos(Email);
    CREATE INDEX IX_Alunos_Nome ON Alunos(Nome);
END
GO

-- =============================================
-- Tabela: Turmas
-- Descrição: Armazena informações das turmas
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Turmas')
BEGIN
    CREATE TABLE Turmas (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(100) NOT NULL,
        Descricao NVARCHAR(250) NOT NULL,
        DataCadastro DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_Turmas_Nome ON Turmas(Nome);
END
GO

-- =============================================
-- Tabela: Matriculas
-- Descrição: Relacionamento entre alunos e turmas
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Matriculas')
BEGIN
    CREATE TABLE Matriculas (
        Id INT PRIMARY KEY IDENTITY(1,1),
        AlunoId INT NOT NULL,
        TurmaId INT NOT NULL,
        DataMatricula DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Matriculas_Alunos FOREIGN KEY (AlunoId) 
            REFERENCES Alunos(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Matriculas_Turmas FOREIGN KEY (TurmaId) 
            REFERENCES Turmas(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_Matriculas_AlunoTurma UNIQUE (AlunoId, TurmaId)
    );

    CREATE INDEX IX_Matriculas_AlunoId ON Matriculas(AlunoId);
    CREATE INDEX IX_Matriculas_TurmaId ON Matriculas(TurmaId);
END
GO

-- =============================================
-- Dados iniciais
-- =============================================

-- Inserir usuário administrador padrão
-- Email: admin@fiap.com.br
-- Senha: Admin@123
-- Hash gerado com BCrypt (work factor 11)
IF NOT EXISTS (SELECT * FROM Usuarios WHERE Email = 'admin@fiap.com.br')
BEGIN
    INSERT INTO Usuarios (Email, SenhaHash, Role, DataCadastro)
    VALUES (
        'admin@fiap.com.br', 
        '$2a$11$VLlebv3MmaQOKCHKetdnpuoyzrxtnlqgkhyEU318crDlchO8Llcdq', 
        'Admin', 
        '2024-01-01T00:00:00'
    );
END
GO

-- =============================================
-- Views úteis para consultas
-- =============================================

-- View: Alunos com quantidade de turmas matriculadas
IF OBJECT_ID('vw_AlunosComTurmas', 'V') IS NOT NULL
    DROP VIEW vw_AlunosComTurmas;
GO

CREATE VIEW vw_AlunosComTurmas AS
SELECT 
    a.Id,
    a.Nome,
    a.Email,
    a.Cpf,
    a.DataNascimento,
    a.DataCadastro,
    COUNT(m.Id) AS QuantidadeTurmas
FROM Alunos a
LEFT JOIN Matriculas m ON a.Id = m.AlunoId
GROUP BY a.Id, a.Nome, a.Email, a.Cpf, a.DataNascimento, a.DataCadastro;
GO

-- View: Turmas com quantidade de alunos matriculados
IF OBJECT_ID('vw_TurmasComAlunos', 'V') IS NOT NULL
    DROP VIEW vw_TurmasComAlunos;
GO

CREATE VIEW vw_TurmasComAlunos AS
SELECT 
    t.Id,
    t.Nome,
    t.Descricao,
    t.DataCadastro,
    COUNT(m.Id) AS QuantidadeAlunos
FROM Turmas t
LEFT JOIN Matriculas m ON t.Id = m.TurmaId
GROUP BY t.Id, t.Nome, t.Descricao, t.DataCadastro;
GO

-- =============================================
-- Procedures úteis
-- =============================================

-- Procedure: Matricular aluno em turma
IF OBJECT_ID('sp_MatricularAluno', 'P') IS NOT NULL
    DROP PROCEDURE sp_MatricularAluno;
GO

CREATE PROCEDURE sp_MatricularAluno
    @AlunoId INT,
    @TurmaId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar se aluno existe
    IF NOT EXISTS (SELECT 1 FROM Alunos WHERE Id = @AlunoId)
    BEGIN
        RAISERROR('Aluno não encontrado', 16, 1);
        RETURN;
    END
    
    -- Verificar se turma existe
    IF NOT EXISTS (SELECT 1 FROM Turmas WHERE Id = @TurmaId)
    BEGIN
        RAISERROR('Turma não encontrada', 16, 1);
        RETURN;
    END
    
    -- Verificar se já está matriculado
    IF EXISTS (SELECT 1 FROM Matriculas WHERE AlunoId = @AlunoId AND TurmaId = @TurmaId)
    BEGIN
        RAISERROR('Aluno já está matriculado nesta turma', 16, 1);
        RETURN;
    END
    
    -- Inserir matrícula
    INSERT INTO Matriculas (AlunoId, TurmaId, DataMatricula)
    VALUES (@AlunoId, @TurmaId, GETUTCDATE());
    
    SELECT SCOPE_IDENTITY() AS MatriculaId;
END
GO

-- =============================================
-- Consultas úteis para validação
-- =============================================

-- Verificar estrutura das tabelas
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE t.name IN ('Usuarios', 'Alunos', 'Turmas', 'Matriculas')
ORDER BY t.name, c.column_id;

-- Verificar índices criados
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE t.name IN ('Usuarios', 'Alunos', 'Turmas', 'Matriculas')
ORDER BY t.name, i.name;

-- Verificar constraints
SELECT 
    t.name AS TableName,
    c.name AS ConstraintName,
    c.type_desc AS ConstraintType
FROM sys.check_constraints c
INNER JOIN sys.tables t ON c.parent_object_id = t.object_id
WHERE t.name IN ('Usuarios', 'Alunos', 'Turmas', 'Matriculas')
ORDER BY t.name;

PRINT '=============================================';
PRINT 'Banco de dados criado com sucesso!';
PRINT '=============================================';
PRINT 'Usuário admin criado:';
PRINT 'Email: admin@fiap.com.br';
PRINT 'Senha: Admin@123';
PRINT '=============================================';
GO
