using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Controls the movement of the player in its entirety

    [Header("Art")]
    public Sprite normalPlayer;
    public Sprite crouchedPlayer;
    public Sprite playerBlink;
    public Sprite playerCrouchBlink;
    public GameObject playerGFX;
    public ParticleSystem wallGrabParticles;
    [Space]

    [Header("Dimensions")]
    public float normalHeight;
    public float width;
    public float crouchedHeight;
    [Space]

    [Header("Speeds")]
    public float normalSpeed;
    public float crouchSpeed;
    public float pushSpeed;
    public float maxSpeed;
    public float inAirForce;
    public float currSpeed;
    [Space]

    [Header("Jumping")]
    public float jumpForce;
    [Space]

    public float wallJumpSideVel;
    public float wallJumpWaitTime;
    public float wallGrabFloorDist;
    public float wallGrabEndGraceTime;
    public float wallGrabSlideSpeed;
    [Space]

    [Header("Sliding")]
    public float slideForce;
    public float frictionForce;
    [Space]

    [Header("Physics/Collisions")]
    public float gravForce;
    public int groundLayer;
    public bool touchingHook = false;
    public int hookSwingForce;
    public float hookMinSwingSpeed;
    int groundLayerMask;
    [Space]

    [HideInInspector]
    public float grapplerHookDefaultDist;
    [HideInInspector]
    public bool talking;

    public string state = "normal";

    bool waiting = false;
    bool forceCrouch = false;

    bool equippedGrappler;
    string grapplerState;

    Vector3 parentPos = Vector2.zero;
    GameObject parent = null;

    GameObject hook;
    Grappler grappler;

    Camera mainCam;
    InputManager input;

    Rigidbody2D rb;
    CollManager collisions;

    GameObject pushing = null;
    int pushDir = 0;
    Vector2 lastPos;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        collisions = gameObject.GetComponentInChildren<CollManager>();

        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //Sets variables to default values
        currSpeed = normalSpeed;
        groundLayerMask = 1 << groundLayer;

        //Sets up the grappler if one exists, otherwise sets it to stationary
        if (gameObject.GetComponentInChildren<Grappler>())
        {
            SetUpGrappler(true);
        } else
        {
            grapplerState = "stationary";
        }
    }

    //Physics updates 
    void FixedUpdate()
    {
        //Establishes Axial Inputs for the 0.2 second time span
        float x = input.Axis("Horizontal");
        float y = input.Axis("Vertical");

        int xRaw = input.AxisRaw("Horizontal");
        int yRaw = input.AxisRaw("Vertical");

        if (!touchingHook && ((xRaw > 0 && !collisions.GetBools()[2]) || (xRaw < 0 && !collisions.GetBools()[1]) || xRaw == 0 || state.Equals("crouching")) && (state.Equals("normal") || state.Equals("crouching")) && ((!waiting || (xRaw > 0 && collisions.GetBools()[1])) || (!waiting || (xRaw < 0 && collisions.GetBools()[2]))) && !grapplerState.Equals("pulling"))
        {
            if (collisions.GetBools()[0])
            {
                //Sets velocity to the inputs times speed if the grappler isn't being used, the player isn't trying to walk into a wall,
                //the player is walking or crouching, the player isn't trying to go back to a wall they just jumped from, and the player is on the ground

                if (parent != null)
                {
                    //If the player has a manual parent, this changes the player's position relative to the parent and updates the logged parent position
                    gameObject.transform.position += parent.transform.position - parentPos;
                    parentPos = parent.transform.position;
                }

                rb.velocity = new Vector2(currSpeed * xRaw, rb.velocity.y);
            } else if(!state.Equals("crouching"))
            {
                //Adds a force if the player is in the air and moving and clamps the speed so the player can't go faster than intended
                rb.AddForce(new Vector2(inAirForce * xRaw, 0));
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -currSpeed, currSpeed), rb.velocity.y);
            }
        }
        else if (touchingHook)
        {
            //If the player is grappling, apply a force to swing around
            rb.AddForce(new Vector2(xRaw * hookSwingForce, 0));

            if (input.AxisRaw("Horizontal") > 0)
            {
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, hookMinSwingSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));
            }
            else if (input.AxisRaw("Horizontal") < 0)
            {
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, -hookMinSwingSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));
            }
        }

        if (state.Equals("sliding"))
        {
            //If player is crouch sliding, apply a customizable friction force
            rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * frictionForce, 0));
        }

        //Prevent the player from going too fast while falling, etc.
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));

        //Customizable gravity force
        rb.AddForce(new Vector2(0, -gravForce));

        if(state.Equals("pushing"))
        {
            pushing.transform.position += gameObject.transform.position - (Vector3)lastPos;

            if((pushDir == -1 && xRaw == 1) || (pushDir == 1 && xRaw == -1) || !pushing.GetComponent<MoveableBlock>().collWithGround)
            {
                pushing = null;
                currSpeed = normalSpeed;
                pushDir = 0;
                state = "normal";
            }
        }

        lastPos = gameObject.transform.position;

        if(state.Equals("grabbing"))
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0, -gravForce * wallGrabSlideSpeed));
        }
    }

    private void Update()
    {
        //Flips the player graphics object if the mouse is on the other side of the player
        float mouseXPos = mainCam.ScreenToWorldPoint(Input.mousePosition).x;

        if(mouseXPos >= gameObject.transform.position.x)
        {
            playerGFX.transform.localScale = new Vector3(1, 1, 1);
        } else
        {
            playerGFX.transform.localScale = new Vector3(-1, 1, 1);
        }

        //Updates the grappler state variable
        if(equippedGrappler)
        {
            grapplerState = grappler.GetState();
        } 

        //Finds the distance to the floor
        RaycastHit2D floor = Physics2D.Raycast(gameObject.GetComponentInChildren<PlayerFootCollider>().gameObject.transform.position, -gameObject.transform.up, wallGrabFloorDist, groundLayerMask);

        //Wall-Grabbing
        if ((((collisions.GetBools()[1] && collisions.GetLayers()[1] == groundLayer) || (collisions.GetBools()[2] && collisions.GetLayers()[2] == groundLayer)) && !collisions.GetBools()[0] && !waiting) && grapplerState.Equals("stationary") && (!floor || state.Equals("grabbing")))
        {
            //If the player is about to start wallgrabbing, generate particles on that side of the player
            if (collisions.GetObjects()[1] != null)
            {
                GenerateParticles(collisions.GetObjects()[1], -(width / 2));
            }
            else if (collisions.GetObjects()[2] != null)
            {
                GenerateParticles(collisions.GetObjects()[2], width / 2);
            }

            //If player is touching a wall, didn't just wallgrab, is far enough from the ground, and isn't grappling, start wallgrabbing
            state = "grabbing";

            //Wall-Jumping
            if (input.ButtonDown("Jump"))
            {
                state = "normal";

                //Generate particles where you wall-jump
                if (collisions.GetObjects()[1] != null)
                {
                    GenerateParticles(collisions.GetObjects()[1], -(width / 2));
                }
                else if (collisions.GetObjects()[2] != null)
                {
                    GenerateParticles(collisions.GetObjects()[2], width / 2);
                }

                //Set the wall-jump velocity
                if (collisions.GetBools()[1] && !collisions.GetBools()[0])
                {
                    rb.velocity = new Vector2(wallJumpSideVel, jumpForce);
                }
                else if (collisions.GetBools()[2])
                {
                    rb.velocity = new Vector2(-wallJumpSideVel, jumpForce);
                }

                waiting = true;
                StartCoroutine(WaitForGrab());
            } else if((input.AxisRaw("Horizontal") < 0 && collisions.GetBools()[2]) || (input.AxisRaw("Horizontal") > 0 && collisions.GetBools()[1]))
            {
                //Fall off the wall if the player is moving off of it and wall-grabbing
                StartCoroutine(FallOffWall());
            }
        }
        else if (state.Equals("grabbing"))
        {
            //Failsafe if the state wasn't correctly set
            state = "normal";
        }
        else if (input.ButtonDown("Jump") && collisions.GetBools()[0] && state.Equals("normal") && !touchingHook)
        {
            //Normal jump if the player is not wall-grabbing and on the ground and not grappling
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (currSpeed != normalSpeed && state.Equals("normal"))
        {
            //Failsafe if the speed is not set correctly
            currSpeed = normalSpeed;
        }

        //Sets up basic crouching and uncrouching
        if (input.Button("Crouch") && state.Equals("normal"))
        {
            state = "crouching";
            Crouch();
        }
        else if ((state.Equals("crouching") || state.Equals("sliding")) && !input.Button("Crouch"))
        {
            state = "normal";
            Crouch();
        }

        //Updates the player if they've been forced to crouch and are out of the tunnel
        if (!UnCrouchTest() && forceCrouch)
        {
            forceCrouch = false;
            state = "normal";
            Crouch();
        }

        //Resets velocity after sliding
        if (state.Equals("sliding") && Mathf.Abs(rb.velocity.x) <= crouchSpeed)
        {
            state = "crouching";
            Crouch();
        }

        if (hook != null)
        {
            if (Mathf.Round(Vector2.Distance(grappler.transform.position, hook.transform.position)) == 0 && grappler.GetPulling() != null)
            {
                grappler.Retract();
            }
        }

        //Failsafe if the sprites don't update correctly
        if (!state.Equals("crouching") && !state.Equals("sliding") && playerGFX.GetComponent<SpriteRenderer>().sprite != normalPlayer && playerGFX.GetComponent<SpriteRenderer>().sprite != playerBlink)
        {
            playerGFX.GetComponent<SpriteRenderer>().sprite = normalPlayer;
        }
        else if (state.Equals("crouching") && state.Equals("sliding") && playerGFX.GetComponent<SpriteRenderer>().sprite != crouchedPlayer && playerGFX.GetComponent<SpriteRenderer>().sprite != playerCrouchBlink)
        {
            playerGFX.GetComponent<SpriteRenderer>().sprite = crouchedPlayer;
        }

        //Failsafe if the collider didn't update after crouching
        if(state.Equals("normal") && gameObject.GetComponent<BoxCollider2D>().size.y == crouchedHeight)
        {
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x, normalHeight);
        }
    }

    public void Crouch()
    {
        if (state.Equals("crouching"))
        {
            if(!grapplerState.Equals("stationary"))
            {
                grappler.Retract();
            }

            //If the player wants to crouch, start crouching
            playerGFX.GetComponent<SpriteRenderer>().sprite = crouchedPlayer;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x, crouchedHeight);
            currSpeed = crouchSpeed;

            //If the player's velocity is high enough, start sliding
            if (Mathf.Abs(rb.velocity.x) > crouchSpeed && !state.Equals("sliding"))
            {
                state = "sliding";
                rb.AddForce(new Vector2(Mathf.Sign(rb.velocity.x) * slideForce, 0));
            }
        } else
        {
            //If the player doesn't want to crouch, check if there's anything above the player and uncrouch if there's nothing
            if(!UnCrouchTest())
            {
                state = "normal";
                playerGFX.GetComponent<SpriteRenderer>().sprite = normalPlayer;
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x, normalHeight);
                currSpeed = normalSpeed;
            } else
            {
                //Force the player to crouch if there's something in its way
                state = "crouching";
                forceCrouch = true;
            }
        }
    }

    public void SetUpGrappler (bool equip)
    {
        //Sets the values of the grappler if one exists, if not, delete the grappler values
        equippedGrappler = equip;

        if(equip)
        {
            grappler = gameObject.GetComponentInChildren<Grappler>();
            hook = gameObject.GetComponentInChildren<Hook>().gameObject;
            grapplerHookDefaultDist = Vector2.Distance(grappler.transform.position, hook.transform.position);
        } else
        {
            grappler = null;
            hook = null;
            grapplerHookDefaultDist = 0;

            grapplerState = "stationary";
        }
    }

    bool UnCrouchTest()
    {
        //Draws raycasts above the player on the two sides and in the middle up to the height of the normal player, returns true if there's something above the player
        RaycastHit2D hit1 = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, normalHeight / 2, groundLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - (width / 2), gameObject.transform.position.y), gameObject.transform.up, normalHeight / 2, groundLayerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + (width / 2), gameObject.transform.position.y), gameObject.transform.up, normalHeight / 2, groundLayerMask);

        if(hit1 || hit2 || hit3)
        {
            return true;
        }

        return false;
    }

    void GenerateParticles(GameObject collision, float dir)
    {
        //If this if statement wasn't here, it would play every frame, don't remove it
        if (!state.Equals("grabbing") && collision.GetComponent<Tilemap>())
        {
            //Sets the particle generator to the side of the player it needs to be on
            wallGrabParticles.gameObject.transform.localPosition = new Vector2(dir, 0);

            //Finds the position of the tile the particles will sample from
            RaycastHit2D hit;

            if (dir > 0)
            {
                hit = Physics2D.Raycast(gameObject.transform.position, Vector2.right);
            } else
            {
                hit = Physics2D.Raycast(gameObject.transform.position, Vector2.left);
            }

            int xDiff = (int)(Mathf.Abs(hit.point.x) - Mathf.Abs(gameObject.transform.position.x)) + 1;
            xDiff *= (int)Mathf.Sign(dir);

            if (collision.GetComponent<Tilemap>().GetTile<Tile>(Vector3Int.FloorToInt(gameObject.transform.position + new Vector3(xDiff, 0, 0))))
            {
                //Gets a reference to the sprite on the tile the particles sample from
                Sprite tileSprite = collision.GetComponent<Tilemap>().GetTile<Tile>(Vector3Int.FloorToInt(gameObject.transform.position + new Vector3(xDiff, 0, 0))).sprite;

                //Finds the position of the sprite in the texture
                Texture2D tileTexture = tileSprite.texture;

                Vector2Int spriteOffset = new Vector2Int((int)tileSprite.rect.x, (int)tileSprite.rect.y);

                //Sets the color of the particles and plays them
                var main = wallGrabParticles.main;

                main.startColor = tileTexture.GetPixel(spriteOffset.x, spriteOffset.y);
                main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 1);

                wallGrabParticles.Play();
            }
        }
    }

    public string GetState()
    {
        return state;
    }

    public void SetSpeed(float temp)
    {
        currSpeed = temp;
    }

    public string GetGrapplerState()
    {
        return grapplerState;
    }

    IEnumerator FallOffWall()
    {
        //Falls off the wall after a certain amount of time when grappling to prevent the player from falling off when they wanted to wall-jump
        yield return new WaitForSecondsRealtime(wallGrabEndGraceTime);

        if(state.Equals("grabbing"))
        {
            state = "normal";
            waiting = true;
            StartCoroutine(WaitForGrab());
        }
    }

    IEnumerator WaitForGrab()
    {
        //Forces the player to not wall-grab for a certain amount of time
        yield return new WaitForSecondsRealtime(wallJumpWaitTime);
        waiting = false;
    }

    //Sets the manual parent of the player to a moving platform when colliding with one and logs when the player is touching the edge of the grappler
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == hook)
        {
            touchingHook = true;
        } else if (collision.gameObject.CompareTag("Moving Platform"))
        {
            parent = collision.gameObject;
            parentPos = collision.gameObject.transform.position;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject == hook)
        {
            touchingHook = false;
        } else if(collision.gameObject.CompareTag("Moving Platform"))
        {
            parent = null;
            parentPos = Vector2.zero;
        }
    }
}
