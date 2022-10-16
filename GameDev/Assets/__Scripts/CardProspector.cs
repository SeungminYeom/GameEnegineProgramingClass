using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]
    public eCardState           state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    public int                  layoutID;
    public SlotDefProspector    slotDef;

    bool isSuitSame = false;
    Vector3 setPos;
    Vector3 originPos;

    private void Start()
    {
        //카드의 원래 위치를 저장
        originPos = transform.position;
    }

    override public void OnMouseUpAsButton()
    {
        //Prospector.S.CardClicked(this);
        base.OnMouseUpAsButton();

        //마우스를 놓았을 때 조건이 일치하는지 확인
        if (isSuitSame)
        {
            SetSortingLayerName("Set");
            SetSortOrder(rank);

            //CardManager의 Suit 리스트에 현재 카드를 저장
            CardManager.CM.getList(this).Add(this);

            transform.position = setPos;
            isSuitSame = false;
        }
        // 카드를 원래 위치로 되돌림
        else transform.position = originPos;
    }

    public void OnMouseDrag()
    {
        //마우스를 클릭하면 스크린 상의 마우스 위치를 저장하고, 선택한 카드가 가장 앞에 보이도록 z를 -15로 만듦
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        pos.z = -15;

        transform.position = pos;
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
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isSuitSame = false;
    }
}
