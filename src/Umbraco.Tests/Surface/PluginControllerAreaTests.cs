using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Umbraco.Tests.Surface
{
	[TestFixture]
	public class PluginControllerAreaTests : BaseWebTest
	{
        protected override DatabaseBehavior DatabaseTestBehavior
        {
            get { return DatabaseBehavior.NoDatabasePerFixture; }
        }

		[Test]
		public void Ensure_Same_Area1()
		{
			Assert.Throws<InvalidOperationException>(() =>
			                                         new PluginControllerArea(new PluginControllerMetadata[]
			                                         	{
															PluginController.GetMetadata(typeof(Plugin1Controller)),
															PluginController.GetMetadata(typeof(Plugin2Controller)),
															PluginController.GetMetadata(typeof(Plugin3Controller)) //not same area
			                                         	}));
		}

		[Test]
		public void Ensure_Same_Area3()
		{
			Assert.Throws<InvalidOperationException>(() =>
													 new PluginControllerArea(new PluginControllerMetadata[]
			                                         	{
															PluginController.GetMetadata(typeof(Plugin1Controller)),
															PluginController.GetMetadata(typeof(Plugin2Controller)),
															PluginController.GetMetadata(typeof(Plugin4Controller)) //no area assigned
			                                         	}));
		}

		[Test]
		public void Ensure_Same_Area2()
		{
			var area = new PluginControllerArea(new PluginControllerMetadata[]
				{
					PluginController.GetMetadata(typeof(Plugin1Controller)),
					PluginController.GetMetadata(typeof(Plugin2Controller))
				});
			Assert.Pass();
		}

		#region Test classes

		[PluginController("Area1")]
		public class Plugin1Controller : PluginController
		{
			public Plugin1Controller(UmbracoContext umbracoContext) : base(umbracoContext)
			{
			}
		}

		[PluginController("Area1")]
		public class Plugin2Controller : PluginController
		{
			public Plugin2Controller(UmbracoContext umbracoContext)
				: base(umbracoContext)
			{
			}
		}

		[PluginController("Area2")]
		public class Plugin3Controller : PluginController
		{
			public Plugin3Controller(UmbracoContext umbracoContext)
				: base(umbracoContext)
			{
			}
		}

		public class Plugin4Controller : PluginController
		{
			public Plugin4Controller(UmbracoContext umbracoContext)
				: base(umbracoContext)
			{
			}
		}

		#endregion

	}
}
