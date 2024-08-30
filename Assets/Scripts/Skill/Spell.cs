using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public static Spell Instance;

    public GameObject Scream;

    public GameObject Roar;

    public GameObject Poison;

    public Transform FakeShroomPosition;
    public GameObject FakeShroom;

    public GameObject MyController;

    /*------------------------------*/

    public bool IntoGame = false;

    public bool SurOrEsc = false; // True for Survive, False for Escape

    public bool DemonOrShroom = false;  // True for Demon, False for Shroom

    /*------------------------------*/

    public int ScreamCount = 1;

    public int RoarCount = 1;

    public int PoisonCount = 1;

    public int FakeShroomCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(IntoGame)
        {
            if(DemonOrShroom)
            {
                if( OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) && ScreamCount > 0)
                {
                    ScreamCount--;
                    if(SurOrEsc) 
                    {
                        MyController.GetComponent<SinglePlayerControl>().ScreamFlag = true;
                        GameManager.Instance.UpdateSkill(0);
                    }
                    else 
                    {
                        MyController.GetComponent<EscapePlayerControl>().ScreamFlag = true;
                        EscapeGameManager.Instance.UpdateSkill(0);
                    }
                    VoiceControl.Instance.PlayVoice(9);

                    Debug.Log("Screamming");
                    Scream.SetActive(true);
                    
                }

                if( OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) && !SurOrEsc && RoarCount > 0)
                {
                    RoarCount--;
                    EscapeGameManager.Instance.UpdateSkill(1);
                    MyController.GetComponent<EscapePlayerControl>().DestroyFlag = true;
                    
                    VoiceControl.Instance.PlayVoice(10);
                    Debug.Log("Roaring");
                    Roar.SetActive(true);
                    
                }
            }
            else
            {
                if( OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) && PoisonCount > 0)
                {
                    PoisonCount--;
                    if(SurOrEsc) 
                    {
                        MyController.GetComponent<SinglePlayerControl>().PoisonFlag = true;
                        GameManager.Instance.UpdateSkill(2);
                    }
                    else 
                    {
                        MyController.GetComponent<EscapePlayerControl>().PoisonFlag = true;
                        EscapeGameManager.Instance.UpdateSkill(2);
                    }
                    VoiceControl.Instance.PlayVoice(11);
                    Debug.Log("Poison!");
                    Poison.SetActive(true);
                    
                }

                if( OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) && !SurOrEsc && FakeShroomCount > 0) // && !SurOrEsc
                {
                    FakeShroomCount--;
                    MyController.GetComponent<EscapePlayerControl>().FakeShroomFlag = true;
                    EscapeGameManager.Instance.UpdateSkill(3);

                    VoiceControl.Instance.PlayVoice(6);
                    Debug.Log("Fake Shroom!");
                    
                }
            }
            if( OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) Debug.Log("SIT");
        }
    }
}
