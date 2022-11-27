using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Prospector : MonoBehaviour
{
    static public Prospector S;
    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter;
    public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
    public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
    public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
    public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
    public float reloadDelay = 2f;
    public Text gameOverText, roundResultText, highScoreText;

    [Header("Set Dynamically")]
    public Deck deck;
    public LayoutProspector layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;
    public FloatingScore fsRun;

    List<CardProspector> ConvertListCardsToListCardProspector(List<Card> lCD)
    {
        List<CardProspector> lCP = new List<CardProspector>();
        CardProspector tCP;
        foreach (var tCD in lCD)
        {
            tCP = tCD as CardProspector;
            lCP.Add(tCP);
        }
        return lCP;
    }

    CardProspector Draw()
    {
        CardProspector cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }

    void LayoutGame()
    {
        if(layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        CardProspector cp;
        foreach (var tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUp = tSD.faceup;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3(
                    layout.multiplier.x * tSD.x,
                    layout.multiplier.y * tSD.y,
                    -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            cp.state = eCardState.tableau;
            cp.SetSortingLayerName(tSD.layerName);

            tableau.Add(cp);
        }

        foreach (var tCP in tableau)
        {
            foreach (var hid in tCP.slotDef.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }

        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach (var tCP in tableau)
        {
            if (tCP.layoutID == layoutID) return tCP;
        }
        return null;
    }

    void SetTableauFaces()
    {
        foreach (var cd in tableau)
        {
            bool faceUp = true;
            foreach (var cover in cd.hiddenBy)
            {
                if(cover.state == eCardState.tableau) faceUp = false;
            }
            cd.faceUp = faceUp;
        }
    }

    void MoveToDiscard(CardProspector cd)
    {
        Debug.Log("discard");
        //cd.state = eCardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID + 0.5f);

        //cd.faceUp = false;
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    void MoveToTarget(CardProspector cd)
    {
        if (target != null) MoveToDiscard(target);
        target = cd;
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID);
        cd.faceUp = true;
        //cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.originPos = cd.transform.localPosition;
        cd.SetSortingLayerName("Discard");
        cd.transform.Find("back").GetComponent<SpriteRenderer>().sortingLayerName = "Discard";
        cd.SetSortOrder(0);
    }

    void MoveToDrawPile()
    {
        if (drawPile.Count > 0) return;
        foreach (CardProspector cd in discardPile)
        {
            drawPile.Add(cd);
            cd.transform.Find("back").GetComponent<SpriteRenderer>().sortingLayerName = "Draw";
        }
        discardPile.Clear();

        //for (int i = 0; i < discardPile.Count; i++)
        //{
        //    drawPile.Add(discardPile.);
        //    discardPile.RemoveAt(0);
        //}
        UpdateDrawPile();
    }

    void UpdateDrawPile()
    {
        CardProspector cd;

        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(
            layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
            layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
            -layout.discardPile.layerID + 0.1f * i);

            cd.faceUp = false;
            cd.state = eCardState.drawpile;

            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }
    }

    public void CardClicked(CardProspector cd, ref bool isSuitSame)
    {
        switch (cd.state)
        {
            case eCardState.target:
                //cd.state = eCardState.set;
                if (isSuitSame)
                {
                    cd.SetSortingLayerName("Set");
                    cd.SetSortOrder(cd.rank);

                    target = null;

                    //CardManager의 Suit 리스트에 현재 카드를 저장
                    CardManager.CM.getList(cd).Add(cd);

                    cd.transform.position = cd.setPos;
                    isSuitSame = false;
                    cd.state = eCardState.set;
                }
                else
                {
                    cd.transform.position = cd.originPos;

                    cd.SetSortingLayerName(cd.originSortingLayerName);
                    cd.SetSortOrder(0);
                }
                break;
            case eCardState.drawpile:
                //if (target != null) return;
                MoveToTarget(Draw());
                UpdateDrawPile();
                MoveToDrawPile();
                ScoreManager.EVENT(eScoreEvent.draw);
                FloatingScoreHandler(eScoreEvent.draw);
                break;
            case eCardState.tableau:
                /*//bool validMatch = true;
                //if (!cd.faceUp)
                //{
                //    validMatch = false;
                //}
                //if (!AdjacentRank(cd, target))
                //{
                //    validMatch = false;
                //}
                //if (!validMatch) return;

                tableau.Remove(cd);
                //MoveToTarget(cd);
                cd.state = eCardState.set;
                SetTableauFaces();
                //ScoreManager.EVENT(eScoreEvent.mine);
                //FloatingScoreHandler(eScoreEvent.mine); */
                //마우스를 놓았을 때 조건이 일치하는지 확인
                if (isSuitSame)
                {
                    cd.SetSortingLayerName("Set");
                    cd.SetSortOrder(cd.rank);

                    //CardManager의 Suit 리스트에 현재 카드를 저장
                    CardManager.CM.getList(cd).Add(cd);

                    cd.transform.position = cd.setPos;
                    isSuitSame = false;
                    cd.state = eCardState.set;
                }
                // 카드를 원래 위치로 되돌림
                else
                {
                    cd.transform.position = cd.originPos;

                    cd.SetSortingLayerName(cd.originSortingLayerName);
                    cd.SetSortOrder(0);
                }
                break;
        }
        SetTableauFaces();
        //CheckForGameOver();
    }

    void FloatingScoreHandler(eScoreEvent evt)
    {
        List<Vector2> fsPts = new List<Vector2>();
        switch (evt)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLose:
                if (fsRun != null)
                {
                    fsPts.Add(fsPosRun);
                    fsPts.Add(fsPosMid2);
                    fsPts.Add(fsPosEnd);
                    fsRun.reportFinishTo = Scoreboard.S.gameObject;
                    fsRun.Init(fsPts, 0, 1);
                    fsRun.fontSizes = new List<float>() { 28, 36, 4 };
                    fsRun = null;
                }
                break;
            case eScoreEvent.mine:
                FloatingScore fs;
                Vector2 p0 = Input.mousePosition;
                p0.x /= Screen.width;
                p0.y /= Screen.height;
                fsPts.Add(p0);
                fsPts.Add(fsPosMid);
                fsPts.Add(fsPosRun);
                fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
                fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
                if (fsRun == null)
                {
                    fsRun = fs;
                    fsRun.reportFinishTo = null;
                }
                else
                {
                    fs.reportFinishTo = fsRun.gameObject;
                }
                break;
        }
    }

    public bool AdjacentRank(CardProspector c0, CardProspector c1)
    {
        if(!c0.faceUp || !c1.faceUp) return false;

        int diff = Mathf.Abs(c0.rank - c1.rank);
        if (diff == 1 || diff == 12) return true;

        return false;
    }

    void CheckForGameOver()
    {
        if (tableau.Count == 0)
        {
            GameOver(true);
            return;
        }

        if (drawPile.Count > 0) return;

        foreach (var cd in tableau)
        {
            if (AdjacentRank(cd, target)) return;
        }
        GameOver(false);
    }

    void GameOver(bool won)
    {
        int score = ScoreManager.SCORE;
        if (fsRun != null) score += fsRun.score;

        if (won)
        {
            gameOverText.text = "Round Over";
            roundResultText.text = "You won this round!\nRound Score: " + score;
            ShowResultsUI(true);

            //print("Game Over. you Win!");
            ScoreManager.EVENT(eScoreEvent.gameWin);
            FloatingScoreHandler(eScoreEvent.gameWin);
        }
        else
        {
            gameOverText.text = "Game Over";
            if (ScoreManager.HIGH_SCORE <= score)
            {
                string str = "You got the high score!\nHigh score: " + score;
                roundResultText.text = str;
            }
            else
            {
                roundResultText.text = "Your final score was: " + score;
            }
            ShowResultsUI(true);

            //print("Game Over. you Lose!");
            ScoreManager.EVENT(eScoreEvent.gameLose);
            FloatingScoreHandler(eScoreEvent.gameLose);

        }
        //SceneManager.LoadScene("SampleScene");
        Invoke("ReloadLevel", reloadDelay);
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void SetUpUITexts()
    {
        GameObject go = GameObject.Find("HighScore");
        highScoreText = go.GetComponent<Text>();
        int highScore = ScoreManager.HIGH_SCORE;
        string hScore = $"High Score: {highScore:N0}";
        highScoreText.text = hScore;
        go = GameObject.Find("GameOver");
        gameOverText = go.GetComponent<Text>();
        go = GameObject.Find("RoundResult");
        roundResultText = go.GetComponent<Text>();
        ShowResultsUI(false);
    }

    void ShowResultsUI(bool show)
    {
        gameOverText.gameObject.SetActive(show);
        roundResultText.gameObject.SetActive(show);
    }

    void Awake()        //모든 변수와 게임의상태 초기화 씬이 로드된 후 가장 처음호출
    {
        S = this;
        SetUpUITexts();
    }
    void Start()        //Update 메소드가 처음 호출되기 바로전에 호출
    {
        Scoreboard.S.score = ScoreManager.SCORE;

        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);

        /*Card c;
        for (int cNum = 0; cNum < deck.cards.Count; cNum++)
        {
            c = deck.cards[cNum];
            c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        }*/

        layout = GetComponent<LayoutProspector>();
        layout.ReadLayout(layoutXML.text);

        drawPile = ConvertListCardsToListCardProspector(deck.cards);
        LayoutGame();
        //target.transform.Find("back").GetComponent<SpriteRenderer>().sortingLayerName = ""
    }
}
