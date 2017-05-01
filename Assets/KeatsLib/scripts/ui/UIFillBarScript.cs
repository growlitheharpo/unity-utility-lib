﻿using System.Collections;
using UnityEngine;
using UIImage = UnityEngine.UI.Image;

/// <summary>
/// A utility UI class attached to a prefab. This is useful for a bar that needs to fill/drain, such as progress or health.
/// </summary>
public class UIFillBarScript : MonoBehaviour
{
    [SerializeField] private UIImage mBackground;
    [SerializeField] private UIImage mDelayBar;
    [SerializeField] private UIImage mFillBar;

    [SerializeField] private UIImage.FillMethod mFillDirection;
    [SerializeField] private float mDelayTime;

    private void Awake()
    {
        if (mBackground == null || mDelayBar == null || mFillBar == null)
        {
            Destroy(this);
            return;
        }

        mDelayBar.fillMethod = mFillDirection;
        mFillBar.fillMethod = mFillDirection;
    }

    /// <summary>
    /// Set the new fill amount of the bar (from 0 to 1)
    /// </summary>
    /// <param name="amount">The new amount.</param>
    public void setFillAmount(float amount)
    {
        mFillBar.fillAmount = amount;

        StopAllCoroutines();
        StartCoroutine(drainRoutine(amount));
    }

    private IEnumerator drainRoutine(float newAmount)
    {
        float startAmount = mDelayBar.fillAmount;
        float currentTime = 0.0f;

        while (currentTime < mDelayTime)
        {
            mDelayBar.fillAmount = Mathf.Lerp(startAmount, newAmount, Mathf.Sqrt(currentTime / mDelayTime));

            currentTime += Time.deltaTime;
            yield return null;
        }

        mDelayBar.fillAmount = newAmount;
    }
}
