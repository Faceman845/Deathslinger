using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance; // Singleton para acesso fácil

    public GameObject bulletPrefab; // O modelo da bala
    public int quantidadeInicial = 20; // Quantas balas criar de início

    private List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();

        // Cria as balas iniciais e as desativa
        for (int i = 0; i < quantidadeInicial; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false); // Começa desligado
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        // Procura na lista por um objeto que não esteja em uso (inativo)
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // Se faltar bala, cria uma nova e adiciona à lista (expansão dinâmica)
        GameObject obj = Instantiate(bulletPrefab);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }
}