using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager CM;

    public List<CardProspector> Spade;
    public List<CardProspector> Diamond;
    public List<CardProspector> Heart;
    public List<CardProspector> Club;

    int layer_s, layer_d, layer_h, layer_c;

    private void Awake()
    {
        CM = this;
    }

    void Start()
    {
        layer_s = layer_d = layer_h = layer_c = 0;
    }

    public List<CardProspector> getList(CardProspector cd)
    {
        switch(cd.suit)
        {
            case "S":
                return Spade;
            case "D":
                return Diamond;
            case "H":
                return Heart;
        }
        return Club;
    }

    //public void setCard(CardProspector cd)
    //{
    //    switch(cd.suit)
    //    {
    //        case "S":
    //            Spade.Add(cd);
    //            layer_s++;
    //            break;
    //        case "D":
    //            Diamond.Add(cd);
    //            layer_d++;
    //            break;
    //        case "H":
    //            Heart.Add(cd);
    //            layer_h++;
    //            break;
    //        case "C":
    //            Club.Add(cd);
    //            layer_c++;
    //            break;
    //    }
    //    Debug.Log("S: " + Spade.Count + " / D: " + Diamond.Count + " / H: " + Heart.Count + " / C: " + Club.Count);
    //}


    void Update()
    {
        
    }
}
