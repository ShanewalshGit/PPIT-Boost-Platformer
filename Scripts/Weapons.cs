using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapons : MonoBehaviour
{
    [SerializeField] private InputActionReference shoot;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform BulletParent;
    [SerializeField] private float bulletSpeed = 2.0f;
    [SerializeField] public Transform orient;


    void OnEnable() {
        shoot.action.performed += Shoot;
    }

    void OnDisable() {
        shoot.action.performed -= Shoot;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        // Create a new bullet
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        // Set the bullet's parent to the BulletParent
        bullet.transform.SetParent(BulletParent);
        //Position the bullet in desired orientation
        bullet.transform.position = orient.position;
        // Give it velocity in the direction of the orientation, no up or down
        bullet.GetComponent<Rigidbody>().velocity = orient.forward * bulletSpeed;

        // Shoot sfx
        FindObjectOfType<AudioManager>().PlayPlayerAttack1();
    }
}
