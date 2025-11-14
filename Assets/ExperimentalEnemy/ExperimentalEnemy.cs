using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class ExperimentalEnemy : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Arrastra aquí el Player u otro objetivo. Si se deja vacío busca por tag al iniciar.")]
    [SerializeField] private Transform target; // Serializable so it can be dragged
    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    [Header("NavMesh (Opcional)")]
    [Tooltip("Si existe un NavMeshAgent se usará para pathfinding.")]
    public bool useNavMeshIfAvailable = true;
    [Tooltip("Tiempo mínimo entre recalcular destino en NavMesh.")]
    public float navUpdateInterval = 0.1f;

    [Header("Detection")]
    public string playerTag = "Player";

    private CharacterController cc;
    private NavMeshAgent agent;
    private float _nextNavUpdateTime;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // Sin control automático de rotación para que coincida con nuestra lógica
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0f;
        }
    }

    void Start()
    {
        TryAcquireTarget();
    }

    // Public method to set target at runtime by external scripts or UI
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    [ContextMenu("Asignar Player por Tag Ahora")]
    private void ContextAssignPlayer()
    {
        TryAcquireTarget();
    }

    private void TryAcquireTarget()
    {
        if (target == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    void OnValidate()
    {
        // Garantiza que los valores tengan límites razonables
        moveSpeed = Mathf.Max(0f, moveSpeed);
        rotateSpeed = Mathf.Max(0f, rotateSpeed);
    }

    void Update()
    {
        if (target == null)
        {
            // Attempt to reacquire if lost
            TryAcquireTarget();
            if (target == null) return;
        }

        bool usingAgent = false;
        if (useNavMeshIfAvailable && agent != null && agent.isOnNavMesh)
        {
            usingAgent = true;
            if (Time.time >= _nextNavUpdateTime)
            {
                agent.speed = moveSpeed;
                agent.stoppingDistance = 0f;
                agent.SetDestination(target.position);
                _nextNavUpdateTime = Time.time + navUpdateInterval;
            }

            // Rotación manual hacia la velocidad deseada para smooth
            Vector3 agentVel = agent.desiredVelocity;
            if (agentVel.sqrMagnitude > 0.01f)
            {
                Vector3 flat = new Vector3(agentVel.x, 0f, agentVel.z);
                var lookRot = Quaternion.LookRotation(flat.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
            }
        }

        if (!usingAgent)
        {
            Vector3 dir = (target.position - transform.position);
            dir.y = 0f;
            // Siempre se mueve; no checamos distancia mínima

            Vector3 moveDir = dir.normalized * moveSpeed;
            var lookRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);

            if (cc != null)
            {
                cc.Move(moveDir * Time.deltaTime);
            }
            else
            {
                transform.position += moveDir * Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position, 0.3f);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
