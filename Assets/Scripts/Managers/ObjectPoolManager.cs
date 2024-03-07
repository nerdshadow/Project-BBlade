using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    #region BloodVFX
    public ObjectPool<BloodStreamParticle> bloodStreamPool;
    [SerializeField]
    BloodStreamParticle bloodStreamVFXPrefab = null;
    int bloodParticleArraySize = 20;
    int bloodParticleMaxArraySize = 50;
    #endregion BloodVFX
    #region BloodDecal
    public ObjectPool<BloodDecal> bloodDecalPool;
    [SerializeField]
    BloodDecal bloodDecalPrefab = null;
    int bloodDecalArraySize = 1000;
    int bloodDecalMaxArraySize = 2000;
    #endregion BloodDecal
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitManager();
    }
    void InitManager()
    {
        //Debug.Log("Init manager");
        bloodStreamPool = new ObjectPool<BloodStreamParticle>
            (CreateBloodParticle, OnTakeBloodParticleFromPool, OnReturnBloodParticleToPool, OnDestroyBloodParticleStream, true, bloodParticleArraySize, bloodParticleMaxArraySize);
        bloodDecalPool = new ObjectPool<BloodDecal>
            (CreateBloodDecal, OnTakeBloodDecalFromPool, OnReturnBloodDecalToPool, OnDestroyBloodDecalStream, true, bloodDecalArraySize, bloodDecalMaxArraySize);
    }
    private BloodStreamParticle CreateBloodParticle()
    {
        //Debug.Log("Creating VFX");
        BloodStreamParticle _bloodStream = Instantiate(bloodStreamVFXPrefab, this.transform, true);

        _bloodStream.SetPool(bloodStreamPool);

        return _bloodStream;
    }
    void OnTakeBloodParticleFromPool(BloodStreamParticle _bloodStream)
    {
        //Debug.Log("Taking Pooled");
        _bloodStream.transform.position = Vector3.zero;
        _bloodStream.transform.rotation = Quaternion.identity;

        _bloodStream.gameObject.SetActive(true);
    }
    void OnReturnBloodParticleToPool(BloodStreamParticle _bloodStream)
    {
        //Debug.Log("Returning pooled");
        _bloodStream.gameObject.SetActive(false);
    }
    void OnDestroyBloodParticleStream(BloodStreamParticle _bloodStream)
    {
        //Debug.Log("Destroing Pooled");
        Destroy(_bloodStream);
    }
    private BloodDecal CreateBloodDecal()
    {
        //Debug.Log("Creating VFX");
        BloodDecal _bloodDecal = Instantiate(bloodDecalPrefab, this.transform, true);

        _bloodDecal.SetPool(bloodDecalPool);

        return _bloodDecal;
    }
    void OnTakeBloodDecalFromPool(BloodDecal _bloodDecal)
    {
        //Debug.Log("Taking Pooled");
        _bloodDecal.transform.position = Vector3.zero;
        _bloodDecal.transform.rotation = Quaternion.identity;

        _bloodDecal.gameObject.SetActive(true);
    }
    void OnReturnBloodDecalToPool(BloodDecal _bloodDecal)
    {
        //Debug.Log("Returning pooled");
        _bloodDecal.gameObject.SetActive(false);
    }
    void OnDestroyBloodDecalStream(BloodDecal _bloodDecal)
    {
        //Debug.Log("Destroing Pooled");
        Destroy(_bloodDecal);
    }

    public void ClearPools()
    {
        bloodStreamPool.Clear();
        bloodDecalPool.Clear();
    }
}
