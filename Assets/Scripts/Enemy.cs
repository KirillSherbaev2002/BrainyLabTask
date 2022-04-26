using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Shoter))]
public class Enemy : MonoBehaviour
{
    [Header("Shot")]
    public BulletScript Bullet;
    public Transform BulletSpawner;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float reloadingTime;
    [SerializeField] private float recoilBack;

    [Header("Movement")]
    [SerializeField] private float characterMoveSpeed;
    [SerializeField] private float characterRotateSpeed;
    private Vector3 startPosition;

    [Header("Life")]
    public ScoreScripts scoreScripts;

    [Header("Classes")]
    private IMover mover;
    private IShoter shoter;
    private IResetable resetor;
    private ISelfGuidable selfGuidable;

    [Header("Self-Guide")]
    [SerializeField] private float distanceToDetect;
    [SerializeField] private float angleAllowedToShot;

    private void Start()
    {
        mover = new Mover();
        resetor = new Resertor();
        selfGuidable = new SelfGuide();

        mover.Initialize(GetComponent<Rigidbody>(), characterMoveSpeed, characterRotateSpeed);
        shoter = gameObject.GetComponent<Shoter>();
        shoter.Initialize(Bullet, BulletSpawner, GetComponent<Rigidbody>(), bulletSpeed, reloadingTime, recoilBack);

        Player player = FindObjectOfType<Player>();
        selfGuidable.Initialize(characterRotateSpeed, characterMoveSpeed, player, shoter, angleAllowedToShot);
    }

    private void FixedUpdate()
    {
        selfGuidable.SearchNear(gameObject, distanceToDetect);
    }

    private void OnCollisionEnter(Collision collision)
    {
        resetor.CheckReset(collision, scoreScripts, mover, false);
    }

    public void OnBecameInvisible()
    {
        mover.ReturnToStartState();
    }

    public void ReturnToStartState()
    {
        mover.ReturnToStartState();
    }
}
interface ISelfGuidable
{
    void Initialize(float _characterRotateSpeed, float _characterMoveSpeed, Player _player, IShoter _shoter, float angleAllowedToShot);
    void SearchNear(GameObject characterWhichAiming, float distanceToDetect);
    void AimingToTheTarget(GameObject target, GameObject characterWhichAiming);
    void MoveToTheTarget(GameObject characterWhichAiming, Vector3 directionToMove);
}

internal class SelfGuide : ISelfGuidable
{
    private float characterRotateSpeed;
    private float characterMoveSpeed;
    private Quaternion aimingQuaternion { get; set; }
    private const int Z_AXIS_CORRECTION = 90;
    private Player player;
    private IShoter shoter;
    private float angleAllowedToShot;

    public void Initialize(float _characterRotateSpeed, float _characterMoveSpeed, Player _player, IShoter _shoter, float _angleAllowedToShot)
    {
        characterMoveSpeed = _characterMoveSpeed;
        characterRotateSpeed = _characterRotateSpeed;
        angleAllowedToShot = _angleAllowedToShot;
        if (characterRotateSpeed < 0) _characterRotateSpeed = 0;
        player = _player;
        shoter = _shoter;
    }

    public void SearchNear(GameObject characterWhichAiming, float distanceToDetect)
    {
        if (player == null) return;
        if (Vector3.Distance(player.gameObject.transform.position, characterWhichAiming.transform.position) < distanceToDetect)
        {
            AimingToTheTarget(player.gameObject, characterWhichAiming);
        }
    }

    public void AimingToTheTarget(GameObject target, GameObject characterWhichAiming)
    {
        float angle = Mathf.Atan2(target.transform.position.y, target.transform.position.x) * Mathf.Rad2Deg;
        Vector3 directionToMove = new Vector3(0, 0, angle + Z_AXIS_CORRECTION); 
        aimingQuaternion = Quaternion.Euler(directionToMove);

        characterWhichAiming.transform.rotation = Quaternion.Slerp(characterWhichAiming.transform.rotation, aimingQuaternion,
            characterRotateSpeed * Time.fixedDeltaTime);
        MoveToTheTarget(characterWhichAiming, target.transform.position - characterWhichAiming.transform.position);

        if (angleAllowedToShot > Quaternion.Angle(characterWhichAiming.transform.rotation, aimingQuaternion))
        {
            shoter.TryShot();
        }
    }

    public void MoveToTheTarget(GameObject characterWhichAiming, Vector3 directionToMove)
    {
        characterWhichAiming.transform.position += directionToMove * characterMoveSpeed * Time.fixedDeltaTime;
    }
}
