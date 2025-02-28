﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vertr.Adapters.DataAccess;

#nullable disable

namespace Vertr.Adapters.DataAccess.Migrations
{
    [DbContext(typeof(VertrDbContext))]
    partial class VertrDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Vertr.Domain.Operation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("account_id");

                    b.Property<Guid?>("AssetUid")
                        .HasColumnType("uuid")
                        .HasColumnName("asset_uid");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<string>("InstrumentType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("instrument_type");

                    b.Property<Guid?>("InstrumentUid")
                        .HasColumnType("uuid")
                        .HasColumnName("instrument_uid");

                    b.Property<int>("OperationType")
                        .HasColumnType("integer")
                        .HasColumnName("operation_type");

                    b.Property<Guid?>("ParentOperationId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_operation_id");

                    b.Property<decimal>("Payment")
                        .HasColumnType("numeric")
                        .HasColumnName("payment");

                    b.Property<Guid>("PositionUid")
                        .HasColumnType("uuid")
                        .HasColumnName("position_uid");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.Property<long>("Quantity")
                        .HasColumnType("bigint")
                        .HasColumnName("quantity");

                    b.Property<long>("QuantityRest")
                        .HasColumnType("bigint")
                        .HasColumnName("quantity_rest");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("tinvest_operations_pkey");

                    b.ToTable("tinvest_operations", (string)null);
                });

            modelBuilder.Entity("Vertr.Domain.PortfolioPosition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("AveragePositionPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("average_position_price");

                    b.Property<decimal>("AveragePositionPriceFifo")
                        .HasColumnType("numeric")
                        .HasColumnName("average_position_price_fifo");

                    b.Property<bool>("Blocked")
                        .HasColumnType("boolean")
                        .HasColumnName("blocked");

                    b.Property<decimal>("BlockedLots")
                        .HasColumnType("numeric")
                        .HasColumnName("blocked_lots");

                    b.Property<decimal>("CurrentNkd")
                        .HasColumnType("numeric")
                        .HasColumnName("current_nkd");

                    b.Property<decimal>("CurrentPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("current_price");

                    b.Property<decimal>("ExpectedYield")
                        .HasColumnType("numeric")
                        .HasColumnName("expected_yield");

                    b.Property<decimal>("ExpectedYieldFifo")
                        .HasColumnType("numeric")
                        .HasColumnName("expected_yield_fifo");

                    b.Property<string>("InstrumentType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("instrument_type");

                    b.Property<Guid>("InstrumentUid")
                        .HasColumnType("uuid")
                        .HasColumnName("instrument_uid");

                    b.Property<Guid>("PortfolioSnapshotId")
                        .HasColumnType("uuid")
                        .HasColumnName("portfolio_snapshot_id");

                    b.Property<Guid>("PositionUid")
                        .HasColumnType("uuid")
                        .HasColumnName("position_uid");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric")
                        .HasColumnName("quantity");

                    b.Property<decimal>("VarMargin")
                        .HasColumnType("numeric")
                        .HasColumnName("var_margin");

                    b.HasKey("Id")
                        .HasName("tinvest_portfolio_positions_pkey");

                    b.HasIndex("PortfolioSnapshotId");

                    b.ToTable("tinvest_portfolio_positions", (string)null);
                });

            modelBuilder.Entity("Vertr.Domain.PortfolioSnapshot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("account_id");

                    b.Property<decimal>("ExpectedYield")
                        .HasColumnType("numeric")
                        .HasColumnName("expected_yield");

                    b.Property<DateTime>("TimeUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_utc");

                    b.Property<decimal>("TotalAmountBonds")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_bonds");

                    b.Property<decimal>("TotalAmountCurrencies")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_currencies");

                    b.Property<decimal>("TotalAmountEtf")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_etf");

                    b.Property<decimal>("TotalAmountFutures")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_futures");

                    b.Property<decimal>("TotalAmountOptions")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_options");

                    b.Property<decimal>("TotalAmountPortfolio")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_portfolio");

                    b.Property<decimal>("TotalAmountShares")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_shares");

                    b.Property<decimal>("TotalAmountSp")
                        .HasColumnType("numeric")
                        .HasColumnName("total_amount_sp");

                    b.HasKey("Id")
                        .HasName("tinvest_portfolio_snapshots_pkey");

                    b.ToTable("tinvest_portfolio_snapshots", (string)null);
                });

            modelBuilder.Entity("Vertr.Domain.PortfolioPosition", b =>
                {
                    b.HasOne("Vertr.Domain.PortfolioSnapshot", "PortfolioSnapshot")
                        .WithMany("Positions")
                        .HasForeignKey("PortfolioSnapshotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("tinvest_portfolio_position_snapshot_fk");

                    b.Navigation("PortfolioSnapshot");
                });

            modelBuilder.Entity("Vertr.Domain.PortfolioSnapshot", b =>
                {
                    b.Navigation("Positions");
                });
#pragma warning restore 612, 618
        }
    }
}
