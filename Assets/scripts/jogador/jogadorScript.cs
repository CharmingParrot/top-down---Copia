﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jogadorScript : MonoBehaviour, AcoesNoTutorial
{
    public static jogadorScript Instance { get; private set; }
    //variáveis publicas
    [Header("Valores Numéricos")]
    [SerializeField] private bool tutorial;
    [SerializeField] private float velocidade;
    //private float vidaMaxima = 1;
    [SerializeField] private float velocidadeProjetil;
    [SerializeField] private float taxaDeDisparo;
    [SerializeField] private float taxaDeAtaqueMelee;
    [SerializeField] private float alcanceMelee;
    private Vector2 distanciaAtaqueMelee;
    [SerializeField] private float danoMelee;
    [SerializeField] private float danoProjetil;
    [Header("Componentes")]
    //[SerializeField] private JogadorAnimScript animScript;
    [SerializeField] private Transform posicaoMelee;
    public Camera mainCamera;
    [SerializeField] private GameObject projetilPrefab;
    [SerializeField] private GameObject armaMelee;
    [SerializeField] private Transform pontoDeDisparo;
    [SerializeField] private LayerMask objetosAcertaveisLayer;
    public UIinventario InterfaceJogador;
    [SerializeField] private SpriteRenderer iconeInteracao;
    public CinemachineBehaviour comportamentoCamera;
    //variáveis privadas
    //private float vidaAtual;
    private Vector2 movimento;
    private Rigidbody2D rb;
    private Animator animatorPicareta;
    private bool atirando = false;
    private bool atacando = false;
    private float forcaEmpurrao;
    private Vector2 direcaoEmpurrao;
    [Header("Não Mexer")]
    private Vector2 baguncarControles = new Vector2(0,0);
    public enum estados
    {
        EmAcao,
        EmMenus,
        EmContrucao,
        EmDialogo,
        SendoEmpurrado
    };
    private estados estadosJogador = estados.EmAcao;// 0 = em acao, 1 = em menus, 2 = em construcao 
    private ReceitaDeCrafting moduloCriado;
    private bool podeAnimar = true;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //vidaAtual = vidaMaxima;
        //inventario.BarraDeVida.GetComponent<barraDeVida>().AtualizaBarraDeVida(vidaAtual);
        animatorPicareta = armaMelee.GetComponent<Animator>();
        distanciaAtaqueMelee.x = posicaoMelee.localPosition.x;
        distanciaAtaqueMelee.y = posicaoMelee.localPosition.y;
        Tutorial();
    }

    // Update is called once per frame
    void Update()
    {
        switch (estadosJogador)
        {
            case estados.EmAcao: //movimentando
                MovimentoInput();
                InputAtirar();
                InputAtaqueMelee();
                break;
            case estados.EmMenus://interface aberta

                break;
            case estados.EmContrucao://construindo
                mousePrecionado();
                break;
            case estados.EmDialogo:
                InputProsseguirDialogo();
                break;
        }
    }
    private void FixedUpdate()
    {
         if (estadosJogador == estados.EmAcao)
        {
            rb.velocity = movimento * velocidade + baguncarControles;
        }
         else if (estadosJogador == estados.SendoEmpurrado)
        {
            rb.velocity = -forcaEmpurrao * direcaoEmpurrao;
        }
         else
        {
            rb.velocity = Vector2.zero;
        }
    }
    private void MovimentoInput()
    {
        float movX = Input.GetAxisRaw("Horizontal");
        float movY = Input.GetAxisRaw("Vertical");
        MudaAreaAtaque(movX, movY);
        movimento = new Vector2(movX, movY).normalized;//normalized faz com q o movimento seja igual para todas as direções, não passando de um limite de 1
    }
    private Vector3 PegaPosicoMouse()
    {
        Vector3 posicaoMouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        posicaoMouse.z = 0f;
        return posicaoMouse;
    }
    private void InputAtirar()//dispara ao apertar o botão direito do mouse
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            StartCoroutine(this.atirar());
        }
    }
    private void InputAtaqueMelee()// animação e inflinge dano caso encontre algo
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) //&& Time.time > podeAtacar)
        {
            StartCoroutine(this.atacarMelee());
        }
    }
    
    private void InputProsseguirDialogo()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DialogeManager.Instance.MostraProximoDialogo();
        }
    }
    private void MudaAreaAtaque(float movX, float movY)// atualiza a direção do ataque melee sempre que o jogador muda de direção
    {
        if (Input.GetButton("Horizontal")) //&& movX != 0)
        {
            posicaoMelee.localPosition =  new Vector3(movX * Mathf.Abs(distanciaAtaqueMelee.x), distanciaAtaqueMelee.y, 0f);
            pontoDeDisparo.localPosition = new Vector3(movX * Mathf.Abs(pontoDeDisparo.localPosition.x), 0f, 0f);
        } 
        if (Input.GetButton("Vertical")) //&& movY != 0)
        {
            posicaoMelee.localPosition = new Vector3(0f, movY * Mathf.Abs(distanciaAtaqueMelee.x), 0f);
            pontoDeDisparo.localPosition = new Vector3(pontoDeDisparo.localPosition.x, movY * Mathf.Abs(pontoDeDisparo.localPosition.y), 0f);
        }
    }
    private void mousePrecionado()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePos = PegaPosicoMouse();
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit)
            {
                Clicavel clicavel = hit.collider.GetComponent<Clicavel>();
                clicavel?.Click(this.gameObject);
            }
        }
    }
    public void MudarEstadoJogador(int i)
    {
        switch (i)
        {
            case 0:
                estadosJogador = estados.EmAcao;
                podeAnimar = true;
                break;
            case 1:
                estadosJogador = estados.EmMenus;
                podeAnimar = false;
                break;
            case 2:
                estadosJogador = estados.EmContrucao;
                podeAnimar = false;
                break;
            case 3:
                estadosJogador = estados.EmDialogo;
                podeAnimar = false;
                break;
            case 4:
                estadosJogador = estados.SendoEmpurrado;
                podeAnimar = false;
                break;
        }
    }
    public void Knockback(float duracaoEmpurrao, float forca, Transform obj)
    {
        MudarEstadoJogador(4);
        direcaoEmpurrao = (obj.transform.position - this.transform.position).normalized;
        forcaEmpurrao = forca;;
        StartCoroutine(this.duracaoKnockback(duracaoEmpurrao));
    }
    private IEnumerator duracaoKnockback(float duracaoEmpurrao)
    {
        yield return new WaitForSeconds(duracaoEmpurrao);
        MudarEstadoJogador(0);
    }
    IEnumerator atirar()
    {
        if (!atirando)
        {
            atirando = true;
            Vector3 dirAnim = (PegaPosicoMouse() - transform.position).normalized;
            JogadorAnimScript.Instance.AnimarDisparo(dirAnim.x, dirAnim.y);
            GameObject bala = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);
            Vector3 direcao = PegaPosicoMouse() - pontoDeDisparo.position;
            bala.GetComponent<Rigidbody2D>().velocity = direcao.normalized * velocidadeProjetil;
            bala.GetComponent<balaHit>().dano = danoProjetil;
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(taxaDeDisparo);
            atirando = false;
        }
    }
    IEnumerator atacarMelee()
    {
        if (!atacando)
        {
            atacando = true;
            animatorPicareta.SetTrigger("ATACANDO");
            JogadorAnimScript.Instance.AnimarAtaqueMelee(posicaoMelee.localPosition.x, posicaoMelee.localPosition.y);
            Collider2D[] objetosAcertados = Physics2D.OverlapCircleAll(posicaoMelee.position, alcanceMelee, objetosAcertaveisLayer);//hit em objetos
            foreach (Collider2D objeto in objetosAcertados)
            {
                if (objeto.gameObject.layer == 8)
                {
                    objeto.GetComponent<hitbox_inimigo>().inimigo.GetComponent<inimigoScript>().mudancaVida(-danoMelee);
                }
                else if (objeto.gameObject.layer == 9)
                {
                    objeto.gameObject.GetComponent<CentroDeRecurso>().RecebeuHit();
                }
            }
            yield return new WaitForSeconds(taxaDeAtaqueMelee);
            atacando = false;
        }
    }
    public void mudancaRelogio(float valor)
    {
        if (desastreManager.Instance.VerificarSeUmDesastreEstaAcontecendo())
        {
            desastreManager.Instance.MudarTempoAcumuladoParaDesastre(desastreManager.Instance.GetTempoAcumuladoParaDesastre() + valor);
        }
        else
        {
            desastreManager.Instance.DiminuirTempoRestanteParaDesastre(valor);
            desastreManager.Instance.ConfigurarTimer(desastreManager.Instance.GetTempoRestanteParaDesastre(), 0f);
            //desastreManager.Instance.ConfigurarTimer(desastreManager.Instance.tempoRestante -= valor, 0f);
        }
        //vidaAtual += valor;
        //inventario.BarraDeVida.GetComponent<barraDeVida>().AtualizaBarraDeVida(vidaAtual);
        //if (vidaAtual > vidaMaxima)
        //{
        //    vidaAtual = vidaMaxima;
        //}
        //else if (vidaAtual <= 0)
        //{
        //    vidaAtual = 0f;
        //}
    }
    public void IndicarInteracaoPossivel(Sprite imagem, bool visivel)
    {
        if (visivel)
        {
            iconeInteracao.sprite = imagem;
            iconeInteracao.enabled = true;
        }
        else if(!visivel)
        {
            iconeInteracao.enabled = false;
        }
    }
    public void BaguncaControles(float unidadesX, float unidadesY, float forcaGeral)
    {
        baguncarControles = new Vector2(Random.Range(-unidadesX, unidadesX),Random.Range(-unidadesY, unidadesY)) * forcaGeral;
        if (baguncarControles.x == 0)
            baguncarControles.x = 1;
        if (baguncarControles.y == 0)
            baguncarControles.y = 1;
    }
    private void OnDrawGizmosSelected()
    {
        if (posicaoMelee == null)
            return;
        Gizmos.DrawWireSphere(posicaoMelee.position, alcanceMelee);
    }
    public bool GetPodeAnimar()
    {
        return podeAnimar;
    }

    public void SetTutorial(bool b)
    {
        tutorial = b;
    }
    public void Tutorial()
    {
        if (tutorial)
        {
            TutorialSetUp.Instance.SetupInicialJogador();
            tutorial = false;
        }
    }
    public void AoLevantar()
    {
        JogadorAnimScript.Instance.Levantar(false);
        TutorialSetUp.Instance.IniciarDialogo();
    }

    public void AoFinalizarDialogo(object origem, System.EventArgs args)
    {
        TutorialSetUp.Instance.AoTerminoDoDialogoFocarCameraNoJogador();
    }
    public void SetDirecaoDeMovimentacaoAleatoria(Vector2 vec)
    {
        baguncarControles = vec;
    }
    public ReceitaDeCrafting GetModuloConstruido()
    {
        return moduloCriado;
    }
    public void SetModuloConstruido(ReceitaDeCrafting modulo)
    {
        moduloCriado = ScriptableObject.CreateInstance<ReceitaDeCrafting>();
        moduloCriado = modulo;
    }
}
