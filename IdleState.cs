using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class IdleState : State
{
    public StateManager stateManager;
    public GameObject centrePoint;
    public bool idle = false, newLocation = true, isPredator;
    
    void Start()
    {
        isPredator = stateManager.isPredator;
        centrePoint = GameObject.FindGameObjectWithTag("CentrePoint");
    }
    
    public override State RunCurrentState()
    {
        if(!idle)
        {
            if(newLocation == true)
            { 
                //get location if it doesnt have one
                newLocation = false;
                if(isPredator)
                {
                    stateManager.myDestination = GetNextDestination(centrePoint, -15f, 15f, -15f, 15f);
                    //create location around camp
                }
                else
                {
                    stateManager.myDestination = GetNextDestination(stateManager.myParent, -15f, 15f, -15f, 15f);
                    //create location around itself
                }
            }

            if(Vector3.Distance(stateManager.agent.destination, stateManager.myParent.transform.position) < 2)
            {
                //distance check to see if agent has reached its idle location
                idle = true;
                Invoke("Wait", Random.Range(1f,5f));
                //wait at location for some time.
            }

        }

        return this;

    }

    public void Wait()
    {
        //wait is complete
        newLocation = true;
        idle = false;
    }

}
