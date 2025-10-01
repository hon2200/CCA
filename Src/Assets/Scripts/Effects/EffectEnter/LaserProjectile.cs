//using UnityEngine;
//using System.Collections;

//public class LaserProjectile : MonoBehaviour, IPathvfx
//{
//    public ParticleSystem PSystem;

//    public void show(Vector3 startPos, Vector3 endPos, float duration)
//    {
//        float distance = Vector3.Distance(startPos, endPos);
//        ParticleSystem.MainModule mainModule = PSystem.main;
//        mainModule.startLifetime = duration; 
//        mainModule.startSpeed = distance / duration;
//        mainModule.gravityModifier = 0f;
//        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

//        transform.position = startPos;
//        transform.rotation = Quaternion.LookRotation(endPos - startPos);

//        PSystem.Play();
//        StartCoroutine(TravelAndExplode(duration));
//    }

//    private IEnumerator TravelAndExplode(float duration)
//    {
//        yield return new WaitForSeconds(duration);
//        Destroy(gameObject);
//    }
//}