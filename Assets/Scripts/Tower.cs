using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _towerPlace;
    [SerializeField] private SpriteRenderer _towerHead;

    [SerializeField] private int _shootPower = 1;
    [SerializeField] private float _shootDistance = 1f;
    [SerializeField] private float _shootDelay = 5f;
    [SerializeField] private float _bulletSpeed = 1f;
    [SerializeField] private float _bulletSplashRaduis = 0f;

    [SerializeField] private Bullet _bulletPrefab;

    private float _runningShootDelay;
    private Enemy _targetEnemy;
    private Quaternion _targetRotation;

    public void CheckNearestEnemy(List<Enemy> enemies)
    {
        if(_targetEnemy != null)
        {
            if(!_targetEnemy.gameObject.activeSelf || Vector3.Distance(transform.position, _targetEnemy.transform.position) > _shootDistance)
            {
                _targetEnemy = null;
            }
            else
            {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        //Target neareest enemy inside 
        foreach(Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance > _shootDistance)
            {
                continue;
            }

            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        _targetEnemy = nearestEnemy;
    }

    public void ShootTarget()
    {
        if(_targetEnemy == null)
        {
            return;
        }

        _runningShootDelay -= Time.unscaledDeltaTime;
        if(_runningShootDelay <= 0f)
        {
            bool headHasAimed = Mathf.Abs(_towerHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) < 10f;
            if(!headHasAimed)
            {
                return;
            }

            Bullet bullet = LevelManager.Instance.GetBulletFromPool(_bulletPrefab);
            /*print(bullet.name);*/
            print(_targetEnemy);
            bullet.transform.position = transform.position;
            bullet.SetProperty(_shootPower, _bulletSpeed, _bulletSplashRaduis);
            bullet.SetTargetEnemy(_targetEnemy);
            bullet.gameObject.SetActive(true);

            _runningShootDelay = _shootDelay;
        }
    }

    //Tower face enemy
    public void SeekTarget()
    {
        if(_targetEnemy == null)
        {
            return;
        }

        Vector3 direction = _targetEnemy.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));

        _towerHead.transform.rotation = Quaternion.RotateTowards(_towerHead.transform.rotation, _targetRotation, Time.deltaTime * 180f);
    }

    //Tower position when it got dragged
    public Vector2? PlacePosition { get; private set; }

    //Get Tower Head sprite
    public Sprite GetTowerHeadIcon()
    {
        return _towerHead.sprite;
    }

    public void SetPlacePosition(Vector2? newPosition)
    {
        PlacePosition = newPosition;
    }

    public void LockPlacement()
    {
        transform.position = (Vector2)PlacePosition;
    }

    //Change order in layer for dragged tower
    public void ToggleOrderInLayer(bool toFront)
    {
        int orderInLayer = toFront ? 2 : 0;
        _towerPlace.sortingOrder = orderInLayer;
        _towerHead.sortingOrder = orderInLayer;
    }
}
