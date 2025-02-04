﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CusTween
{
    public class TweenIntValue
    {
        private int value;
        private int tweenId;
        private float maxTweenTime;
        private float minTweenTime;
        private Action<float> onUpdate;
        private GameObject g;
        private bool onlyPositive;

        public TweenIntValue(GameObject g, int initValue, float minTweenTime, float maxTweenTime, bool onlyPositive, Action<float> onUpdate)
        {
            value = initValue;
            this.maxTweenTime = Mathf.Max(0, maxTweenTime);
            this.minTweenTime = Mathf.Clamp(minTweenTime, 0, maxTweenTime);
            this.g = g;
            this.onlyPositive = onlyPositive;
            this.onUpdate = onUpdate;
        }

        public void Tween(int newValue, int valuePerSecond)
        {
            SimpleTween.Cancel(tweenId, false);
            float add = newValue - value;

            if ((add > 0 && onlyPositive) || !onlyPositive)
            {
                valuePerSecond = Mathf.Max(1, valuePerSecond);
                float tT = Mathf.Abs((float)add / (float)valuePerSecond);
                tT = Mathf.Clamp(tT, minTweenTime, maxTweenTime);
                int oldValue = value;
                tweenId = SimpleTween.Value(g, 0, 1, tT).SetOnUpdate((float val) =>
                {
                    if (this != null)
                    {
                        value = MathMk.Lerp(oldValue, newValue, val);
                        onUpdate?.Invoke(value);
                    }
                }).ID;
            }
            else if (onlyPositive)
            {
                if (this != null)
                {
                    value = newValue;
                    onUpdate?.Invoke(value);
                }
            }
        }
    }

    public class TweenLongValue
    {
        private long value;
        private int tweenId;
        private float maxTweenTime;
        private float minTweenTime;
        private Action<long> onUpdate;
        private GameObject g;
        private bool onlyPositive;

        public TweenLongValue(GameObject g, long initValue, float minTweenTime, float maxTweenTime, bool onlyPositive, Action<long> onUpdate)
        {
            value = initValue;
            this.maxTweenTime = Mathf.Max(0, maxTweenTime);
            this.minTweenTime = Mathf.Clamp(minTweenTime, 0, maxTweenTime);
            this.g = g;
            this.onlyPositive = onlyPositive;
            this.onUpdate = onUpdate;
        }

        public void Tween(long newValue, long valuePerSecond)
        {
            SimpleTween.Cancel(tweenId, false);
            long add = newValue - value;

            if ((add > 0 && onlyPositive) || !onlyPositive)
            {
                valuePerSecond = Math.Max(1, valuePerSecond);
                float tT = Mathf.Abs((float)add / (float)valuePerSecond);
                tT = Mathf.Clamp(tT, minTweenTime, maxTweenTime);
                long oldValue = value;
                tweenId = SimpleTween.Value(g, 0, 1, tT).SetOnUpdate((float val) =>
                {
                    if (this != null)
                    {
                        value = MathMk.Lerp(oldValue, newValue, val);
                        onUpdate?.Invoke(value);
                    }
                }).ID;
            }
            else if (onlyPositive)
            {
                if (this != null)
                {
                    value = newValue;
                    onUpdate?.Invoke(value);
                }
            }
        }
    }

    public class TweenFloatValue
    {
        private float value;
        private int tweenId;
        private float maxTweenTime;
        private float minTweenTime;
        private Action<float> onUpdate;
        private GameObject g;
        private bool onlyPositive;

        public TweenFloatValue(GameObject g, float initValue, float minTweenTime, float maxTweenTime, bool onlyPositive, Action<float> onUpdate)
        {
            this.value = initValue;
            this.maxTweenTime = Mathf.Max(0, maxTweenTime);
            this.minTweenTime = Mathf.Clamp(minTweenTime, 0, maxTweenTime);
            this.g = g;
            this.onlyPositive = onlyPositive;
            this.onUpdate = onUpdate;
        }

        public void Tween(float master, float value, float graceVal)
        {
            float difference = master - value;
            // Tween this value to the base of it
            Debug.Log($"This value is bigger than {difference}");
            float effectiveVal = difference + graceVal;
            if (effectiveVal > master)
                Debug.Log("Effective is greater");
            else
                Debug.Log("Effective is not valid");
        }

        public void Tween(float newValue, float valuePerSecond)
        {
            SimpleTween.Cancel(tweenId, false);
            float addBy = newValue - value;

            if ((addBy > 0 && onlyPositive) || !onlyPositive)
            {
                valuePerSecond = Math.Max(1, valuePerSecond);
                float tT = Mathf.Abs(addBy / valuePerSecond);
                tT = Mathf.Clamp(tT, minTweenTime, maxTweenTime);
                float oldValue = value;
                tweenId = SimpleTween.Value(g, 0, 1, tT).SetOnUpdate((float val) =>
                {
                    if (this != null)
                    {
                        value = MathMk.Lerp(oldValue, newValue, val);
                        onUpdate?.Invoke(value);
                    }
                }).ID;
            }
            else if (onlyPositive)
            {
                if (this != null)
                {
                    value = newValue;
                    onUpdate?.Invoke(value);
                }
            }
        }
    }

}
