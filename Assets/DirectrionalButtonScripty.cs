using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class DirectrionalButtonScripty : MonoBehaviour {
	
	public KMAudio Audio;
    public KMColorblindMode CB;
	static int moduleIdCounter = 1;
	int moduleID;
	private int Presses = 0;
	private bool up = false;
	public KMSelectable Bottun;
	public KMSelectable Up;
	public KMSelectable Down;
	public Renderer Buttoncolor;
	public TextMesh BottunText;
    public TextMesh CBText;
	private int a = 0;
	private int b = 0;
	private int CorrectPres;
    private bool Colorblind = false;
	private bool ModuleSolved = false;
	private int Stage = 1;
	private static string[] Color = {"Red", "Blue", "White"};
	private static string[] Texts = {"Abort", "Detonate", "GG M8"};
	public Color[] Colors;
	void Awake () {
		moduleID = moduleIdCounter++;
		Bottun.OnInteract += delegate () { BottunPres(); return false; };
		Down.OnInteract += delegate () { Downa(); return false; };
		Up.OnInteract += delegate () { Upa(); return false; };
	}
	// Use this for initialization
	void Start () {
        if (CB.ColorblindModeActive)
            Colorblind = true;
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void Generate() {
		a = Random.Range(0,3);
		b = Random.Range(0,2);
		BottunText.text = Texts[b];
		Buttoncolor.material.SetColor("_Color", Colors[a]);
        if (Colorblind)
            CBText.text = Color[a][0].ToString();
		Presses = 0;
	    Debug.LogFormat("[Directional Button #{0}] <Stage {1}> The phrase on the button is '{2}', and the color is {3}.", moduleID, Stage, Texts[b], Color[a]);
		Ruling();
	}
	void BottunPres() {
	Bottun.AddInteractionPunch();
	GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Bottun.transform);

        if (ModuleSolved == true){
            return;
        }
        Presses = Presses+1;
	}
	void Downa(){
	GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Down.transform);
    Down.AddInteractionPunch();

		if (ModuleSolved==true){
			return;
		}
        if (up==false && Presses == CorrectPres){
            Debug.LogFormat("[Directional Button #{0}] <Stage {1}> You pressed the button {2} and then went down, which is correct.", moduleID, Stage, NumToString(Presses));
            Stage = Stage+1;
			if(Stage==6){
                Debug.LogFormat("[Directional Button #{0}] Module solved.", moduleID);
                GetComponent<KMBombModule>().HandlePass();
				Solve();
				ModuleSolved = true;
			}
			else {
				Generate();
			}
		}
		else {
        Debug.LogFormat("[Directional Button #{0}] <Stage {1}> You pressed the button {2} and then went down, which is incorrect. Strike!", moduleID, Stage, NumToString(Presses));
        Generate();
		GetComponent<KMBombModule>().HandleStrike();
		}
	}
	void Upa(){
	GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Up.transform);
	Up.AddInteractionPunch();

		if (ModuleSolved==true){
			return;
		}
		if(up==true && Presses == CorrectPres){
            Debug.LogFormat("[Directional Button #{0}] <Stage {1}> You pressed the button {2} and then went up, which is correct.", moduleID, Stage, NumToString(Presses));
            Stage = Stage+1;
			if(Stage==6){
                Debug.LogFormat("[Directional Button #{0}] Module solved.", moduleID);
                GetComponent<KMBombModule>().HandlePass();
				Solve();
				ModuleSolved = true;
			}
			else {
				Generate();
			}
		}
		else {
        Debug.LogFormat("[Directional Button #{0}] <Stage {1}> You pressed the button {2} and then went up, which is incorrect. Strike!", moduleID, Stage, NumToString(Presses));
        Generate();
		GetComponent<KMBombModule>().HandleStrike();
		}
	}
	void Ruling() {
		if (a==1&&b==1){
			CorrectPres = 1;
			up = false;
		}
		else if (a==0){
			CorrectPres = 2;
			up = false;
		}
		else if (b==0){
			CorrectPres = 3;
			up = true;
		}
		else if (a==2){
			CorrectPres = 4;
			up = true;
		}
        Debug.LogFormat("[Directional Button #{0}] <Stage {1}> You need to press the button {2} and then go {3}.", moduleID, Stage, NumToString(CorrectPres), up ? "up" : "down");
    }
	void Solve() {
	BottunText.text = Texts[2];
	}

    string NumToString(int n)
    {
        if (n == 0)
            return "zero times";
        else if (n == 1)
            return "once";
        else if (n == 2)
            return "twice";
        else if (n == 3)
            return "three times";
        else if (n == 4)
            return "four times";
        else
            return "more than four times";
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <#> [Presses the main button '#' times] | !{0} press <up/down> [Presses the up or down directional button] | !{0} colorblind [Toggles colorblind mode]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (Colorblind)
            {
                Colorblind = false;
                CBText.text = "";
            }
            else
            {
                Colorblind = true;
                CBText.text = Color[a][0].ToString();
            }
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
                yield break;
            }
            else if (parameters.Length == 2)
            {
                int temp = 0;
                if (!int.TryParse(parameters[1], out temp) && !parameters[1].EqualsIgnoreCase("up") && !parameters[1].EqualsIgnoreCase("down"))
                {
                    yield return "sendtochaterror The specified parameter '" + parameters[1] + "' is invalid!";
                    yield break;
                }
                if (parameters[1].EqualsIgnoreCase("up"))
                    Up.OnInteract();
                else if (parameters[1].EqualsIgnoreCase("down"))
                    Down.OnInteract();
                else
                {
                    for (int i = 0; i < temp; i++)
                    {
                        Bottun.OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a directional button or a number of times to press the main button!";
                yield break;
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (Presses > CorrectPres)
            Presses = 0;
        int start = Stage;
        for (int i = start; i < 6; i++)
        {
            while (Presses < CorrectPres)
            {
                Bottun.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            if (up)
            {
                Up.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Down.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
