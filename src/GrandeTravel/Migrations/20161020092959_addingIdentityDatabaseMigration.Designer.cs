using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GrandeTravel.Services;

namespace GrandeTravel.Migrations
{
    [DbContext(typeof(DbContextService))]
    [Migration("20161020092959_addingIdentityDatabaseMigration")]
    partial class addingIdentityDatabaseMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GrandeTravel.Models.Package", b =>
                {
                    b.Property<int>("PackageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Location");

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<int>("ProviderId");

                    b.Property<string>("ThumbnailUrl");

                    b.HasKey("PackageId");

                    b.HasIndex("ProviderId");

                    b.ToTable("tblPackage");
                });

            modelBuilder.Entity("GrandeTravel.Models.Provider", b =>
                {
                    b.Property<int>("ProviderId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ABN");

                    b.Property<string>("DisplayName");

                    b.Property<int>("ProviderUserId");

                    b.HasKey("ProviderId");

                    b.ToTable("tblProvider");
                });

            modelBuilder.Entity("GrandeTravel.Models.Package", b =>
                {
                    b.HasOne("GrandeTravel.Models.Provider", "Provider")
                        .WithMany("Packages")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
