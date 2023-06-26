using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]GameObject hitVFX;
    [SerializeField] private AudioData[] hitSFX;
    [SerializeField]float damage;
    [SerializeField]protected float moveSpeed = 10f;
    [SerializeField]protected Vector2 moveDirection;

    protected GameObject target;

    protected virtual void OnEnable() 
    {
        StartCoroutine(nameof(MoveDirectly));    
    }

    IEnumerator MoveDirectly()
    {
        while(gameObject.activeSelf)
        {
            Move();

            yield return null;
        }
    }

    public void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(damage);

            //var contactPoint = other.GetContact(0);
            //PoolManager.Release(hitVFX,contactPoint.point,Quaternion.LookRotation(contactPoint.normal));
            PoolManager.Release(hitVFX,other.GetContact(0).point,Quaternion.LookRotation(other.GetContact(0).normal));
            AudioManager.Instance.PlayRandomSFX(hitSFX);
            gameObject.SetActive(false);
        }
    }

    protected void SetTarget(GameObject _target)
    {
        this.target = _target;
    }
}
