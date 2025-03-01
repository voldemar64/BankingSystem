using FluentMigrator;

namespace Infrastructure.DataAccess.Migrations;

[Migration(1, "Initial")]
public class Initial : Migration
{
    public override void Up()
    {
        Create.Table("accounts")
            .WithColumn("account_number")
            .AsString(50)
            .PrimaryKey()
            .WithColumn("pin")
            .AsString(50)
            .NotNullable()
            .WithColumn("balance")
            .AsDecimal(18, 2)
            .NotNullable();

        Create.Table("operations")
            .WithColumn("id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("account_number")
            .AsString(50)
            .NotNullable()
            .WithColumn("type")
            .AsString(20)
            .NotNullable()
            .WithColumn("amount")
            .AsDecimal(18, 2)
            .NotNullable()
            .WithColumn("timestamp")
            .AsDateTime()
            .NotNullable();

        Create.ForeignKey("fk_operations_accounts")
            .FromTable("operations")
            .ForeignColumn("account_number")
            .ToTable("accounts")
            .PrimaryColumn("account_number");

        Create.Table("admins")
            .WithColumn("id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("password")
            .AsString(100)
            .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("operations");
        Delete.Table("accounts");
        Delete.Table("admins");
    }
}