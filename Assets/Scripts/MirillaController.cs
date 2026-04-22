using UnityEngine;

public class MirillaController : MonoBehaviour
{
    // Start se ejecuta una sola vez al arrancar el juego
    void Start()
    {
        // Ocultamos el cursor del ratón de Windows
        Cursor.visible = false;
    }

    // Update se ejecuta en cada fotograma
    void Update()
    {
        // Obligamos a la imagen de la mirilla a ir a donde está el ratón invisible
        transform.position = Input.mousePosition;
    }
}
