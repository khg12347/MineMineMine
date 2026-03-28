using UnityEngine;

namespace MI.Domain.Status
{
    public interface IMIStatusListener
    {
        /// <summary>
        /// EXP ��ġ�� ����� �� ȣ�� (������ ���� ����).
        /// </summary>
        /// <param name="currentExp">���� ���� �� ���� EXP</param>
        /// <param name="requiredExp">���� �������� ���� �������� �ʿ��� EXP</param>
        /// <param name="ratio">����� [0, 1]</param>
        void OnExpChanged(int currentExp, int requiredExp, float ratio);

        /// <summary>
        /// �������� �߻��� ������ ȣ��. �� ���� ���� ���� ���� ��� �������� ���� ȣ��.
        /// </summary>
        /// <param name="newLevel">���� ���� �� ����</param>
        void OnLevelUp(int newLevel);

        /// <summary>
        /// ����(depth)�� ����� �� ȣ��.
        /// </summary>
        /// <param name="newDepth">���ο� ���� ��</param>
        void OnDepthUpdated(int newDepth);
    }
}
