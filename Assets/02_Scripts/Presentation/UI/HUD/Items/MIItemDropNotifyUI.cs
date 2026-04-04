using MI.Data.UIRes;
using System.Collections.Generic;
using MI.Core.Pool;
using MI.Domain.UserState.Inventory;
using UnityEngine;

namespace MI.Presentation.UI.HUD.Items
{
    using IMIItemViewer = Interface.IMIItemViewer;
    public class MIItemDropNotifyUI : MonoBehaviour, IMIItemDropEventListener
    {
        [SerializeField] private MIItemIconDataTable _iconDataTable;
        [SerializeField] private GameObject _itemViewerTemplate; //템플릿 오브젝트
        [SerializeField] private Transform _transformViewerParent; //뷰어 생성 후 세팅할 Vertical Group transform
        [SerializeField] private int _maxCacheSize = 20;
        private IMIItemViewer _templateViewer; // 템플릿 캐시
        
        // UI 레이아웃상 너무 많으면 화면 밖으로 튀어나가기 때문에
        // List에 캐싱하여 MaxQueueSize를 초과하면 순서대로 삭제한다.
        private List<GameObject> _cachedList = new List<GameObject>(); 
        

        private FPoolConfig _poolConfig = new FPoolConfig()
        {
            InitialSize = 10,
            GrowSize = 10
        };
        #region  Unity Events

        private void Awake()
        {
            if (_itemViewerTemplate != null)
            {
                //뷰어 캐시 - 템플릿에 데이터를 미리 세팅 후 생성시킬 목적
                _templateViewer = _itemViewerTemplate.GetComponent<IMIItemViewer>();
                _templateViewer.SetIconDataTable(_iconDataTable);
                _templateViewer.OnHideAction += ReturnElement;
                _itemViewerTemplate.SetActive(false); // 템플릿은 비활성화 상태로 보관
            }
            
            MIItemDropEvent.Register(this); // 생명주기: 메인씬에서 항상 살아있음
        }

        private void Start()
        {
            MIPoolManager.Instance.InitPool<MIDropItemViewer>(_itemViewerTemplate, _poolConfig);
        }

        private void OnDestroy()
        {
            MIItemDropEvent.Unregister(this); //씬 전환시 파괴 -> 등록 해제
        }
        
        #endregion

        public void OnItemDropped(FDropItemData data)
        {
            if (_cachedList.Count >= _maxCacheSize)
            {
                var first = _cachedList[0];
                _cachedList.RemoveAt(0);
                ReturnElement(first);
            }
            var viewer = MIPoolManager.Instance.Get<MIDropItemViewer>(_itemViewerTemplate, _transformViewerParent);
            viewer.SetIconDataTable(_iconDataTable);
            viewer.UpdateItemViewer(data.ItemType, data.Amount);
            viewer.OnHideAction += ReturnElement;
            viewer.transform.localScale = Vector3.one;
            _cachedList.Add(viewer.gameObject);
            viewer.gameObject.SetActive(true);
        }

        public void ReturnElement(GameObject go)
        {
            go.SetActive(false);
            _cachedList.Remove(go);
            MIPoolManager.Instance.Return<MIDropItemViewer>(go.GetComponent<MIDropItemViewer>());
        }
    }
}
