using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class Final
    {
        [Dependency] private ITestDI _testDi;

        public Final()
        {
            _testDi.Foo();
        }
    }
    
    internal interface ITestDI
    {
        void Foo();
    }
    
    internal class TestDI : ITestDI
    {
        [Dependency] private ITestDependency _testDependency;
        
        public void Foo()
        {
            _testDependency.Foo();
        }
    }
    
    internal interface ITestDependency
    {
        void Foo();
    }
    
    internal class TestDependency : ITestDependency
    {
        public void Foo()
        {
            Debug.LogError("TestDependency.Foo");
        }
    }
}