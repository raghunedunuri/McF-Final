using McF.Business;
using McF.DataAccess.Repositories.Implementors;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;


namespace MCFJobService
{
    public static class UnityResolver
    {
        public static IUnityContainer _unityContainer;
        public static void Init()
        {
            _unityContainer = new UnityContainer();
            _unityContainer.RegisterType<IConnectionManager, SqlConnectionManager>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IDbHelper, SqlDbHelper>(new ContainerControlledLifetimeManager());

            _unityContainer.RegisterType<ICOTService, COTService>();
            _unityContainer.RegisterType<ICropService, CropService>();
            _unityContainer.RegisterType<IDTNService, DTNService>();
            _unityContainer.RegisterType<IEthanolService, EthanolService>();
            _unityContainer.RegisterType<IJobService, JobService>();
            _unityContainer.RegisterType<IUSWeeklyService, USWeeklyService>();

            _unityContainer.RegisterType<ICOTRepository, COTRepository>();
            _unityContainer.RegisterType<ICropProgressRepository, CropProgressRepository>();
            _unityContainer.RegisterType<IDTNRepository, DTNRepository>();
            _unityContainer.RegisterType<IEthanolRepository, EthanolRepository>();
            _unityContainer.RegisterType<IJobRepository, JobRepository>();
            _unityContainer.RegisterType<IUSWeeklyRepository, USWeeklyRepository>();
        }
    }
}
