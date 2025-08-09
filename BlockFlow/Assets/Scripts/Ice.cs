using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ice : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;

    private int count;

    private void Awake()
    {
        BoardController.OnSuccessfulGrind += CheckIceCount;
    }

    public void Initialize(int count)
    {
        this.count = count;
        UpdateCountText();
    }

    private void CheckIceCount(Cell cell, Grinder grinder, Block block)
    {
        count--;
        UpdateCountText();

        transform.DOKill();
        transform.DOScale(transform.localScale * 1.25f, 0.25f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
            {
                if(count == 0)
                    BreakIce();
            });
        });
    }

    private void UpdateCountText()
    {
        countText.text = count.ToString();
    }

    private void BreakIce()
    {
        transform.DOKill();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        BoardController.OnSuccessfulGrind -= CheckIceCount;
    }
}
