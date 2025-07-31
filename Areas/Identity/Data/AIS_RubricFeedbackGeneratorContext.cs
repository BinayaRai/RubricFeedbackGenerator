using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using AIS_RubricFeedbackGenerator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AIS_RubricFeedbackGenerator.Data;

public class AIS_RubricFeedbackGeneratorContext : IdentityDbContext<AIS_RubricFeedbackGeneratorUser>
{
    public AIS_RubricFeedbackGeneratorContext(DbContextOptions<AIS_RubricFeedbackGeneratorContext> options)
        : base(options)
    {
    }
    public DbSet<Student> Students { get; set; }
    public DbSet<Models.Task> Tasks { get; set; }
    public DbSet<Rubric> Rubrics { get; set; }
    public DbSet<ScoreDefinition> ScoreDefinitions { get; set; }
    public DbSet<Criterion> Criteria { get; set; }
    public DbSet<ScoreLevel> ScoreLevels { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId);

            entity.Property(e => e.TaskId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.TotalMark)
                .HasPrecision(4, 1)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(450); // Match Identity UserId type

            entity.HasOne(e => e.CreatedByNavigation)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Task_User");
        });


        builder.Entity<Rubric>(entity =>
        {
            entity.HasKey(e => e.RubricId);
            entity.Property(e => e.RubricId)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.TaskId)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(e => e.Description)
                .IsUnicode(false);
            entity.Property(e => e.TotalMark)
                .HasPrecision(4, 1)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.CreatedByNavigation)
                .WithMany(u => u.Rubrics)
                .HasForeignKey(e => e.CreatedBy)
                .HasConstraintName("FK_Rubric_User");

            entity.HasOne(t => t.Task)
                .WithMany(e => e.Rubrics)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Rubric_Task");
        });

        builder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId);

            entity.Property(e => e.StudentId)
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.CreatedByNavigation)
                .WithMany(u => u.Students)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Students_Users");
        });

        builder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.CriterionId);

            entity.Property(e => e.CriterionId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.RubricId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(c => c.Rubric)
                .WithMany(r => r.Criteria)
                .HasForeignKey(c => c.RubricId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ScoreLevel>(entity =>
        {
            entity.HasKey(e => e.ScoreLevelId);

            entity.Property(e => e.ScoreLevelId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CriterionId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ScoreDefinitionId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Description)
                .IsRequired()
                .IsUnicode(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Criterion)
                .WithMany(c => c.ScoreLevels)
                .HasForeignKey(e => e.CriterionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ScoreLevel_Criterion");

            entity.HasOne(e => e.ScoreDefinition)
                .WithMany(sd => sd.ScoreLevels)
                .HasForeignKey(e => e.ScoreDefinitionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ScoreLevel_ScoreDefinition");
        });

        builder.Entity<ScoreDefinition>(entity =>
        {
            entity.HasKey(e => e.ScoreDefinitionId);

            entity.Property(e => e.ScoreDefinitionId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.RubricId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ScoreValue)
                .HasPrecision(4, 1) 
                .IsRequired();

            entity.Property(e => e.ScoreName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Rubric)
                .WithMany(r => r.ScoreDefinitions)
                .HasForeignKey(e => e.RubricId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ScoreDefinition_Rubric");

            entity.HasIndex(e => new { e.RubricId, e.ScoreValue }).IsUnique();
        });

    }
}
