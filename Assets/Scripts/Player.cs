using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Vector3 _startCenterPos;

    [SerializeField]
    private float _rotateRadius, _rotateSpeed,_moveSpeed;

    private Vector3 centerPos;

    private bool canRotate;
    private bool canMove;
    private bool canShoot;

    private float currentRotateAngle;
    private Vector3 moveDirection;

    [SerializeField]
    private GameObject _whiteExplosionPrefab,_yellowExplosionPrefab;

    private ParticleSystem.EmissionModule _trailParticle;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _loseClip;

    private void Awake()
    {
        canRotate = false;
        canMove = false;
        canShoot = false;
        centerPos = _startCenterPos;
        currentRotateAngle = 90f;
        _trailParticle = GetComponent<ParticleSystem>().emission;
        _trailParticle.enabled = false;
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
    }

    private void GameStarted()
    {
        canShoot = true;
        canRotate = true;
    }

    private void Update()
    {
        if(canShoot && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if(canRotate)
        {
            currentRotateAngle += _rotateSpeed * Time.fixedDeltaTime;
            moveDirection = new Vector3(Mathf.Cos(currentRotateAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentRotateAngle * Mathf.Deg2Rad), 0f).normalized;
            transform.position = centerPos + _rotateRadius * moveDirection;
            if (currentRotateAngle >= 360) currentRotateAngle = 0f;
        }
        else if (canMove)
        {
            transform.position += _moveSpeed * Time.fixedDeltaTime * moveDirection;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            EndGame();
        }
        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            if(collision.gameObject.GetComponent<Score>().currentId != GameManager.Instance.currentTargetIndex)
            {
                EndGame();
                return;
            }
            // Update The Score
            // Reverse the moveDirection
            AudioManager.Instance.PlaySound(_pointClip);
            Destroy(Instantiate(_yellowExplosionPrefab, transform.position, Quaternion.identity), 3.2f);
            _moveSpeed *= -1f;
            GameManager.Instance.UpdateScore(collision.gameObject.GetComponent<Score>().currentId);
        }

        if(collision.CompareTag(Constants.Tags.CENTER))
        {
            centerPos = collision.gameObject.transform.position;
            _trailParticle.enabled = false;
            canMove = false;
            canShoot = true;
            canRotate = true;
            Vector3 direction = (transform.position - centerPos).normalized;
            float cosAngle = Mathf.Acos(direction.x) * Mathf.Rad2Deg;
            float sinAngle = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
            currentRotateAngle = cosAngle * (sinAngle < 0 ? -1f : 1f);
            _moveSpeed *= -1f;
            _rotateSpeed *= -1f;
        }
    }

    private void EndGame()
    {
        AudioManager.Instance.PlaySound(_loseClip);
        Destroy(Instantiate(_whiteExplosionPrefab, transform.position, Quaternion.identity), 5f);
        GameManager.Instance.EndGame();
        Destroy(gameObject);
    }

    private void Shoot()
    {
        _trailParticle.enabled = true;
        AudioManager.Instance.PlaySound(_moveClip);
        canRotate = false;
        canShoot = false;
        canMove = true;
        moveDirection = (transform.position - centerPos).normalized;
    }
}