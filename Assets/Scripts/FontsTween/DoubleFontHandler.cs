using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CusTween
{
    public class DoubleFontHandler : MonoBehaviour
    {
        [SerializeField]
        TMP_Text[] texts;
        [SerializeField]
        bool canTweenAmount = false;

        private List<TweenLongValue> tweenAmountList;

        public TMP_Text[] Texts { get => texts; set => texts = value; }
        public bool CanTweenAmount { get => canTweenAmount; set => canTweenAmount = value; }

        public void setText(double amount)
        {
            if (amount <= 0)
            {
                foreach (var text in texts) text.text = "0";
                return;
            }
            if (canTweenAmount)
            {
                setTweenFunctionality();
                foreach (var tween in tweenAmountList)
                {
                    tween.Tween((long)amount, 100);
                }
                return;
            }
            foreach (var text in texts) text.text = amount < 10 ? amount.ToString() : amount.ToString("0,0");
        }
        private void setTweenFunctionality()
        {
            tweenAmountList = new List<TweenLongValue>();
            foreach (var text in texts)
            {
                var a = new TweenLongValue(text.gameObject, (long)0,1 ,1, true,
                        (value) => { text.text = String.Format("{0}", (value > 0) ? value.ToString("0") : value.ToString()); });
                tweenAmountList.Add(a);
            }

        }

        public void setStringText(string stringText)
        {
            foreach (var text in texts) text.text = stringText;
        }
    }

}
