using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.IO;

namespace Umbraco.Web.Install.Steps
{
    internal class FilePermissions : InstallerStep
    {
        public override string Alias
        {
            get { return "filepermissions"; }
        }

        public override string Name
        {
            get { return "Confirm permissions"; }
        }

        public override string UserControl
        {
            get { return SystemDirectories.Install + "/steps/validatepermissions.ascx"; }
        }

        public override bool HideFromNavigation {
          get {
            return true;
          }
        }
        
        public override bool Completed()
        {
            return FilePermissionHelper.RunFilePermissionTestSuite();
        }
    }
}