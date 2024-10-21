using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class State : MonoBehaviour
{
    public Vector3 newDestination;
   
    public abstract State RunCurrentState();
  
    public virtual Vector3 GetNextDestination(GameObject creature, float minX, float maxX, float minZ, float maxZ)
    {
        newDestination = new Vector3(creature.transform.position.x + Random.Range(minX,maxX), creature.transform.position.y, creature.transform.position.z + Random.Range(minZ,maxZ));
        return newDestination;
        //get a new destination within a certain range of a certain point
    }

    public virtual Vector3 GetRunAwayPos(GameObject myPos, GameObject predatorPos)
    {
        Vector3 dirToPlayer = myPos.transform.position - predatorPos.transform.position;
        Vector3 newPos = myPos.transform.position + dirToPlayer;

        return newPos;

        //when prey us running, calculate a point away from Predator
    }
}
