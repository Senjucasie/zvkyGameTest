using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

namespace CusTween
{
    public class MultipleFontHandler : MonoBehaviour
    {
        [SerializeField]
        TMP_Text[] texts;
        [SerializeField]
        bool canTweenAmount = false;

        private List<TweenFloatValue> tweenAmountList;
        private List<TweenIntValue> tweenIntAmountList;

        public TMP_Text[] Texts { get => texts; set => texts = value; }
        public bool CanTweenAmount { get => canTweenAmount; set => canTweenAmount = value; }

        public void setText(float amount)
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
                    tween.Tween(amount, 100);
                }
                return;
            }
            foreach (var text in texts) text.text = amount < 10 ? amount.ToString() : amount.ToString("0,0");
        }

        public void setText(int amount)
        {
            // New implementation for int
            if (amount <= 0)
            {
                foreach (var text in texts) text.text = "0";
                return;
            }

            if (canTweenAmount)
            {
                setIntTweenFunctionality();
                foreach (var tween in tweenIntAmountList)
                {
                    tween.Tween(amount, 100);
                }
                return;
            }

            foreach (var text in texts) text.text = amount < 10 ? amount.ToString() : amount.ToString("0,0");
        }

        private void setTweenFunctionality()
        {
            tweenAmountList = new List<TweenFloatValue>();
            foreach (var text in texts)
            {
                float initValue = float.Parse(text.text, CultureInfo.InvariantCulture.NumberFormat);
                var a = new TweenFloatValue(text.gameObject, initValue, 1f, 1f, true,
                        (value) =>
                        {
                            //Debug.Log("Tween Value: " + value);
                            value = MathF.Round(value, 2);
                            text.text = value.ToString();
                            // text.text = String.Format("{0}", (value > 0) ? value.ToString("0") : value.ToString());
                        });
                tweenAmountList.Add(a);
            }
        }

        private void setIntTweenFunctionality()
        {
            tweenIntAmountList = new List<TweenIntValue>();
            foreach (var text in texts)
            {
                Int32 initValue = Int32.Parse(text.text, CultureInfo.InvariantCulture.NumberFormat);
                var a = new TweenIntValue(text.gameObject, initValue, 1f, 1f, true,
                        (value) =>
                        {
                            //Debug.Log("Tween Value: " + value);
                            //value = MathF.Round(value, 2);
                            text.text = value.ToString();
                            // text.text = String.Format("{0}", (value > 0) ? value.ToString("0") : value.ToString());
                        });
                tweenIntAmountList.Add(a);
            }
        }

        public void setStringText(string stringText)
        {
            foreach (var text in texts) text.text = stringText;
        } 
    }

}
