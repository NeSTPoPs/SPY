using UnityEngine;
using FYFY;
using DIG.GBLXAPI;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SendStatements : FSystem {

    private Family f_actionForLRS = FamilyManager.getFamily(new AllOfComponents(typeof(ActionPerformedForLRS)));

    public static SendStatements instance;

    private int nb_paused;
    private int nb_play;
    private float timer;
    private float mini_timer;

    public int score;
    public int scoreMaxNiveau;
    public int nb_stars;


    public SendStatements()
    {
        instance = this;
    }
	
	protected override void onStart()
    {
		initGBLXAPI();
        nb_paused = 0;
        timer = 0;
        mini_timer = 0;
        start_Timer();
    }

 

    public void initGBLXAPI()
    {
        if (!GBLXAPI.IsInit)
            GBLXAPI.Init(GBL_Interface.lrsAddresses);

        GBLXAPI.debugMode = false;

        string sessionID = Environment.MachineName + "-" + DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss");
        //Generate player name unique to each playing session (computer name + date + hour)
        GBL_Interface.playerName = String.Format("{0:X}", sessionID.GetHashCode());
        GBL_Interface.userUUID = GBL_Interface.playerName;
    }

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        // Do not use callbacks because in case in the same frame actions are removed on a GO and another component is added in another system, family will not trigger again callback because component will not be processed
        foreach (GameObject go in f_actionForLRS)
        {
            ActionPerformedForLRS[] listAP = go.GetComponents<ActionPerformedForLRS>();
            int nb = listAP.Length;
            ActionPerformedForLRS ap;
            if (!this.Pause)
            {
                for (int i = 0; i < nb; i++)
                {
                    ap = listAP[i];
                    //If no result info filled
                    if (!ap.result)
                    {
                        GBL_Interface.SendStatement(ap.verb, ap.objectType, ap.activityExtensions);
                    }
                    else
                    {
                        bool? completed = null, success = null;

                        if (ap.completed > 0)
                            completed = true;
                        else if (ap.completed < 0)
                            completed = false;

                        if (ap.success > 0)
                            success = true;
                        else if (ap.success < 0)
                            success = false;

                        GBL_Interface.SendStatementWithResult(ap.verb, ap.objectType, ap.activityExtensions, ap.resultExtensions, completed, success, ap.response, ap.score, ap.duration);
                    }
                }
            }
            for (int i = nb - 1; i > -1; i--)
            {
                GameObjectManager.removeComponent(listAP[i]);
            }
        }
	}

    private void autre() {
        Debug.Log(GBL_Interface.playerName + " Send Statement sent");
        GameObjectManager.addComponent<ActionPerformedForLRS>(MainLoop.instance.gameObject, new
        {
            verb = "interacted",
            activity = "value"
        });
    }

    public void sendStatement()
    {
        GameObject gameDataGO = GameObject.Find("GameData");
        GameData gameData = gameDataGO.GetComponent<GameData>();
        string[] titre = (gameData.levelToLoad).Split('/');
        string campagne = titre[titre.Length - 2];
        string levelName = (titre[titre.Length - 1]).Split('.')[0];
        Debug.Log(GBL_Interface.playerName + " Send Statement sent");
        Debug.Log("Time : " + timer.ToString());
        string nb_reset = File.ReadAllText("Assets/StreamingAssets/TracesFile/ResetCounter.txt");
        GameObjectManager.addComponent<ActionPerformedForLRS>(MainLoop.instance.gameObject, new
        {
            verb = "interacted",
            objectType = "value",
            //activity = "value",
            activityExtensions = new Dictionary<string, string>() {
                { "campagne" , campagne },
                { "level" , levelName },
                { "temps_editmode" , timer.ToString() },
                /*{ "score" , score.ToString() },
                { "meilleurscore" , scoreMaxNiveau.ToString() },
                { "nb_stars" , nb_stars.ToString() },*/
                { "nb_pause", nb_paused.ToString() },
                { "nb_play", nb_play.ToString() },
                { "nb_reset", nb_reset }
            }
        });
        System.IO.File.WriteAllText("Assets/StreamingAssets/TracesFile/ResetCounter.txt", "0");
    }

    public void sendStatementserie()
    {
        string nb_chain = File.ReadAllText("Assets/StreamingAssets/TracesFile/serieCounter.txt");
        GameObjectManager.addComponent<ActionPerformedForLRS>(MainLoop.instance.gameObject, new
        {
            verb = "interacted",
            objectType = "value",
            activityExtensions = new Dictionary<string, string>() {
                { "nb_chain", nb_chain }
            }
        });
    }


    public void initFiles()
    {
        System.IO.File.WriteAllText("Assets/StreamingAssets/TracesFile/ResetCounter.txt", "0");
        System.IO.File.WriteAllText("Assets/StreamingAssets/TracesFile/serieCounter.txt", "0");
    }


    public void start_Timer()
    {
        Debug.Log("START TIMER");
        mini_timer = Time.time;
    }

    public void pause_Timer()
    {
        Debug.Log("PAUSE TIMER");
        float pause = Time.time;
        timer = timer + (pause - mini_timer);
    }

    public void pushedPause()
    {
        nb_paused += 1;
    }

    public void pushedPlay()
    {
        nb_play += 1;
    }

    public void pushedRestart()
    {
        string compteur = File.ReadAllText("Assets/StreamingAssets/TracesFile/ResetCounter.txt");
        int cmpt = int.Parse(compteur);
        cmpt++;
        System.IO.File.WriteAllText("Assets/StreamingAssets/TracesFile/ResetCounter.txt", cmpt.ToString());
    }

    public void nextLevel()
    {
        string compteur = File.ReadAllText("Assets/StreamingAssets/TracesFile/serieCounter.txt");
        int cmpt = int.Parse(compteur);
        cmpt++;
        System.IO.File.WriteAllText("Assets/StreamingAssets/TracesFile/serieCounter.txt", cmpt.ToString());
    }


    public void registerScore(int score, int scoreMaxNiveau, int nb_stars)
    {
        //int score, int scoreMaxNiveau, int nb_stars
        this.score = score;
        this.scoreMaxNiveau = scoreMaxNiveau;
        this.nb_stars = nb_stars;
    }
}