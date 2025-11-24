using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OpenDoor : MonoBehaviour
{
    [SerializeField] float timeBetweenMove, rotationY, timer;

    Coroutine coroutine;
    bool startCoroutine;
    public bool starMoveDoor;
    [SerializeField] bool doorIsOpen, thisIsLeftDoor, thisIsRightDoor, pushedDoor, test, isOpening, openWhileOpened;
    BoxCollider boxCollider;
    Quaternion rotation;
    Vector3 initialRotation;


    void Start()
    {
        //PushDoor(timeBetweenMove, rotationY);
        initialRotation.y = gameObject.transform.rotation.eulerAngles.y;
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        /*if (test)
        {
            test = false;
            Invoke(nameof(StartOpen), 0.1f);
        }*/
        /*rotation.y = Mathf.Clamp(rotation.y, initialRotation.y, initialRotation.y + 106f);
        rotation = gameObject.transform.rotation;
        
        if (gameObject.transform.rotation.eulerAngles.y >= initialRotation.y + 106f || gameObject.transform.rotation.eulerAngles.y < initialRotation.y)
        {
            //Debug.Log($"Rotation y = {rotation.y}");
            gameObject.transform.rotation = rotation;
        }*/
    }
    public void CallStartJustOpen()
    {
        Invoke(nameof(StartJustOpen), 0.1f);
    }
    public void CallStartPushOpen()
    {
        Invoke(nameof(StartPushOpen), 0.1f);
    }
    void StartJustOpen()
    {
        if(!isOpening) JustOpenDoor(timeBetweenMove, rotationY);
    }
    void StartPushOpen()
    {
        if (!isOpening) PushDoor(timeBetweenMove, rotationY);
    }
    void JustOpenDoor(float timeBetweenMove, float rotationY)
    {
        Debug.Log("starCoroutine");
        if (thisIsLeftDoor)
        {
            coroutine = StartCoroutine(OpensDoor(timeBetweenMove, -rotationY));
        }
        if (thisIsRightDoor)
        {
            coroutine = StartCoroutine(OpensDoor(timeBetweenMove, rotationY));
        }
    }
    public void PushDoor(float timeBetweenMove,float rotationY)
    {
        Debug.Log("starCoroutine");
        pushedDoor = true;
        if (thisIsLeftDoor)
        {
            coroutine = StartCoroutine(OpensDoor(timeBetweenMove, -rotationY * 10));
        }
        if (thisIsRightDoor)
        {
            coroutine = StartCoroutine(OpensDoor(timeBetweenMove, rotationY * 10));
        }
        
    }
    void CloseCoroutineOpenDoor(float timeBetweenMove)
    {
        timer += Time.fixedDeltaTime;
        if (pushedDoor)
        {
            if (timer >= timeBetweenMove * 45f)
            {
                starMoveDoor = false;
                doorIsOpen = true;
                StopCoroutine(coroutine);
                pushedDoor = false;
                isOpening = false;
                timer = 0;
            }
        }
        else
        {
            if (timer >= timeBetweenMove * 450f)
            {
                starMoveDoor = false;
                doorIsOpen = true;
                StopCoroutine(coroutine);
                isOpening = false;
                timer = 0;
            }
        }
        
    }
    void CloseCoroutineCloseDoor(float timeBetweenMove)
    {
        timer += Time.fixedDeltaTime;
        if (pushedDoor)
        {
            if (timer >= timeBetweenMove * 45f)
            {
                openWhileOpened = false;
                starMoveDoor = false;
                doorIsOpen = false;
                StopCoroutine(coroutine);
                pushedDoor = false;
                isOpening = false;
                timer = 0;
            }
        }
        else
        {
            if (timer >= timeBetweenMove * 450f)
            {
                openWhileOpened = false;
                starMoveDoor = false;
                doorIsOpen = false;
                StopCoroutine(coroutine);
                isOpening = false;
                timer = 0;
            }
        }
            
        
    }

    IEnumerator OpensDoor(float timeBetweenMove, float rotationY)
    {
        float step = 10 * Time.deltaTime;
        while (!startCoroutine)
        {            
            if (starMoveDoor && !doorIsOpen)
            {
                isOpening = true;
                CloseCoroutineOpenDoor(timeBetweenMove);
                gameObject.transform.Rotate(transform.rotation.x, rotationY, transform.rotation.z);
                yield return new WaitForSeconds(timeBetweenMove);                
                
            }   
            else if (starMoveDoor && doorIsOpen)
            {
                isOpening = true;
                CloseCoroutineCloseDoor(timeBetweenMove);
                gameObject.transform.Rotate(transform.rotation.x, -rotationY, transform.rotation.z);
                /*transform.rotation = Quaternion.RotateTowards(Quaternion.identity, Quaternion.Euler(0, -180, 0), step);
                step = Mathf.Clamp(step, initialRotation.y, initialRotation.y + 106f);
                step += 10 * Time.deltaTime;*/
                yield return new WaitForSeconds(timeBetweenMove);
            }
            yield return new WaitForSeconds(0f);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (doorIsOpen)
            {
                Vector3 playerPos = other.ClosestPoint(other.transform.position);
                Debug.Log(Vector3.Distance(playerPos, boxCollider.center.normalized));
                Debug.Log("normalized:" +Vector3.Distance(playerPos, boxCollider.center));
                if(Vector3.Distance(playerPos.normalized, boxCollider.center.normalized) < 115.1f)
                {
                    openWhileOpened = true;
                    starMoveDoor = true;
                    CallStartJustOpen();
                }
                else if (Vector3.Distance(playerPos, boxCollider.center) > 115.1f)
                {
                    openWhileOpened = true;
                    starMoveDoor = true;
                    doorIsOpen = false;
                    CallStartJustOpen();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (openWhileOpened)
            {
                starMoveDoor = false;
                StopCoroutine(coroutine);
                doorIsOpen = true;
                isOpening = false;
                timer = 0;
            }
            
        }
    }
}
