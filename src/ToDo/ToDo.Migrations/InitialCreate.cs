using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace ToDo.Migrations
{
    [Migration(1)]
    public class InitialCreate : Migration
    {
        public override void Up()
        {
            Create.Table("User")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("Email").AsString(128).NotNullable()
                .WithColumn("EncryptedPassword").AsBinary(32).NotNullable()
                .WithColumn("Salt").AsBinary(32).NotNullable();

            Create.Table("ToDo")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("UserId").AsGuid().ForeignKey().References("FK_ToDo_User", "User", new[] {"Id"}).NotNullable()
                .WithColumn("Text").AsString(1024).NotNullable()
                .WithColumn("Added").AsDateTime().NotNullable()
                .WithColumn("Done").AsDateTime().Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
