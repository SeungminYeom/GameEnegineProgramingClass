using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard,
    set
}
public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]
    public eCardState           state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    public int                  layoutID;
    public SlotDefProspector    slotDef;
    public string               originSortingLayerName;

    bool isSuitSame = false;
    public Vector3 setPos;
    public Vector3 originPos;

    private void Start()
    {
        //카드의 원래 위치를 저장
        originPos = transform.position;
        originSortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
    }

    override public void OnMouseUpAsButton()
    {
        Prospector.S.CardClicked(this, ref isSuitSame);
        //base.OnMouseUpAsButton();

    }

    public void OnMouseDrag()
    {
        //if ((faceUp && state == eCardState.tableau) || (faceUp && state == eCardState.target))
        if (faceUp && state != eCardState.set)
        {
            //마우스를 클릭하면 스크린 상의 마우스 위치를 저장하고, 선택한 카드가 가장 앞에 보이도록 z를 -15로 만듦
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            pos.z = -15;

            transform.position = pos;

            SetSortingLayerName("Clicking");
            SetSortOrder(10);
        }
        //else if (faceUp && state == eCardState.drawpile)
        //{
        //    //마우스를 클릭하면 스크린 상의 마우스 위치를 저장하고, 선택한 카드가 가장 앞에 보이도록 z를 -15로 만듦
        //    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        //    pos.z = -15;

        //    transform.position = pos;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //카드를 Set으로 옮길 시 Set의 태그와 카드의 suit를 비교
        if (other.tag == suit)
        {
            //CardManager의 Suit 리스트의 Count와 Rank를 비교하여 카드를 놓을 수 있는지 확인
            if (rank - CardManager.CM.getList(this).Count == 1)
            {
                isSuitSame = true;
                //카드가 자리해야 할 위치를 setPos에 저장
                setPos = new Vector3(other.transform.position.x, other.transform.position.y, (float)CardManager.CM.getList(this).Count * -0.5f);
                Debug.Log(isSuitSame);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isSuitSame = false;
    }
}
