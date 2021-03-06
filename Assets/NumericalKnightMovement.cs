﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class NumericalKnightMovement : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
	public KMBombModule Module;
	
	public KMSelectable[] Tiles;
	public AudioClip[] SFX;
	
	int[][] ValidTiles = new int[][]{
		new int[] {6, 9},
		new int[] {8, 10, 7},
		new int[] {9, 11, 4},
		new int[] {5, 10},
		new int[] {2, 10, 13},
		new int[] {3, 11, 14, 12},
		new int[] {0, 8, 15, 13},
		new int[] {1, 9, 14},
		new int[] {6, 14, 1},
		new int[] {2, 0, 7, 15},
		new int[] {1, 3, 4, 12},
		new int[] {5, 13, 2},
		new int[] {10, 5},
		new int[] {4, 6, 11},
		new int[] {5, 7, 8},
		new int[] {9, 6}
	};
	
	int Table = -1;
	int Counter = 0;
	List<int> Movement = new List<int>();
	List<string> Coordinates = new List<string>();
	List<string> Count = new List<string>();
	string[] CoordinatesCodename = {"A1", "B1", "C1", "D1", "A2", "B2", "C2", "D2", "A3", "B3", "C3", "D3", "A4", "B4", "C4", "D4"};
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;
	
	void Awake()
	{
		moduleId = moduleIdCounter++;
		for (int i = 0; i < 16; i++)
		{
			int Tile = i;
			Tiles[i].OnInteract += delegate
			{
				PressTile(Tile);
				return false; 
			};
		}
	}
	
	void Start()
	{
		int AmountOfMovement = UnityEngine.Random.Range(24,33);
		for (int x = 0; x < AmountOfMovement; x++)
		{
			int StartingMovement = x == 0 ? UnityEngine.Random.Range(0 , Tiles.Length) : ValidTiles[Movement[x-1]][UnityEngine.Random.Range(0,ValidTiles[Movement[x-1]].Length)];
			Tiles[StartingMovement].GetComponentInChildren<TextMesh>().text = (Int32.Parse(Tiles[StartingMovement].GetComponentInChildren<TextMesh>().text) + 1).ToString();
			Movement.Add(StartingMovement);
			Coordinates.Add(CoordinatesCodename[StartingMovement]);
		}
		
		Coordinates.Reverse();
		string Log = "Movement performed by the knight: ";
		for (int x = 0; x < Coordinates.Count(); x++)
		{
			Log += x != Coordinates.Count() - 1 ? Coordinates[x] + " > " : Coordinates[x];
		}
		Debug.LogFormat("[Numerical Knight Movement #{0}] {1}", moduleId, Log);
		Debug.LogFormat("[Numerical Knight Movement #{0}] ---------------------------------------------------------------------------------------------", moduleId);
		Debug.LogFormat("[Numerical Knight Movement #{0}] The grid generated by the module: ", moduleId, Log);
		string Grid = "";
		for (int x = 0; x < Tiles.Length; x++)
		{
			Grid += Tiles[x].GetComponentInChildren<TextMesh>().text;
			if (x % 4 == 3)
			{
				Debug.LogFormat("[Numerical Knight Movement #{0}] {1}", moduleId, Grid);
				Grid = "";
			}
		}
		
		for (int a = 0; a < Tiles.Length; a++)
		{
			Count.Add(Tiles[a].GetComponentInChildren<TextMesh>().text);
		}
	}
	
	void PressTile(int Tile)
	{
		Tiles[Tile].AddInteractionPunch(0.2f);
		if (!ModuleSolved)
		{
			if (Table == -1)
			{
				if (Tiles[Tile].GetComponentInChildren<TextMesh>().text != "0")
				{
					Debug.LogFormat("[Numerical Knight Movement #{0}] ---------------------------------------------------------------------------------------------", moduleId);
					Debug.LogFormat("[Numerical Knight Movement #{0}] You stepped on {1}", moduleId, CoordinatesCodename[Tile]);
					Audio.PlaySoundAtTransform(SFX[0].name, transform);
					Table = Tile;
					Tiles[Tile].GetComponentInChildren<TextMesh>().text = (Int32.Parse(Tiles[Tile].GetComponentInChildren<TextMesh>().text) - 1).ToString();
					Tiles[Tile].GetComponentInChildren<TextMesh>().color = new Color (0.5f, 0.5f, 0.5f);
				}
				
				else
				{
					Debug.LogFormat("[Numerical Knight Movement #{0}] ---------------------------------------------------------------------------------------------", moduleId);
					Debug.LogFormat("[Numerical Knight Movement #{0}] You tried to stepped on {1}. You were unable to do that. The module striked.", moduleId, CoordinatesCodename[Tile]);
					Counter = 0;
					Module.HandleStrike();
				}
			}
			
			else
			{
				if (Tile == Table)
				{
					Audio.PlaySoundAtTransform(SFX[0].name, transform);
					Counter++;
					if (Counter == 2)
					{
						Debug.LogFormat("[Numerical Knight Movement #{0}] ---------------------------------------------------------------------------------------------", moduleId);
						Debug.LogFormat("[Numerical Knight Movement #{0}] The knight's tile was selected 3 tiles. The module resets back to the original grid.", moduleId);
						Table = -1;
						for (int x = 0; x < Tiles.Length; x++)
						{
							Tiles[x].GetComponentInChildren<TextMesh>().text = Count[x];
							Tiles[x].GetComponentInChildren<TextMesh>().color = x.EqualsAny(0,2,5,7,8,10,13,15) ? Color.black : Color.white;
						}
						Counter = 0;
					}
				}
				
				else if (ValidTiles[Table].Contains(Tile) && Tiles[Tile].GetComponentInChildren<TextMesh>().text != "0")
				{
					Debug.LogFormat("[Numerical Knight Movement #{0}] You stepped on {1}.", moduleId, CoordinatesCodename[Tile]);
					Counter = 0;
					Audio.PlaySoundAtTransform(SFX[0].name, transform);
					Tiles[Table].GetComponentInChildren<TextMesh>().color = Table.EqualsAny(0,2,5,7,8,10,13,15) ? Color.black : Color.white;
					Tiles[Tile].GetComponentInChildren<TextMesh>().color = new Color (0.5f, 0.5f, 0.5f);
					Tiles[Tile].GetComponentInChildren<TextMesh>().text = (Int32.Parse(Tiles[Tile].GetComponentInChildren<TextMesh>().text) - 1).ToString();
					Table = Tile;
					if (Tiles[Tile].GetComponentInChildren<TextMesh>().text == "0")
					{
						for (int x = 0; x < 16; x++)
						{
							if (Tiles[x].GetComponentInChildren<TextMesh>().text != "0")
							{
								return;
							}
						}
						
						Module.HandlePass();
						for (int a = 0; a < 16; a++)
						{
							Tiles[a].GetComponentInChildren<TextMesh>().text = "";
							Tiles[a].GetComponent<MeshRenderer>().material.color = a.EqualsAny(0,2,5,7,8,10,13,15) ? Color.green : Color.white;
						}
						Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
						ModuleSolved = true;
						Debug.LogFormat("[Numerical Knight Movement #{0}] ---------------------------------------------------------------------------------------------", moduleId);
						Debug.LogFormat("[Numerical Knight Movement #{0}] The knight has travelled properly. Module solved.", moduleId);
						
					}
				}
				
				else
				{
					Debug.LogFormat("[Numerical Knight Movement #{0}] You tried to stepped on {1}. You were unable to do that. The module striked.", moduleId, CoordinatesCodename[Tile]);
					Counter = 0;
					Module.HandleStrike();
				}
			}
		}
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To press a certain coordinate on the module, use the command !{0} [A-D][1-4] (You can perform this action in a chain. Example: !{0} A1 B3 C4) | To reset the grid, use the command !{0} reset";
    #pragma warning restore 414
	
    IEnumerator ProcessTwitchCommand(string command)
    {
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (Table == -1)
			{
				yield return "sendtochaterror The board cannot be reset as the knight is not standing on a tile. The command was not continued.";
				yield break;
			}
			for (int x = 0; x < 2; x++)
			{
				Tiles[Table].OnInteract();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		
		else
		{
			yield return null;
			for (int x = 0; x < parameters.Length; x++)
			{
				if (!parameters[x].ToUpper().EqualsAny(CoordinatesCodename))
				{
					yield return "sendtochaterror An invalid coordinate was detected. The command was not continued.";
					yield break;
				}
				
				if (Table != -1 && parameters[x].ToUpper() == CoordinatesCodename[Table])
				{
					yield return "sendtochaterror The command wants to press the knight's tile. The handler prevented the action. The command was not continued.";
					yield break;
				}
				
				Tiles[Array.IndexOf(CoordinatesCodename, parameters[x].ToUpper())].OnInteract();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
	}

	IEnumerator TwitchHandleForcedSolve()
    {
		if (Table != -1)
        {
			for (int x = 0; x < 2; x++)
			{
				Tiles[Table].OnInteract();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		for (int x = 0; x < Movement.Count; x++)
		{
			Tiles[Movement[x]].OnInteract();
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}
}
