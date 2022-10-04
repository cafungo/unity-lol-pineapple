using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Piña explosiva que envia a volar a jugadores o personajes dentro de su área de explosión
/// </summary>
public class PineApple : MonoBehaviour
{
    
    [Header("Basic")]
    public float pineAppleCooldown = 2.0f;

    [Header("Fly Explotion")]
    public float explotionFlyDistance = 5.0f;                       //Distancia de vuelo de la explosión
    public float explotionFlySpeed = 10.0f;                         //Velocidad de translado de la explosion hacia el punto final
    public float flyArcHeight = 1.0f;                            //Altura del arco de vuelo
    [Header("Visual Element")]
    public GameObject corpse;                                       //Cuerpo principal de la piña
    public GameObject eyeLocalPlayerFly;                         //Circulo para indicar la posicion final del vuelo del jugador en explosión
    [Header("Cursor")]
    public Texture2D cursorWeaponTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private bool isExploded = false;
    private List<Player> charactersInside = new List<Player>();         //Lista de jugadores o personajes dentro del area de la piña

    Player localPlayerStay = null;
    private float nextRestart;
    bool inCooldown = false;

    private void Update()
    {
        eyeLocalPlayerFly.SetActive(localPlayerStay != null && !inCooldown);

        
        if (inCooldown)
        {
            if (Time.time > nextRestart)
            {
                Restart();
            }
        }else
        {
            if (localPlayerStay != null)
            {
                Vector3 flyEndPos = ExplotionFlyPosition(localPlayerStay.gameObject);
                eyeLocalPlayerFly.transform.position = new Vector3(flyEndPos.x, eyeLocalPlayerFly.transform.position.y, flyEndPos.z);
            }
        }
        
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            charactersInside.Add(player);

            if (player.isLocalPlayer)
                localPlayerStay = player;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            charactersInside.Remove(player);
            if (player.isLocalPlayer)
                localPlayerStay = null;
        }
    }
    

    public void PointerEnter()
    {
        Cursor.SetCursor(cursorWeaponTexture, hotSpot, cursorMode);
    }
    public void PointerExit()
    {
        Cursor.SetCursor(null, hotSpot, cursorMode);
    }
    public void PointerClick()
    {
        Explode();
    }
    /// <summary>
    /// Efectua una explosion para cada personaje dentro del diametro de la piña
    /// </summary>
    void Explode()
    {
        Cooldown();


        Debug.Log("Exploded");
        foreach( var character in charactersInside)
        {
            Vector3 explotionFinalPos = ExplotionFlyPosition(character.gameObject);
            character.Fly(explotionFinalPos, explotionFlySpeed, flyArcHeight);
        } 
    }

    Vector3 ExplotionFlyPosition(GameObject target)
    {
        Vector3 pos = transform.position;
        Vector3 dir = (target.transform.position - pos).normalized;
        dir.y = 0;

        return pos + dir * explotionFlyDistance;
    }

    void Restart()
    {
        corpse.SetActive(true);
        inCooldown = false;
    }
    void Cooldown()
    {
        inCooldown = true;
        corpse.SetActive(false);
        nextRestart = Time.time + pineAppleCooldown;
    }
}
