﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JogadorAnimScript : MonoBehaviour
{
    public static JogadorAnimScript Instance { get; private set; }
    private Animator animator;
    private Vector2 movimento;
    [SerializeField] private jogadorScript jogador;
    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (jogador.GetPodeAnimar())
        {
            movimento.x = Input.GetAxisRaw("Horizontal");
            movimento.y = Input.GetAxisRaw("Vertical");
            animator.SetFloat("HORIZONTAL", movimento.x);
            animator.SetFloat("VERTICAL", movimento.y);
            animator.SetFloat("VELOCIDADE", movimento.sqrMagnitude);
        }
    }
    public void AnimarAtaqueMelee(float dirX, float dirY)
    {
        animator.SetFloat("HORZMELEE", dirX);
        animator.SetFloat("VERTMELEE", dirY);
        animator.SetTrigger("MELEE");
    }
    public void AnimarDisparo(float dirX, float dirY)
    {
        animator.SetFloat("HORZDISPARO", dirX);
        animator.SetFloat("VERTDISPARO", dirY);
        animator.SetTrigger("DISPARO");
    }
    public void Levantar(bool b)
    {
        animator.SetBool("CAIDO", b);
    }
}
