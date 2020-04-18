using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class BulletCatapult : MonoBehaviour
    {
        public GameObject bullet;
        public Vector3 direction;
        public float speed;

        public Vector3 rotation;
        public float rotationSpeed;

        public void Shoot()
        {
            GameObject ins = Instantiate(bullet, this.transform.position, transform.rotation);

            ins.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(direction.normalized) * speed, ForceMode.VelocityChange);
            ins.GetComponent<Rigidbody>().AddTorque(transform.TransformDirection(rotation.normalized) * rotationSpeed, ForceMode.Impulse);
        }
    }
}