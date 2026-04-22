using UnityEngine;

public class ManchaController : MonoBehaviour
{
    // Creado con "public" para que el botón de Unity pueda ver esta orden
    public void Limpiar()
    {
        // Busca al GameManager en el juego y le dice que sume 1 punto
        FindObjectOfType<GameManager>().SumarPunto();

        // Esto envía un mensaje a la consola
        Debug.Log("¡Mancha eliminada!");

        // Esto destruye (borra) de la pantalla el objeto que tiene este código
        Destroy(gameObject);
    }
}
