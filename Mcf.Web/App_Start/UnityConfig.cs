using McF.Business;
using McF.DataAccess;
using McF.DataAccess.Repositories.Implementors;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
using System;

using Unity;
using Unity.Lifetime;

namespace Mcf
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IConnectionManager, SqlConnectionManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDbHelper, SqlDbHelper>(new ContainerControlledLifetimeManager());

            container.RegisterType<ICOTService, COTService>();
            container.RegisterType<ICropService, CropService>();
            container.RegisterType<IDTNService, DTNService>();
            container.RegisterType<ISugarService, SugarService>();
            container.RegisterType<IEthanolService, EthanolService>();
            container.RegisterType<IJobService, JobService>();
            container.RegisterType<IUSWeeklyService, USWeeklyService>();
            container.RegisterType<ICommonService, CommonService>();

            container.RegisterType<ICOTRepository, COTRepository>();
            container.RegisterType<ICropProgressRepository, CropProgressRepository>();
            container.RegisterType<IDTNRepository, DTNRepository>();
            container.RegisterType<ISugarRepository, SugarRepository>();
            container.RegisterType<IEthanolRepository, EthanolRepository>();
            container.RegisterType<IJobRepository, JobRepository>();
            container.RegisterType<IUSWeeklyRepository, USWeeklyRepository>();
            container.RegisterType<ICommonRepository, CommonRepository>();
        }
    }
}