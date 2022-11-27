using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDefProspector
{
    public float x;
    public float y;
    public bool faceup = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}

public class LayoutProspector : MonoBehaviour
{
    public PT_XMLReader     xmlr;
    public PT_XMLHashtable  xml;
    public Vector2          multiplier;

    public List<SlotDefProspector> slotDefs;
    public SlotDefProspector drawPile;
    public SlotDefProspector discardPile;

    public string[] sortingLayerName = new string[]
    {
        "Row0","Row1","Row2","Row3","Draw","Discard"
    };

    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0];

        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        SlotDefProspector tSD;
        PT_XMLHashList slotsX = xml["slot"];

        for(int i=0; i < slotsX.Count; i++)
        {
            tSD = new SlotDefProspector();

            if (slotsX[i].HasAtt("type"))
                {
                    tSD.type = slotsX[i].att("type");
                }
            else
            {
                tSD.type = "slot";
            }

            tSD.x = float.Parse(slotsX[i].att("x"));
            tSD.y = float.Parse(slotsX[i].att("y"));
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            tSD.layerName = sortingLayerName[tSD.layerID];

            switch (tSD.type)
            {
                case "slot":
                    tSD.faceup = (slotsX[i].att("faceup") == "1");
                    tSD.id = int.Parse(slotsX[i].att("id"));
                    if (slotsX[i].HasAtt("hiddenby"))
                    {
                        //string[] hidings = slotsX[i].att("hiddenby").Split(',');
                        //foreach (var s in hidings)
                        //{
                        //    tSD.hiddenBy.Add(int.Parse(s));
                        //}
                        string hiding = slotsX[i].att("hiddenby");
                        if (hiding != "")
                            tSD.hiddenBy.Add(int.Parse(hiding));
                    }
                    slotDefs.Add(tSD);
                    break;
                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
                    tSD.faceup = true;
                    drawPile = tSD;
                    break;
                case "discardpile":
                    discardPile = tSD;
                    break;
            }
        }
    }
}
