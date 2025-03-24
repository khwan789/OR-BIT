using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PlanetObjectDistributor : MonoBehaviour
{
    public GameObject objectToPlace; // ��ġ�� ������
    public float radius = 10f;         // ��ġ�� ������
    public int numberOfObjects = 10;   // ��ġ�� ������Ʈ ����

    // ������Ʈ�� �༺ �ѷ��� �յ��ϰ� ��ġ�մϴ�.
    public void PlaceObjects()
    {
        if (objectToPlace == null)
        {
            Debug.LogError("��ġ�� ������Ʈ�� ���õ��� �ʾҽ��ϴ�!");
            return;
        }

        // ���� �ڽ� ������Ʈ ��� ���� (������ ��忡�� ����)
        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        // ������ ����Ͽ� ������Ʈ�� ��ġ
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfObjects;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 position = new Vector3(x, y, 0f);

#if UNITY_EDITOR
            GameObject obj = PrefabUtility.InstantiatePrefab(objectToPlace) as GameObject;
            if (obj != null)
            {
                obj.transform.position = transform.position + position;
                obj.transform.parent = transform;
            }
#else
            GameObject obj = Instantiate(objectToPlace, transform.position + position, Quaternion.identity, transform);
#endif
        }
    }
}
