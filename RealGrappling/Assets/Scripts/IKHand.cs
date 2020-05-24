using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHand : MonoBehaviour
{
    public Vector3 target;
    public GameObject gun;
    public Camera playerCam;

    Animator anim;
    float weight = 100;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        target = gun.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        target = gun.transform.position;
        transform.forward = playerCam.transform.forward;
        transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPosition(AvatarIKGoal.RightHand, target);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
    }
}
