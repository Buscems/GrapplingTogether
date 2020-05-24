using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Death : MonoBehaviour
{

    public Vector3 direction;
    public float speed;
    public float waitTime;
    bool start;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartMove");
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            transform.position += direction * speed;
        }
    }

    IEnumerator StartMove()
    {
        yield return new WaitForSeconds(waitTime);
        start = true;
    }

}
