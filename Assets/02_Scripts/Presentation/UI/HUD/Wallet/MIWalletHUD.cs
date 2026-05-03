using MI.Data.UIRes;
using MI.Data.User.Wallet;
using MI.Domain.UserState.Wallet;
using MI.Presentation.UI.Common;
using UnityEngine;

namespace MI.Presentation.UI.HUD.Wallet
{
    public class MIWalletHUD : MonoBehaviour
    {
        [SerializeField] private MINumberShaker[] _goldNumbers;
        [SerializeField] private MINumberShaker[] _diamondNumbers;

        private MIUINumberResources _numberResources;
        private MIUserWallet _wallet;


        public void InjectWallet(MIUserWallet wallet)
        {
            _wallet = wallet;
            _wallet.OnCurrencyUpdated += OnUpdateCurrency;
            OnUpdateCurrency(ECurrencyType.Gold, 0, _wallet.GetAmount(ECurrencyType.Gold));
            OnUpdateCurrency(ECurrencyType.Diamond, 0, _wallet.GetAmount(ECurrencyType.Diamond));
        }

        public void InjectNumberResources(MIUINumberResources numberResources)
        {
            _numberResources = numberResources;
        }

        /// <summary>
        /// UI������ long ��� int�� ��ȯ�Ͽ� ���
        /// ���Ŀ� k, m ������ ǥ���ϴ� ����� �߰��Ǹ�
        /// longŸ���� �޾� �ϰ� k, m���� ó���ϵ��� ���� ����
        /// </summary>
        /// <param name="type"></param>
        /// <param name="delta"></param>
        /// <param name="total"></param>
        private void OnUpdateCurrency(ECurrencyType type, long delta, long total)
        {
            switch (type)
            {
                case ECurrencyType.Gold:
                    UpdateGold((int)total);
                    break;
                case ECurrencyType.Diamond:
                    UpdateDiamond((int)total);
                    break;
            }
        }


        private void UpdateGold(int gold)
        {
            MINumberShaker.UpdateSmallNumberDisplay(_goldNumbers, gold, _numberResources, enableLastNum: true);
        }
        private void UpdateDiamond(int diamond)
        {
            MINumberShaker.UpdateSmallNumberDisplay(_diamondNumbers, diamond, _numberResources, enableLastNum: true);
        }
    }
}
