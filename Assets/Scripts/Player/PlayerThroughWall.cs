using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerThroughWall : MonoBehaviour
{
    public static int posId = Shader.PropertyToID("_playerPosition");
    public static int sizeId = Shader.PropertyToID("_size");
    public Material wallMaterial;
    public Camera playerCamera;
    public LayerMask collisionMasks;
    public Collider playerCollRef;

    private void Start()
    {
        playerCollRef = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        CutOut2();
    }
    void CutOut2()
    {
        Vector3 dir = playerCamera.transform.position - playerCollRef.bounds.center;
        Vector3 startPoint = playerCollRef.bounds.center + dir.normalized * 1f;
        //RaycastHit[] hits = Physics.RaycastAll(playerCollRef.bounds.center, dir, 100, collisionMasks);
        //Collider[] hits = Physics.OverlapCapsule(playerCollRef.bounds.center, dir * 100, 1f, collisionMasks);
        Collider[] hits = Physics.OverlapCapsule(startPoint, dir * 100, 0.5f, collisionMasks);
        Vector3 viewPos = playerCamera.WorldToViewportPoint(playerCollRef.bounds.center);

        for (int i = 0; i < hits.Length; i++)
        {
            Material[] materials = hits[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                Vector3 pos = Vector3.Lerp(materials[j].GetVector("_playerPosition"), viewPos, 1f * Time.deltaTime);
                materials[j].SetVector("_playerPosition", pos);
                if (changedMaterials.Any(cm => cm.material == materials[j]) != true)
                {
                    //Debug.Log("Added CutoutMat");
                    changedMaterials.Add(new CutOutMaterial(materials[j]));
                }
                CutOutMaterial cutMat = changedMaterials.Find(cm => cm.material == materials[j]);
                cutMat.ChangeTime(0);
            }
        }
        ChangeMaterialCutOutSize();
    }
    class CutOutMaterial
    {
        public Material material;
        public float timePassed;
        public CutOutMaterial(Material _material)
        {
            material = _material;
            timePassed = 0;
        }
        public void IncreaseTimeBy(float byT)
        {
            timePassed += byT;
        }
        public void ChangeTime(float time)
        {
            timePassed = time;
        }
    }
    List<CutOutMaterial> changedMaterials = new List<CutOutMaterial>();
    void ChangeMaterialCutOutSize()
    {
        if (changedMaterials.Count <= 0)
            return;
        foreach (CutOutMaterial cutMat in changedMaterials.ToList())
        {
            //Debug.Log(cutMat.material);
            float _timePassed = cutMat.timePassed;
            float currentSize = cutMat.material.GetFloat("_size");
            float _time = currentSize;
            //Debug.Log("Time passed " + _timePassed);
            if (_timePassed >= 3f)
            {
                _time -= Time.deltaTime;
                //Decrease hole
                if (currentSize > 0)
                {
                    currentSize = Mathf.Lerp(0, 1, _time);
                    cutMat.material.SetFloat("_size", currentSize);
                }
                if(currentSize <= 0)
                    changedMaterials.Remove(cutMat);
            }
            else
            {
                _time += Time.deltaTime;
                //Increase hole
                if (currentSize < 1)
                {
                    currentSize = Mathf.Lerp(0, 1, _time);
                    cutMat.material.SetFloat("_size", currentSize);
                }
                cutMat.timePassed += Time.deltaTime;
            }
        }
    }
}
