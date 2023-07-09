using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class InterfaceDrawer : MonoBehaviour
{

    [SerializeField] bool border;
    [SerializeField] Color borderColor;
    [SerializeField] Color mainColor;
    [SerializeField] Dictionary<char, Tuple<int, int>> alphabet;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject charObject;
    List<GameObject> chars;

    void Start()
    {
        chars = new List<GameObject>();
        alphabet = new Dictionary<char, Tuple<int, int>>();
        alphabet.Add('0', new Tuple<int, int>(0, 3));
        alphabet.Add('1', new Tuple<int, int>(1, 3));
        alphabet.Add('2', new Tuple<int, int>(2, 3));
        alphabet.Add('3', new Tuple<int, int>(3, 3));
        alphabet.Add('4', new Tuple<int, int>(4, 3));
        alphabet.Add('5', new Tuple<int, int>(5, 3));
        alphabet.Add('6', new Tuple<int, int>(6, 3));
        alphabet.Add('7', new Tuple<int, int>(7, 3));
        alphabet.Add('8', new Tuple<int, int>(8, 3));
        alphabet.Add('9', new Tuple<int, int>(9, 3));
        alphabet.Add('/', new Tuple<int, int>(10, 3));
        alphabet.Add('G', new Tuple<int, int>(11, 4));
        alphabet.Add(' ', new Tuple<int, int>(12, 3));
    }

    float Lenth(string value, int interval)
    {
        Debug.Log(value);
        float size = 0;
        foreach (char x in value)
        {
            alphabet.TryGetValue(x, out Tuple<int, int> result);
            size += result.Item2 + interval;
            Debug.Log(size);
        }
        size-= 1;
        if (size % 2 == 1) size -= 1/2;
        return (size - interval - 2) / 16f;
    }

    public void Draw(float offsetX, float offsetY, string value, int interval) {
        Clear();
        int itr = 0;
        float size = Lenth(value, interval);
        foreach (char x in value)
        {
            alphabet.TryGetValue(x, out Tuple<int, int> result);
            GameObject newChar = Instantiate(charObject, transform.position + new Vector3(offsetX + itr * ((float) result.Item2 + interval) / 16f - size / 2f, offsetY, 0f), Quaternion.identity);
            newChar.transform.parent = transform;
            chars.Add(newChar);
            newChar.GetComponent<SpriteRenderer>().sprite = sprites[result.Item1];
            if (!border) borderColor = new Color(0, 0, 0, 0);
            newChar.GetComponent<SpriteRenderer>().material.SetColor("MainColor", mainColor);
            newChar.GetComponent<SpriteRenderer>().material.SetColor("BorderColor", borderColor);
            itr++;
        }
    }

    void Clear() {
        foreach (var element in chars)
        {
            Destroy(element.gameObject);
        }
        chars.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
