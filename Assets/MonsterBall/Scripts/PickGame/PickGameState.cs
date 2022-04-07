using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Framework
{
    public class PickGameState : NestedStateManager
    {
        [SerializeField] private WinPresentationState _WinPresentationState;
        public PickObject[] Values;
        public GameObject PickGameView;

        [HideInInspector] public List<int> PickScript = new List<int>();
        private int TotalWon = 0;
        private bool ReadyToExit = false;

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _WinPresentationState.HighlightWinBackgrounds();
            ClearTexts();
            AssignValuesToScript();
            PickGameView.gameObject.SetActive(true);
            ReadyToExit = false;
        }

        public override State OnUpdate()
        {
            State rtn = null;

            base.OnUpdate(); // Updated nested states

            if(ReadyToExit)
            {
                rtn = _WinPresentationState;
            }

            return rtn;
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            PickGameView.gameObject.SetActive(false);
        }

        public void GameComplete()
        {
            ReadyToExit = true;
        }

        private void AssignValuesToScript()
        {
            TotalWon = Central.GlobalData.GameData.TotalWon.Value;
            int NewTotalWon = ReAssignWin();
            ShuffleScript();
            SetBombAsLast();
            Central.GlobalData.GameData.TotalWon.Value = NewTotalWon;

            //LogScript();
        }

        private void ClearTexts()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i].Clear();
            }
        }

        private int ReAssignWin()
        {
            int pickCount = Random.Range(5, Values.Length);
            int valueCount = pickCount - 3;
            int[] possibleValues = new int[4] { 0, 5, 10, 15 };
            int equalValue = (TotalWon / valueCount) + (5 - ((TotalWon / valueCount) % 5));
            int newTotalWon = 0;

            PickScript.Clear();

            for (int i = 0; i < pickCount; i++)
            {
                if (i < valueCount)
                {
                    int value = equalValue;
                    if (Random.Range(0, 3) == 0)
                    {
                        value += possibleValues[Random.Range(0, possibleValues.Length)];
                    }

                    newTotalWon += value;
                    PickScript.Add(value);
                }
                else
                {
                    PickScript.Add(-1);
                }
            }

            return newTotalWon;
        }

        private void SetBombAsLast()
        {
            for (int i = 0; i < PickScript.Count; i++)
            {
                if (PickScript[i] == -1)
                {
                    PickScript.RemoveAt(i);
                    break;
                }
            }

            PickScript.Add(-1);
        }

        private void LogScript()
        {
            string dbg = "Pick script Total: " + Central.GlobalData.GameData.TotalWon + "   (" + PickScript.Count.ToString() + ") = ";

            for (int i = 0; i < PickScript.Count; i++)
            {
                dbg += PickScript[i] + " ";
            }

            Debugger.Instance.LogError(dbg);
        }

        void ShuffleScript()
        {
            for (int i = 0; i < PickScript.Count; i++)
            {
                int temp = PickScript[i];
                int randomIndex = Random.Range(i, PickScript.Count);
                PickScript[i] = PickScript[randomIndex];
                PickScript[randomIndex] = temp;
            }
        }
    }
}