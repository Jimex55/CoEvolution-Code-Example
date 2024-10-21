using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

public class ReproduceMechanic : MonoBehaviour
{

    public string path = Application.dataPath + "/Log.txt";

    public float i;

    public GameObject predator, prey;
    
    public void CreateChildren(GameObject maleParent, GameObject femaleParent, bool isPredator)
    {
        StateManager maleStateManager = maleParent.GetComponentInChildren<StateManager>();
        StateManager femaleStateManager = femaleParent.GetComponentInChildren<StateManager>();

        if(isPredator)
        {
            i = UnityEngine.Random.Range(1, 3);
            
        }
        else
        {
            i = UnityEngine.Random.Range(1,3);
        }
        //Chooses how many children, Done this way so you can change amounts of Predators // Prey

        for (int j = 0; j < i;)
        {
            
            float childMaxHunger = GetGeneticCombo(maleStateManager.myGene.maxHunger,femaleStateManager.myGene.maxHunger);
            float childSpeed = GetGeneticCombo(maleStateManager.myGene.speed,femaleStateManager.myGene.speed);
            float childEyeSight = GetGeneticCombo(maleStateManager.myGene.eyeSight,femaleStateManager.myGene.eyeSight);
            float childReactionTime = GetGeneticCombo(maleStateManager.myGene.reactionTime,femaleStateManager.myGene.reactionTime);
            float childReproduceThreshHold = GetGeneticCombo(maleStateManager.myGene.ReproduceThreshHold,femaleStateManager.myGene.ReproduceThreshHold);
            float childDeathAge = GetGeneticCombo(maleStateManager.myGene.deathAge,femaleStateManager.myGene.deathAge);
            float childGenerationNum = GetGenerationNumber(maleStateManager.myGene.generationNum,femaleStateManager.myGene.generationNum);

            //Gets new characteristics for child
            
            if(!File.Exists(path))
            {
                File.WriteAllText(path, "Gene Developments \n \n");
            }

            //creates text document to store gene data

            if(isPredator)
            {
                File.AppendAllText(path, "Predator \n");
            }
            else
            {
                File.AppendAllText(path, "Prey \n");
            }

            string content = GetLogEntry(childGenerationNum.ToString(), childMaxHunger.ToString(), childSpeed.ToString(), childEyeSight.ToString(), childReactionTime.ToString(), childReproduceThreshHold.ToString(), childDeathAge.ToString());

            File.AppendAllText(path, content);

            //creates and adds a log entry of new entity

            if(isPredator)
            {
                
                GameObject newPredator = Instantiate(predator, femaleParent.transform.position, transform.rotation);
                GiveGenes(newPredator, childMaxHunger, childSpeed, childEyeSight, childReactionTime, childReproduceThreshHold, childDeathAge, childGenerationNum);
            }
            else
            {
                GameObject newPrey = Instantiate(prey, femaleParent.transform.position, transform.rotation);
                GiveGenes(newPrey, childMaxHunger, childSpeed, childEyeSight, childReactionTime, childReproduceThreshHold, childDeathAge, childGenerationNum);
            }

            // instantiates child with new genes

            j += 1;
        }

        File.AppendAllText(path, "Population \n" + "Prey Population: " + StaticVars.preyPop + "\n" + "Predator Population: " + StaticVars.predPop + "\n \n \n");


        
    }

    void GiveGenes(GameObject newChild, float maxHunger, float speed, float eyeSight, float reactionTime, float reproduceThreshHold, float deathAge, float generationNum)
    {
        StateManager newChildStateManager = newChild.GetComponentInChildren<StateManager>();
        newChildStateManager.myGene.maxHunger = maxHunger;
        newChildStateManager.myGene.speed = speed;
        newChildStateManager.myGene.eyeSight = eyeSight;
        newChildStateManager.myGene.reactionTime = reactionTime;
        newChildStateManager.myGene.ReproduceThreshHold = reproduceThreshHold;
        newChildStateManager.myGene.deathAge = deathAge;
        newChildStateManager.myGene.generationNum = generationNum;

        //gives child genes

        int i = UnityEngine.Random.Range(0, 2);
        if(i == 0)
        {
            newChildStateManager.isMale = true;
        }
        else
        {
            newChildStateManager.isMale = false;
        }
        //decides if child is male or female

    }

    string GetLogEntry(string generation, string maxHunger, string speed, string eyeSight, string reactionTime, string reproduceThreshhold, string deathAge)
    {
        string newLogEntry = "Generation " + generation + "\n" + "Max Hunger " + maxHunger + "\n" + "Speed " + speed + "\n" + "Eye Sight " + eyeSight + "\n" + "Reaction Time " + reactionTime + "\n" + "Reproduction ThreshHold " + reproduceThreshhold + "\n" + "Death Age " + deathAge + "\n \n \n";

        return newLogEntry;

        //easily puts text together
    }

    public float GetGenerationNumber(float maleStat, float femaleStat)
    {
        if(maleStat >= femaleStat)
        {
            return maleStat + 1f;
        }
        else
        {
            return femaleStat + 1f;
        }

        //increases generation number
    }

    public float GetGeneticCombo(float maleStat, float femaleStat)
    {
        int k = UnityEngine.Random.Range(0,2);
        float statToUse;
        if(k == 0) //male dominate
        {
            statToUse = maleStat;
        }
        else // female dominate
        {
            statToUse = femaleStat;
        }

        //chooses wether the male or female state is dominant

        k = UnityEngine.Random.Range(0,3);

        if(k == 0) //mutation
        {
            statToUse = statToUse * UnityEngine.Random.Range(0.8f,1.2f);
        }
        else
        {

        }

        // gives a 33% chance of a gene mutation - gene mutation +/- 20% randomly

        return statToUse;
    }
}
