using System;
using System.Collections.Generic;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.ObjectResolution;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Mappers;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionSix;
using Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionSixZeroOne;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Profiling;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Publishing;
using Umbraco.Core.Macros;
using Umbraco.Core.Services;
using Umbraco.Core.Sync;
using Umbraco.Core.Strings;
using MigrationsVersionFourNineZero = Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionFourNineZero;

namespace Umbraco.Core
{
	/// <summary>
	/// A bootstrapper for the Umbraco application which initializes all objects for the Core of the application 
	/// </summary>
	/// <remarks>
	/// This does not provide any startup functionality relating to web objects
	/// </remarks>
	public class CoreBootManager : IBootManager
	{

		private DisposableTimer _timer;
		private bool _isInitialized = false;
		private bool _isStarted = false;
		private bool _isComplete = false;
        private readonly UmbracoApplicationBase _umbracoApplication;
		protected ApplicationContext ApplicationContext { get; private set; }

	    protected UmbracoApplicationBase UmbracoApplication
	    {
	        get { return _umbracoApplication; }
	    }

	    public CoreBootManager(UmbracoApplicationBase umbracoApplication)
        {            
            if (umbracoApplication == null) throw new ArgumentNullException("umbracoApplication");
            _umbracoApplication = umbracoApplication;
        }

	    public virtual IBootManager Initialize()
		{
			if (_isInitialized)
				throw new InvalidOperationException("The boot manager has already been initialized");

	        InitializeProfilerResolver();

            _timer = DisposableTimer.DebugDuration<CoreBootManager>("Umbraco application starting", "Umbraco application startup complete");

			//create database and service contexts for the app context
			var dbFactory = new DefaultDatabaseFactory(GlobalSettings.UmbracoConnectionName);
		    Database.Mapper = new PetaPocoMapper();
			var dbContext = new DatabaseContext(dbFactory);
			var serviceContext = new ServiceContext(
				new PetaPocoUnitOfWorkProvider(dbFactory), 
				new FileUnitOfWorkProvider(), 
				new PublishingStrategy());

            CreateApplicationContext(dbContext, serviceContext);

            InitializeApplicationEventsResolver();

			InitializeResolvers();

            //initialize the DatabaseContext
            dbContext.Initialize();

            //now we need to call the initialize methods
            ApplicationEventsResolver.Current.ApplicationEventHandlers
                .ForEach(x => x.OnApplicationInitialized(UmbracoApplication, ApplicationContext));

			_isInitialized = true;

			return this;
		}

        /// <summary>
        /// Creates and assigns the application context singleton
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="serviceContext"></param>
        protected virtual void CreateApplicationContext(DatabaseContext dbContext, ServiceContext serviceContext)
        {
            //create the ApplicationContext
            ApplicationContext = ApplicationContext.Current = new ApplicationContext(dbContext, serviceContext);
        }

        /// <summary>
        /// Special method to initialize the ProfilerResolver
        /// </summary>
        protected virtual void InitializeProfilerResolver()
        {
            //By default we'll initialize the Log profiler (in the web project, we'll override with the web profiler)
            ProfilerResolver.Current = new ProfilerResolver(new LogProfiler())
                {
                    //This is another special resolver that needs to be resolvable before resolution is frozen
                    //since it is used for profiling the application startup
                    CanResolveBeforeFrozen = true
                };
        }

        /// <summary>
        /// Special method to initialize the ApplicationEventsResolver and any modifications required for it such 
        /// as adding custom types to the resolver.
        /// </summary>
        protected virtual void InitializeApplicationEventsResolver()
        {
            //find and initialize the application startup handlers, we need to initialize this resolver here because
            //it is a special resolver where they need to be instantiated first before any other resolvers in order to bind to 
            //events and to call their events during bootup.
            //ApplicationStartupHandler.RegisterHandlers();
            //... and set the special flag to let us resolve before frozen resolution
            ApplicationEventsResolver.Current = new ApplicationEventsResolver(
                PluginManager.Current.ResolveApplicationStartupHandlers())
            {
                CanResolveBeforeFrozen = true
            };
            //add custom types here that are internal
            ApplicationEventsResolver.Current.AddType<PublishedContentHelper>();
        }

        /// <summary>
        /// Special method to extend the use of Umbraco by enabling the consumer to overwrite
        /// the absolute path to the root of an Umbraco site/solution, which is used for stuff
        /// like Umbraco.Core.IO.IOHelper.MapPath etc.
        /// </summary>
        /// <param name="rootPath">Absolute</param>
        protected virtual void InitializeApplicationRootPath(string rootPath)
        {
            Umbraco.Core.IO.IOHelper.SetRootDirectory(rootPath);
        }

		/// <summary>
		/// Fires after initialization and calls the callback to allow for customizations to occur & 
        /// Ensure that the OnApplicationStarting methods of the IApplicationEvents are called
		/// </summary>
		/// <param name="afterStartup"></param>
		/// <returns></returns>
		public virtual IBootManager Startup(Action<ApplicationContext> afterStartup)
		{
			if (_isStarted)
				throw new InvalidOperationException("The boot manager has already been initialized");

            //call OnApplicationStarting of each application events handler
            ApplicationEventsResolver.Current.ApplicationEventHandlers
                .ForEach(x => x.OnApplicationStarting(UmbracoApplication, ApplicationContext));

            if (afterStartup != null)
            {
                afterStartup(ApplicationContext.Current);
            }

			_isStarted = true;

			return this;
		}

		/// <summary>
		/// Fires after startup and calls the callback once customizations are locked
		/// </summary>
		/// <param name="afterComplete"></param>
		/// <returns></returns>
		public virtual IBootManager Complete(Action<ApplicationContext> afterComplete)
		{
			if (_isComplete)
				throw new InvalidOperationException("The boot manager has already been completed");

		    FreezeResolution();

			//stop the timer and log the output
			_timer.Dispose();

            //call OnApplicationStarting of each application events handler
            ApplicationEventsResolver.Current.ApplicationEventHandlers
                .ForEach(x => x.OnApplicationStarted(UmbracoApplication, ApplicationContext));

            //Now, startup all of our legacy startup handler
            ApplicationEventsResolver.Current.InstantiateLegacyStartupHandlers();

            if (afterComplete != null)
            {
                afterComplete(ApplicationContext.Current);
            }

			_isComplete = true;

            // we're ready to serve content!
            ApplicationContext.IsReady = true;

			return this;
		}

        /// <summary>
        /// Freeze resolution to not allow Resolvers to be modified
        /// </summary>
        protected virtual void FreezeResolution()
        {
            Resolution.Freeze();
        }
        
		/// <summary>
		/// Create the resolvers
		/// </summary>
		protected virtual void InitializeResolvers()
		{           

            //by default we'll use the standard configuration based sync
            ServerRegistrarResolver.Current = new ServerRegistrarResolver(
                new ConfigServerRegistrar()); 

            //by default (outside of the web) we'll use the default server messenger without
            //supplying a username/password, this will automatically disable distributed calls
            // .. we'll override this in the WebBootManager
            ServerMessengerResolver.Current = new ServerMessengerResolver(
                new DefaultServerMessenger());

            MappingResolver.Current = new MappingResolver(
                () => PluginManager.Current.ResolveAssignedMapperTypes());

			RepositoryResolver.Current = new RepositoryResolver(
				new RepositoryFactory());

		    SqlSyntaxProvidersResolver.Current = new SqlSyntaxProvidersResolver(
                new[] { typeof(MySqlSyntaxProvider), typeof(SqlCeSyntaxProvider), typeof(SqlServerSyntaxProvider) })
		        {
		            CanResolveBeforeFrozen = true
		        };

			CacheRefreshersResolver.Current = new CacheRefreshersResolver(
				() => PluginManager.Current.ResolveCacheRefreshers());

			DataTypesResolver.Current = new DataTypesResolver(
				() => PluginManager.Current.ResolveDataTypes());

			MacroFieldEditorsResolver.Current = new MacroFieldEditorsResolver(
				() => PluginManager.Current.ResolveMacroRenderings());

			PackageActionsResolver.Current = new PackageActionsResolver(
				() => PluginManager.Current.ResolvePackageActions());

			ActionsResolver.Current = new ActionsResolver(
				() => PluginManager.Current.ResolveActions());

            //the database migration objects
            MigrationResolver.Current = new MigrationResolver(
                () => PluginManager.Current.ResolveMigrationTypes());
            


			PropertyEditorValueConvertersResolver.Current = new PropertyEditorValueConvertersResolver(
				PluginManager.Current.ResolvePropertyEditorValueConverters());
			//add the internal ones, these are not public currently so need to add them manually
			PropertyEditorValueConvertersResolver.Current.AddType<DatePickerPropertyEditorValueConverter>();
			PropertyEditorValueConvertersResolver.Current.AddType<TinyMcePropertyEditorValueConverter>();
			PropertyEditorValueConvertersResolver.Current.AddType<YesNoPropertyEditorValueConverter>();

            // this is how we'd switch over to DefaultShortStringHelper _and_ still use
            // UmbracoSettings UrlReplaceCharacters...
            //ShortStringHelperResolver.Current = new ShortStringHelperResolver(
            //    new DefaultShortStringHelper().WithConfig(DefaultShortStringHelper.ApplyUrlReplaceCharacters));

            // use the Legacy one for now
            ShortStringHelperResolver.Current = new ShortStringHelperResolver(
		        new LegacyShortStringHelper());

		    UrlSegmentProviderResolver.Current = new UrlSegmentProviderResolver(
		        typeof (DefaultUrlSegmentProvider));
		}
	}
}
