using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using System.Linq;
//using Unity.VisualScripting;
using XNode;
using ColorUtility = UnityEngine.ColorUtility;

#if UNITY_EDITOR
using XNodeEditor;
using UnityEditor;
#endif

// Ren'Py Maker
// By Alan Baylis 
// https://www.renpymaker.com

namespace RenPy_Maker
{
	[CreateAssetMenu]
	public class RenpyMaker : NodeGraph
	{
		public BaseNode current;
		private string _filePath;
		private string _fileName;
		private string _fullPath;
		private int jumpIndex;
		private int optionIndex;
		private NodeParser _nodeParser;
		private List<BaseNode> _nodes;
		public static bool initialized;
		private static NodeGraph _currentGraph;
		private float _zoom;
		private Vector2 _panOffset;

		private void OnEnable()
		{
#if UNITY_EDITOR
			EditorApplication.update += OnEditorUpdate;
#endif
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			EditorApplication.update -= OnEditorUpdate;
#endif
		}

		private void OnEditorUpdate()
		{
#if UNITY_EDITOR
			if (!initialized)
			{
				_currentGraph = NodeEditorWindow.current.graph;
				
				initialized = true;
			}

			if (_currentGraph != NodeEditorWindow.current.graph)
			{
				RenpyMaker tempGraph = _currentGraph as RenpyMaker;
				tempGraph._zoom = NodeEditorWindow.current.zoom;
				tempGraph._panOffset = NodeEditorWindow.current.panOffset;
				tempGraph = NodeEditorWindow.current.graph as RenpyMaker;
				NodeEditorWindow.current.zoom = tempGraph._zoom;
				NodeEditorWindow.current.panOffset = tempGraph._panOffset;
				_currentGraph = NodeEditorWindow.current.graph;
				NodeEditorWindow.current.titleContent = new GUIContent(_currentGraph.name);
			}
#endif
		}

		[ContextMenu("Options/Toggle Tracking")]
		void ToggleNodeTracking()
		{
			GameObject renpymaker = GameObject.Find("RenPy Maker");
			_nodeParser = renpymaker.GetComponent<NodeParser>();
			_nodeParser.ToggleTracking();
		}

		[ContextMenu("Options/Toggle Debugging")]
		void ToggleDebugging()
		{
			GameObject renpymaker = GameObject.Find("RenPy Maker");
			_nodeParser = renpymaker.GetComponent<NodeParser>();
			_nodeParser.ToggleDebugging();
		}

		[ContextMenu("Home")]
		void CenterWindow()
		{
#if UNITY_EDITOR
			NodeEditorWindow.current.zoom = 1f;
			NodeEditorWindow.current.panOffset = new Vector2(0, 0);
#endif
		}
		
		[ContextMenu("Make Ren'Py Script")]
		void MakeRenpyScript()
		{
#if UNITY_EDITOR
			GameObject renpymaker = GameObject.Find("RenPy Maker");
			_nodeParser = renpymaker.GetComponent<NodeParser>();
			List<BaseNode> _nodes = _nodeParser.GetNodeList("All");
			BaseNode startNode = null;

			// Set all node id numbers
			int id = 0;
			foreach (BaseNode n in _nodes)
				n.SetNodeId(id++);

			// Find the Start node
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "StartNode")
				{
					startNode = n;
					break;
				}
			}

			if (startNode == null)
			{
				Debug.LogError("Could not find a Start node");
				return;
			}

			// Check for errors
			if (ErrorCheck())
			{
				Debug.LogError("Cancelled due to errors");
				return;
			}

			Debug.Log("Creating Ren'Py Script");

			// Open the file
			_filePath = "Assets/RenPy Maker/RenPyScripts/";
			_fileName = "script.rpy";
			_fullPath = GetFilePath("Get File Path", _fileName, "rpy", "Message", _filePath);

			if (string.IsNullOrEmpty(_fullPath))
			{
				Debug.Log("Cancelled!");
				return;
			}

			// Write description
			WriteString(_fullPath, "# Made with Ren'Py Maker", false);
			WriteString(_fullPath, "# By Alan Baylis", true);
			WriteString(_fullPath, "# https://www.renpymaker.com", true);
			WriteString(_fullPath, "", true);

			WriteStats(_fullPath);

			bool init_python = false;

			_nodes = _nodeParser.GetNodeList("CharacterNode");

			if (_nodes.Count > 0)
			{
				if (!init_python)
				{
					WriteString(_fullPath, "init python:", true);
					init_python = true;
				}

				WriteString(_fullPath, "    class Characters:", true);
				WriteString(_fullPath, "        def __init__(self, name: str, color: str):", true);
				WriteString(_fullPath, "            self.name = name", true);
				WriteString(_fullPath, "            self.color = color", true);
			}

			ProcessSpecialNodes(_fullPath);

			_nodes = _nodeParser.GetNodeList("All");

			HashSet<string> list = new HashSet<string>();

			// Define all characters
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "CharacterNode")
				{
					//list.Add("define " + n.GetCharacter().Replace(" ", "_") + "_ = Character(\"" + n.GetCharacter().Replace(" ", "_") + "\", color=\"#" + ColorUtility.ToHtmlStringRGB(n.GetColor()).ToLower() + "\")");
					list.Add("define " + n.GetCharacter().Replace(" ", "_") + "_ = Character(\"" + n.GetCharacter() +
					         "\", image=\"" + n.GetImage().name.Replace("side_", "").Replace("side ", "").ToLower() +
					         "\", color=\"#" +
					         ColorUtility.ToHtmlStringRGB(n.GetColor()).ToLower() + "\")");
				}
			}

			foreach (string str in list)
			{
				WriteString(_fullPath, str, true);
			}

			WriteString(_fullPath, "", true);

			list.Clear();

			// Create the images
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "ShowNode")
				{
					list.Add("image " + n.GetImage().name.Replace("_", " ").ToLower() + " = \"" +
					         n.GetImage().name.ToLower() + ".png\"");
				}
			}

			foreach (string str in list)
			{
				WriteString(_fullPath, str, true);
			}

			WriteString(_fullPath, "", true);

			list.Clear();

			// Create the side images
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "CharacterNode" ||
				    n.GetNodeType() == "DialogNode" ||
				    n.GetNodeType() == "MenuNode" ||
				    n.GetNodeType() == "NarrateNode")
				{
					list.Add("image " + n.GetImage().name.Replace("_", " ").ToLower() + " = \"" +
					         n.GetImage().name.ToLower() + ".png\"");
				}
			}

			foreach (string str in list)
			{
				WriteString(_fullPath, str, true);
			}

			WriteString(_fullPath, "", true);

			// Clear all
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() != "CharacterNode")
				{
					n.SetJumpIndex(-1);
					n.ClearLabelList();
					n.SetEvaluated(false);
				}
			}

			jumpIndex = 0;

			// Process all graphs
			string resourcesFolder = Application.dataPath;
			resourcesFolder += "/RenPy Maker/Resources/";
			string[] directories = Directory.GetDirectories(resourcesFolder, "*", SearchOption.AllDirectories);

			foreach (string item in directories)
			{
				if (Path.GetFileName(item) == _nodeParser.projectName)
				{
					DirectoryInfo folder = new DirectoryInfo(item);
					var files = folder.GetFiles("*.asset");

					for (int i = 0; i < files.Length; i++)
					{
						NodeGraph tempGraph =
							Resources.Load(_nodeParser.projectName + "/" +
							               Path.GetFileNameWithoutExtension(files[i].Name)) as NodeGraph;
						if (tempGraph != null)
						{
							foreach (BaseNode n in tempGraph.nodes)
							{
								if (n.GetNodeType() == "StartNode" ||
								    (n.GetNodeType() == "LabelNode" && !n.GetInputPort("entry").IsConnected))
								{
									PreProcessNodes(n);
								}
							}

							foreach (BaseNode n in tempGraph.nodes)
							{
								if (n.GetNodeType() != "CharacterNode")
								{
									n.SetEvaluated(false);
								}
							}

							foreach (BaseNode n in tempGraph.nodes)
							{
								if (n.GetNodeType() == "StartNode" ||
								    (n.GetNodeType() == "LabelNode" && !n.GetInputPort("entry").IsConnected))
								{
									ProcessNodes(n);
								}
							}
						}
					}
				}
			}

			Debug.Log("Done!");
#endif
		}

		public void ProcessSpecialNodes(string fullPath)
		{
			_nodes = _nodeParser.GetNodeList("CharacterNode");

			if (_nodes.Count > 0)
			{
				WriteString(fullPath, "", true);
				WriteString(fullPath, "# Character variables", true);
				WriteString(fullPath, "    CharacterList = []", true);

				foreach (BaseNode node in _nodes)
				{
					CharacterNode tempNode = node as CharacterNode;
					WriteString(fullPath, "    " + tempNode.GetCharacter() + " = Characters(" +
					                      "\"" + tempNode.GetCharacter() + "\", " +
					                      "\"" + tempNode.GetColor() + "\")", true);

					WriteString(fullPath, "    CharacterList.append(" + tempNode.GetCharacter() + ")", true);
				}
			}

			WriteString(fullPath, "", true);
		}

		void PreProcessNodes(BaseNode node)
		{
			if (node.GetNodeType() == "MenuNode")
			{
				if (node.GetEvaluated())
				{
					Debug.LogError("Menu node was already evaluated");
					return;
				}

				if (node.GetMenuOptions().Count() != node.DynamicOutputs.Count())
				{
					Debug.LogError("Mismatch between number of ports and options");
					return;
				}

				int index = 0;
				List<MenuNode.MenuOption> optionList = node.GetMenuOptions();
				foreach (NodePort port in node.DynamicOutputs)
				{
					BaseNode tempNode = port.Connection.node as BaseNode;
					tempNode.AddToLabelList(optionList[index].option + "_" + node.GetNodeId());
					node.SetEvaluated(true);
					PreProcessNodes(port.Connection.node as BaseNode);
					index++;
				}

				return;
			}

			node.SetEvaluated(true);

			if (node.GetNodeType() == "JumpNode")
			{
				PreProcessNodes(GetLabel(node.GetString()));
				return;
			}

			NodePort exitPort = node.GetOutputPort("exit");
			if (exitPort != null)
			{
				BaseNode nextNode = exitPort.Connection.node as BaseNode;
				if (nextNode != null)
				{
					if (nextNode.GetEvaluated())
					{
						if (node.GetJumpIndex() >= 0)
						{
							nextNode.AddToLabelList("destination_" + node.GetJumpIndex());
						}
						else
						{
							node.SetJumpIndex(jumpIndex);
							nextNode.AddToLabelList("destination_" + jumpIndex);
							jumpIndex++;
						}

						return;
					}

					PreProcessNodes(nextNode);
				}
			}
		}

		void ProcessNodes(BaseNode node)
		{
#if UNITY_EDITOR
			string musicFadeValues;
			string[] splitString;
			string attributes;

			if (node == null)
			{
				Debug.LogError("Node was null");
				return;
			}

			// Write all labels before this node
			foreach (string label in node.GetLabelList().Distinct())
			{
				WriteString(_fullPath, "label " + label + ":", true);
				WriteString(_fullPath, "", true);
			}

			node.ClearLabelList();

			// Process the node
			if (node.GetEvaluated() == false)
			{
				switch (node.GetNodeType())
				{
					case "CallNode":
					{
						WriteString(_fullPath, "    call " + node.GetString(), true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "CommentNode":
					{
						WriteString(_fullPath, "# " + node.GetString(), true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "DialogueNode":
					{
						splitString = SplitString("_", node.GetImage().name);
						attributes = "";
						if (splitString.Length > 2)
						{
							for (int i = 2; i < splitString.Length; i++)
							{
								attributes += splitString[i];
								attributes += " ";
							}
						}

						//Debug.Log(node.GetCharacter());

						WriteString(_fullPath,
							"    " + node.GetCharacter().Replace(" ", "_") + "_ " + attributes + "\"" +
							node.GetDialogue() +
							"\"",
							true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "HideNode":
					{
						WriteString(_fullPath, "    hide " + node.GetImage().name.Replace("_", " "), true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "JumpNode":
					{
						WriteString(_fullPath, "    jump " + node.GetString(), true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "LabelNode":
					{
						WriteString(_fullPath, "label " + node.GetString() + ":", true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "MenuNode":
					{
						WriteString(_fullPath, "menu:", true);
						WriteString(_fullPath, "", true);
						WriteString(_fullPath,
							"    " + node.GetCharacter().Replace(" ", "_") + "_ \"" + node.GetDialogue() + "\"",
							true);
						WriteString(_fullPath, "", true);

						List<MenuNode.MenuOption> optionList = node.GetMenuOptions();

						for (int i = 0; i < optionList.Count(); i++)
						{
							WriteString(_fullPath, "    \"" + optionList[i].dialogue + "\":", true);
							WriteString(_fullPath, "        jump " + optionList[i].option + "_" + node.GetNodeId(),
								true);
							WriteString(_fullPath, "", true);
						}

						break;
					}

					case "MusicNode":
					{
						musicFadeValues = " fadeout " + node.GetFadeout() + " fadein " + node.GetFadein();
						WriteString(_fullPath,
							"    play music \"audio/" + node.GetString().Replace(" ", "_") + ".mp3\"" + musicFadeValues,
							true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "NarrateNode":
					{
						WriteString(_fullPath, "    \"" + node.GetDialogue() + "\"", true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "PauseNode":
					{
						if (node.GetSeconds() == 0)
							WriteString(_fullPath, "    pause", true);
						else
							WriteString(_fullPath, "    pause " + node.GetSeconds(), true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "QueueMusicNode":
					{
						WriteString(_fullPath,
							"    queue music \"audio/" + node.GetString().Replace(" ", "_") + ".mp3\"",
							true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "ReturnNode":
					{
						WriteString(_fullPath, "return", true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "SceneNode":
					{
						WriteString(_fullPath, "    scene " + node.GetImage().name.Replace("_", " "), true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "ShowNode":
					{
						WriteString(_fullPath,
							"    show " + node.GetImage().name.Replace("_", " ") + " at " +
							node.GetPosition().ToLower(),
							true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "SoundNode":
					{
						WriteString(_fullPath,
							"    play sound \"audio/" + node.GetString().Replace(" ", "_") + ".mp3\"",
							true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "StartNode":
					{
						WriteString(_fullPath, "label start:", true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "StopMusicNode":
					{
						musicFadeValues = " fadeout " + node.GetFadeout();
						WriteString(_fullPath, "    stop music" + musicFadeValues, true);
						WriteString(_fullPath, "", true);
						break;
					}

					case "TransitionNode":
					{
						break;
					}

					default:
					{
						Debug.LogError("Fell through the switch block");
						break;
					}
				}
			}

			if (node.GetJumpIndex() >= 0 && node.GetEvaluated() == false)
			{
				NodePort port = node.GetOutputPort("exit");
				if (port == null)
					return;
				BaseNode nextNode = port.Connection.node as BaseNode;
				if (nextNode == null)
				{
					//Debug.LogError("nextNode was null");
					return;
				}

				//List<string> labels = nextNode.GetLabelList();	
				WriteString(_fullPath, "    jump destination_" + node.GetJumpIndex(), true);
				WriteString(_fullPath, "", true);
				node.SetEvaluated(true);
				if (nextNode.GetEvaluated() == false)
				{
					ProcessNodes(port.Connection.node as BaseNode);
				}

				return;
			}

			if (node.GetNodeType() == "MenuNode")
			{
				if (node.GetEvaluated())
					return;

				node.SetEvaluated(true);

				foreach (NodePort port in node.DynamicOutputs)
				{
					ProcessNodes(port.Connection.node as BaseNode);
				}

				return;
			}

			node.SetEvaluated(true);

			if (node.GetNodeType() == "JumpNode")
			{
				ProcessNodes(GetLabel(node.GetString()));
				return;
			}

			NodePort exitPort = node.GetOutputPort("exit");
			if (exitPort != null)
			{
				ProcessNodes(exitPort.Connection.node as BaseNode);
			}
#endif
		}

		public void WriteStats(string fullPath)
		{
			List<BaseNode> nodes;
			nodes = _nodeParser.GetNodeList("CharacterNode");
			WriteString(_fullPath, "# " + nodes.Count + " Character Nodes", true);
			WriteString(_fullPath, "", true);
		}

		[ContextMenu("Select All")]
		void SelectAll()
		{
#if UNITY_EDITOR
			Selection.objects = nodes.ToArray(); // nodes from current graph
#endif
		}

		[ContextMenu("Check For Errors")]
		void TestGraph()
		{
			if (!ErrorCheck())
			{
				Debug.Log("No Errors Found");
			}
		}

		private string GetFilePath(string title, string fileName, string fileExtension, string message,
			string startDirectory)
		{
			string fullPath = "";
#if UNITY_EDITOR
			fullPath = EditorUtility.SaveFilePanelInProject(title, fileName, fileExtension, message, startDirectory);
#endif
			return fullPath;
		}

		private void WriteString(string thePath, string theString, bool append)
		{
			//Write some text to the file
			StreamWriter writer = new StreamWriter(thePath, append);
			writer.WriteLine(theString);
			writer.Flush();
			writer.Close();
		}

		BaseNode GetLabel(string label)
		{
			_nodes = _nodeParser.GetNodeList("LabelNode");
			foreach (BaseNode n in _nodes)
			{
				if (n.GetString() == label)
				{
					return n;
				}
			}

			if (label == "start")
			{
				// Find the Start node
				_nodes = _nodeParser.GetNodeList("All");

				foreach (BaseNode n in _nodes)
				{
					if (n.GetNodeType() == "StartNode")
					{
						return n;
					}
				}
			}

			Debug.LogError("Couldn't find node with label \"" + label + "\"");

			return null;
		}

		[ContextMenu("Take Screenshot")]
		void TakeScreenshot()
		{
#if UNITY_EDITOR
			int resWidth = 1920;
			int resHeight = 1080;

			// Open the file
			_filePath = "Assets/Renpy Maker/Images/";
			_fileName = string.Format("Background_{0}.png", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			_fullPath = GetFilePath("Get File Path", _fileName, "png", "Message", _filePath);

			if (string.IsNullOrEmpty(_fullPath))
			{
				Debug.Log("Cancelled!");
				return;
			}

			SceneView tempView = EditorWindow.GetWindow<SceneView>();
			Camera tempCamera = tempView.camera;
			RenderTexture backupTexture = RenderTexture.active;
			tempCamera.targetTexture = RenderTexture.GetTemporary(resWidth, resHeight, 24, RenderTextureFormat.ARGB32);
			RenderTexture.active = tempCamera.targetTexture;
			tempCamera.Render();
			Texture2D renderedTexture = new Texture2D(resWidth, resHeight);
			renderedTexture.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			RenderTexture.active = backupTexture;
			byte[] byteArray = renderedTexture.EncodeToPNG();
			File.WriteAllBytes(_filePath + _fileName, byteArray);
			Debug.Log(string.Format("Took screenshot to: {0}", _fileName));
			AssetDatabase.Refresh();
			//UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			//EditorWindow view = EditorWindow.GetWindow<SceneView>();
			//view.Repaint();
			//EditorApplication.ExecuteMenuItem("Window/General/Game");
#endif
		}
/*
	[ContextMenu("Reset Character List")]
	void ResetCharacters()
	{
		characterList.Clear();
		foreach (BaseNode n in nodes)
		{
			if (n.GetNodeType() == "CharacterNode")
			{
				characterList.Add(n.GetCharacter());
			}
		}
	}
*/

		/// <summary>
		/// ^ - means start of the string
		/// []* - could contain any number of characters between brackets
		/// a-zA-Z0-9 - any alphanumeric characters
		/// \s - any space characters (space/tab/etc.)
		/// \w - any word character or underscore
		/// , - commas
		/// $ - end of the string
		/// </summary>
		public static bool CheckForAlphaNumeric(string strToCheck)
		{
			if (string.IsNullOrEmpty(strToCheck))
				return false;

			Regex rg = new Regex(@"^[a-zA-Z]+[\w\s]*$");
			return rg.IsMatch(strToCheck);
		}

		/// <summary>
		/// This will look for NEEDLE in HAYSTACK and return an array of split strings.
		/// NOTE: If the returned array has a length of 1 (meaning it only contains
		///       element [0]) then that means NEEDLE was NOT found.
		/// </summary>
		public string[] SplitString(string needle, string haystack)
		{
			return haystack.Split(new string[] { needle }, System.StringSplitOptions.None);
		}

		public bool ErrorCheck()
		{
			int index = 0;
			int errorCount = 0;

			GameObject renpymaker = GameObject.Find("RenPy Maker");
			_nodeParser = renpymaker.GetComponent<NodeParser>();
			_nodes = _nodeParser.GetNodeList("All");

			if (nodes == null)
			{
				Debug.LogError("Please add your graph to the Node Parser script");
				return true;
			}

			if (_nodes.Count == 0)
			{
				Debug.LogError("Please add some nodes to the graph");
				return true;
			}

			// Check for a Start node
			bool foundStart = false;
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "StartNode")
				{
					foundStart = true;
					break;
				}
			}

			if (!foundStart)
			{
				Debug.LogError("Could not find a Start node");
				return true;
			}

			// Check for an Exit node
			bool foundExit = false;
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "ReturnNode")
				{
					foundExit = true;
					break;
				}
			}

			if (!foundExit)
			{
				Debug.LogError("Could not find an Exit node");
				return true;
			}

			// Check if a Dialogue or Menu node exists
			bool found = false;
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "DialogueNode" || n.GetNodeType() == "MenuNode")
					found = true;
			}

			if (!found)
			{
				Debug.LogError("You need to add at least one Dialogue or Menu node");
				return true;
			}

			// Check for duplicate labels
			List<string> labels = new List<string>();
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "LabelNode")
				{
					if (labels.Contains(n.GetString()))
					{
						n.SetError();
						Debug.LogError("Found duplicate labels");
						return true;
					}

					labels.Add(n.GetString());
				}
			}

			// Check for missing jump labels
			foreach (BaseNode n in _nodes)
			{
				if (n.GetNodeType() == "JumpNode")
				{
					if (!labels.Contains(n.GetString()) && n.GetString() != "start")
					{
						n.SetError();
						Debug.LogError("Missing jump label");
						return true;
					}

					labels.Add(n.GetString());
				}
			}

			// Check for missing connections and other errors
			foreach (BaseNode n in _nodes)
			{
				if (_nodes[index] == null)
				{
					Debug.Log("Found Null Nodes");
					continue;
				}

				string errorMessage = n.GetError();
				if (errorMessage != "No Error")
				{
					Debug.Log(errorMessage);
					errorCount++;
				}

				index++;
			}

			if (errorCount > 0)
			{
				string tempString =
					errorCount < 2 ? "Found " + errorCount + " Error" : "Found " + errorCount + " Errors";
				Debug.Log(tempString);
				return true;
			}

			return false;
		}

		public static Texture2D MakeTex(int width, int height, Color textureColor, RectOffset border, Color bordercolor)
		{
			int widthInner = width;
			width += border.left;
			width += border.right;

			Color[] pix = new Color[width * (height + border.top + border.bottom)];

			for (int i = 0; i < pix.Length; i++)
			{
				if (i < (border.bottom * width))
					pix[i] = bordercolor;
				else if (i >= ((border.bottom * width) + (height * width))) //Border Top
					pix[i] = bordercolor;
				else
				{
					//Center of Texture

					if ((i % width) < border.left) // Border left
						pix[i] = bordercolor;
					else if ((i % width) >= (border.left + widthInner)) //Border right
						pix[i] = bordercolor;
					else
						pix[i] = textureColor; //Color texture
				}
			}

			Texture2D result = new Texture2D(width, height + border.top + border.bottom);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}
	}
}