using System;
using MI.Core.Pool;
using MI.Data.Config;
using MI.Domain.Status;
using MI.Data.Tile;
using MI.Data.User.Inventory;
using MI.Domain.Tile;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;
using MI.Untility;
using MI.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Presentation.World.Tile
{
    /// <summary>
    /// 타일의 데미지 수신 / HP 관리 / 파괴 판정 담당.
    /// IMIBreakable을 구현하며, 시각 이벤트는 MITileView에 위임.
    ///
    /// 클래스 관계:
    ///   MIPickaxeController
    ///       └─ IMIBreakable.TakeDamage(damage, hitPoint)
    ///                   ↓
    ///           [MITileModel]  ──직접 참조──▶  [MITileView]
    ///           · FTileData (HP)               · UpdateVisual(spriteIndex)
    ///           · IMIBreakable                 · ShowDamageText(damage, hitPoint)
    ///           · 파괴 판정                     · PlayBreakEffect()
    /// </summary>
    [RequireComponent(typeof(MITileView))]
    public class MITileModel : MonoBehaviour, IMIBreakable
    {
        [SerializeField, ReadOnly] private FTileData _data;

        /// <summary>광물 밀도 → 드랍 수량 변환용 Config (씬 또는 프리팹에서 할당)</summary>
        [SerializeField] private MIMineralConfig _mineralConfig;

        // 파괴 시 호출되는 콜백 (MITileSpawner 에서 주입)
        private Action<MITileModel> _onBroken;
        private MITileView          _view;

        public int CurrentRow { get; set; } // 스포너가 관리하는 현재 행 인덱스, 파괴 시 O(n) 탐색 방지용
        public bool  IsBreakable     => !_data.IsDestroyed;
        public float BounceMultiplier => _data.BounceMultiplier;

        private void Awake()
        {
            _view = GetComponent<MITileView>();
        }

        #region Initialization
        /// <summary>스테이지 생성 시 호출 (내부적으로 ResetTile 위임)</summary>
        public void Initialize(MITileConfig tileConfig) => ResetTile(tileConfig);

        /// <summary>
        /// 풀에서 꺼낼 때 타일 상태를 초기화합니다.
        /// MITileConfig 에서 FTileData 를 생성합니다.
        /// </summary>
        public void ResetTile(MITileConfig tileConfig)
        {
            _data        = tileConfig.CreateTileData();
            _onBroken = null;
            _view.UpdateTileData(tileConfig, _data);
        }

        /// <summary>
        /// 풀에서 꺼낼 때 타일 상태를 초기화합니다.
        /// 알고리즘이 생성한 FTileData (광물 정보 포함)를 직접 받습니다.
        /// </summary>
        public void ResetTile(MITileConfig tileConfig, FTileData data)
        {
            _data        = data;
            _onBroken = null;
            _view.UpdateTileData(tileConfig, _data);
        }

        /// <summary>파괴 시 호출될 콜백을 등록합니다 (MITileSpawner 에서 주입).</summary>
        public void SetBrokenCallback(Action<MITileModel> callback) => _onBroken = callback;
        #endregion

        #region Damage Handling
        //추후 Take Damage Domain으로 분리 고려
        public EBreakResult TakeDamage(int damage, Vector3 hitPoint = default)
        {
            var result = _data.ApplyDamage(damage);
            MILog.Log($"Tile took {damage} damage. Result: {result}. Remaining Durability: {_data.CurrentDurability}/{_data.MaxDurability}");

            _view.ShowDamageText(damage, hitPoint);
            _view.SetShakeParameter();
            _view.SetCrackParameter(_data.GetCrackLevel());

            if (result == EBreakResult.Destroyed || result == EBreakResult.DestroyWithOneHit)
                Break();

            return result;
        }

        public void Break()
        {
            MIStatusManager.Instance.AddExp(_data.DropExp);

            // 아이템 드롭 이벤트 발행
            BroadcastDropItems();

            // 골드 드롭 이벤트 발행
            BroadcastGoldDrop();

            // 파괴 이펙트
            _view.PlayBreakEffect();

            // 풀에 반환
            MIPoolManager.Instance.Return(this);

            // 스포너에 파괴 알림 → _tilesByRow 에서 참조 제거
            var cb = _onBroken;
            _onBroken = null; // 재호출 방지
            cb?.Invoke(this);
        }

        /// <summary>
        /// FTileData의 드롭 정보를 파싱하여 MIItemDropEvent로 브로드캐스트합니다.
        /// 타일 재료는 항상, 광물은 MineralDrop이 존재할 때만 발행합니다.
        /// </summary>
        private void BroadcastDropItems()
        {
            // 타일 재료 드롭 (항상 존재)
            int tileAmount = UnityEngine.Random.Range(_data.TileDrop.MinAmount, _data.TileDrop.MaxAmount + 1);
            if (tileAmount > 0)
            {
                MIItemDropEvent.Broadcast(new FDropItemData
                {
                    ItemType = MIItemTypeConverter.FromTileType(_data.TileDrop.TileType),
                    Amount   = tileAmount
                });
            }

            // 광물 드롭 (nullable)
            if (_data.MineralDrop.HasValue && _mineralConfig != null)
            {
                var mineral = _data.MineralDrop.Value;
                MIIntRange range = _mineralConfig.GetDropRange(mineral.Density);
                if (range != null)
                {
                    int mineralAmount = UnityEngine.Random.Range(range.Min, range.Max + 1);
                    MIItemDropEvent.Broadcast(new FDropItemData
                    {
                        ItemType = MIItemTypeConverter.FromMineralType(mineral.MineralType),
                        Amount   = mineralAmount
                    });
                }
            }
        }
        // 골드 드랍 정보를 파싱하여 MIGoldDropEvent로 브로드캐스트
        private void BroadcastGoldDrop()
        {
            if (_data.GoldDropMax <= 0) return;

            int goldAmount = UnityEngine.Random.Range(_data.GoldDropMin, _data.GoldDropMax + 1);
            if (goldAmount <= 0) return;

            MIGoldDropEvent.Broadcast(new FGoldDropData { Amount = goldAmount });
        }

        #endregion Damage Handling
    }
}
