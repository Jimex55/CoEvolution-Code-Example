using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Timeline.Actions;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;




[Serializable] 
public class MyGene
{
    public float maxHunger, speed, eyeSight, reactionTime, ReproduceThreshHold, generationNum, deathAge, foodWorth;
    
}

//class to hold the genes for the agents

public class StateManager : MonoBehaviour
{
    public MyGene myGene;
    public State currentState;
    public State eatState, idleState, returnState, huntState, reproduceState, runState;
    public Vector3 currentDestination;
    public float myHungerLevels, age, fertility, foodHold;
    
    public GameObject foodStore, myParent, incomingPredator;
    public NavMeshAgent agent;
    public bool isPredator, runFromPredator, isMale, canMate, starving, isEating;

    public Vector3 myDestination;
    void Update()
    {
        RunStateMachine();

    }

    void Start()
    {
        InvokeRepeating("HungerAgeReduce", 1f, 2f); //age // create hunger over time
        myParent = transform.parent.gameObject;
        agent = myParent.GetComponent<NavMeshAgent>();
        Invoke("GetGenes", 0.1f); // makes genes take effect
        foodStore = GameObject.FindGameObjectWithTag("FoodStore");
        InvokeRepeating("SetAgentDestination", 0f, 1f); //automatic movement update every second
        
    }

    void SetAgentDestination()
    {
        agent.SetDestination(myDestination);
    }

    void GetGenes()
    {

        if(myGene.generationNum == 0)
        {
            myGene.maxHunger = UnityEngine.Random.Range(20f,30f);

            if(isPredator)
            {
                myGene.speed = UnityEngine.Random.Range(3f,4.5f);
            }
            else
            {
                myGene.speed = UnityEngine.Random.Range(2.5f,4f);
                myGene.foodWorth = UnityEngine.Random.Range(40,60);   
            }
            
            myGene.eyeSight = UnityEngine.Random.Range(15f,20f);
            myGene.reactionTime = UnityEngine.Random.Range(0.5f,2f);
            myGene.ReproduceThreshHold = UnityEngine.Random.Range(20f,30f);
            myGene.deathAge = UnityEngine.Random.Range(40f,60f);
        }

        //if generation num is 0, create new genes for the first generation randomly

        agent.speed = myGene.speed;

        if(isPredator)
        {
            huntState.GetComponent<CapsuleCollider>().radius = myGene.eyeSight; 
            StaticVars.predPop += 1; 
        }
        else
        {
            runState.GetComponent<CapsuleCollider>().radius = myGene.eyeSight;
            idleState.GetComponent<CapsuleCollider>().radius = myGene.eyeSight;
            StaticVars.preyPop += 1; 
        }

        //apply all the relative genes to the right place

    }

    private void RunStateMachine()
    {
        StateCheck(); //checks which state should be next

        State nextState = currentState?.RunCurrentState(); //runs the next state
        


        if(nextState != null)
        {
            SwitchToTheNextState(nextState);
        }
    }

    private void SwitchToTheNextState(State nextState)
    {
        currentState = nextState;
    }

    public void StateCheck ()
    {
        if(isPredator)
        {
            PredatorStateCheck(); //state checks specific to Predators
        }
        else
        {
            PreyStateCheck(); //Statechecks specific to Prey
        }

        if(currentState == idleState && canMate && isMale)
        {
            SwitchToTheNextState(reproduceState);
            //reporduction statecheck for both
        }

        if(currentState == reproduceState && canMate == false && fertility < 50)
        {
            SwitchToTheNextState(idleState);
            //check to return to idle after reproducing
        }
    }

    public void PreyStateCheck()
    {
        if(runFromPredator)
        {
            SwitchToTheNextState(runState);
        }
        else
        {
            SwitchToTheNextState(idleState);
        }
    
    }

    public void PredatorStateCheck()
    {
        
        if (currentState == eatState && myHungerLevels >= myGene.maxHunger)
        {
            eatState.GetComponent<EatState>().Reset();
            SwitchToTheNextState(idleState);
            //return to idle if fully eaten
        }

        if(HuntOrEatCheck()) //check to see if predator should hunt
        {
            eatState.GetComponent<EatState>().Reset();
            SwitchToTheNextState(huntState);
        }

        if(currentState == huntState && foodHold != 0)
        {
            SwitchToTheNextState(returnState);
            //if hunting and has food (they have successfully hunted) return to deposit food
        }

        if(currentState == returnState && foodHold == 0)
        {
            SwitchToTheNextState(idleState);
            //if they have deposited food, return to idle
        }

        
    }

    public void HungerAgeReduce()
    {

        if(age == myGene.deathAge || myHungerLevels <= 0)
        {

            if(isPredator)
            {
                StaticVars.predPop -= 1;
            }
            else
            {
                StaticVars.preyPop -= 1;
            }
            Destroy(myParent);
        }
        age += 0.5f;
        // age the agent and check to see if they have reached max age

        if(age >= 5f && fertility <= myGene.ReproduceThreshHold)
        {
            fertility += 0.5f;
            //raise fertility
        }

        if(fertility >= myGene.ReproduceThreshHold & !canMate)
        {
            canMate = true;
        }

        if(isPredator)
        {
            if(!isEating)
            {
                myHungerLevels -=0.5f;
            }
            //only reduce hunger if not eating

            if(myHungerLevels <= (myGene.maxHunger * 0.25))
            {
                if(StaticVars.totalFoodStorage >= 0)
                {
                    starving = true;
                    SwitchToTheNextState(eatState);
                }
                else
                {
                    starving = false;
                }
            }
            //if hungry go to eat state
        }
        
    }

    public bool HuntOrEatCheck()
    {
        if(currentState != huntState && currentState != returnState && !starving)
        {
            if(StaticVars.totalFoodStorage <= 0 && currentState != returnState)
            {
                //if no food in storage, go to hunt
                return true;
            }
            
            if(StaticVars.totalFoodStorage <= myGene.maxHunger)
            {
                if(currentState == eatState)
                {
                    //if eating dont hunt
                    return false;
                }else if(currentState == returnState)
                {
                    //if returning dont hunt
                    return false;
                }
                else
                {
                    //hunt
                    return true;
                }
            }
        }


        return false;
        
    }

}
