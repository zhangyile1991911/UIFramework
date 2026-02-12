using Common;
using Framework.UIFramework;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GlobalContainer : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IResourceManager,TestResourceManager>(Lifetime.Singleton);
        builder.Register<UIManager>(Lifetime.Singleton);
        builder.Register<CreateWidgetHelper>(Lifetime.Singleton);
    }
}
