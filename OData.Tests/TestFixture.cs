using System;

namespace OData.Tests
{
    public class TestFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public TestFixture()
        {
            //ServiceProvider = null;
        }

        public void Dispose()
        {
            //
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public T GetService<T>()
            where T : class
        {
            return ServiceProvider.GetService(typeof(T)) as T;
        }

    }
}
