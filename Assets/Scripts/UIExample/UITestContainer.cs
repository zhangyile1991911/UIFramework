using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UIExample
{
    public class UITestContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<UITransition>(Lifetime.Singleton);
            builder.RegisterEntryPoint<UITestEntry>();
        }
    }

}
