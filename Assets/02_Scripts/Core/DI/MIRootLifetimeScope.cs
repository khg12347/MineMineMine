using MI.Data.Config;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Craft;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.User;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;
using MI.Presentation;

using VContainer;
using VContainer.Unity;

using UnityEngine;

namespace MI.Core.DI
{
    /// <summary>
    /// 프로젝트 전역 의존성 등록. 모든 Domain 서비스를 RootLifetimeScope에서 등록한다.
    /// BootStrap 씬에 배치하여 사용한다.
    /// MIUserState는 IDisposable을 구현하며, LifetimeScope 파괴 시(앱 종료) 자동 Dispose된다.
    /// </summary>
    public class MIRootLifetimeScope : LifetimeScope
    {
        [Header("Config")]
        [SerializeField] private MIPickaxeDataRegistry _pickaxeData;

        protected override void Configure(IContainerBuilder builder)
        {
            // Config Registry
            builder.RegisterInstance<IMIPickaxeDataRegistry>(_pickaxeData);

            // Domain — User
            builder.Register<MIUserInventory>(Lifetime.Singleton);
            builder.Register<MIUserWallet>(Lifetime.Singleton);
            builder.Register<MIPickaxeInventory>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.Register<MIUserState>(Lifetime.Singleton);

            // Domain — Service
            builder.Register<MIPickaxeCraftService>(Lifetime.Singleton)
                .As<IMIPickaxeCraftService>();

            // 컨테이너 빌드 후 초기화
            builder.RegisterBuildCallback(InitializeGame);
        }

        #region Helper

        /// <summary>컨테이너 빌드 완료 후 게임 초기화를 수행한다.</summary>
        private void InitializeGame(IObjectResolver resolver)
        {
            // 유저 상태 활성화
            var userState = resolver.Resolve<MIUserState>();
            userState.Inventory.Enable();
            userState.Wallet.Enable();

            // 기본 곡괭이 지급
            GrantDefaultPickaxe(userState);
        }

        /// <summary>기본 곡괭이를 지급하고 Main 슬롯에 장착한다. 이미 보유 중이면 스킵.</summary>
        private void GrantDefaultPickaxe(MIUserState userState)
        {
            if (_pickaxeData == null) return;

            var defaultType = _pickaxeData.SpecDataTable.DefaultPickaxeType;
            if (defaultType == EPickaxeType.None) return;
            if (userState.PickaxeInventory.IsOwned(defaultType)) return;

            var stats = _pickaxeData.SpecDataTable.GetStats(defaultType);
            if (!stats.HasValue) return;

            var instance = FPickaxeInstance.Create(defaultType, stats.Value, _pickaxeData.EnhanceConfig);
            userState.PickaxeInventory.AddPickaxe(instance);
            userState.PickaxeInventory.Equip(defaultType, EEquipSlot.Main);
        }

        #endregion Helper
    }
}
