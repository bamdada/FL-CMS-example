using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.PublishedCache.XmlPublishedCache;
using umbraco;

namespace Umbraco.Tests
{

	/// <summary>
	/// Tests for the legacy library class
	/// </summary>
	[TestFixture]
	public class LibraryTests : BaseRoutingTest
	{
		public override void Initialize()
		{            
			base.Initialize();

			var routingContext = GetRoutingContext("/test", 1234);
			UmbracoContext.Current = routingContext.UmbracoContext;

            var currDir = new DirectoryInfo(TestHelper.CurrentAssemblyDirectory);

            var configPath = Path.Combine(currDir.Parent.Parent.FullName, "config");
            if (Directory.Exists(configPath) == false)
                Directory.CreateDirectory(configPath);

            var umbracoSettingsFile = Path.Combine(currDir.Parent.Parent.FullName, "config", "umbracoSettings.config");
            if (File.Exists(umbracoSettingsFile) == false)
                File.Copy(
                    currDir.Parent.Parent.Parent.GetDirectories("Umbraco.Web.UI")
                        .First()
                        .GetDirectories("config").First()
                        .GetFiles("umbracoSettings.Release.config").First().FullName,
                    Path.Combine(currDir.Parent.Parent.FullName, "config", "umbracoSettings.config"),
                    true);

            Core.Configuration.UmbracoSettings.SettingsFilePath = Core.IO.IOHelper.MapPath(Core.IO.SystemDirectories.Config + Path.DirectorySeparatorChar, false);
		}

		public override void TearDown()
		{
            //TODO: Deleting the umbracoSettings.config file makes a lot of tests fail

            //var currDir = new DirectoryInfo(TestHelper.CurrentAssemblyDirectory);

            //var umbracoSettingsFile = Path.Combine(currDir.Parent.Parent.FullName, "config", "umbracoSettings.config");
            //if (File.Exists(umbracoSettingsFile))
            //    File.Delete(umbracoSettingsFile);
            
			base.TearDown();
			UmbracoContext.Current = null;
		}

        protected override DatabaseBehavior DatabaseTestBehavior
        {
            get { return DatabaseBehavior.NoDatabasePerFixture; }
        }

		[Test]
		public void Get_Item_User_Property()
		{
			var val = library.GetItem(1173, "content");
			var legacyVal = LegacyGetItem(1173, "content");
			Assert.AreEqual(legacyVal, val);
			Assert.AreEqual("<div>This is some content</div>", val);
		}

		[Test]
		public void Get_Item_Document_Property()
		{
			//first test a single static val
			var val = library.GetItem(1173, "template");
			var legacyVal = LegacyGetItem(1173, "template");
			Assert.AreEqual(legacyVal, val);
			Assert.AreEqual("1234", val);

			//now test them all to see if they all match legacy
			foreach(var s in new[]{"id","parentID","level","writerID","template","sortOrder","createDate","updateDate","nodeName","writerName","path"})
			{
				val = library.GetItem(1173, s);
				legacyVal = LegacyGetItem(1173, s);
				Assert.AreEqual(legacyVal, val);				
			}			
		}

		[Test]
		public void Get_Item_Invalid_Property()
		{
			var val = library.GetItem(1173, "dontfindme");
			var legacyVal = LegacyGetItem(1173, "dontfindme");
			Assert.AreEqual(legacyVal, val);
			Assert.AreEqual("", val);
		}

		/// <summary>
		/// The old method, just using this to make sure we're returning the correct exact data as before.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		private string LegacyGetItem(int nodeId, string alias)
		{
            var cache = UmbracoContext.Current.ContentCache.InnerCache as PublishedContentCache;
            if (cache == null) throw new Exception("Unsupported IPublishedContentCache, only the Xml one is supported.");
            var umbracoXML = cache.GetXml(UmbracoContext.Current, UmbracoContext.Current.InPreviewMode);

            string xpath = UmbracoSettings.UseLegacyXmlSchema ? "./data [@alias='{0}']" : "./{0}";
			if (umbracoXML.GetElementById(nodeId.ToString()) != null)
				if (
					",id,parentID,level,writerID,template,sortOrder,createDate,updateDate,nodeName,writerName,path,"
						.
						IndexOf("," + alias + ",") > -1)
					return umbracoXML.GetElementById(nodeId.ToString()).Attributes.GetNamedItem(alias).Value;
				else if (
					umbracoXML.GetElementById(nodeId.ToString()).SelectSingleNode(string.Format(xpath, alias)) !=
					null)
					return
						umbracoXML.GetElementById(nodeId.ToString()).SelectSingleNode(string.Format(xpath, alias)).ChildNodes[0].
							Value; //.Value + "*";
				else
					return string.Empty;
			else
				return string.Empty;
		}
	}
}
