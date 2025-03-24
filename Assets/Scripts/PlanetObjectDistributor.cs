using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PlanetObjectDistributor : MonoBehaviour
{
    public GameObject objectToPlace; // 배치할 프리팹
    public float radius = 10f;         // 배치할 반지름
    public int numberOfObjects = 10;   // 배치할 오브젝트 개수

    // 오브젝트를 행성 둘레에 균등하게 배치합니다.
    public void PlaceObjects()
    {
        if (objectToPlace == null)
        {
            Debug.LogError("배치할 오브젝트가 선택되지 않았습니다!");
            return;
        }

        // 기존 자식 오브젝트 모두 제거 (에디터 모드에서 실행)
        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        // 각도를 계산하여 오브젝트를 배치
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
