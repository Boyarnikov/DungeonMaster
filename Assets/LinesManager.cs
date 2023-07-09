using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinesManager : MonoBehaviour
{
    [SerializeField] LineRenderer leftVertical;
    [SerializeField] LineRenderer rightVertical;
    [SerializeField] LineRenderer horisontal;
    [SerializeField] LineRenderer horisontalBold;
    [SerializeField] float currentLenth;
    [SerializeField] float currentVerticalLenth;
    [SerializeField] float width;
    [SerializeField] InterfaceDrawer legend;
    [SerializeField] InterfaceDrawer left;
    [SerializeField] InterfaceDrawer right;

    float verticalLenth;
    float lenth;
    int level;

    [SerializeField] float value;
    [SerializeField] float currentValue;
    [SerializeField] float minValue;
    [SerializeField] float maxValue;
    [SerializeField] bool adding;

    void Start()
    {
        level = 0;
        leftVertical.sortingLayerName = "Interface";
        rightVertical.sortingLayerName = "Interface";
        horisontal.sortingLayerName = "Interface";
        horisontalBold.sortingLayerName = "Interface";
        verticalLenth = currentVerticalLenth;
        legend.Draw(0, - verticalLenth * 3 / 2, Mathf.Floor(currentValue).ToString() + " / " + Mathf.Floor(maxValue).ToString(), 1);
        left.Draw(0, verticalLenth * 3 / 2, level.ToString(), 1);
        right.Draw(0, verticalLenth * 3 / 2, (level + 1).ToString(), 1);
    }

    void AddToValue(float add) {
        currentValue += add;
    }

    void MoveToCurrectValue()
    {
        if (Mathf.Abs(value - currentValue) < 0.1) value = currentValue;
        else value += (currentValue - value) / 16;

        if (Mathf.Abs(lenth - currentLenth) < 0.05) lenth = currentLenth;
        else lenth += (currentLenth - lenth) / 32;

        if (Mathf.Abs(verticalLenth - currentVerticalLenth) < 0.05) verticalLenth = currentVerticalLenth;
        else verticalLenth += (currentVerticalLenth - verticalLenth) / 32;
    }

    void SetBar()
    {
        Vector3[] leftVerticalPositions = new Vector3[2];
        leftVerticalPositions[0] = new Vector3(transform.position.x - lenth / 2, transform.position.y + verticalLenth / 2, 0);
        leftVerticalPositions[1] = new Vector3(transform.position.x - lenth / 2, transform.position.y - verticalLenth / 2, 0);
        leftVertical.SetPositions(leftVerticalPositions);
        leftVertical.startWidth = width;
        leftVertical.endWidth = width;

        Vector3[] rightVerticalPositions = new Vector3[2];
        rightVerticalPositions[0] = new Vector3(transform.position.x + lenth / 2, transform.position.y + verticalLenth / 2, 0);
        rightVerticalPositions[1] = new Vector3(transform.position.x + lenth / 2, transform.position.y - verticalLenth / 2, 0);
        rightVertical.SetPositions(rightVerticalPositions);
        rightVertical.startWidth = width;
        rightVertical.endWidth = width;

        Vector3[] horisontalPositions = new Vector3[2];
        horisontalPositions[0] = new Vector3(transform.position.x - lenth / 2, transform.position.y, 0);
        horisontalPositions[1] = new Vector3(transform.position.x + lenth / 2, transform.position.y, 0);
        horisontal.SetPositions(horisontalPositions);
        horisontal.startWidth = width;
        horisontal.endWidth = width;

        Vector3[] horisontalBoldPositions = new Vector3[2];
        horisontalBoldPositions[0] = new Vector3(transform.position.x - lenth / 2, transform.position.y, 0);
        horisontalBoldPositions[1] = new Vector3(transform.position.x - lenth / 2 + lenth * (value - minValue) / (maxValue - minValue), transform.position.y, 0);
        horisontalBold.SetPositions(horisontalBoldPositions);
        horisontalBold.startWidth = verticalLenth;
        horisontalBold.endWidth = verticalLenth;

        left.transform.position = new Vector3(transform.position.x - lenth / 2, transform.position.y, 0);
        right.transform.position = new Vector3(transform.position.x + lenth / 2, transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SetBar();
        MoveToCurrectValue();
        if (value == maxValue)
        {
            minValue = currentValue;
            legend.Draw(0, -verticalLenth * 3 / 2, Mathf.Floor(currentValue).ToString() + " / " + Mathf.Floor(maxValue).ToString(), 1);
            level++;
            left.Draw(0, verticalLenth * 3 / 2, level.ToString(), 1);
            right.Draw(0, verticalLenth * 3 / 2, (level + 1).ToString(), 1);
            maxValue += 100;
        }
        if (adding)
        {
            adding = false;
            currentValue = Mathf.Min(value + Random.Range(1, 40), maxValue);
            legend.Draw(0, -verticalLenth * 3 / 2, Mathf.Floor(currentValue).ToString() + " / " + Mathf.Floor(maxValue).ToString(), 1);
            verticalLenth *= Mathf.Min(Mathf.Max((maxValue - minValue) / (value - minValue), 1.05f), 1.1f);
            lenth *= Mathf.Min(Mathf.Max((maxValue - minValue) / (value - minValue), 1.03f), 1.08f);
        }
    }
}
