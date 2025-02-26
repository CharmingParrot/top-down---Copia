﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item;
    [SerializeField] private Text qntdItemText;
    public int qntdRecurso;

    private void Start()
    {
        GetComponent<Image>().sprite = item.icone;
    }
    public void atualizaQuantidade(int quantidade)
    {
        qntdRecurso += quantidade;
        qntdItemText.text = qntdRecurso.ToString("000");
    }
    public void destroiSlot()
    {
        Destroy(gameObject);
    }
}