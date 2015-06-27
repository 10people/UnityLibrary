using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Public Fields

    public enum IncreaseType
    {
        Linear,
        Exponential,
    }

    public int Capacity = DefaultCapcity;
    public GameObject SpawnObject;

    public const int DefaultCapcity = 4;

    public IncreaseType TheIncreaseType;

    #endregion

    #region Public Properties

    public List<GameObject> ObjectList;

    public GameObject CurrentObject { get; set; }

    #endregion

    #region Private Fields

    /// <summary>
    /// Flag indicates if initialized or not.
    /// </summary>
    private bool initialized;

    #endregion

    #region Public Methods

    /// <summary>
    /// Take one game object from pool.
    /// </summary>
    /// <returns>Pooled game object</returns>
    public GameObject Take()
    {
        ValidateObjectList();

        var inactiveObjectEnum = ObjectList.Where(item => !item.activeSelf);
        var inactiveObjectList = inactiveObjectEnum as IList<GameObject> ?? inactiveObjectEnum.ToList();
        if (!inactiveObjectList.Any())
        {
            Debug.Log("Take too more than Capacity as " + Capacity + ", make it twice more larger.");
            EnlargeCapacity();
            Debug.Log("Object list: " + ObjectList.Count + ", index: " + Capacity);
            // return the last one.
            CurrentObject = ObjectList[Capacity - 1];
            return CurrentObject;
        }

        CurrentObject = inactiveObjectList.First();
        return CurrentObject;
    }

    /// <summary>
    /// Return back game object to pool manager.
    /// </summary>
    /// <param name="returnObject">Pooled back game object</param>
    public void Return(GameObject returnObject)
    {
        ValidateObjectList();

        returnObject.SetActive(false);
        returnObject.transform.parent = gameObject.transform;

        if (CurrentObject == returnObject)
        {
            CurrentObject = null;
        }
    }

    /// <summary>
    /// Initialize pool manager.
    /// </summary>
    public void Initialize()
    {
        ValidateObjectList();

        if (initialized)
        {
            return;
        }

        initialized = true;

        if (ObjectList == null)
        {
            ObjectList = new List<GameObject>(Capacity);
        }
        else
        {
            while (ObjectList.Count > Capacity)
            {
                // double capacity if object list already hold more than capacity value.
                Capacity += Capacity;
            }
        }

        FillObjectList(Capacity - ObjectList.Count);
    }

    /// <summary>
    /// Cleanup pool manager.
    /// </summary>
    public void Cleanup()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;

        ObjectList.ForEach(item => Destroy(item));
        ObjectList.Clear();
    }

    #endregion

    #region Private Methods

    private GameObject Spawn()
    {
        var spawn = Instantiate(SpawnObject) as GameObject;
        spawn.SetActive(false);
        spawn.transform.parent = gameObject.transform;
        return spawn;
    }

    private void EnlargeCapacity()
    {
        Capacity = (Capacity == 0) ? 1 : ((TheIncreaseType == IncreaseType.Linear) ? (Capacity + 1) : (Capacity * 2));
        FillObjectList(Capacity - ObjectList.Count);
    }

    private void FillObjectList(int num)
    {
        for (var i = 0; i < num; ++i)
        {
            ObjectList.Add(Spawn());
        }
    }

    private void ValidateObjectList()
    {
        if (ObjectList == null || ObjectList.Count == 0)
        {
            return;
        }

        for (int i = ObjectList.Count - 1; i >= 0; i--)
        {
            if (ObjectList[i] == null)
            {
                ObjectList.RemoveAt(i);
                Debug.Log("[MyPoolManager]Remove a null item in objectList.");
            }
        }
    }

    #endregion

    #region Mono

    void Awake()
    {
        // you need to call Initialize() manually since the spawn object is not ready.
        if (SpawnObject == null)
        {
            return;
        }

        Initialize();
    }

    #endregion
}
