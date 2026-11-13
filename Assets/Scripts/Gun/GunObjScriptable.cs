using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "gun", menuName = "Guns/Gun", order = 0)]
public class GunObjScriptable : ScriptableObject
{
    public GunType type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 GunSpawn;
    public Vector3 GunSpawnRotation;

    public ShootObjConScript ShootCon;
    public TrailObjConScript TrailCon;

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    float LastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;

    public float damage = 10f; 
    public float range = 100f;
    

    // change this method ( A
    public void spawn (Transform parent, MonoBehaviour ActiveMonoBehaviour)
    {
        GameObject model = Instantiate(ModelPrefab); // Instantiate the model prefab
        model.transform.SetParent(parent, false); // Set the parent of the model
        model.transform.localPosition = GunSpawn; // Set the local position of the model
        model.transform.localRotation = Quaternion.Euler(GunSpawnRotation); // Set the local rotation of the model
        Setup(model, ActiveMonoBehaviour);

        // old method
        //this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        //LastShootTime = 0;
        //TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        //Model = Instantiate(ModelPrefab);
        //Model.transform.SetParent(parent, false);
        //Model.transform.localPosition = GunSpawn;
        //Model.transform.localRotation = Quaternion.Euler(GunSpawnRotation);
        //ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
    }

    public void Setup(GameObject model, MonoBehaviour ActiveMonoBehaviour)  // New method to set up the gun with the model and active MonoBehaviour
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        LastShootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        Model = model;
        ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
    }

    // change this method ( A
    
    public bool shoot()
    {
        if (Time.time > ShootCon.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;
            ShootSystem.Play();
           
            Vector3 shootDirection = ShootSystem.transform.forward
                + new Vector3(
                    Random.Range(-ShootCon.Spread.x, ShootCon.Spread.x),
                    Random.Range(-ShootCon.Spread.y, ShootCon.Spread.y),
                    Random.Range(-ShootCon.Spread.z, ShootCon.Spread.z)
                    );
           
            shootDirection.Normalize();
            if (Physics.Raycast
                (
                ShootSystem.transform.position,
                shootDirection,
                out RaycastHit hit,
                float.MaxValue,
                ShootCon.HitMask
                ))
            {
                ActiveMonoBehaviour.StartCoroutine
                (
                    PlayTrail
                    (
                    ShootSystem.transform.position,
                    hit.point,
                    hit
                    )

                );
                
                Debug.Log(hit.transform.name);

                Target enemy = hit.transform.GetComponent<Target>();
                if (enemy != null)
                {
                    enemy.takeDamage(damage);
                }
            }

            else
            {
                ActiveMonoBehaviour.StartCoroutine(PlayTrail
                (
                     ShootSystem.transform.position,
                    ShootSystem.transform.position + (shootDirection * TrailCon.MissDistance),
                     new RaycastHit()
                ));
            }
            return true; // successful shot
        }
        return false; // not fired, waiting on fire rate
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null;
        
        instance.emitting = true;

        float Distance = Vector3.Distance(StartPoint, EndPoint );
        float remainingDistance = Distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1- (remainingDistance/ Distance))
                );
            remainingDistance -= TrailCon.SimSpeed * Time.deltaTime;
            yield return null;
        }
        instance.transform.position = EndPoint;

        yield return new WaitForSeconds(TrailCon.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);

    }
    
    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailCon.Color;
        trail.material = TrailCon.Material;
        trail.widthCurve = TrailCon.WidthCurve;
        trail.time = TrailCon.Duration;
        trail.minVertexDistance = TrailCon.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}
