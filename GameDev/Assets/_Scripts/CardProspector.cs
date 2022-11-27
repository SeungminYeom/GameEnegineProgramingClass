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
        //ī���� ���� ��ġ�� ����
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
            //���콺�� Ŭ���ϸ� ��ũ�� ���� ���콺 ��ġ�� �����ϰ�, ������ ī�尡 ���� �տ� ���̵��� z�� -15�� ����
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            pos.z = -15;

            transform.position = pos;

            SetSortingLayerName("Clicking");
            SetSortOrder(10);
        }
        //else if (faceUp && state == eCardState.drawpile)
        //{
        //    //���콺�� Ŭ���ϸ� ��ũ�� ���� ���콺 ��ġ�� �����ϰ�, ������ ī�尡 ���� �տ� ���̵��� z�� -15�� ����
        //    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        //    pos.z = -15;

        //    transform.position = pos;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //ī�带 Set���� �ű� �� Set�� �±׿� ī���� suit�� ��
        if (other.tag == suit)
        {
            //CardManager�� Suit ����Ʈ�� Count�� Rank�� ���Ͽ� ī�带 ���� �� �ִ��� Ȯ��
            if (rank - CardManager.CM.getList(this).Count == 1)
            {
                isSuitSame = true;
                //ī�尡 �ڸ��ؾ� �� ��ġ�� setPos�� ����
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
