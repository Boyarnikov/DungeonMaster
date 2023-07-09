using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpriteManager : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PlayerMovement playerMovement;
    int tick;
    int animationSpeed;
    [SerializeField] float tiredness = 0;
    float tirednessMax = 150;
    [SerializeField] int holdSpeed = 200;
    [SerializeField] int moveSpeed = 50;
    [SerializeField] Vector2 offset;
    [SerializeField] Sprite[] spriteWays;
    [SerializeField] public bool flag = false;
    [SerializeField] public int add = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        animationSpeed = holdSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        tick++;
        if (flag) spriteRenderer.sprite = spriteWays[playerMovement.directional + add];
        else spriteRenderer.sprite = spriteWays[playerMovement.directional];
        string status = playerMovement.status;
        if (status == "hold")
        {
            animationSpeed = holdSpeed;
            tiredness -= 0.08f;
            if (tiredness < 0) tiredness = 0;
        }
        else
        {
            animationSpeed = moveSpeed;
            tiredness += 1f;
            if (tiredness > tirednessMax) tiredness = tirednessMax;
        }

        if (((status == "hold") && ((tick % (animationSpeed - Mathf.Floor(tiredness)) == 0) || (tick > (animationSpeed - Mathf.Floor(tiredness))))) || ((status == "move") && (tick % animationSpeed == 0)))
        {
            tick = 0;
            if (offset.y < 1) offset.y++;
            else offset.y--;
        }
        spriteRenderer.material.SetVector("_Offset", offset);
    }
}
