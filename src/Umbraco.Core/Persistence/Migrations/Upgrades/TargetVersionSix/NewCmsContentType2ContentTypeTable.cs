using Umbraco.Core.Configuration;

namespace Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionSix
{
    [Migration("6.0.0", 4, GlobalSettings.UmbracoMigrationName)]
    public class NewCmsContentType2ContentTypeTable : MigrationBase
    {
        public override void Up()
        {
            Create.Table("cmsContentType2ContentType")
                  .WithColumn("parentContentTypeId").AsInt16().NotNullable()
                  .WithColumn("childContentTypeId").AsInt16().NotNullable();

            Create.PrimaryKey("PK_cmsContentType2ContentType")
                  .OnTable("cmsContentType2ContentType")
                  .Columns(new[] {"parentContentTypeId", "childContentTypeId"});
        }

        public override void Down()
        {
            Delete.PrimaryKey("PK_cmsContentType2ContentType").FromTable("cmsContentType2ContentType");

            Delete.Table("cmsContentType2ContentType");
        }
    }
}