using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using XNode;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace RenPy_Maker
{
    public class NodeParser : MonoBehaviour
    {
        private List<BaseNode> nodeStack = new List<BaseNode>();
        private List<RenpyMaker> graphStack = new List<RenpyMaker>();

        public string projectName = "";
        public TMP_Text speaker;
        public TMP_Text dialogue;
        public GameObject images;
        public Image speakerImage;
        public Image sceneImage;
        public AudioSource musicSource;
        public AudioSource soundSource;
        public RenpyMaker graph;
        [HideInInspector] public int _resolutionIndex;

        private bool enableTracking = true;
        private bool enableDebugging = false;

        private List<string> resolutions = new List<string>
            { "1066x600", "1280x720", "1920x1080", "2560x1440", "3840x2160" };

        private List<Texture2D> _textures = new List<Texture2D>();
        private List<string> _positions = new List<string>();
        private Queue<AudioClip> clipQueue = new Queue<AudioClip>();
        private int _indexButtonPressed;
        private Texture2D _speakerTexture;
        private Texture2D _sceneTexture;
        private Texture2D _showTexture;
        private Coroutine _parser;

#if UNITY_EDITOR
        private NodeEditorWindow _window;
#endif

        public void Awake()
        {
#if UNITY_EDITOR
            _window = (NodeEditorWindow)EditorWindow.GetWindow(typeof(NodeEditorWindow), false, "Ren'Py Maker");
            _window.titleContent.text = "Ren'Py Maker";
            _window.wantsMouseMove = true;
            _window.Show();
            
            RenpyMaker.initialized = false;
#endif
        }

        public List<string> GetResolutions()
        {
            return resolutions;
        }

        public int GetResolutionIndex()
        {
            return _resolutionIndex;
        }

        public void SetResolutonIndex(int res)
        {
            _resolutionIndex = res;
        }

        public void Update()
        {
            if (musicSource.isPlaying == false && clipQueue.Count > 0)
            {
                musicSource.clip = clipQueue.Dequeue();
                musicSource.Play();
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            musicSource.Stop();
            clipQueue.Clear();
            clipQueue.Enqueue(clip);
        }

        public void PlaySound(AudioClip clip)
        {
            soundSource.Stop();
            soundSource.clip = clip;
            soundSource.Play();
        }

        public void QueueAudio(AudioClip clip)
        {
            clipQueue.Enqueue(clip);
        }

        public void StopAudio()
        {
            musicSource.Stop();
            clipQueue.Clear();
        }

        public void TestArea()
        {
        }

        public void OnDestroy()
        {
        }

        public void ToggleTracking()
        {
            enableTracking = !enableTracking;
        }

        public void ToggleDebugging()
        {
            // Todo: Update this to show the current state of enableDebugging
            enableDebugging = !enableDebugging;
        }

        public void UpdateNodeEditorWindow()
        {
#if UNITY_EDITOR
            NodeEditorWindow window = NodeEditorWindow.current;
            window.titleContent = new GUIContent(graph.name);

            if (Selection.count == 1 && enableTracking == true)
            {
                Vector2 nodeDimension = new Vector2(100, 150);
                NodeEditorWindow.current.panOffset = -graph.current.position - nodeDimension;
            }
#endif
        }

        public void Start()
        {
            TestArea();

            if (graph.ErrorCheck())
                return;

            graph.current = null;

#if UNITY_EDITOR
            // Try to start from selected node
            if (Selection.count == 1 && enableDebugging == true)
            {
                List<UnityEngine.Object> selectionCache;
                selectionCache = new List<UnityEngine.Object>(Selection.objects);

                UnityEngine.Object[] assets = Resources.LoadAll(projectName);

                foreach (UnityEngine.Object asset in assets)
                {
                    if (asset.GetType() == typeof(RenpyMaker))
                    {
                        RenpyMaker newGraph = asset as RenpyMaker;

                        if (newGraph != null)
                        {
                            foreach (BaseNode node in newGraph.nodes)
                            {
                                if (selectionCache.Contains(node))
                                {
                                    Debug.Log("Starting from selected node");

                                    if (enableTracking)
                                    {
                                        NodeEditorWindow window = NodeEditorWindow.current;
                                        window.graph = newGraph;
                                    }

                                    graph = newGraph;
                                    graph.current = node;
                                    UpdateNodeEditorWindow();

                                    StopLastCoroutine();
                                    _parser = StartCoroutine(ParseNode());
                                    return;
                                }
                            }
                        }
                    }
                }
            }
#endif
            FindStart();
        }

        public void FindStart()
        {
            graph.current = null;

            UnityEngine.Object[] assets = Resources.LoadAll(projectName);

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset.GetType() == typeof(RenpyMaker))
                {
                    RenpyMaker newGraph = asset as RenpyMaker;

                    if (newGraph != null)
                    {
                        foreach (BaseNode n in newGraph.nodes)
                        {
                            if (n.GetNodeType() == "StartNode")
                            {
#if UNITY_EDITOR
                                if (enableTracking)
                                {
                                    NodeEditorWindow window = NodeEditorWindow.current;
                                    window.graph = newGraph;
                                }
#endif
                                graph = newGraph;
                                graph.current = n;
                                UpdateNodeEditorWindow();

                                NextNode("exit");
                                return;
                            }
                        }
                    }
                }
            }

            Debug.LogError("Unable to find the Start node");
        }

        public void CallLabel(string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
            {
                Debug.LogError("Missing label name");
                return;
            }

            nodeStack.Add(graph.current);
            graphStack.Add(graph);

            UnityEngine.Object[] assets = Resources.LoadAll(projectName);

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset.GetType() == typeof(RenpyMaker))
                {
                    RenpyMaker newGraph = asset as RenpyMaker;

                    if (newGraph != null)
                    {
                        foreach (BaseNode n in newGraph.nodes)
                        {
                            if (n.GetNodeType() == "LabelNode" && n.GetString() == labelName)
                            {
#if UNITY_EDITOR
                                if (enableTracking)
                                {
                                    NodeEditorWindow window = NodeEditorWindow.current;
                                    window.graph = newGraph;
                                }
#endif
                                graph = newGraph;
                                graph.current = n;
                                UpdateNodeEditorWindow();

                                NextNode("exit");
                                return;
                            }
                        }
                    }
                }
            }

            Debug.LogError("Unable to find the Label node");
        }

        public void JumpLabel(string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
            {
                Debug.LogError("Missing label name");
                return;
            }

            UnityEngine.Object[] assets = Resources.LoadAll(projectName);

            if (labelName == "start")
            {
                foreach (UnityEngine.Object asset in assets)
                {
                    if (asset.GetType() == typeof(RenpyMaker))
                    {
                        RenpyMaker newGraph = asset as RenpyMaker;

                        if (newGraph != null)
                        {
                            foreach (BaseNode n in newGraph.nodes)
                            {
                                if (n.GetNodeType() == "StartNode")
                                {
#if UNITY_EDITOR
                                    if (enableTracking)
                                    {
                                        NodeEditorWindow window = NodeEditorWindow.current;
                                        window.graph = newGraph;
                                    }
#endif
                                    graph = newGraph;
                                    graph.current = n;
                                    UpdateNodeEditorWindow();

                                    NextNode("exit");
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset.GetType() == typeof(RenpyMaker))
                {
                    RenpyMaker newGraph = asset as RenpyMaker;

                    if (newGraph != null)
                    {
                        foreach (BaseNode n in newGraph.nodes)
                        {
                            if (n.GetNodeType() == "LabelNode" && n.GetString() == labelName)
                            {
#if UNITY_EDITOR
                                if (enableTracking)
                                {
                                    NodeEditorWindow window = NodeEditorWindow.current;
                                    window.graph = newGraph;
                                }
#endif
                                graph = newGraph;
                                graph.current = n;
                                UpdateNodeEditorWindow();

                                NextNode("exit");
                                return;
                            }
                        }
                    }
                }
            }

            Debug.LogError("Unable to find the Label node");
        }

        public List<BaseNode> GetNodeList(string nodeType)
        {
            List<BaseNode> nodes = new List<BaseNode>();

            UnityEngine.Object[] assets = Resources.LoadAll(projectName);

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset.GetType() == typeof(RenpyMaker))
                {
                    RenpyMaker newGraph = asset as RenpyMaker;

                    if (newGraph != null)
                    {
                        foreach (BaseNode n in newGraph.nodes)
                        {
                            if (n.GetEnabledStatus())
                            {
                                if (nodeType == "All")
                                {
                                    nodes.Add(n);
                                }
                                else if (n.GetNodeType() == nodeType)
                                {
                                    nodes.Add(n);
                                }
                            }
                        }
                    }
                }
            }

            return nodes;
        }

        IEnumerator ParseNode()
        {
            BaseNode node = graph.current;

#if UNITY_EDITOR
            Selection.activeGameObject = null;
            Selection.activeObject = node;
            _window.Repaint();
#endif

            string theNodeType = node.GetNodeType();

            if (node.GetEnabledStatus() == false)
            {
                NextNode("exit");
            }
            else
            {
                switch (theNodeType)
                {
                    case "CallNode":
                    {
                        string newGraphName = node.GetString();
                        CallLabel(newGraphName);

                        break;
                    }

                    case "CommentNode":
                    {
                        //this.ShowNotification(new GUIContent("Text"));
                        NextNode("exit");

                        break;
                    }

                    case "DialogueNode":
                    {
                        speaker.text = node.GetCharacter();
                        speaker.color = node.GetColor();
                        dialogue.text = node.GetDialogue();
                        dialogue.fontStyle = FontStyles.Normal;
                        _speakerTexture = node.GetImage();
                        speakerImage.overrideSprite = Sprite.Create(_speakerTexture,
                            new Rect(0, 0, _speakerTexture.width, _speakerTexture.height), new Vector2(0.5f, 0.5f),
                            100);

                        yield return new WaitUntil(() => Mouse.current.leftButton.wasPressedThisFrame);
                        yield return new WaitUntil(() => Mouse.current.leftButton.wasReleasedThisFrame);

                        NextNode("exit");

                        break;
                    }

                    case "HideNode":
                    {
                        int index = 0;
                        Texture2D hideTexture = node.GetImage();
                        foreach (Texture2D tex in _textures)
                        {
                            if (tex == hideTexture)
                            {
                                _textures.RemoveAt(index);
                                _positions.RemoveAt(index);
                                break;
                            }

                            index++;
                        }

                        DestroyChildren(images.transform);
                        index = 0;
                        foreach (Texture2D tex in _textures)
                        {
                            GameObject imageObject = new GameObject();
                            imageObject.name = tex.name;
                            Image newImage = imageObject.AddComponent<Image>();
                            float tempWidth;
                            float tempHeight;
                            if (_resolutionIndex == 0)
                            {
                                tempWidth = tex.width * 1.8f;
                                tempHeight = tex.height * 1.8f;
                            }
                            else if (_resolutionIndex == 1)
                            {
                                tempWidth = tex.width * 1.5f;
                                tempHeight = tex.height * 1.5f;
                            }
                            else if (_resolutionIndex == 2)
                            {
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }
                            else if (_resolutionIndex == 3)
                            {
                                Debug.Log("2560x1440");
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }
                            else if (_resolutionIndex == 4)
                            {
                                Debug.Log("3840x2160");
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }
                            else
                            {
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }

                            newImage.rectTransform.sizeDelta = new Vector2(tempWidth, tempHeight);
                            newImage.preserveAspect = true;
                            if (_positions[index] == "Right")
                                newImage.rectTransform.position = new Vector3(1560, tempHeight / 2, 0);
                            else if (_positions[index] == "Left")
                                newImage.rectTransform.position = new Vector3(360, tempHeight / 2, 0);
                            else if (_positions[index] == "TrueCenter")
                                newImage.rectTransform.position = new Vector3(960, 540, 0);
                            else
                                newImage.rectTransform.position = new Vector3(960, tempHeight / 2, 0);
                            Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height),
                                new Vector2(0.5f, 0.5f), 100.0f);
                            newSprite.name = tex.name;
                            newImage.sprite = newSprite;
                            newImage.preserveAspect = true;
                            imageObject.transform.SetParent(images.transform);
                            imageObject.transform.localPosition = Vector3.zero;
                            imageObject.transform.localScale = Vector3.one;
                            index++;
                        }

                        NextNode("exit");

                        break;
                    }

                    case "JumpNode":
                    {
                        JumpLabel(node.GetString());

                        break;
                    }

                    case "LabelNode":
                    {
                        NextNode("exit");

                        break;
                    }

                    case "MenuNode":
                    {
                        speaker.text = node.GetCharacter();
                        speaker.color = node.GetColor();
                        dialogue.text = node.GetDialogue();
                        dialogue.fontStyle = FontStyles.Normal;
                        _speakerTexture = node.GetImage();
                        speakerImage.overrideSprite = Sprite.Create(_speakerTexture,
                            new Rect(0, 0, _speakerTexture.width, _speakerTexture.height), new Vector2(0.5f, 0.5f),
                            100);

                        _indexButtonPressed = 0;
                        List<Button> activeAndInactiveButtons = FindObjectsOfType<Button>(true).ToList();
                        activeAndInactiveButtons = activeAndInactiveButtons.OrderBy(go => go.name).ToList();

                        // Quick hack, update this later
                        int exitButtonIndex = 0;
                        for (int i = 0; i < activeAndInactiveButtons.Count(); i++)
                        {
                            if (activeAndInactiveButtons[i].gameObject.name == "Exit")
                                exitButtonIndex = i;
                        }

                        activeAndInactiveButtons.RemoveAt(exitButtonIndex);

                        for (int i = 0; i < activeAndInactiveButtons.Count(); i++)
                        {
                            activeAndInactiveButtons[i].gameObject.SetActive(false);
                        }

                        List<MenuNode.MenuOption> optionList = node.GetMenuOptions();

                        for (int i = 0; i < optionList.Count(); i++)
                        {
                            activeAndInactiveButtons[i].gameObject.SetActive(true);
                            activeAndInactiveButtons[i].GetComponentInChildren<TMP_Text>().text =
                                optionList[i].dialogue;
                        }

                        // Show relevant buttons 
                        yield return new WaitUntil(() => _indexButtonPressed != 0);

                        // Hide all buttons
                        for (int i = 0; i < activeAndInactiveButtons.Count(); i++)
                        {
                            activeAndInactiveButtons[i].gameObject.SetActive(false);
                        }

                        NextNode(optionList[_indexButtonPressed - 1].option);

                        break;
                    }

                    case "MusicNode":
                    {
                        // Add Fadein and Fadeout to Unity player
                        PlayMusic(node.GetAudioSource());

                        NextNode("exit");

                        break;
                    }

                    case "NarrateNode":
                    {
                        speaker.text = "";
                        speaker.color = node.GetColor();
                        dialogue.text = node.GetDialogue();
                        dialogue.fontStyle = FontStyles.Italic;
                        _speakerTexture = node.GetImage();
                        speakerImage.overrideSprite = Sprite.Create(_speakerTexture,
                            new Rect(0, 0, _speakerTexture.width, _speakerTexture.height), new Vector2(0.5f, 0.5f),
                            100);

                        yield return new WaitUntil(() => Mouse.current.leftButton.wasPressedThisFrame);
                        yield return new WaitUntil(() => Mouse.current.leftButton.wasReleasedThisFrame);

                        NextNode("exit");

                        break;
                    }

                    case "PauseNode":
                    {
                        if (node.GetSeconds() == 0)
                        {
                            yield return new WaitUntil(() => Mouse.current.leftButton.wasPressedThisFrame);
                            yield return new WaitUntil(() => Mouse.current.leftButton.wasReleasedThisFrame);
                        }
                        else
                            yield return new WaitForSeconds(node.GetSeconds());

                        NextNode("exit");

                        break;
                    }

                    case "QueueMusicNode":
                    {
                        QueueAudio(node.GetAudioSource());

                        NextNode("exit");

                        break;
                    }

                    case "ReturnNode":
                    {
                        if (graphStack.Count == 0)
                        {
#if UNITY_EDITOR
                            FindStart();
#endif
                        }
                        else
                        {
                            graph = graphStack[graphStack.Count - 1];
                            graph.current = nodeStack[nodeStack.Count - 1];
                            graphStack.RemoveAt(graphStack.Count - 1);
                            nodeStack.RemoveAt(nodeStack.Count - 1);
#if UNITY_EDITOR
                            NodeEditorWindow window = NodeEditorWindow.current;
                            window.graph = Resources.Load(projectName + "/" + graph.name) as NodeGraph;
#endif
                            NextNode("exit");

                            break;
                        }

                        ExitApplication();

                        break;
                    }

                    case "SceneNode":
                    {
                        DestroyChildren(images.transform);
                        _textures.Clear();
                        _positions.Clear();
                        _sceneTexture = node.GetImage();
                        sceneImage.overrideSprite = Sprite.Create(_sceneTexture,
                            new Rect(0, 0, _sceneTexture.width, _sceneTexture.height), new Vector2(0.5f, 0.5f), 100);

                        NextNode("exit");

                        break;
                    }

                    case "ShowNode":
                    {
                        DestroyChildren(images.transform);
                        _showTexture = node.GetImage();
                        _textures.Add(_showTexture);
                        _positions.Add(node.GetPosition());
                        int index = 0;
                        foreach (Texture2D tex in _textures)
                        {
                            GameObject imageObject = new GameObject();
                            imageObject.name = tex.name;
                            Image newImage = imageObject.AddComponent<Image>();
                            float tempWidth;
                            float tempHeight;
                            if (_resolutionIndex == 0)
                            {
                                // 1066x600
                                tempWidth = tex.width * 1.8f;
                                tempHeight = tex.height * 1.8f;
                            }
                            else if (_resolutionIndex == 1)
                            {
                                // 1280x720
                                tempWidth = tex.width * 1.5f + 5f;
                                tempHeight = tex.height * 1.5f;
                                //Debug.Log(tempWidth + " " + tempHeight);
                            }
                            else if (_resolutionIndex == 2)
                            {
                                // 1920x1080
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }
                            else if (_resolutionIndex == 3)
                            {
                                // 2560x1440
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }
                            else if (_resolutionIndex == 4)
                            {
                                // 3840x2160
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }
                            else
                            {
                                tempWidth = tex.width;
                                tempHeight = tex.height;
                            }


                            newImage.rectTransform.sizeDelta = new Vector2(tempWidth, tempHeight);
                            newImage.preserveAspect = true;
                            if (_positions[index] == "Right")
                                newImage.rectTransform.position = new Vector3(1560, tempHeight / 2, 0);
                            else if (_positions[index] == "Left")
                                newImage.rectTransform.position = new Vector3(360, tempHeight / 2, 0);
                            else if (_positions[index] == "TrueCenter")
                                newImage.rectTransform.position = new Vector3(960, 540, 0);
                            else
                                newImage.rectTransform.position = new Vector3(960, tempHeight / 2, 0);
                            Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height),
                                new Vector2(0.5f, 0.5f), 100.0f);
                            newSprite.name = tex.name;
                            newImage.sprite = newSprite;
                            imageObject.transform.SetParent(images.transform);
                            imageObject.transform.localPosition = Vector3.zero;
                            imageObject.transform.localScale = Vector3.one;
                            index++;
                        }

                        NextNode("exit");

                        break;
                    }

                    case "SoundNode":
                    {
                        PlaySound(node.GetAudioSource());

                        NextNode("exit");

                        break;
                    }

                    case "StartNode":
                    {
                        NextNode("exit");

                        break;
                    }

                    case "StopMusicNode":
                    {
                        // Add Fadeout to Unity player
                        StopAudio();

                        NextNode("exit");

                        break;
                    }

                    case "TransitionNode":
                    {
                        // Add transitions to Unity player
                        string transition = node.GetTransition();

#if UNITY_EDITOR
/*            
                foreach(SceneView scene in SceneView.sceneViews)
                {
                    scene.ShowNotification(new GUIContent(transition));
                }

                _window.ShowNotification(new GUIContent(transition));
*/
#endif
                        NextNode("exit");

                        break;
                    }

                    default:
                    {
                        Debug.LogError("Fell through the switch block");
                        break;
                    }
                }
            }
        }

        private IEnumerator MouseOver()
        {
            while (true)
            {
                Vector2 currentPos = Mouse.current.position.ReadValue();

                Canvas theCanvas = null;
                GameObject tempObject = GameObject.Find("Canvas");
                if (tempObject != null)
                {
                    theCanvas = tempObject.GetComponent<Canvas>();
                    if (theCanvas == null)
                        Debug.Log("Could not locate Canvas component on " + tempObject.name);
                }

                currentPos = Mouse.current.position.ReadValue() / theCanvas.scaleFactor;

                //float posX = 1280f / 1920f * currentPos.x; // Update this to the selected resolution
                //float posY = 720f / 1080f * currentPos.y;

                float posX = currentPos.x;
                float posY = currentPos.y;

                //Debug.Log(posX + " " + posY);

                yield return null;
            }
        }

        public void StopLastCoroutine()
        {
            if (_parser != null)
            {
                StopCoroutine(_parser);
                _parser = null;
            }
        }

        public void NextNode(string fieldName)
        {
            // Find the port with this name
            foreach (NodePort p in graph.current.Ports)
            {
                if (p.fieldName == fieldName)
                {
                    graph.current = p.Connection.node as BaseNode;
                    UpdateNodeEditorWindow();
                    break;
                }
            }

            StopLastCoroutine();
            _parser = StartCoroutine(ParseNode());
        }

        public void SetIndexOfButton(int index)
        {
            _indexButtonPressed = index;
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        /// <summary>
        /// Calls GameObject.Destroy on all children of transform and immediately detaches the children
        /// from transform so after this call transform.childCount is zero.
        /// </summary>
        public static void DestroyChildren(Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }

            transform.DetachChildren();
        }
    }
}