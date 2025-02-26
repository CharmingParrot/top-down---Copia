﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour, AcoesNoTutorial
{
    public static BaseScript Instance { get; private set; }
    [Header("COMPONENTES DA BASE")]
    [SerializeField] private Transform posicaoDeChegadaPorTeleporte;
    [SerializeField] private int vidaMax;
    [SerializeField] private GameObject areaDeInteracao;
    [SerializeField] private float intervaloDuranteADefesa;
    [SerializeField] private int QntdDeDefesasNecessarias;
    private bool duranteMelhoria = false;
    [SerializeField] private bool tutorial;
    private List<SlotModulo> listaModulos = new List<SlotModulo>();
    private int vidaAtual;
    private int DefesasFeitas = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        vidaAtual = vidaMax;
    }
    public void VerificarModulos()
    {
        int defendido = 0;
        for (int a = 0; a < desastreManager.Instance.GetQntdDesastresParaOcorrer(); a++)
        {
            for (int i = 0; i < listaModulos.Count; i++)
            {
                if (desastreManager.Instance.GetDesastreSorteado(a) == listaModulos[i].GetnomeDesastre() && desastreManager.Instance.GetForcaSorteada(a) == listaModulos[i].GetvalorResistencia())
                {
                    listaModulos[i].RemoverModulo();
                    defendido++;
                }
            }
        }
        if (desastreManager.Instance.GetQntdDesastresParaOcorrer() == defendido)
        {
            Debug.Log("defendido");
        }
        else
        {
            //if (!tutorial)
            //{
                for (int i = 0; i < desastreManager.Instance.GetQntdDesastresParaOcorrer() - defendido; i++)
                {
                    vidaAtual--;
                    if (vidaAtual <= 0)
                    {
                        Debug.Log("Perdeu");
                    }
                }
                Debug.Log(vidaAtual);
            //}
        }
    }
    public void AbreEFechaMenuDeTrocaDeTempo()
    {
       //if (duranteMelhoria)
       //{
       //    Debug.Log("espere a defesa acabar para interagir");
       //}
       //else
       //{
            if (!jogadorScript.Instance.InterfaceJogador.InventarioAberto)
            {
                jogadorScript.Instance.InterfaceJogador.abreMenuDeTempos();
            }
            else
            {
                jogadorScript.Instance.InterfaceJogador.fechaMenuDeTempos();
            }
        //}
    } 
    public void AdicionarModulo(SlotModulo modulo)
    {
        listaModulos.Add(modulo);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (desastreManager.Instance.VerificarSeUmDesastreEstaAcontecendo())
            {
                EncerrarDesastresEVerificarDefesa();
                if (tutorial)
                {
                    Tutorial();
                    return;
                }
                RecomecarDesastres();
            }
        }
    }
    public void Tutorial()
    {
        if (tutorial)
        {
            DialogeManager.Instance.DialogoFinalizado += AoFinalizarDialogo;
            tutorial = false;
            TutorialSetUp.Instance.IniciarDialogo();
            return;
        }
        areaDeInteracao.SetActive(true);
    }
    private void EncerrarDesastresEVerificarDefesa()
    {
        VerificarModulos();
        desastreManager.Instance.encerramentoDesastres();
    }
    private void RecomecarDesastres()
    {
        if (duranteMelhoria)
        {
            DefesasFeitas++;
            if (DefesasFeitas == QntdDeDefesasNecessarias)
            {
                duranteMelhoria = false;
                DefesasFeitas = 0;
                if (UIinventario.Instance.VerificarSeLiberouBossFinal())
                    return;
                desastreManager.Instance.ConfigurarTimer(desastreManager.Instance.GetIntervaloDeTempoEntreOsDesastres(), desastreManager.Instance.GetTempoAcumuladoParaDesastre());
                DesastresList.Instance.LiberarNovosDesastres(UIinventario.Instance.GetTempoAtual());//ativa a possibilidade do evento desse tempo acontecer
            }
            else
                desastreManager.Instance.ConfigurarTimer(intervaloDuranteADefesa, desastreManager.Instance.GetTempoAcumuladoParaDesastre());
        }
        else
        {
            desastreManager.Instance.ConfigurarTimer(desastreManager.Instance.GetIntervaloDeTempoEntreOsDesastres(), desastreManager.Instance.GetTempoAcumuladoParaDesastre());
        }
        desastreManager.Instance.MudarTempoAcumuladoParaDesastre(0f);
        StartCoroutine(desastreManager.Instance.LogicaDesastres(true));
    }
    public void Ativar_DesativarInteracao(bool b)
    {
        areaDeInteracao.SetActive(b);
    }
    protected virtual void AoFinalizarDialogo(object origem, System.EventArgs args)
    {
        TutorialSetUp.Instance.AoTerminoDoDialogoTerminadoOPrimeiroDesastre();
    }
    public void CancelarInscricaoEmDialogoFinalizado()
    {
        DialogeManager.Instance.DialogoFinalizado -= AoFinalizarDialogo;
    }
    public void DesligarTutorialDosModulos()
    {
        for (int i = 0; i < listaModulos.Count; i++)
        {
            listaModulos[i].SetTutorial(false);
        }
    }
    public void Ativar_DesativarDuranteDefesaParaMelhorarBase(bool b)
    {
        duranteMelhoria = b;
    }
    public Transform GetPosicaoParaTeleporte()
    {
        return posicaoDeChegadaPorTeleporte;
    }
    public bool GetDuranteDefesaParaMelhorarBase()
    {
        return duranteMelhoria;
    }
    public float GetIntervaloDuranteOAprimoramentoDaBase()
    {
        return intervaloDuranteADefesa;
    }
}
