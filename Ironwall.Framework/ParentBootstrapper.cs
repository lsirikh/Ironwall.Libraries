using Autofac;
using Autofac.Core.Registration;
using Autofac.Features.Metadata;
using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Framework
{
    public abstract class ParentBootstrapper<T> : BootstrapperBase, IParentBootstrapper, IAsyncDisposable
    {
        /// <summary>
        /// A simple container for Dependency Injection
        /// </summary>

        public ParentBootstrapper()
        {
            CancellationTokenSourceHandler = new CancellationTokenSource();
            _log = new LogService();

            string projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            _log.Info($"############### Program{projectName} was started. ###############");
        }

        #region - Abstracts -
        /// <summary>
        /// Register classes, services ,or modules in this method
        /// </summary>
        /// <param name="builder"></param>
        protected abstract void ConfigureContainer(ContainerBuilder builder);

        /// <summary>
        /// StartProgram execute ShellViewModel to display after finished running services and providers.
        /// </summary>
        protected abstract void StartPrograme();
        public async Task Start()
        {
            var token = CancellationTokenSourceHandler.Token;
            try
            {
                foreach (var service in _container.Resolve<IEnumerable<Meta<IService>>>()
                                        .OrderBy(s => s.Metadata["Order"])
                                        .Select(s => s.Value))
                {
                    _log.Info($"@@@@Starting Service Instance({service.GetType()})", true);
                    //await Task.Delay(500);
                    await service.ExecuteAsync(token).ConfigureAwait(false);
                }

                await Task.Delay(3000);

                foreach (var service in _container.Resolve<IEnumerable<Meta<ILoadable>>>()
                                        .OrderBy(s => s.Metadata["Order"])
                                        .Select(s => s.Value))
                {
                    _log.Info($"####Starting Provider Instance({service.GetType()})", true);
                    DispatcherService.Invoke((System.Action)(async () =>
                    {
                        await service.Initialize(token).ConfigureAwait(false);

                    }));
                }

                StartPrograme();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(Start)} of {nameof(ParentBootstrapper<T>)} : {ex}", true);
            }
            
        }

        public async Task BaseStart()
        {
            var token = CancellationTokenSourceHandler.Token;
            foreach (var service in _container.Resolve<IEnumerable<IService>>())
            {
                await service.ExecuteAsync(token).ConfigureAwait(false);
            }
        }
        

        public virtual void Stop()
        {
            CancellationTokenSourceHandler?.Cancel();
            CancellationTokenSourceHandler?.Dispose();
            Task.Run(async ()=> await DisposeAsync());
        }
        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {

            await _container.DisposeAsync().ConfigureAwait(false);

            // Registries are not likely to have async tasks to dispose of,
            // so we will leave it as a straight dispose.
            //ComponentRegistry.Dispose();

            // Do not call the base, otherwise the standard Dispose will fire.
        }

        /// <summary>
        /// Ons the startup.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnExit(object sender, EventArgs e)
        {
            try
            {
                var connection = Container.Resolve<IDbConnection>();
                if (connection?.State == ConnectionState.Open)
                {
                    connection?.Close();
                    connection?.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnExit)} of {nameof(ParentBootstrapper<T>)} : {ex}", true);
            }

            try
            {
                var subconn = Container.Resolve<ISubscriber>();
                if (subconn.IsConnected() && subconn.Multiplexer.IsConnected)
                {
                    subconn?.Multiplexer?.Close();
                    subconn?.Multiplexer?.Dispose();
                }
            }
            catch (ComponentNotRegisteredException ex)
            {
                _log.Error($"Raised {nameof(ComponentNotRegisteredException)} in {nameof(OnExit)} of {nameof(ParentBootstrapper<T>)} : {ex}", true);
            }
            catch (ObjectDisposedException ex)
            {
                _log.Error($"Raised {nameof(ObjectDisposedException)} in {nameof(OnExit)} of {nameof(ParentBootstrapper<T>)} : {ex}", true);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnExit)} of {nameof(ParentBootstrapper<T>)} : {ex}", true);
            }

            try
            {
                foreach (var service in _container.Resolve<IEnumerable<Meta<IService>>>()
                                        .OrderBy(s => s.Metadata["Order"])
                                        .Select(s => s.Value))
                {
                    _log.Info($"@@@@Initializing Service Instance({service.GetType()})", true);
                    //await Task.Delay(500);
                    service.StopAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnExit)} of {nameof(ParentBootstrapper<T>)} : {ex}", true);
            }

            Stop();
            base.OnExit(sender, e);
        }

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            #region - Auto Register Logic? -
            /*
            //  register view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              //  must be a type that ends with ViewModel
              .Where(type => type.GroupId.EndsWith("ViewModel"))
              //  must be in a namespace ending with ViewModels
              .Where(type => !(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("ViewModels"))
              //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will statisfy this)
              //.Where(type => type.GetInterface(typeof(INotifyPropertyChanged).GroupId) != null)
              //  registered as self
              .AsSelf()
              //  always create a new one
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              //  must be a type that ends with View
              .Where(type => type.GroupId.EndsWith("View"))
              //  must be in a namespace that ends in Views
              .Where(type => !(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("Views"))
              //  registered as self
              .AsSelf()
              //  always create a new one
              .InstancePerDependency();
            */
            #endregion

            /*RegisterBaseType(builder);
            ConfigureContainer(builder);
            _container = builder.Build();*/

            RegisterBaseType(builder);
            builder.RegisterType<T>().SingleInstance();

            ConfigureContainer(builder);

            _container = builder.Build();
        }


        private void RegisterBaseType(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventAggregator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TokenGeneration>().SingleInstance();
            builder.RegisterInstance(_log).As<ILogService>().SingleInstance();
            //builder.RegisterType<T>().SingleInstance();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.IsRegistered(service))
                    return Container.Resolve(service);
            }
            else
            {
                if (Container.IsRegisteredWithKey(key, service))
                    return Container.ResolveKeyed(key, service);
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));

            //return key == null ? _container.Resolve(service) : _container.ResolveKeyed(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            /*var enumerableOfServiceType = typeof(IEnumerable<>).MakeGenericType(service);
            return (IEnumerable<object>)_container.Resolve(enumerableOfServiceType);*/
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }
        #endregion

        #region - Fields -
        private IContainer _container;
        #endregion

        #region - Properties -
        protected IContainer Container
        {
            get { return _container; }
        }


        /// <summary>
        /// Top level cancellation token for cancel task.
        /// </summary>
        protected CancellationTokenSource CancellationTokenSourceHandler { get; }

        protected ILogService _log;
        #endregion
    }
}
