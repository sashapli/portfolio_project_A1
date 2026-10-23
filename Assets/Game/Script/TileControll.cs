using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;

public class TileControll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject CursorPanel;
    public GameObject CursorGrag;
    public Vector3 startPos;
    public Vector3Int Derect;
    public float Derec1t;
    public bool activ;
    public bool drag = true;
    public Vector2 CurentPos;

    public static TileControll currentHovered; // кто сейчас подсвечен

    public void Ineciliztion()
    {
        activ = true;
        startPos = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        drag = true;
        CursorPanel.SetActive(false);
    }    
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOMove(startPos, 0.15f);
        drag = false;
        Derect = Vector3Int.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!drag) return;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        transform.position = worldPos;
        CurentPos = worldPos - startPos;
        if (CurentPos.magnitude > 0.7f)
        {
            Derect = new Vector3Int(Mathf.RoundToInt(CurentPos.x), Mathf.RoundToInt(CurentPos.y));
            currentHovered = TileEliment.Singalton.PosEliment
                .Where(t => t.grapPosl.position == new Vector3(startPos.x + Derect.x, startPos.y + Derect.y))
                .Select(t => t.grapPosl.GetComponent<TileControll>())
                .FirstOrDefault();
            transform.DOMove(startPos + Derect, 0.5f);
            currentHovered.transform.DOMove(currentHovered.startPos - Derect,0.5f);
            currentHovered.startPos -= Derect;
            startPos += Derect;
            drag = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!drag)
        CursorPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) 
    {

    }
}