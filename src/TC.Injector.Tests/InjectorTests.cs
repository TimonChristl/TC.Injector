using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TC.Injector.Tests
{

    interface IPropertyContract
    {
    }

    interface IContract
    {
        void Method();

        IPropertyContract Property1 { get; set; }

        IPropertyContract Property2 { get; set; }
    }

    class ContractImplementation1 : IContract
    {
        public void Method()
        {
        }

        [Inject(Tag = "A")]
        public IPropertyContract Property1 { get; set; }

        [Inject(Tag = "B")]
        public IPropertyContract Property2 { get; set; }

        public bool Debug { get; set; }

    }

    class ContractImplementation2 : IContract
    {
        public void Method()
        {
        }

        [Inject(Tag = "A")]
        public IPropertyContract Property1 { get; set; }

        [Inject(Tag = "B")]
        public IPropertyContract Property2 { get; set; }

    }

    class PropertyContractImplementation1 : IPropertyContract
    {
    }

    class PropertyContractImplementation2 : IPropertyContract
    {
    }

    [TestClass]
    public class InjectorTests
    {

        [TestMethod]
        public void Injector_BindToInstance_Works()
        {
            var contractImplementation = new ContractImplementation1();

            var injector = new Injector();
            injector.Bind<IContract>().To(contractImplementation);

            Assert.AreEqual(injector.Get<IContract>(), contractImplementation);
        }

        [TestMethod]
        public void Injector_BindToFactory_Works()
        {
            var injector = new Injector();
            injector.Bind<IContract>().To(() => new ContractImplementation1());

            Assert.IsInstanceOfType(injector.Get<IContract>(), typeof(ContractImplementation1));
            Assert.AreNotEqual(injector.Get<IContract>(), injector.Get<IContract>());
        }

        [TestMethod]
        public void Injector_BindToImplementationType_Works()
        {
            var injector = new Injector();
            injector.Bind<IContract>().To<ContractImplementation1>();

            Assert.IsInstanceOfType(injector.Get<IContract>(), typeof(ContractImplementation1));
            Assert.AreNotEqual(injector.Get<IContract>(), injector.Get<IContract>());
        }

        [TestMethod]
        public void Injector_BindToFactory_Singleton_Works()
        {
            var injector = new Injector();
            injector.Bind<IContract>().ToSingleton(() => new ContractImplementation1());

            Assert.IsInstanceOfType(injector.Get<IContract>(), typeof(ContractImplementation1));
            Assert.AreEqual(injector.Get<IContract>(), injector.Get<IContract>());
        }

        [TestMethod]
        public void Injector_BindToImplementationType_Singleton_Works()
        {
            var injector = new Injector();
            injector.Bind<IContract>().ToSingleton<ContractImplementation1>();

            Assert.IsInstanceOfType(injector.Get<IContract>(), typeof(ContractImplementation1));
            Assert.AreEqual(injector.Get<IContract>(), injector.Get<IContract>());
        }

        [TestMethod]
        public void Injector_BindWithIf_Works_1()
        {
            var injector = new Injector();

            injector.Bind<IContract>().To<ContractImplementation1>();
            injector.Bind<IPropertyContract>().If((request) => request.Attribute.Tag == "A").To<PropertyContractImplementation1>();
            injector.Bind<IPropertyContract>().If((request) => request.Attribute.Tag == "B").To<PropertyContractImplementation2>();

            var instance = injector.Get<IContract>();

            Assert.IsInstanceOfType(instance.Property1, typeof(PropertyContractImplementation1));
            Assert.IsInstanceOfType(instance.Property2, typeof(PropertyContractImplementation2));
        }

        // ------------------------------------------------------------------------------------------------------------

        interface INetworkService
        {
            bool Debug { get; }
        }

        interface INetworkTracer
        {
            void Trace(string msg);
        }

        class NetworkService : INetworkService
        {
            public bool Debug { get; set; }

            [Inject]
            public INetworkTracer Tracer { get; set; }
        }

        class DebugNetworkTracer : INetworkTracer
        {
            public void Trace(string msg) { }
        }

        class NonDebugNetworkTracer : INetworkTracer
        {
            public void Trace(string msg) { }
        }

        [TestMethod]
        public void Injector_BindWithIf_Works_2()
        {
            var injector = new Injector();

            injector.Bind<INetworkTracer>().If<INetworkService>((request) => request.EnclosingObject.Debug).To<DebugNetworkTracer>();
            injector.Bind<INetworkTracer>().If<INetworkService>((request) => !request.EnclosingObject.Debug).To<NonDebugNetworkTracer>();

            var instance1 = new NetworkService { Debug = true, };
            injector.Resolve(instance1);

            var instance2 = new NetworkService { Debug = false, };
            injector.Resolve(instance2);

            Assert.IsInstanceOfType(instance1.Tracer, typeof(DebugNetworkTracer));
            Assert.IsInstanceOfType(instance2.Tracer, typeof(NonDebugNetworkTracer));
        }

        // ------------------------------------------------------------------------------------------------------------

        class ContractImplementation3 : IContract
        {

            private INetworkService networkService;

            public ContractImplementation3(INetworkService networkService)
            {
                this.networkService = networkService;
            }

            public INetworkService NetworkService
            {
                get { return networkService; }
            }

            public void Method()
            {
            }

            [Inject(Tag = "A")]
            public IPropertyContract Property1 { get; set; }

            [Inject(Tag = "B")]
            public IPropertyContract Property2 { get; set; }

        }

        [TestMethod]
        public void Injector_BindToImplementationTypeWithConstructorInjection_Works()
        {
            var injector = new Injector();
            injector.Bind<IContract>().To<ContractImplementation3>();
            injector.Bind<INetworkService>().To<NetworkService>();

            Assert.IsInstanceOfType(injector.Get<IContract>(), typeof(ContractImplementation3));
            Assert.IsInstanceOfType((injector.Get<IContract>() as ContractImplementation3).NetworkService, typeof(NetworkService));
            Assert.AreNotEqual(injector.Get<IContract>(), injector.Get<IContract>());
        }

        [TestMethod]
        public void Injector_BindToImplementationTypeWithConstructorInjection_Singleton_Works()
        {
            var injector = new Injector();
            injector.Bind<IContract>().ToSingleton<ContractImplementation3>();
            injector.Bind<INetworkService>().To<NetworkService>();

            Assert.IsInstanceOfType(injector.Get<IContract>(), typeof(ContractImplementation3));
            Assert.IsInstanceOfType((injector.Get<IContract>() as ContractImplementation3).NetworkService, typeof(NetworkService));
            Assert.AreEqual(injector.Get<IContract>(), injector.Get<IContract>());
        }

        private interface IDisposableContract : IDisposable
        {
        }

        private class DisposableImplementation : IDisposableContract 
        {

            public DisposableImplementation()
            {
                NumLiveObjects++;
            }

            #region IDisposable implementation

            private bool isDisposed = false;

            ~DisposableImplementation()
            {
                if(!isDisposed)
                    Dispose(false);
            }

            public void Dispose()
            {
                if(!isDisposed)
                {
                    Dispose(true);
                    isDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }

            private void Dispose(bool disposing)
            {
                if(disposing)
                {
                    NumLiveObjects--;
                }
            }

            #endregion

            public static int NumLiveObjects = 0;

        }

        [TestMethod]
        public void Injector_Dispose_Works()
        {
            using(var injector = new Injector())
            {
                injector.Bind<IDisposableContract>().To<DisposableImplementation>();

                injector.Get<IDisposableContract>();
                injector.Get<IDisposableContract>();
            }

            Assert.AreEqual(0, DisposableImplementation.NumLiveObjects);
        }

        [TestMethod]
        public void Injector_Dispose_Singleton_Works()
        {
            using(var injector = new Injector())
            {
                injector.Bind<IDisposableContract>().ToSingleton<DisposableImplementation>();

                injector.Get<IDisposableContract>();
                injector.Get<IDisposableContract>();
            }

            Assert.AreEqual(0, DisposableImplementation.NumLiveObjects);
        }

        [TestMethod]
        public void Injector_Dispose_Instance_Works()
        {
            using(var injector = new Injector())
            using(var instance = new DisposableImplementation())
            {
                injector.Bind<IDisposableContract>().To(instance);

                injector.Get<IDisposableContract>();
                injector.Get<IDisposableContract>();
            }

            Assert.AreEqual(0, DisposableImplementation.NumLiveObjects);
        }

        [TestMethod]
        public void Injector_Dispose_Factory_Works()
        {
            using(var injector = new Injector())
            {
                injector.Bind<IDisposableContract>().To(() => new DisposableImplementation());

                injector.Get<IDisposableContract>();
                injector.Get<IDisposableContract>();
            }

            Assert.AreEqual(0, DisposableImplementation.NumLiveObjects);
        }

        [TestMethod]
        public void Injector_Dispose_SingletonFactory_Works()
        {
            using(var injector = new Injector())
            {
                injector.Bind<IDisposableContract>().ToSingleton(() => new DisposableImplementation());

                injector.Get<IDisposableContract>();
                injector.Get<IDisposableContract>();
            }

            Assert.AreEqual(0, DisposableImplementation.NumLiveObjects);
        }

    }

}
