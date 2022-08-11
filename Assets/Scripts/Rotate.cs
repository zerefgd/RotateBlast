using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float _startDelay;

    [SerializeField]
    private float _rotateTime, _maxAngle;

    private bool isGameFinished;

    private void Start()
    {
        isGameFinished = false;
        StartCoroutine(IRotate());
    }

    private void OnEnable()
    {
        GameManager.GameEnded += GameEnded;
    }

    private void OnDisable()
    {
        GameManager.GameEnded -= GameEnded;
    }

    private void GameEnded()
    {
        isGameFinished = true;
    }

    private IEnumerator IRotate()
    {
        yield return new WaitForSeconds(_startDelay);

        float timeElapsed = 0f;
        float timeToComplete = Random.Range(0.75f,1.25f) * _rotateTime;
        float angle = Random.Range(0.75f, 1.25f) * _maxAngle;        
        float speed = 1 / timeToComplete;
        float directionMagnitude = 1f;
        var fixedUpdate = new WaitForFixedUpdate();
        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 tempRotation;

        while(!isGameFinished)
        {
            while(timeElapsed <= 1f && !isGameFinished)
            {
                timeElapsed += speed * Time.fixedDeltaTime;
                tempRotation = startRotation + directionMagnitude * timeElapsed * angle * Vector3.forward;
                transform.rotation = Quaternion.Euler(tempRotation);
                yield return fixedUpdate;
            }

            timeElapsed = 0f;
            timeToComplete = Random.Range(0.75f, 1.25f) * _rotateTime;
            angle = Random.Range(0.75f, 1.25f) * _maxAngle;
            speed = 1 / timeToComplete;
            directionMagnitude *= Random.Range(0,2) == 0  ? -1f : 1f;
            startRotation = transform.rotation.eulerAngles;
        }

    }

}
