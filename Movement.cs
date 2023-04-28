using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    private InputDevice rightConInput;
    private InputDevice leftConInput;
    public GameObject rightCon;
    public GameObject leftCon;
    public GameObject XROrigin;
    Vector3 direction=new Vector3();
    [SerializeField] private float speed = 5f;
    public LineRenderer lineRenderer;
    public float laserWidth = .1f;

    [SerializeField]private bool started=false;

    public float timeBetweenShots=1;
    private float rShotTimer;
    private float lShotTimer;

    private bool checkDevices=true;

    public Rigidbody rb;

    public GameObject projectile;

    void Start()
    {
        rb= GetComponent<Rigidbody>();
        rShotTimer = timeBetweenShots;
        lShotTimer = timeBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        if(checkDevices)
        {
            List<InputDevice> inputDevices = new List<InputDevice>();
            InputDeviceCharacteristics foundControllers = InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(foundControllers, inputDevices);
            if (inputDevices.Count > 1)
            {
                rightConInput = inputDevices[1];
                leftConInput = inputDevices[0];
                checkDevices=false;
            }
        }

        // Movement
        if (started)
        {
            rightConInput.TryGetFeatureValue(CommonUsages.trigger, out float rSpeed);
            leftConInput.TryGetFeatureValue(CommonUsages.trigger, out float lSpeed);
            direction = (rightCon.transform.forward * rSpeed + leftCon.transform.forward * lSpeed) * speed;
            ShootLaserFromTargetPosition(transform.position, direction, direction.magnitude);
            lineRenderer.enabled = true;
            rb.velocity = direction;

            //Shoot
            if (rShotTimer > 0)
                rShotTimer -= Time.deltaTime;
            if (lShotTimer > 0)
                lShotTimer -= Time.deltaTime;
            rightConInput.TryGetFeatureValue(CommonUsages.primaryButton, out bool rShoot);
            leftConInput.TryGetFeatureValue(CommonUsages.primaryButton, out bool lShoot);
            if (rShoot && rShotTimer <= 0)
            {
                Instantiate(projectile, rightCon.transform.position, rightCon.transform.rotation);
                rShotTimer = timeBetweenShots;
            }
            if (lShoot && lShotTimer <= 0)
            {
                Instantiate(projectile, leftCon.transform.position, leftCon.transform.rotation);
                lShotTimer = timeBetweenShots;
            }
        }

        

    }


    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            endPosition = raycastHit.point;
        }

        lineRenderer.SetPosition(0, targetPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    public void startGame()
    {
        started=true;
    }
}
