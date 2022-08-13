using UnityEngine;

[CreateAssetMenu(menuName = "SO/WEAPON/BulletData")]
public class BulletDataSO : ScriptableObject
{
    [Range(1, 10)] public int damage = 1;
    [Range(1, 100)] public float bulletSpeed = 1;

    public GameObject impactObstaclePrefab; //��ֹ��� �ε��������� ȿ��
    public GameObject impactEnemtPrefab; //�÷��̾ �ε����� ���� ȿ�� 

    public float lifeTime = 2f;
}
