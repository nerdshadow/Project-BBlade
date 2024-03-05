using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Pool;

public class BloodDecal : MonoBehaviour
{
    [SerializeField]
    List<Material> decalMaterials = new List<Material>();
    ObjectPool<BloodDecal> bloodDecalPool;
    DecalProjector projector;
    [SerializeField]
    float timeToChange = 1f;
    private void Start()
    {
        projector = GetComponentInChildren<DecalProjector>();
    }
    private void OnEnable()
    {
        projector = GetComponentInChildren<DecalProjector>();
        if (decalMaterials.Count == 1)
        {
            projector.material = decalMaterials[0];
            return;
        }
        int randI = Random.Range(0, decalMaterials.Count);
        projector.material = decalMaterials[randI];
    }
    [ContextMenu("Test Decal Size change")]
    void ChangeDecalScaleTest()
    {
        ChangeDecalScale(0.1f, 0.1f, 2f, 2f, timeToChange);
    }
    public void ChangeDecalScale(float _startWidth, float _startHeight, float _endWidth, float _endHeight, float _timeToChange)
    {
        projector.size = new Vector3(_startWidth, _startHeight, projector.size.z);
        StartCoroutine(ChangingDecalScale(_startWidth, _startHeight, _endWidth, _endHeight, _timeToChange));
    }
    IEnumerator ChangingDecalScale(float _startWidth, float _startHeight, float _endWidth, float _endHeight, float _timeToChange)
    {
        float timePassed = 0;
        float time = _timeToChange;
        while (timePassed < time) 
        {
            projector.size = new Vector3(Mathf.Lerp(_startWidth, _endWidth, timePassed / time),
                    Mathf.Lerp(_startHeight, _endHeight, timePassed / time),
                    projector.size.z);
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        projector.size = new Vector3(_endWidth, _endHeight, projector.size.z);
    }
    public void SetPool(ObjectPool<BloodDecal> _pool)
    {
        bloodDecalPool = _pool;
    }
    public void ReturnToPool()
    {
        bloodDecalPool.Release(this);
    }
}
