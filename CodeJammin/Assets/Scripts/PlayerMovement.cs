using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	private BoxCollider2D coll;
	private SpriteRenderer sprite;
	private Animator anim;

	[SerializeField] private LayerMask jumpableGround;

	private float dirX = 0f;
	[SerializeField] private float moveSpeed = 7f;
	[SerializeField] private float jumpForce = 14f;

	private enum MovementState { idle, running, jumping, falling, doubleJump }

	[SerializeField] private AudioSource jumpSoundEffect;
	[SerializeField] private AudioSource dashSoundEffect;

	private int jumps = 0;


	//For Wall Jump
	[SerializeField] private LayerMask wallLayer;
	[SerializeField] private Transform wallCheck;
	[SerializeField] private float wallCheckDistance;

	private bool isWallDetectedR;
	private bool isWallDetectedL;
	private bool canWallSlide;
	private bool isWallSliding;
	private float direction;
	private Vector2 wallJumpDirection = new Vector2();


	// for dash skill
	private bool canDash = true;
	private bool isDashing;
	private float dashingPower = 24f;
	private float dashingTime = 0.2f;
	private float dashingCooldown = 0.5f;
	[SerializeField] private TrailRenderer tr;

	// Start is called before the first frame update
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<BoxCollider2D>();
		sprite = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (isDashing)
		{
			return;
		}

		// Horizontal movement
		dirX = Input.GetAxisRaw("Horizontal");
		rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

		//Find Direction of jump 
		if (dirX == -1) { direction = 1f; }
		else if (dirX == 1) { direction = -1f; }

		// Jumping
		if (Input.GetButtonDown("Jump") && (IsGrounded() || jumps < 1))
		{
			jumps++;
			Jump();
			/*jumpSoundEffect.Play();
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);*/
		}
		if (IsGrounded())
		{
			jumps = 0;
		}

		//Check wall and jump key
		if (Input.GetButtonDown("Jump") && isWallDetectedR || Input.GetButtonDown("Jump") && isWallDetectedL) 
		{ Jump(); }

		//Check if the layer is a wall layer
		CheckWall();
		UpdateAnimationState();

		if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
		{
			StartCoroutine(Dash());
		}
	}

	private void CheckWall()
	{
		isWallDetectedR = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, wallLayer);
		isWallDetectedL = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, wallLayer);

		if (!IsGrounded() && rb.velocity.y < 0f)
		{
			canWallSlide = true;
		}
		else if (!IsGrounded() && rb.velocity.y > 0f)
		{
			canWallSlide = true;
		}
	}

	private void Jump()
	{
		jumpSoundEffect.Play();
		if (isWallDetectedR)
		{
			//Wall jump code
			rb.velocity = new Vector2(wallJumpDirection.x * -direction, jumpForce);
		}
		else
		{
			//normal jump code
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);

		}

	}

	private void UpdateAnimationState()
	{
		MovementState state;

		if (dirX > 0f)
		{
			state = MovementState.running;
			sprite.flipX = false;
		}
		else if (dirX < 0f)
		{
			state = MovementState.running;
			sprite.flipX = true;
		}
		else
		{
			state = MovementState.idle;
		}

		if (rb.velocity.y > 0.1f)
		{
			if (jumps == 0)
			{
				state = MovementState.jumping;
			}
			else
			{
				state = MovementState.doubleJump;
			}
		}
		else if (rb.velocity.y < -0.1f)
		{
			state = MovementState.falling;
		}

		anim.SetInteger("state", (int)state);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
	}

	private bool IsGrounded()
	{
		return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
	}

	private IEnumerator Dash()
	{
		dashSoundEffect.Play();
		canDash = false;
		isDashing = true;
		float originalGravity = rb.gravityScale;
		rb.gravityScale = 0f;
		rb.velocity = new Vector2(dirX * dashingPower, 0f);
		tr.emitting = true;
		yield return new WaitForSeconds(dashingTime);
		tr.emitting = false;
		rb.gravityScale = originalGravity;
		isDashing = false;
		yield return new WaitForSeconds(dashingCooldown);
		canDash = true;
	}
}
