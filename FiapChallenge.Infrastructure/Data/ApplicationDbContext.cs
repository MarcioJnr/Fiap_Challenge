using System;
using System.Collections.Generic;
using FiapChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapChallenge.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aluno> Alunos { get; set; }

    public virtual DbSet<Matricula> Matriculas { get; set; }

    public virtual DbSet<Turma> Turmas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VwAlunosComTurma> VwAlunosComTurmas { get; set; }

    public virtual DbSet<VwTurmasComAluno> VwTurmasComAlunos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Alunos__3214EC07FD9A4B27");

            entity.HasIndex(e => e.Cpf, "IX_Alunos_Cpf");

            entity.HasIndex(e => e.Email, "IX_Alunos_Email");

            entity.HasIndex(e => e.Nome, "IX_Alunos_Nome");

            entity.HasIndex(e => e.Email, "UQ__Alunos__A9D105345A6834D4").IsUnique();

            entity.HasIndex(e => e.Cpf, "UQ__Alunos__C1FF9309237D3769").IsUnique();

            entity.Property(e => e.Cpf).HasMaxLength(11);
            entity.Property(e => e.DataCadastro).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Matricul__3214EC077BC8C7FE");

            entity.HasIndex(e => e.AlunoId, "IX_Matriculas_AlunoId");

            entity.HasIndex(e => e.TurmaId, "IX_Matriculas_TurmaId");

            entity.HasIndex(e => new { e.AlunoId, e.TurmaId }, "UQ_Matriculas_AlunoTurma").IsUnique();

            entity.Property(e => e.DataMatricula).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Aluno).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.AlunoId)
                .HasConstraintName("FK_Matriculas_Alunos");

            entity.HasOne(d => d.Turma).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.TurmaId)
                .HasConstraintName("FK_Matriculas_Turmas");
        });

        modelBuilder.Entity<Turma>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Turmas__3214EC0728B4950D");

            entity.HasIndex(e => e.Nome, "IX_Turmas_Nome");

            entity.Property(e => e.DataCadastro).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Descricao).HasMaxLength(250);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07ABFCC459");

            entity.HasIndex(e => e.Email, "IX_Usuarios_Email");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D10534EB495EB1").IsUnique();

            entity.Property(e => e.DataCadastro).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Admin");
        });

        modelBuilder.Entity<VwAlunosComTurma>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AlunosComTurmas");

            entity.Property(e => e.Cpf).HasMaxLength(11);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<VwTurmasComAluno>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TurmasComAlunos");

            entity.Property(e => e.Descricao).HasMaxLength(250);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
