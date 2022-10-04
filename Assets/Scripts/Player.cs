using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    StarterAssets.ThirdPersonController characterController;
    public bool isLocalPlayer;                                  //Identificador de jugador principal para diferenciar de otros en el juego
    
    
    bool isFlying = false;


    private void Awake()
    {
        characterController = GetComponent<StarterAssets.ThirdPersonController>();
    }
    public void Fly(Vector3 targetPos, float flySpeed, float flyArcHeight)
    {
        if (isFlying)
            return;

        StartCoroutine(FlyingSecuence(targetPos, flySpeed, flyArcHeight));
    }

    IEnumerator FlyingSecuence(Vector3 targetPos, float flySpeed, float flyArcHeight)
    {
        ChangeFlyingStatus(true);

        //float progress = 0.0f;
        //float stepScale = flySpeed / Vector3.Distance(transform.position, targetPos);
        Vector3[] point = new Vector3[3];
        point[0] = transform.position;          //Posición inicial
        point[2] = targetPos;                   //Posición final
        point[1] = point[0] + (point[2] - point[0]) / 2 + Vector3.up * flyArcHeight;        //Control del arco
        

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.position, targetPos);

        //Bucle de vuelo
        while (Vector3.Distance(transform.position, targetPos)>0.1f)
        {
            float distCovered = (Time.time - startTime) * flySpeed;
            float fractionOfJourney = distCovered / journeyLength;

            //Usamos Bezier Curve para calcular el arco de vuelo entre A-B
            Vector3 p1 = Vector3.Lerp(point[0], point[1], fractionOfJourney);
            Vector3 p2 = Vector3.Lerp(point[1], point[2], fractionOfJourney);

            transform.position = Vector3.Lerp(p1, p2, fractionOfJourney);

            yield return null;
        }

        ChangeFlyingStatus(false);
    }
    void ChangeFlyingStatus(bool _isFlying)
    {
        isFlying = _isFlying;
        if(characterController!=null)
            characterController.enabled = !isFlying;
    }
}
