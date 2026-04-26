using UnityEngine;
using TMPro; // Esto nos permite hablar con los textos de la interfaz
using System.Collections.Generic; // Permite el uso de listas
using System.Collections; // Herramientas para la gestión de los tiempos en las acciones del juego

public class GameManager : MonoBehaviour
{
    [Header("Generador Aleatorio")]
    public GameObject manchaPrefab;   // El molde de la mancha
    public RectTransform areaJuego;   // Canvas, para saber los límites de la pantalla
    // variables de calibración
    public float distanciaMinima = 80f; // Espacio mínimo entre manchas
    public float margenSuperior = 130f; // Espacio libre arriba para los textos
    public float margenInferior = 150f; // Espacio libre abajo para la pistola

    [Header("Configuración del Nivel")]
    public int puntosParaGanar = 25;// Puntuación máxima
    public float tiempoRestante = 30f;// Tiempo límite de 30 segundos

    [Header("Estado del Juego")]
    public int puntosTotales = 0;
    public bool juegoTerminado = false;
    public bool juegoIniciado = false;

    [Header("Interfaz (UI)")]
    public TextMeshProUGUI textoScore;
    public TextMeshProUGUI textoTime;
    public GameObject panelFinJuego;
    public TextMeshProUGUI textoResultado;

    [Header("Colores de Texto")]
    public Color colorVictoria = Color.blue;
    public Color colorDerrota = Color.red;

    [Header("Audio")]
    public AudioSource sfxReproductor;
    public AudioSource musicaFondo;
    public AudioClip sonidoManchaEliminada;
    public float retrasoSonido = 0.80f;

    // Retrasamos el pop up de cada mancha 0,05segundos
    public AudioClip sonidoAparicionMancha;
    public float retrasoAparicion = 0.15f;

    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;

    void Start()
    {
        // Al arrancar, ocultamos el panel final, repartimos manchas y actualizamos textos
        panelFinJuego.SetActive(false);
        StartCoroutine(RepartirManchas());
        ActualizarUI();
    }

    void Update()
    {
        // Si el juego ni ha terminado ni ha empezado no corre el tiempo
        if (juegoTerminado || !juegoIniciado) return;

        // Cuenta atrás
        tiempoRestante -= Time.deltaTime;

        // Si el tiempo llega a cero...
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            FinDelJuego(false); // HAS PERDIDO
        }

        ActualizarUI();
    }

    // Esta función la llamaremos luego desde las manchas cuando exploten
    public void SumarPunto()
    {
        if (juegoTerminado || !juegoIniciado) return;
        
        puntosTotales ++;
        ActualizarUI();

        if (sfxReproductor != null && sonidoManchaEliminada != null)
        {
            sfxReproductor.PlayOneShot(sonidoManchaEliminada);
        }

        if (puntosTotales >= puntosParaGanar)
        {
            FinDelJuego(true); // HAS GANADO
        }
    }

    IEnumerator ReproducirConRetraso()
    {
        // Esperamos el tiempo que hayamos definido
        yield return new WaitForSeconds(retrasoSonido);

        // Pasado ese tiempo, reproducimos el sonido
        if (sfxReproductor != null && sonidoManchaEliminada != null)
        {
            sfxReproductor.PlayOneShot(sonidoManchaEliminada);
        }
    }

    void ActualizarUI()
    {
        // Mathf.CeilToInt redondea los decimales para que el reloj se vea limpio
        textoTime.text = "time: " + Mathf.CeilToInt(tiempoRestante).ToString() + "s";
        textoScore.text = "score: " + puntosTotales.ToString("000");
    }

    void FinDelJuego(bool victoria)
    {
        juegoTerminado = true;
        panelFinJuego.SetActive(true);

        if (musicaFondo != null)
        {
            musicaFondo.Stop();
        }

        if (victoria) // Evaluamos si el jugador ha ganado o perdido
        {
            textoResultado.text = "¡HAS GANADO!";
            textoResultado.color = colorVictoria; // Seleccionamos color del texto

            if (sfxReproductor != null && sonidoVictoria != null) // Si gana suena melodía victoria
            {
                sfxReproductor.PlayOneShot(sonidoVictoria);
            }
        }
        else
        {
            textoResultado.text = "¡HAS PERDIDO!";
            textoResultado.color = colorDerrota; // Seleccionamos color del texto

            if (sfxReproductor != null && sonidoDerrota != null) // Si pierde suena melodía derrota
            {
                sfxReproductor.PlayOneShot(sonidoDerrota);
            }
        }
    }

    IEnumerator RepartirManchas()
    {
        // Calculamos el tamaño disponible de la pantalla restando un margen (50 píxeles) para que no toquen
        // los bordes
        float anchoMitad = areaJuego.rect.width / 2f - 50f;

        // Calculamos los nuevos "techos" y "suelos" respetando los textos y la pistola
        float topeSuperior = (areaJuego.rect.height / 2f) - margenSuperior;
        float topeInferior = -(areaJuego.rect.height / 2f) + margenInferior;

        // Creamos una lista para recordar las posiciones que ya hemos usado
        List<Vector2> posicionesUsadas = new List<Vector2>();

        // Un bucle que se repite 25 veces (los puntosParaGanar)
        for (int i = 0; i < puntosParaGanar; i++)
        {
            Vector2 posicionCandidata = Vector2.zero;
            bool posicionValida = false;
            int intentos = 0;

            // Buscamos un hueco. Si falla, lo reintenta hasta 100 veces para no bloquear el PC.
            while (!posicionValida && intentos < 100)
            {
                // Generamos coordenadas X e Y al azar dentro de los límites
                float randomX = Random.Range(-anchoMitad, anchoMitad);
                // Limitamos la Y por debajo (-alto + 100) para que no aparezcan justo encima de la pistola
                float randomY = Random.Range(topeInferior, topeSuperior);
                posicionCandidata = new Vector2(randomX, randomY);

                posicionValida = true; // Hueco guardado válido

                // Evaluamos las posiciones si no se superponen
                foreach (Vector2 posGuardada in posicionesUsadas)
                {
                    if (Vector2.Distance(posicionCandidata, posGuardada) < distanciaMinima)
                    {
                        posicionValida = false; // ¡Choca! Rompemos el bucle y probamos otra vez
                        break;
                    }
                }

                intentos++;

            }

            // Fabricamos el clon de la mancha y lo metemos dentro del Canvas en la posición correcta
            GameObject nuevaMancha = Instantiate(manchaPrefab, areaJuego);

            // Aplicamos la posición aleatoria a la nueva mancha y se reparte por Canvas en la posición correcta
            nuevaMancha.GetComponent<RectTransform>().anchoredPosition = posicionCandidata;

            // Guardamos esta coordenada en la Lista
            posicionesUsadas.Add(posicionCandidata);
            
            // Reproduce sonido en cada pop up de cada mancha
            if (sfxReproductor != null && sonidoAparicionMancha != null)
            {
                sfxReproductor.PlayOneShot(sonidoAparicionMancha);
            }

            // Se hace una pausa de 0,05segundos entre cada pop up
            yield return new WaitForSeconds(retrasoAparicion);
        }

        juegoIniciado = true;

        if (musicaFondo != null)
        {
            musicaFondo.Play();
        }
    }
}