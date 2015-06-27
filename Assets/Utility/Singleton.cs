using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // ReSharper disable StaticFieldInGenericType
    private static bool applicationIsQuitting;
    // ReSharper restore StaticFieldInGenericType

    private static T instance;

    public static T Instance
    {
        get
        {
            var editMode = Application.isEditor && !Application.isPlaying;
            if (!editMode && applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                    return instance;
                }

                if (instance == null)
                {
                    var singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T);

                    Debug.Log("Singleton name is : " + singleton.name);

                    DontDestroyOnLoad(singleton);

                    Debug.Log("[Singleton] An instance of " + typeof(T) +
                        " is needed in the scene, so '" + singleton +
                        "' was created with DontDestroyOnLoad.");
                }
                else
                {
                    Debug.Log("[Singleton] Using instance already created: " +
                        instance.gameObject.name);
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
