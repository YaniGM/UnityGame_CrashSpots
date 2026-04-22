using UnityEngine;

public class PistolaController : MonoBehaviour
{
    // Esta variable nos permitirá calibrar el cañón desde Unity si queda torcido
    public float ajusteRotacion = 0f;

    // Restará píxeles a la longitud del láser para que no tape la mirilla
    public float recorteLaser = 25f;

    // Referencia visual a la imagen de tu láser
    public RectTransform rayoLaser;

    // Variables para el parpadeo del láser
    private float temporizadorLaser = 0f;
    private float tiempoLaserVisible = 0.15f; // El láser durará 0.15 segundos en pantalla

    // Update se llama una vez por cada "fotograma" (frame) del juego
    void Update()
    {
        // Calculamos la distancia entre la posición del ratón y la posición de la pistola
        Vector3 direccion = Input.mousePosition - transform.position;

        // Usamos matemáticas (Atan2) para convertir esa distancia en un ángulo en grados
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        // Aplicamos la rotación solo en el eje Z (que es el que hace girar cosas en 2D)
        transform.rotation = Quaternion.Euler(0, 0, angulo + ajusteRotacion);

        // Si hacemos clic izquierdo...
        if (Input.GetMouseButtonDown(0))
        {
            DispararLaser(direccion);
        }

        // Temporizador para apagar el láser rápidamente (efecto chispazo)
        if (temporizadorLaser > 0)
        {
            temporizadorLaser -= Time.deltaTime; // Cuenta atrás
            if (temporizadorLaser <= 0)
            {
                rayoLaser.gameObject.SetActive(false); // Apagamos el láser
            }
        }
    }

    void DispararLaser(Vector3 direccion)
    {
        // Encendemos la imagen y reseteamos el reloj
        rayoLaser.gameObject.SetActive(true);
        temporizadorLaser = tiempoLaserVisible;

        // Medimos la distancia exacta en píxeles desde la punta del cañón hasta el ratón
        float distanciaExacta = Vector3.Distance(rayoLaser.position, Input.mousePosition);

        // Estiramos la altura (Y) del láser a esa distancia exacta, sin cambiar su grosor (X) y le restamos
        // el exceso hasta el centro de la mirilla
        rayoLaser.sizeDelta = new Vector2(rayoLaser.sizeDelta.x, distanciaExacta - recorteLaser);

    }
}
