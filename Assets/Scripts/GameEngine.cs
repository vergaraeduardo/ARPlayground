using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

enum GameState
{
    Start,
    FirstStep,
    SecondStep,
    ThirdStep,
    Completed
}

namespace Assets.Scripts
{
    public class GameEngine : MonoBehaviour
    {
        public GameEngine Instance;

        //----GameObject Variables--------
        public GameObject[] gamePages;
        public List<GameObject> trackedObjects;
        public GameObject[] cubesOnPage4;
        public GameObject[] imageTargets;
        public GameObject winningLogo;
        public GameObject flower;
        public GameObject okOnPage3;
        public GameObject okOnPage4;
        public Image levelProgress;
        public Sprite[] NumberSprites;
        public Text debugText;
        //---------------------------------

        GameObject Number2;
        GameObject Number3;
        GameObject Number4;
        GameObject Number5;
        GameObject Number6;
        GameObject Number7;
        GameObject Number8;
        GameObject Number9;
        GameObject Plus;
        GameObject Minus;
        GameObject Divide;
        GameObject Multiply;

        //----String Variables----
        public string targetName;
        public string symbol;
        //------------------------

        //---Float & Int Variables-------
        public float targetYPosition;
        public float targetXPosition;
        public float comparedHeight = 0;
        public float detectedNumber;
        public float firstNumber;
        public float secondNumber;
        public float[] levelTargets;
        float finalResult;
        public int currentLevel;
        public int targetImageNumber;
        public float xDistanceThreshold = 0.13f;
        //------------------------------

        GameState currentGameState;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Matrix4x4 mat = Camera.main.projectionMatrix;
            mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
            Camera.main.projectionMatrix = mat;

            currentGameState = GameState.Start;
            StartCoroutine("DelayShowPage0");

            Number2 = GameObject.FindGameObjectWithTag("number2");
            Number3 = GameObject.FindGameObjectWithTag("number3");
            Number4 = GameObject.FindGameObjectWithTag("number4");
            Number5 = GameObject.FindGameObjectWithTag("number5");
            Number6 = GameObject.FindGameObjectWithTag("number6");
            Number7 = GameObject.FindGameObjectWithTag("number7");
            Number8 = GameObject.FindGameObjectWithTag("number8");
            Number9 = GameObject.FindGameObjectWithTag("number9");
            Plus = GameObject.FindGameObjectWithTag("plus");
            Minus = GameObject.FindGameObjectWithTag("minus");
            Divide = GameObject.FindGameObjectWithTag("divide");
            Multiply = GameObject.FindGameObjectWithTag("supply");
        }

        void Update()
        {
            GameLogic();
            LogState();
        }

        //----------Delay Functions------------
        IEnumerator DelayShowPage0()
        {
            yield return new WaitForSeconds(3);

            gamePages[0].SetActive(false);
            gamePages[1].SetActive(true);
            StartCoroutine("DelayShowPage1");
        }

        IEnumerator DelayShowPage1()
        {
            yield return new WaitForSeconds(3);

            gamePages[1].SetActive(false);
            gamePages[2].SetActive(true);
            GameFormat();
        }

        IEnumerator DelayShowPage2()
        {
            yield return new WaitForSeconds(3);

            GameFormat();
            gamePages[2].SetActive(false);
            gamePages[3].SetActive(true);
        }

        IEnumerator CompletedDelay()
        {
            yield return new WaitForSeconds(2);

            okOnPage4.SetActive(false);
        }

        IEnumerator RestartDelay()
        {
            yield return new WaitForSeconds(1);

            gamePages[2].SetActive(false);
            gamePages[3].SetActive(true);
            gamePages[4].SetActive(false);
            GameFormat();
        }

        IEnumerator WinningDelay1()
        {
            yield return new WaitForSeconds(2);

            okOnPage4.SetActive(false);
            winningLogo.SetActive(true);
            flower.SetActive(true);
            yield return new WaitForSeconds(2);

            gamePages[3].SetActive(false);
            gamePages[4].SetActive(true);
            winningLogo.SetActive(false);
            flower.SetActive(false);
            StartCoroutine("RestartDelay");
        }
        //---------------------------------------

        //-------Format Game State & Variables---------------------------------------------------------------------
        public void GameFormat()
        {
            currentGameState = GameState.FirstStep;
            firstNumber = 0;
            secondNumber = 0;
            finalResult = 0;
            symbol = "";
            targetXPosition = 10000;
            targetYPosition = 10000;
            gamePages[3].GetComponentsInChildren<UnityEngine.UI.Image>()[1].sprite = NumberSprites[currentLevel];
            cubesOnPage4[0].SetActive(false);
            cubesOnPage4[1].SetActive(false);
            cubesOnPage4[2].SetActive(false);
            winningLogo.SetActive(false);
            flower.SetActive(false);
            okOnPage3.SetActive(false);
            okOnPage4.SetActive(false);
        }
        //---------------------------------------------------------------------------------------------------------

        //------------------Detect Numbers & Symbols on Cubes--------------------------------------------------------------
        private static int CompareByPositionY(GameObject a, GameObject b)
        {
            var yA = a.transform.position.y;
            var yB = b.transform.position.y;
            return yA.CompareTo(yB);
        }

        public void Detected(GameObject obj)
        {
            if (!trackedObjects.Contains(obj)) {
                trackedObjects.Add(obj);
            }
            trackedObjects.Sort(CompareByPositionY);
        }

        public void Lost(GameObject obj)
        {
            if (trackedObjects.Contains(obj)) {
                trackedObjects.Remove(obj);
            }
            trackedObjects.Sort(CompareByPositionY);
            GameFormat();
        }

        public void DetectedNumber2() { Detected(Number2); }
        public void LostNumber2() { Lost(Number2); }

        public void DetectedNumber3() { Detected(Number3); }
        public void LostNumber3() { Lost(Number3); }

        public void DetectedNumber4() { Detected(Number4); }
        public void LostNumber4() { Lost(Number4); }

        public void DetectedNumber5() { Detected(Number5); }
        public void LostNumber5() { Lost(Number5); }

        public void DetectedNumber6() { Detected(Number6); }
        public void LostNumber6() { Lost(Number6); }

        public void DetectedNumber7() { Detected(Number7); }
        public void LostNumber7() { Lost(Number7); }

        public void DetectedNumber8() { Detected(Number8); }
        public void LostNumber8() { Lost(Number8); }

        public void DetectedNumber9() { Detected(Number9); }
        public void LostNumber9() { Lost(Number9); }

        public void DetectedPlus() { Detected(Plus); }
        public void LostPlus() { Lost(Plus); }

        public void DetectedMinus() { Detected(Minus); }
        public void LostMinus() { Lost(Minus); }

        public void DetectedDivide() { Detected(Divide); }
        public void LostDivide() { Lost(Divide); }

        public void DetectedMultiply() { Detected(Multiply); }
        public void LostMultiply() { Lost(Multiply); }
        //-------------------------------------------------------------------------------------------------------------------

        //-----------------Logging-----------------
        public void LogState() {
            debugText.text = StateStr();
        }

        public string StateStr() {
            var tracked0x = trackedObjects.Count > 0 ? $"{trackedObjects[0].transform.position}" : "";
            var tracked1x = trackedObjects.Count > 1 ? $"{trackedObjects[1].transform.position}" : "";
            var tracked2x = trackedObjects.Count > 2 ? $"{trackedObjects[2].transform.position}" : "";
            return "State:\n" +
                $"\n - currentGameState: {currentGameState}" +
                $"\n - firstNumber: {firstNumber}" +
                $"\n - secondNumber: {secondNumber}" +
                $"\n - finalResult: {finalResult}" +
                $"\n - symbol: {symbol}" +
                $"\n - targetXPosition: {targetXPosition}" +
                $"\n - targetYPosition: {targetYPosition}" +
                $"\n - currentLevel: {currentLevel}" +
                $"\n - trackedObjects 0: {tracked0x}" +
                $"\n - trackedObjects 1: {tracked1x}" +
                $"\n - trackedObjects 2: {tracked2x}";
        }
        //------------------------------------------

        //-----------------------------MAIN GAME LOGIC-------------------------------------------------------------------------------
        public void GameLogic()
        {
            if (currentGameState == GameState.FirstStep &&
                gamePages[2].active == true &&
                !cubesOnPage4[0].activeSelf)
            {
                var names = trackedObjects.Select(o =>
                    o.GetComponent<DefaultObserverEventHandler>().targetName
                );
                if (names.Any(name => name == "number2")) {
                    cubesOnPage4[0].SetActive(true);
                    okOnPage3.SetActive(true);
                    StartCoroutine("DelayShowPage2");
                }
            }
            else  if (currentGameState == GameState.FirstStep &&
                gamePages[3].active == true &&
                trackedObjects.Count > 0 &&
                firstNumber == 0)
            {
                var name = trackedObjects[0].GetComponent<DefaultObserverEventHandler>().targetName;
                if (name.StartsWith("number")) {
                    firstNumber = Int32.Parse(name.Substring(6));
                    cubesOnPage4[0].SetActive(true);
                    currentGameState = GameState.SecondStep;
                }
            }
            else if (currentGameState == GameState.SecondStep &&
                gamePages[3].active == true &&
                trackedObjects.Count > 1 &&
                symbol == "")
            {
                var x1 = trackedObjects[0].transform.position.x;
                var x2 = trackedObjects[1].transform.position.x;
                var opName = trackedObjects[1].GetComponent<DefaultObserverEventHandler>().targetName;
                if (x2 > x1 - xDistanceThreshold &&
                    x2 < x1 + xDistanceThreshold &&
                    !opName.StartsWith("number"))
                {
                    symbol = opName;
                    cubesOnPage4[1].SetActive(true);
                    currentGameState = GameState.ThirdStep;
                }
                else {
                    symbol = "";
                    cubesOnPage4[1].SetActive(false);
                    currentGameState = GameState.SecondStep;
                }
            }
            else if (currentGameState == GameState.ThirdStep &&
                gamePages[3].active == true &&
                trackedObjects.Count > 2 &&
                secondNumber == 0)
            {
                var x1 = trackedObjects[0].transform.position.x;
                var x2 = trackedObjects[1].transform.position.x;
                var x3 = trackedObjects[2].transform.position.x;
                var name = trackedObjects[2].GetComponent<DefaultObserverEventHandler>().targetName;
                if (x2 > x1 - xDistanceThreshold &&
                    x2 < x1 + xDistanceThreshold)
                {
                    if (x3 > x2 - xDistanceThreshold &&
                        x3 < x2 + xDistanceThreshold &&
                        name.StartsWith("number"))
                    {
                        secondNumber = Int32.Parse(name.Substring(6));

                        cubesOnPage4[1].SetActive(true);
                        cubesOnPage4[2].SetActive(true);
                        currentGameState = GameState.Completed;
                    }
                }
                else {
                    currentGameState = GameState.SecondStep;
                }
            }
            else if (currentGameState == GameState.Completed &&
                gamePages[3].active == true &&
                finalResult != levelTargets[currentLevel])
            {
                if (firstNumber != 0 && symbol != "" && secondNumber != 0)
                {
                    switch (symbol)
                    {
                        case "plus":
                            {
                                finalResult = secondNumber + firstNumber;
                                break;
                            }
                        case "minus":
                            {
                                finalResult = secondNumber - firstNumber;
                                break;
                            }
                        case "supply":
                            {
                                finalResult = secondNumber * firstNumber;
                                break;
                            }
                        case "divide":
                            {
                                finalResult = (int) (secondNumber / firstNumber);
                                break;
                            }
                    }
                }

                if (finalResult == levelTargets[currentLevel] &&
                    !winningLogo.activeSelf)
                {
                    currentGameState = GameState.FirstStep;
                    currentLevel++;
                    // levelProgress.fillAmount = currentLevel / 11f;

                    if (currentLevel == 11)
                    {
                        currentLevel = 0;
                    }

                    okOnPage4.SetActive(true);
                    StartCoroutine("WinningDelay1");
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
    }
}

