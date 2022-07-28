using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerTouch : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] TextMeshProUGUI scoreText;
    public GameObject deathMenuUI;
    public HeartSystem playerDead;
    public HeartSystem TakeDamage;

    public Joystick joyStick;
    

    #region Player Objects

    private Animator player_Animator;
    public Rigidbody2D player_RB2D;
    private GroundChecker groundSensor;
    private bool isGrounded = false;
    private bool playerIdle = false;
    public AudioSource playerAudioSource;
    public Collider2D playerCollider;
    [SerializeField] float playerSpeed = 4.0f;
    [SerializeField] float playerJumpForce = 7.5f;

    #endregion

    #region Drone Objects

    public Animator droneAnimator;
    public Rigidbody2D droneRB2D;
    public Collider2D droneCollider;

    #endregion

    #region Gem Objects

    public AudioSource gemAudio;
    public Collider2D gemCollider;
    public Rigidbody2D gemRB2D;
    public GameObject gemClone;

    #endregion

    // Use this for initialization
    void Start()
    {
        player_Animator = GetComponent<Animator>();
        player_RB2D = GetComponent<Rigidbody2D>();
        playerAudioSource = GetComponent<AudioSource>();
        groundSensor = transform.Find("GroundSensor").GetComponent<GroundChecker>();
        scoreText.text = "Score: " + Scoring.CurrentScore;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gem") && !deathMenuUI.activeSelf)
        {
            Destroy(collision.gameObject);
            gemAudio.Play();
            Scoring.CurrentScore += 1;
            scoreText.text = "Score: " + Scoring.CurrentScore;
        }
    }

    void Update()
    {
        //Check if character just landed on the ground
        if (!isGrounded && groundSensor.State())
        {
            isGrounded = true;
            player_Animator.SetBool("Grounded", isGrounded);
        }

        //Check if character just started falling
        if (isGrounded && !groundSensor.State())
        {
            isGrounded = false;
            player_Animator.SetBool("Grounded", isGrounded);
        }

        // -- Handle input and movement --
        float horizontalMove = 0f;
        
        float verticalMove = joyStick.Vertical;

        if (joyStick.Horizontal >= 0.001f)
        {
            horizontalMove = joyStick.Horizontal * playerSpeed;
        }
        else if (joyStick.Horizontal <= -0.001f)
        {
            horizontalMove = joyStick.Horizontal * playerSpeed;
        }
        else
        {
            horizontalMove = 0f;
        }
       

        // Swap direction of sprite depending on walk direction
        if (horizontalMove > 0)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        }
            
        else if (horizontalMove < 0)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
            
        // Move
        player_RB2D.velocity = new Vector2(horizontalMove * playerSpeed, player_RB2D.velocity.y);

        //Set AirSpeed in animator
        player_Animator.SetFloat("AirSpeed", player_RB2D.velocity.y);

        //teleport animation
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            player_Animator.SetInteger("AnimState", 5);
            player_Animator.SetBool("teleporting", true);
        }

        //Jump
        if (verticalMove >= 0.8f && isGrounded)
        {
            player_Animator.SetInteger("AnimState", 3);
            isGrounded = false;
            player_Animator.SetBool("Grounded", isGrounded);
            player_RB2D.velocity = new Vector2(player_RB2D.velocity.x, verticalMove * playerJumpForce);
            groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(horizontalMove) > Mathf.Epsilon)
        {
            player_Animator.SetInteger("AnimState", 2);
        }

        else if (playerIdle)
        {
            player_Animator.SetInteger("AnimState", 1);
        }

        else if (Mathf.Abs(horizontalMove) <= Mathf.Epsilon)
        {
            player_Animator.SetInteger("AnimState", 0);
        }
            
        if (player_RB2D.position.y < -1.5 || player_RB2D.position.y > 5)
        {
            deathMenuUI.SetActive(true);
        }
    }
}