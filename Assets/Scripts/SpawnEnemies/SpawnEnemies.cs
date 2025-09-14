using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab; // Prefab del enemigo a spawnear
    [SerializeField] private float spawnRadius = 5f; // Radio de spawn
    [SerializeField] private float spawnInterval = 2f; // Intervalo entre spawns en segundos
    [SerializeField] private int maxEnemies = 10; // Máximo número de enemigos activos

    private float nextSpawnTime;
    private int currentEnemyCount = 0;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Spawnear enemigo si es tiempo y no hemos alcanzado el máximo
        if (Time.time >= nextSpawnTime && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Generar posición aleatoria dentro del radio
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Instanciar el enemigo
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemyCount++;

        // Opcional: Asignar un callback para cuando el enemigo muera y decrementar el contador
        // Esto requiere que el enemigo tenga un script que llame a un método cuando muera
        // Por simplicidad, asumimos que el enemigo se destruye solo o manejamos en otro lugar
    }

    // Dibujar gizmo para visualizar el radio de spawn (siempre visible)
    void OnDrawGizmos()
    {
        // Filled semi-transparent red circle on XZ plane
        Color fill = new Color(1f, 0f, 0f, 0.12f);
        Gizmos.color = fill;
        // Draw many small discs by drawing a scaled sphere -- works as approximate ground circle
        Matrix4x4 prev = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(90f, 0f, 0f), Vector3.one);
        Gizmos.DrawSphere(Vector3.zero, spawnRadius);
        Gizmos.matrix = prev;

        // Draw wire ring in red
        Gizmos.color = Color.red;
        DrawWireCircle(transform.position, spawnRadius, 48);
    }

    // Helper to draw a wire circle on XZ plane
    private void DrawWireCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
