//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace umbraco.DataLayer.SqlHelpers.MySql {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SqlResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SqlResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("umbraco.DataLayer.SqlHelpers.MySql.SqlResources", typeof(SqlResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*******************************************************************************************
        ///
        ///
        ///
        ///
        ///
        ///
        ///
        ///    Umbraco database installation script for MySQL
        /// 
        ///IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT
        /// 
        ///    Database version: 4.8.0.5
        ///    
        ///    Please increment this version number if ANY change is made to this script,
        ///    so compatibility with scripts for other database systems can be verified easily.
        ///    The first 3 digits depict the Umbraco version, t [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Total {
            get {
                return ResourceManager.GetString("Total", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*******************************************************************************************
        ///
        ///    Umbraco database installation script for SQL Server (upgrade from Umbraco 4.0.x)
        /// 
        ///	IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT
        /// 
        ///	//CHANGE:Allan Stegelmann Laustsen
        ///    Database version: 4.1.0.3
        ///    //CHANGE:End
        ///    
        ///    
        ///    Please increment this version number if ANY change is made to this script,
        ///    so compatibility with scripts for other database s [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Version4_1_Upgrade {
            get {
                return ResourceManager.GetString("Version4_1_Upgrade", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*******************************************************************************************
        ///
        ///    Umbraco database installation script for SQL Server (upgrade from Umbraco 4.0.x)
        /// 
        ///	IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT IMPORTANT
        /// 
        ///	//CHANGE:Allan Stegelmann Laustsen
        ///    Database version: 4.1.0.3
        ///    //CHANGE:End
        ///    
        ///    
        ///    Please increment this version number if ANY change is made to this script,
        ///    so compatibility with scripts for other database s [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Version4_Upgrade {
            get {
                return ResourceManager.GetString("Version4_Upgrade", resourceCulture);
            }
        }
    }
}
