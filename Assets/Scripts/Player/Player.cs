using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;

public class Player : Character
{
    #region Fields

    [SerializeField]StateBar_HUD stateBar_HUD;
    [SerializeField]bool restoreHealth = true;
    [SerializeField]float restoreHealthTime;
    [SerializeField,Range(0f,1f)]float damageHealthPercent;

    [SerializeField]PlayerInput playerInput;

    [Header("----MOVE----")]
    [SerializeField]float moveSpeed = 10f;
    [SerializeField]float accelerationTime = 3f;
    [SerializeField]float decelerationTime = 3f;
    [SerializeField]float moveRatationAngle = 50f;

    [Header("----FIRE----")]
    [SerializeField]GameObject projectile1;
    [SerializeField]GameObject projectile2;
    [SerializeField]GameObject projectile3;
    [SerializeField]GameObject projectileOverDriven;
    [SerializeField]ParticleSystem muzzleVFX;
    [SerializeField]Transform fireUpPos;
    [SerializeField]Transform fireMiddlePos;
    [SerializeField]Transform fireDownPos;
    [SerializeField]AudioData projectileLaunchSFX;
    [SerializeField,Range(0,2)]int weaponPower = 0;
    [SerializeField]float fireInterval = 0.2f;

    [Header("----DODGE----")] 
    [SerializeField]AudioData dodgeSFX;
    [SerializeField,Range(0,100)]int dodgeEnergyCost = 25;
    [SerializeField] private float maxRoll = 720f;
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);
    private float currentRoll;
    private float dodgeDuration;
    private bool isDodging = false;

    [Header("----OVERDRIVEN----")] 
    [SerializeField] private int overDrivnDodgeFactor = 2;
    [SerializeField] private float overDrivenSpeedFactor = 1.2f;
    [SerializeField] private float overDrivenFireFactor = 1.2f;
    [SerializeField] float slowMotionDuration = 1f;
    private bool isOverDiving = false;
    
    float paddingX;
    float paddingY;
    
    private float t;
    private Vector2 preciousVelocity;
    private Vector2 moveDirection;
    private Quaternion preciousRotation;
    private const float invincibleTime = 1f;

    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    WaitForSeconds waitForFireSeconds;
    WaitForSeconds waitRestoreHealthTime;
    WaitForSeconds waitForOverDrivenFireInterval;
    WaitForSeconds waitForDecelerateTime;
    WaitForSeconds waitForInvincibleTime;

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    Coroutine moveCoroutine;
    Coroutine restoreHealthCoroutine;

    private MissileSystem missile;

    #endregion

    #region Properties

    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2;

    #endregion
    private void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;
        
        waitForFireSeconds = new WaitForSeconds(fireInterval);
        waitRestoreHealthTime = new WaitForSeconds(restoreHealthTime);
        
        // 0.12 / 1.2 = 0.1  =>  0.12 -> 0.1
        waitForOverDrivenFireInterval = new WaitForSeconds(fireInterval / overDrivenFireFactor);

        waitForDecelerateTime = new WaitForSeconds(decelerationTime);
        waitForInvincibleTime = new WaitForSeconds(invincibleTime);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerInput.onMove += Move;
        playerInput.onStopMove += StopMove;
        playerInput.onFire += Fire;
        playerInput.onStopFire += StopFire;
        playerInput.onDodge += Dodge;
        playerInput.onOverDirven += OverDriven;
        playerInput.onLaunchMissile += LaunchMissile;

        PlayerOverDriven.on += OverDrivenOn;
        PlayerOverDriven.off += OverDrivenOff;
    }

    void Disable()
    {
        playerInput.onMove -= Move;
        playerInput.onStopMove -= StopMove;
        playerInput.onFire -= Fire;
        playerInput.onStopFire -= StopFire;
        playerInput.onDodge -= Dodge;
        playerInput.onOverDirven -= OverDriven;
        playerInput.onLaunchMissile -= LaunchMissile;

        PlayerOverDriven.on -= OverDrivenOn;
        PlayerOverDriven.off -= OverDrivenOff;
    }

    private void Start() 
    {
        playerInput.EnableGamePlayInput();

        stateBar_HUD.Initialize(health,maxHealth);

        #if UNITY_EDITOR
        TakeDamage(50f);
        #endif
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        stateBar_HUD.UpdateState(health,maxHealth);
        TimeController.instance.BulletTime(slowMotionDuration);

        if(gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(nameof(InvincibleCoroutine));
            
            if(restoreHealth)
            {
                if(restoreHealthCoroutine != null)
                {
                    StopCoroutine(restoreHealthCoroutine);
                }
                restoreHealthCoroutine = StartCoroutine(RestoreHealthCoroutine(waitRestoreHealthTime,damageHealthPercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        stateBar_HUD.UpdateState(health,maxHealth);

        #if UNITY_EDITOR
        Debug.Log("Restore Health: Current Health:" + health + "Time:" + Time.time);
        #endif
    }

    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        stateBar_HUD.UpdateState(0f,maxHealth);
        base.Die();
    }

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;

        yield return waitForInvincibleTime;

        collider.isTrigger = false;
    }
    
    #region Move
    void Move(Vector2 moveInput)
    {
        //移动量
        //Vector2 moveAmount = moveInput * moveSpeed;

        //rigidbody.velocity = moveInput * moveSpeed;
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        //Quaternion moveRatation = Quaternion.AngleAxis(moveRatationAngle * moveInput.y, Vector3.right);

        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime,moveDirection * moveSpeed,
                                                    Quaternion.AngleAxis(moveRatationAngle * moveInput.y, Vector3.right)));
        StopCoroutine(nameof(DecelerateCoroutine));
        StartCoroutine(nameof(MovePositionLimitCoroutine));
    }

    void StopMove()
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        
        //rigidbody.velocity  = Vector2.zero;
        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime,moveDirection,Quaternion.identity));
        //StopCoroutine(nameof(MovePositionLimitCoroutine));
        StartCoroutine(nameof(DecelerateCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRatation)
    {
        t = 0f;

        while(t < 1f)
        {
            //t increase continuously
            //t += time/1 * Time.fixedDeltaTime;
            t += Time.fixedDeltaTime / time;
            preciousVelocity = rigidbody.velocity;
            preciousRotation = transform.rotation;
            
            rigidbody.velocity = Vector2.Lerp(preciousVelocity,moveVelocity,t);
            transform.rotation = Quaternion.Lerp(preciousRotation,moveRatation,t);

            yield return waitForFixedUpdate;
        }

        // while(t < time)
        // {
        //     //t increase continuously
        //     //t += time/1 * Time.fixedDeltaTime;
        //     t += Time.fixedDeltaTime;
        //     rigidbody.velocity = Vector2.Lerp(rigidbody.velocity,moveVelocity,t / time);
        //     transform.rotation = Quaternion.Lerp(transform.rotation,moveRatation,t / time);

        //     yield return null;
        // }
    }

    IEnumerator MovePositionLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.instance.PlayerMoveablePosition(transform.position,paddingX,paddingY);    

            yield return null;
        }
    }

    IEnumerator DecelerateCoroutine()
    {
        yield return waitForDecelerateTime;

        StartCoroutine(nameof(MovePositionLimitCoroutine));
    }
    
    #endregion

    #region Fire
    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {
        while (true)
        {
            // switch(weaponPower)
            // {
            //     case 0:
            //         Instantiate(projectile1,fireMiddlePos.position,Quaternion.identity);
            //         break;
            //     case 1:
            //         Instantiate(projectile2,fireUpPos.position,Quaternion.identity);
            //         Instantiate(projectile3,fireDownPos.position,Quaternion.identity);
            //         break;
            //     case 2:
            //         Instantiate(projectile2,fireUpPos.position,Quaternion.identity);
            //         Instantiate(projectile1,fireMiddlePos.position,Quaternion.identity);
            //         Instantiate(projectile3,fireDownPos.position,Quaternion.identity);
            //         break;
            // }

            switch(weaponPower)
            {
                case 0:
                    PoolManager.Release(Projectile(),fireMiddlePos.position);
                    break;
                case 1:
                    PoolManager.Release(Projectile(),fireUpPos.position);
                    PoolManager.Release(Projectile(),fireDownPos.position);
                    break;
                case 2:
                    PoolManager.Release(Projectile(),fireUpPos.position);
                    PoolManager.Release(Projectile(),fireMiddlePos.position);
                    PoolManager.Release(Projectile(),fireDownPos.position);
                    break;
                default:
                    break;
            }
            
            AudioManager.Instance.PlaySFX(projectileLaunchSFX);

            //yield return waitForSeconds;

            if (isOverDiving)
            {
                yield return waitForOverDrivenFireInterval;
            }
            else
            {
                yield return waitForFireSeconds;
            }
        }
    }

    GameObject Projectile()
    {
        if (isOverDiving)
        {
            return projectileOverDriven;
        }
        else
        {
            return projectile1;
        }

        //return isOverDiving ? projectileOverDriven : projectile1;
    }


    #endregion

    #region Dodge

    void Dodge()
    {
        if(isDodging || !PlayerEnergy.instance.IsEnough(dodgeEnergyCost)) return;
        
        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine()
    {
        //cost the energy
        //make player invincible when dodge
        //make player rotate x axis
        //change player's scale
        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        PlayerEnergy.instance.Use(dodgeEnergyCost);

        collider.isTrigger = true;

        var scale = transform.localScale;

        currentRoll = 0f;

        #region the first method
        // while (currentRoll < maxRoll)
        // {
        //     currentRoll += rollSpeed * Time.deltaTime;
        //     transform.rotation = Quaternion.AngleAxis(currentRoll,Vector3.right);
        //
        //     if (currentRoll < maxRoll / 2f)
        //     {
        //         //limit scale to Range(dodgeScale,Vector.one)
        //         scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration,dodgeScale.x,1);
        //         scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration,dodgeScale.y,1);
        //         scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration,dodgeScale.z,1);
        //     }
        //     else
        //     {
        //         scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration,dodgeScale.x,1);
        //         scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration,dodgeScale.y,1);
        //         scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration,dodgeScale.z,1);
        //     }
        //
        //     transform.localScale = scale;
        //     
        //     yield return null;
        // }
        #endregion

        #region the second method
        // var t1 = 0f;
        // var t2 = 0f;
        // while (currentRoll < maxRoll)
        // {
        //     currentRoll += rollSpeed * Time.deltaTime;
        //     transform.rotation = Quaternion.AngleAxis(currentRoll,Vector3.right);
        //
        //     if (currentRoll < maxRoll / 2f)
        //     {
        //         //limit scale to Range(dodgeScale,Vector.one)
        //         t1 += Time.deltaTime / dodgeDuration;
        //         transform.localScale = Vector3.Lerp(transform.localScale, dodgeScale, t1);
        //     }
        //     else
        //     {
        //         t2 += Time.deltaTime / dodgeDuration;
        //         transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t2);
        //     }
        //
        //     transform.localScale = scale;
        //     
        //     yield return null;
        // }
        #endregion

        #region the third method

        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll,Vector3.right);
            transform.localScale =
                BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }

        #endregion
        
        collider.isTrigger = false;

        isDodging = false;
    }
    
    #endregion

    #region OverDriven

    void OverDriven()
    {
        if(!PlayerEnergy.instance.IsEnough(PlayerEnergy.MAX))
            return;
        
        PlayerOverDriven.on.Invoke();

    }

    void OverDrivenOn()
    {
        isOverDiving = true;
        dodgeEnergyCost *= overDrivnDodgeFactor;
        moveSpeed *= overDrivenSpeedFactor;
        
        TimeController.instance.BulletTime(slowMotionDuration,slowMotionDuration);
    }

    void OverDrivenOff()
    {
        isOverDiving = false;
        dodgeEnergyCost /= overDrivnDodgeFactor;
        moveSpeed /= overDrivenSpeedFactor;
    }

    #endregion

    #region Missile
    void LaunchMissile()
    {
        missile.Launch(fireMiddlePos);
    }

    public void PickUpMissile()
    {
        missile.PickUp();
    }

    #endregion

    #region Weapon Power

    public void PowerUp()
    {
        // weaponPower++;
        // weaponPower = Mathf.Clamp(weaponPower,0,2);
        weaponPower = Mathf.Min(weaponPower + 1,2);
    }

    public void PowerDown()
    {
        weaponPower = Mathf.Max(--weaponPower,0);
    }
    #endregion
}
