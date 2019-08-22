using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GestureRecognizer;
using System.Linq;

//NOTES: Currently, page 0 = page 1. When have main menu, set it back

public class PageManager : MonoBehaviour {

    Recognizer Recognizer;
    DrawDetector dd;

    //Speech Bubbles
    public int speech;                   //Speech count

    //Pages
    public int page;                     //Page count
    public int curPage;                  //Current page
    private int totalPage;               //Total number of pages
    public List<GameObject> pageSB;      //List of speech bubbles
    public GameObject[] pages;           //List of pages 

    //Determines if puzzle is solved, then can turn the page
    public List<bool> canTurnPage;
    public List<bool> isPuzzlePage;
    public List<bool> isDiaryPage;

    //DrawArea
    GameObject drawArea;
    GameObject eraser;

    //Diary and Puzzle stuff
    public List<GameObject> hintText;
    GameObject target;
    GameObject highlight;
    bool clickRightArrow;
    public int countTilCor;

    //Turn Page Button
    GameObject rightTurnPageButton;

    //Main Menu
    bool mainMenuOn;                       //Main menu is clicked
    GameObject mainMenuScreen;             //Main menu screen

    // Use this for initialization
    void Start () {

        countTilCor = 0;

        //Initialize variables
        pageSB = new List<GameObject>();
        pages = GameObject.FindGameObjectsWithTag("page").OrderBy(go => go.name).ToArray();
        totalPage = pages.Count();

        Recognizer = GameObject.Find("Recognizer").gameObject.GetComponent<Recognizer>();
        dd = GameObject.Find("DrawArea_Free").gameObject.GetComponent<DrawDetector>();
        drawArea = GameObject.Find("freeDraw");
        eraser = GameObject.Find("Eraser");
        rightTurnPageButton = GameObject.Find("forwardPage");
        mainMenuScreen = GameObject.Find("mainMenu");
        target = GameObject.Find("target");


        //Start at page 0 (main menu)
        page = 0;
        turnOnPage(page);
        mainMenuOn = true;
        mainMenuScreen.SetActive(false);

        //Turn off the right turn button in the beginning
        rightTurnPageButton.SetActive(false);

        //Speech Bubbles
        speech = 0;

        //Determining which pages can be turned immediately
        for (int i = 0; i < totalPage; i++)
        {
            canTurnPage.Add(false);
        }

        //Determining which pages are diary pages
        for(int i = 0; i < totalPage; i++ )
        {
            isDiaryPage.Add(false);
        }
        isDiaryPage[2] = true;
        isDiaryPage[6] = true;
        isDiaryPage[10] = true;
        isDiaryPage[14] = true;
        isDiaryPage[18] = true;
        isDiaryPage[22] = true;
        isDiaryPage[26] = true;

        //Determining which pages are puzzle
        for (int i = 0; i < totalPage; i++)
        {
            isPuzzlePage.Add(false);
        }
        isPuzzlePage[3] = true;
        isPuzzlePage[7] = true;
        isPuzzlePage[11] = true;
        isPuzzlePage[15] = true;
        isPuzzlePage[19] = true;
        isPuzzlePage[23] = true;
        isPuzzlePage[27] = true;

        //Set false on all puzzle pages, drawAreas needs to be set by hand
        drawArea.SetActive(false);
        eraser.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        curPage = page;

        //For every time the player turns the page, set the right turn button off
        if (speech == 0)
        {
            rightTurnPageButton.SetActive(false);

            //If the player went through all the dialogue, if go back they can immediately turn the page
            if(canTurnPage[page] == true)
            {
                rightTurnPageButton.SetActive(true);
            }
        }

        //If player is on a puzzle page, they can't turn the page and the pattern they need to draw is updated
        if (canTurnPage[page] == false)
        {
            updateGestureList(page);

            //If the puzzle is solved, turn on animations and allow them to turn the page
            if(dd.puzzleSolved == true)
            {
                puzzleSolved(dd.puzzleSolved);
                dd.puzzleSolved = false;
                isPuzzlePage[page] = false;
                speech = 1;
                rightTurnPageButton.SetActive(true);
                canTurnPage[page] = true;
                drawArea.SetActive(false);
            }
        }
    }

    /* ******************
     *    FUNCTIONS     *
     * **************** */

    //Main homie --> turns on a page
    void turnOnPage(int page)
    {
        turnOffAllPages();
        pages[page].SetActive(true);

        getSpeechBubbles(pages[page]);
        turnOffAllSB();
        speech = 0;

        //Turns on draw areas at puzzle pages
        if (page == 3 || page == 7 || page == 11 || page == 15 || page == 19 || page == 23 || page == 27)
        {
            drawArea.SetActive(true);
            eraser.SetActive(true);
        }
        else
        {
            drawArea.SetActive(false);
            eraser.SetActive(false);
        }
    }

    //Turns off all pages
    void turnOffAllPages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
    }

    //gets all the speech bubbles in this page that we are currently on
    void getSpeechBubbles(GameObject thisPage)
    {
        pageSB.Clear();
        foreach (Transform child in thisPage.transform)
        {
            pageSB.Add(child.gameObject);
        }
    }

    //turns off all speech bubbles in this page that we are currently on
    void turnOffAllSB()
    {
        for (int i = 0; i < pageSB.Count; i++)
        {
            pageSB[i].SetActive(false);
        }
    }

    //Turns on speech bubble
    void turnOnSB(int index)
    {
        turnOffAllSB();
        for (int i = 0; i < index; i++)
        {
            pageSB[i].SetActive(true);
        }
    }

    //Button to turn the page (>>)
    public void rightArrowOnClickPages()
    {
        //If finished going through the speech bubbles, allow the page to be turned
        if (canTurnPage[page])
        {
            page++;
            rightTurnPageButton.SetActive(true);

            //Make sure to update
            if (page > totalPage)
            {
                page = totalPage;
            }

            turnOnPage(page);
        }
        else
        {
            updateGestureList(page);
            rightTurnPageButton.SetActive(false);
        }
    }

    //Button to turn the page (<<)
    public void leftArrowOnClickPages()
    {
        //If finished going through the speech bubbles, allow the page to be turned
        page--;

        if (page < 0)
        {
            page = 0;
        }

        turnOnPage(page);
    }

    //Button to turn on each speech bubble (>)
    public void rightArrowOnClickBubbles()
    {
        Debug.Log("pageSB: " + pageSB.Count);
        Debug.Log("speech: " + speech);
        Debug.Log("CountTilCor: " + countTilCor);

        //If the page is a diary page, allow certain animations to turn on.
        if (curPage == page && isDiaryPage[page] == true && !canTurnPage[page])
        {
            turnOnSB(speech + 1);
            //canTurnPage[page] = false;

            //if ((speech >= pageSB.Count) && countTilCor >= 1)
            if (countTilCor >= 1)
            {
               
                rightClickForDiary(speech);
            }

            else
            {
                countTilCor++;
            }
        }

        //Increase count of speech bubbles
        else if (isPuzzlePage[page] == false)
        {
            speech++;
        }

        //If the player read through all the speech bubbles, they can turn the page
        if ((speech >= pageSB.Count) || (canTurnPage[page] == true))
        {
            canTurnPage[page] = true;
            rightTurnPageButton.SetActive(true);
        }
        else
        {
            rightTurnPageButton.SetActive(false);
        }

        //If it's a normal comic page
        if (isPuzzlePage[page] == false)
        {
            //If they go through all the speech bubbles
            if (speech > pageSB.Count)
            {
                //Turns the page
                if (canTurnPage[page])
                {
                    page++;
                    turnOnPage(page);
                }
                else
                {
                    speech = pageSB.Count - 1;
                }
            }
            //Or else keep showing speech bubbles
            else if (countTilCor <= 0)
            {
                turnOnSB(speech);
            }
        }

        //To keep the count of the children with the speech int for Diary pages
        if (curPage == page && isDiaryPage[page] == true)
        {
            //if (speech > pageSB.Count)
            //{
            //    speech = pageSB.Count - 1;
            //}
        }
    }

    //Right click for diary page toa activate animation
    public void rightClickForDiary(int index)
    {
        //If the page is a diary page, allow certain animations to turn on.
        if (curPage == page && isDiaryPage[page] == true)
        {
            getHintText(pageSB[index]);
            //Fade out diary text
            if (pageSB[index].tag == "DiaryText")
            {
                StartCoroutine(fadeOut(pageSB[index]));
            }

            //Fade out siggy and underline
            for (int i = 0; i < hintText.Count(); i++)
            {
                //Fade out siggy and outline
                if (hintText[i].tag == "siggy")
                {
                    StartCoroutine(fadeOutImage(hintText[i]));
                }

                //Fade in black screen
                if (hintText[i].tag == "blackScreen")
                {
                    StartCoroutine(fadeIn(hintText[i]));
                }

                //Move the hint text to the top of the page
                if (hintText[i].tag == "hintText")
                {
                    StartCoroutine(moveText(hintText[i], target));
                }
            }


        }

        
    }

    //Button to remove each speech bubble (<)
    public void leftArrowOnClickBubbles()
    {
        //Increase count of speech bubbles
        speech--;
        if (speech < 0)
        {
            if (page > 0)
            {
                page--;
                turnOnPage(page);
            }
            else
            {
                speech = 0;
            }
        }
        else
        {
            turnOnSB(speech);
        }
    }

    //Main Menu Button
    public void mainMenuOnClick()
    {
        if(mainMenuOn)
        {
            mainMenuScreen.SetActive(true);
            mainMenuOn = false;
        }
        else
        {
            mainMenuScreen.SetActive(false);
            mainMenuOn = true;
        }
    }

    //Erase Button
    public void eraseButtonClick()
    {
        dd.ClearLines();
    }

    //For puzzle pages, place the appropriate "right" gestures into the recognizer.
    void updateGestureList(int pageNumber)
    {
        switch (pageNumber)
        {
            case 3:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[0]);
                Recognizer.patterns.Add(Recognizer.gestureList[1]);
                break;
            case 7:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[2]);
                break;
            case 11:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[3]);
                break;
            case 15:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[4]);
                Recognizer.patterns.Add(Recognizer.gestureList[5]);
                break;
            case 19:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[6]);
                Recognizer.patterns.Add(Recognizer.gestureList[7]);
                break;
            case 23:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[8]);
                Recognizer.patterns.Add(Recognizer.gestureList[9]);
                break;
            case 27:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[10]);
                Recognizer.patterns.Add(Recognizer.gestureList[11]);
                break;

                //default:
                //    Recognizer.patterns.Clear();
                //    Recognizer.patterns = Recognizer.gestureList;
                //    break;
        }
    }

    //Page 1 Bubble
    void bubbleUpdate(int pageNumber)
    {
        rightArrowOnClickPages();
        switch (pageNumber)
        {
            case 3:
                Recognizer.patterns.Clear();
                Recognizer.patterns.Add(Recognizer.gestureList[0]);
                break;
            default:
                Recognizer.patterns.Clear();
                Recognizer.patterns = Recognizer.gestureList;
                break;
        }
    }

    //If the player solves the puzzle, allow them to see animation
    void puzzleSolved(bool isSolved)
    {
         dd.ClearLines();
         for (int i = 0; i < pageSB.Count(); i++)
         {
             pageSB[i].SetActive(true);
         }
         isSolved = false;       
    }

    //Gets the grandchildren of the children in all diary pages
    void getHintText(GameObject thisHint)
    {
        hintText.Clear();
        foreach (Transform child in thisHint.transform)
        {
            hintText.Add(child.gameObject);
        }
    }

    //Fades out the text 
    IEnumerator fadeOut(GameObject text)
    {
        while (text.GetComponent<Text>().color.a > 0.0f)
        {
           
            text.GetComponent<Text>().color = new Color(text.GetComponent<Text>().color.r,
                text.GetComponent<Text>().color.g, text.GetComponent<Text>().color.b,
                text.GetComponent<Text>().color.a - .01f);
            yield return new WaitForSeconds(1/50f);  
        }
    }

    //Fades out the image
    IEnumerator fadeOutImage(GameObject image)
    {
        while (image.GetComponent<Image>().color.a > 0.0f)
        {

            image.GetComponent<Image>().color = new Color(image.GetComponent<Image>().color.r,
                image.GetComponent<Image>().color.g, image.GetComponent<Image>().color.b,
                image.GetComponent<Image>().color.a - .01f);
            yield return new WaitForSeconds(1 / 50f);
        }
    }

    //Fades in image
    IEnumerator fadeIn(GameObject image)
    {
        while (image.GetComponent<SpriteRenderer>().color.a < 1.0f)
        {
            image.GetComponent<SpriteRenderer>().color = new Color(image.GetComponent<SpriteRenderer>().color.r,
                image.GetComponent<SpriteRenderer>().color.g, image.GetComponent<SpriteRenderer>().color.b,
                image.GetComponent<SpriteRenderer>().color.a + .01f);
            yield return new WaitForSeconds(1 / 50f);
        }
    }

    //Moves text 
    IEnumerator moveText(GameObject hintText, GameObject target)
    {
        //while ((hintText.GetComponent<RectTransform>().position.x < 686) && (hintText.GetComponent<RectTransform>().position.y < 100))
        //{
        //    Vector3 curPos;
        //    curPos = new Vector3(hintText.GetComponent<RectTransform>().position.x, hintText.GetComponent<RectTransform>().position.y,
        //        hintText.GetComponent<RectTransform>().position.z);

        //    hintText.GetComponent<RectTransform>().position = new Vector3(curPos.x + 1.5f, curPos.y + 1.5f, curPos.z);

        //    yield return new WaitForSeconds(1 / 100f);
        //}
       
        Vector3 curPos;
        Vector3 targetPos;

        curPos = new Vector3(hintText.GetComponent<RectTransform>().position.x, hintText.GetComponent<RectTransform>().position.y,
                hintText.GetComponent<RectTransform>().position.z);
        targetPos = new Vector3(target.GetComponent<RectTransform>().position.x, target.GetComponent<RectTransform>().position.y,
            target.GetComponent<RectTransform>().position.z);

        while (curPos.y < targetPos.y)
        {
            float step = 400 * Time.deltaTime;
            curPos = new Vector3(hintText.GetComponent<RectTransform>().position.x, hintText.GetComponent<RectTransform>().position.y,
                hintText.GetComponent<RectTransform>().position.z);

            hintText.GetComponent<RectTransform>().position = Vector3.MoveTowards(curPos, targetPos, step);

            yield return new WaitForSeconds(1 / 200f);
        }

        canTurnPage[page] = true;
        countTilCor = 0;
        speech++;
    }

}
