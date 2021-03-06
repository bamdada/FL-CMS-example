using System;
using Umbraco.Core.Configuration;

namespace Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionSix
{
    [Migration("6.0.0", 9, GlobalSettings.UmbracoMigrationName)]
    public class EnsureAppsTreesUpdated : MigrationBase
    {
        public override void Up()
        {
            var e = new UpgradingEventArgs();

            if (Upgrading != null)
                Upgrading(this, e);
        }

        public override void Down()
        {
        }

        public static event EventHandler<UpgradingEventArgs> Upgrading;

        public class UpgradingEventArgs : EventArgs{}
    }
}