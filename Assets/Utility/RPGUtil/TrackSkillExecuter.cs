using System;
using UnityEngine;
using System.Collections;

public class TrackSkillExecuter : MonoBehaviour
{
    public GameObject SkillPrefab;
    public GameObject SourceObject;
    public GameObject TargetObject;

    public GameObject SkillObject;

    //TODO set ur own speed.
    [HideInInspector]
    public float MovingSpeed = 3f;

    public void Execute()
    {
        if (SkillPrefab == null || SourceObject == null || TargetObject == null)
        {
            return;
        }

        if (SkillObject != null)
        {
            Destroy(SkillObject);
            SkillObject = null;
        }

        SkillObject = Instantiate(SkillPrefab);
        SkillObject.transform.position = SourceObject.transform.position;
        SkillObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (SkillObject != null)
        {
            if (TargetObject != null)
            {
                var distance = Vector3.Distance(SkillObject.transform.position, TargetObject.transform.position);
                if (distance < 0.1f)
                {
                    Destroy(SkillObject);
                    SkillObject = null;
                    return;
                }

                iTween.MoveUpdate(SkillObject, TargetObject.transform.position, distance / MovingSpeed);
                SkillObject.transform.eulerAngles = Utils.GetTrackRotation(SkillObject.transform.position, TargetObject.transform.position);
            }
            else
            {
                Destroy(SkillObject);
                SkillObject = null;
            }
        }
    }

    void OnDestroy()
    {
        Destroy(SkillObject);
        SkillObject = null;
    }
}
