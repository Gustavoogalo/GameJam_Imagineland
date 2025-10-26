using System.Collections;
using UnityEngine;

public class DestroyCutscene : MonoBehaviour
{
    [SerializeField] private GameObject cutScene;
    [SerializeField] private GameObject cutScene_ObjVazio;
   
    void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FalseCutscene()
    {
        cutScene.SetActive(false);
        cutScene_ObjVazio.SetActive(false);
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(50f);
        cutScene.SetActive(false);
        cutScene_ObjVazio.SetActive(false);
    }
}
