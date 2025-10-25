using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OpenDoor : MonoBehaviour
{
    [SerializeField] float timeBetweenMove, rotationY, timer;

    Coroutine coroutine;
    bool startCoroutine;
    [SerializeField] bool starMoveDoor, doorIsOpen, thisIsLeftDoor, thisIsRightDoor, pushedDoor, test;
    
    void Start()
    {
        starMoveDoor = true;
        PushDoor(timeBetweenMove, rotationY);
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            Invoke(nameof(StartOpen), 0.5f);
        }
    }
    public void CallStartOpen()
    {
        Invoke(nameof(StartOpen), 0.5f);
    }
    void StartOpen()
    {
        PushDoor(timeBetweenMove, rotationY);
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
                starMoveDoor = false;
                doorIsOpen = false;
                StopCoroutine(coroutine);
                pushedDoor = false;
                timer = 0;
            }
        }
        else
        {
            if (timer >= timeBetweenMove * 450f)
            {
                starMoveDoor = false;
                doorIsOpen = false;
                StopCoroutine(coroutine);
                timer = 0;
            }
        }
            
        
    }

    IEnumerator OpensDoor(float timeBetweenMove, float rotationY)
    {
        
        while (!startCoroutine)
        {            
            if (starMoveDoor && !doorIsOpen)
            {
                CloseCoroutineOpenDoor(timeBetweenMove);
                gameObject.transform.Rotate(transform.rotation.x, rotationY, transform.rotation.z);
                yield return new WaitForSeconds(timeBetweenMove);                
                
            }   
            else if (starMoveDoor && doorIsOpen)
            {
                CloseCoroutineCloseDoor(timeBetweenMove);
                gameObject.transform.Rotate(transform.rotation.x, -rotationY, transform.rotation.z);
                yield return new WaitForSeconds(timeBetweenMove);                
                
            }
            yield return new WaitForSeconds(0f);
        }
        
    }
}
