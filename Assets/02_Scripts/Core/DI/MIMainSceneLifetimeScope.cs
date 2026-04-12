using MI.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MI.Core.DI
{
    public class MIMainSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private MISceneContext _sceneContext;
        protected override void Configure(IContainerBuilder builder)
        {
            // Scene Context
            builder.RegisterComponent(_sceneContext);
        }
    }
}
