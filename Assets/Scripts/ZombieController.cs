using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Header("Zombie Settings")]
    public float detectRadius = 10f;
    public float attackRadius = 1f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float attackSpeed = 7f;
    public AudioClip deadAudio;
    private AudioSource audioSource;
    private GameObject canvas;
    private GameManager gameManager;

    private bool isDead;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Transform sheep;
    private SheepController sheepController;

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        isDead = false;

        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        sheep = GameObject.FindGameObjectWithTag("Player").transform;
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        sheepController = sheep.GetComponent<SheepController>();

        SetWalking(true);

        startPoint = new Vector3(gameObject.transform.position.x + 15f, gameObject.transform.position.y, gameObject.transform.position.z);
        endPoint = new Vector3(gameObject.transform.position.x - 15f, gameObject.transform.position.y, gameObject.transform.position.z);


        agent.speed = patrolSpeed;
        agent.SetDestination(startPoint);
        agent.isStopped = false;
    }

    void Update()
    {
        if(isDead)
        {
            anim.SetBool("isDead", true);
            StartCoroutine(DestroyZombie());
            return;
        }
        float distanceToSheep = Vector3.Distance(transform.position, sheep.position);

        // Priority: Attack > Chase > Patrol
        if (distanceToSheep <= attackRadius)
        {
            AttackSheep();
        }
        else if (distanceToSheep <= detectRadius)
        {
            ChaseSheep();
        }
        else
        {
            Patrol();
        }

        UpdateAnimation();
    }
    IEnumerator DestroyZombie()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    void SetWalking(bool isWalking)
    {
        if (isWalking)
        {
            int randomAnim = Random.Range(1, 3);
            anim.SetBool(randomAnim == 1 ? "isWalking1" : "isWalking2", true);
        }
        else      
        {
            anim.SetBool("isWalking1", false);
            anim.SetBool("isWalking2", false);
        }
    }

    void Patrol()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (Vector3.Distance(transform.position, startPoint) < 0.5f)
        {
            agent.SetDestination(endPoint);
        }
        else if (Vector3.Distance(transform.position, endPoint) < 0.5f)
        {
            agent.SetDestination(startPoint);
        }
    }


    void ChaseSheep()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        anim.SetBool("isAttacking", false);
        agent.SetDestination(sheep.position);
    }

    void AttackSheep()
    {
        agent.isStopped = true;
        agent.speed = attackSpeed;
        anim.SetBool("isAttacking", true);
        anim.SetBool("isRunning", false);
        SetWalking(false);
    }

    void UpdateAnimation()
    {
        if (anim.GetBool("isAttacking"))
            return;

        if (agent.velocity.magnitude > 0.1f)
        {
            SetWalking(agent.speed == patrolSpeed);
            anim.SetBool("isRunning", agent.speed == chaseSpeed);
        }
        else
        {
            SetWalking(false);
            anim.SetBool("isRunning", false);
        }

        // Khi rời attackRadius → tắt attacking
        if (Vector3.Distance(transform.position, sheep.position) > attackRadius)
        {
            anim.SetBool("isAttacking", false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("HitBox") && !sheepController.isAttacking)
        {
            gameManager.killCount++;
            audioSource.PlayOneShot(deadAudio);
            isDead = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && sheepController.isAttacking)
        {
            canvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
