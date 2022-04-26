using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Shoter))]
public class Player : MonoBehaviour
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

    private void Awake()
    {
        mover = new Mover();
        resetor = new Resertor();
        mover.Initialize(GetComponent<Rigidbody>(), characterMoveSpeed, characterRotateSpeed);

        shoter = gameObject.GetComponent<Shoter>();
        shoter.Initialize(Bullet, BulletSpawner, GetComponent<Rigidbody>(), bulletSpeed, reloadingTime, recoilBack);
    }

    private void OnCollisionEnter(Collision collision)
    {
        resetor.CheckReset(collision, scoreScripts, mover, true);
    }

    private void FixedUpdate()
    {
        float _positionChange = -Input.GetAxis("Vertical") * Time.fixedDeltaTime * characterMoveSpeed;
        float _rotationChange = -Input.GetAxis("Horizontal") * Time.fixedDeltaTime * characterRotateSpeed;

        mover.Move(_positionChange, _rotationChange);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
        {
            shoter.TryShot();
        }
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

interface IMover
{
    void Initialize(Rigidbody player, float _characterMoveSpeed, float _characterRotateSpeed);
    void Move(float _positionChange, float _rotationChange);
    void ReturnToStartState();
}

interface IShoter
{
    void Initialize(BulletScript Bullet, Transform BulletSpawner, Rigidbody recoilObject, float BulletSpeed, float reloadingTime, float recoilBack);
    void TryShot();
    void Recoil();
}

interface IResetable
{
    void CheckReset(Collision collision, ScoreScripts scoreScripts, IMover mover, bool isPlayerKilled);
    void DestroyAllBullets();
}

internal class Resertor: MonoBehaviour, IResetable
{
    public void CheckReset(Collision collision, ScoreScripts scoreScripts, IMover mover, bool isPlayerKilled)
    {
        if (collision.gameObject.GetComponent<BulletScript>() != null) scoreScripts.AddValue(isPlayerKilled);
        else return;
        DestroyAllBullets();
        var enemy = FindObjectOfType<Enemy>();
        enemy.ReturnToStartState();
        var player = FindObjectOfType<Player>();
        player.ReturnToStartState();
    }

    public void DestroyAllBullets()
    {
        var bullets = FindObjectsOfType<BulletScript>();
        foreach (BulletScript bullet in bullets) Destroy(bullet.gameObject);
    }

}

internal class Mover: MonoBehaviour, IMover
{
    private float CharacterMoveSpeed;
    private float CharacterRotateSpeed;
    private Rigidbody CharacterToMove;

    private const float CHARACTER_MOVE_SPEED_DEFAULT = 1f;
    private const float CHARACTER_ROTATE_SPEED_DEFAULT = 1f;

    private Vector3 startPosition;

    public void Initialize(Rigidbody _playerToMove, float _characterMoveSpeed, float _characterRotateSpeed)
    {
        CharacterToMove = _playerToMove;
        CharacterMoveSpeed = _characterMoveSpeed;
        CharacterRotateSpeed = _characterRotateSpeed;

        if (CharacterMoveSpeed <= 0) CharacterMoveSpeed = CHARACTER_MOVE_SPEED_DEFAULT;
        if (CharacterRotateSpeed <= 0) CharacterRotateSpeed = CHARACTER_ROTATE_SPEED_DEFAULT;
        startPosition = CharacterToMove.gameObject.transform.position;
    }

    public void Move(float _positionChange, float _rotationChange)
    {
        CharacterToMove.gameObject.transform.Rotate(0, 0, _rotationChange);
        CharacterToMove.gameObject.transform.position += CharacterToMove.gameObject.transform.up * _positionChange;
    }

    public void ReturnToStartState()
    {
        CharacterToMove.gameObject.transform.position = startPosition;
        CharacterToMove.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}