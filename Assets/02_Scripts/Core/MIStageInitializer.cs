using MI.Data.Config;
using MI.Presentation.Pickaxe;
using MI.Presentation.Tile;
using UnityEngine;

namespace MI.Core
{
    public class MIStageInitializer : MonoBehaviour
    {
        [SerializeField] private MITileConfig[] _tileConfigs;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Transform _tileParent;

        [SerializeField] private int _stageWidth = 8;
        [SerializeField] private int _stageHeight = 12;
        [SerializeField] private float _tileSize = 0.5f;

        [Header("곡괭이")]
        [SerializeField] private MIPickaxeController _pickaxe;
        [SerializeField] private Camera _mainCamera;

        private void Start()
        {
            GenerateStage();
            // 곡괭이를 화면 바깥 상단에서 스폰
            _pickaxe.SpawnAtOffScreen(_mainCamera);
        }

        private void GenerateStage()
        {
            for (int y = 0; y < _stageHeight; y++)
            {
                for (int x = 0; x < _stageWidth; x++)
                {
                    var config = SelectTileConfig(y);
                    var pos = new Vector3(x * _tileSize, -y * _tileSize, 0);
                    var tileObj = Instantiate(_tilePrefab, pos, Quaternion.identity, _tileParent);
                    var tileView = tileObj.GetComponent<MITileView>();
                    tileView.Initialize(config.CreateTileData());
                }
            }
        }

        /// <summary>깊이(y)에 따라 더 강한 타일 Config 선택</summary>
        private MITileConfig SelectTileConfig(int depth)
        {
            float ratio = (float)depth / _stageHeight;
            int configIndex = Mathf.Clamp(
                (int)(ratio * _tileConfigs.Length), 0, _tileConfigs.Length - 1);
            return _tileConfigs[configIndex];
        }
    }
}
