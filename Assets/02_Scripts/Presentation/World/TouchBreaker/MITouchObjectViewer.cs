using MI.Core.Pool;
using UnityEngine;

namespace MI.Presentation.World.TouchBreaker
{
    /// <summary>
    /// ��ġ�� �߻��ϴ� ������Ʈ
    /// 1ȸ ��� �� Active���¸� ��Ȱ��ȭ�ϴ� ���� ��Ģ���� �մϴ�.
    /// </summary>
    public class MITouchObjectViewer : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        
        private static readonly int s_tTouch = Animator.StringToHash("tTouch");
        public void Activate()
        {
            _animator.SetTrigger(s_tTouch);
        }

        public void OnDeactivate()
        {
            MIPoolManager.Instance.Return(this);
            gameObject.SetActive(false);
        }
    }
}
