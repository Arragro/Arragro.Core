﻿// <auto-generated />
using System;
using Arragro.Core.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Arragro.Core.Identity.Migrations.Sqlite
{
    [DbContext(typeof(ArragroCoreIdentitySqliteContext))]
    [Migration("20190917051650_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("identity")
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Arragro.Core.Identity.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnName("normalized_name")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_asp_net_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("role_name_index");

                    b.ToTable("asp_net_roles");
                });

            modelBuilder.Entity("Arragro.Core.Identity.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnName("email_confirmed");

                    b.Property<bool>("External")
                        .HasColumnName("external");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name")
                        .HasMaxLength(255);

                    b.Property<bool>("IsEnabled")
                        .HasColumnName("is_enabled");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name")
                        .HasMaxLength(255);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnName("lockout_end");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnName("modified_by");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnName("modified_date");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnName("normalized_email")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnName("normalized_user_name")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .HasColumnName("user_name")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_asp_net_users");

                    b.HasIndex("NormalizedEmail")
                        .HasName("email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("user_name_index");

                    b.ToTable("asp_net_users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value");

                    b.Property<Guid>("RoleId")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_role_claims");

                    b.HasIndex("RoleId")
                        .HasName("ix_asp_net_role_claims_role_id");

                    b.ToTable("asp_net_role_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value");

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_user_claims");

                    b.HasIndex("UserId")
                        .HasName("ix_asp_net_user_claims_user_id");

                    b.ToTable("asp_net_user_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnName("provider_display_name");

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_asp_net_user_logins");

                    b.HasIndex("UserId")
                        .HasName("ix_asp_net_user_logins_user_id");

                    b.ToTable("asp_net_user_logins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id");

                    b.Property<Guid>("RoleId")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_asp_net_user_roles");

                    b.HasIndex("RoleId")
                        .HasName("ix_asp_net_user_roles_role_id");

                    b.ToTable("asp_net_user_roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_asp_net_user_tokens");

                    b.ToTable("asp_net_user_tokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Arragro.Core.Identity.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_asp_net_role_claims_asp_net_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Arragro.Core.Identity.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_claims_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Arragro.Core.Identity.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_logins_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Arragro.Core.Identity.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Arragro.Core.Identity.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Arragro.Core.Identity.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_tokens_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
