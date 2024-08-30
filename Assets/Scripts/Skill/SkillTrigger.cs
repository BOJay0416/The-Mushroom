using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTrigger : MonoBehaviour
{
    public static SkillTrigger Instance;

    public bool SurOrEsc = false; // True for Escape / False for Survive

    public bool Shock = false;

    public bool Blind = false;

    public GameObject MyController;

    public bool DorM = false; // True for Demon / False for Shroom

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        DorM = Spell.Instance.DemonOrShroom;
        if(Shock && !DorM)
        {
            Debug.Log("Oh no! Scream");

            if(!SurOrEsc) MyController.GetComponent<SinglePlayerControl>().IsShock = true;
            else MyController.GetComponent<EscapePlayerControl>().IsShock = true;
            Shock = false;
        }

        if(Blind && DorM)
        {
            Debug.Log("Oh no! Poison");

            if(!SurOrEsc) MyController.GetComponent<SinglePlayerControl>().IsBlind = true;
            else MyController.GetComponent<EscapePlayerControl>().IsBlind = true;
            Blind = false;
        }
    }
}
