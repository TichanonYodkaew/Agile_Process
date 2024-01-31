﻿// <auto-generated />
using System;
using AgileRap_Process2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AgileRap_Process2.Migrations
{
    [DbContext(typeof(AgileRap_Process2Context))]
    partial class AgileRap_Process2ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AgileRap_Process.Models.Provider", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.Property<int?>("WorkID")
                        .HasColumnType("int");

                    b.Property<int?>("WorkLogID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.HasIndex("WorkID");

                    b.HasIndex("WorkLogID");

                    b.ToTable("Provider");
                });

            modelBuilder.Entity("AgileRap_Process.Models.ProviderLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.Property<int?>("WorkLogID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("ProviderLog");
                });

            modelBuilder.Entity("AgileRap_Process.Models.Status", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("StatusName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("AgileRap_Process.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("LineID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("AgileRap_Process.Models.Work", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("HeadID")
                        .HasColumnType("int");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Project")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StatusID")
                        .HasColumnType("int");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("StatusID");

                    b.ToTable("Work");
                });

            modelBuilder.Entity("AgileRap_Process.Models.WorkLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("CreateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("No")
                        .HasColumnType("int");

                    b.Property<string>("Project")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StatusID")
                        .HasColumnType("int");

                    b.Property<int?>("UpdateBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("WorkID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("StatusID");

                    b.HasIndex("WorkID");

                    b.ToTable("WorkLog");
                });

            modelBuilder.Entity("AgileRap_Process.Models.Provider", b =>
                {
                    b.HasOne("AgileRap_Process.Models.User", null)
                        .WithMany("Provider")
                        .HasForeignKey("UserID");

                    b.HasOne("AgileRap_Process.Models.Work", null)
                        .WithMany("Provider")
                        .HasForeignKey("WorkID");

                    b.HasOne("AgileRap_Process.Models.WorkLog", null)
                        .WithMany("ProviderLog")
                        .HasForeignKey("WorkLogID");
                });

            modelBuilder.Entity("AgileRap_Process.Models.ProviderLog", b =>
                {
                    b.HasOne("AgileRap_Process.Models.User", null)
                        .WithMany("ProviderLog")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("AgileRap_Process.Models.Work", b =>
                {
                    b.HasOne("AgileRap_Process.Models.Status", null)
                        .WithMany("Work")
                        .HasForeignKey("StatusID");
                });

            modelBuilder.Entity("AgileRap_Process.Models.WorkLog", b =>
                {
                    b.HasOne("AgileRap_Process.Models.Status", null)
                        .WithMany("WorkLog")
                        .HasForeignKey("StatusID");

                    b.HasOne("AgileRap_Process.Models.Work", null)
                        .WithMany("WorkLogs")
                        .HasForeignKey("WorkID");
                });

            modelBuilder.Entity("AgileRap_Process.Models.Status", b =>
                {
                    b.Navigation("Work");

                    b.Navigation("WorkLog");
                });

            modelBuilder.Entity("AgileRap_Process.Models.User", b =>
                {
                    b.Navigation("Provider");

                    b.Navigation("ProviderLog");
                });

            modelBuilder.Entity("AgileRap_Process.Models.Work", b =>
                {
                    b.Navigation("Provider");

                    b.Navigation("WorkLogs");
                });

            modelBuilder.Entity("AgileRap_Process.Models.WorkLog", b =>
                {
                    b.Navigation("ProviderLog");
                });
#pragma warning restore 612, 618
        }
    }
}
