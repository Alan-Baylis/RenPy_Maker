using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;

#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

public class NodeParser : MonoBehaviour
{
    public Color speakerColor;
    public TMP_Text speaker;
    public TMP_Text dialogue;
    public GameObject images;
    public Image speakerImage;
    public Image sceneImage;
    public AudioSource musicSource;
    public AudioSource soundSource;
    public RenpyMaker graph;
    [HideInInspector]
    public int _resolutionIndex;
    private List<string> resolutions = new List<string> {"1066x600", "1280x720", "1920x1080"};
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
        _window = (NodeEditorWindow)EditorWindow.GetWindow(typeof(NodeEditorWindow), true, "Ren'Py Maker");
        _window.titleContent.text = "Ren'Py Maker";
        _window.Show();
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
/*        
        // Resize Image 
        GameObject image = GameObject.Find("Ren'Py Maker Canvas/Image");
        if (image != null)
        {
            Vector3 scale = new Vector3(3.0f, 3.0f, 1.0f);
            image.transform.localScale = scale;
            Vector3 pos = image.transform.position;
            pos.y = 150.0f;
            image.transform.position = pos;
        }
*/
/*        
        // Changing the Sprite from a file in the Assets folder
        byte[] data = File.ReadAllBytes(Path.GetFullPath("Assets/RenpyMaker/Images/side_lucy.png"));
        Texture2D texture = new Texture2D(300, 300, TextureFormat.ARGB32, false);
        texture.LoadImage(data);
        speakerImage.overrideSprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height),new Vector2(0.5f, 0.5f),100);
*/
        // Changing an Image's position and scale
    }

    public void Start()
    {
#if UNITY_EDITOR
        TestArea();
        
        if (graph.ErrorCheck())
            return;

        graph.current = null;
        
        if (Selection.count == 1)
        {
            // Get start node if there is only one node selected and it is in graph.nodes
            List<UnityEngine.Object> selectionCache;
            selectionCache = new List<UnityEngine.Object>(Selection.objects);
            
            int index = 0;
            if (graph.nodes != null)
            {
                foreach (BaseNode node in graph.nodes)
                {
                    // Skip these types of nodes
                    if (node == null || node.GetNodeType() == "CharacterNode")
                    {
                        index++;
                        continue;
                    }

                    if (selectionCache.Contains(graph.nodes[index]))
                    {
                        Debug.Log("Starting from selected node");
                        graph.current = node;
                        break;
                    }

                    index++;
                }
            }
        }

        if (graph.current == null)
        {
            // Find a Start node
            foreach (BaseNode n in graph.nodes)
            {
                if (n.GetNodeType() == "StartNode")
                {
                    graph.current = n;
                    break;
                }
            }
        }

        if (graph.current == null)
        {
            Debug.LogError("Unable to find a start node");
            return;
        }
#else
        // Find a Start node
        foreach (BaseNode n in graph.nodes)
        {
            if (n.GetNodeType() == "StartNode")
            {
                graph.current = n;
                break;
            }
        }
#endif
        StopLastCoroutine();
        _parser = StartCoroutine(ParseNode());
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
        string theCharacter = node.GetCharacter();
        string theDialogue = node.GetDialogue();
       
        if (theNodeType == "CommentNode")
        {
            //this.ShowNotification(new GUIContent("Text"));
            NextNode("exit");
        }
        
        if (theNodeType == "DialogueNode")
        {
            speaker.text = theCharacter;
            speakerColor = node.GetColor();
            speaker.color = speakerColor;
            dialogue.text = theDialogue;
            dialogue.fontStyle = FontStyles.Normal;
            _speakerTexture = node.GetImage();
            speakerImage.overrideSprite = Sprite.Create(_speakerTexture, new Rect(0,0, _speakerTexture.width ,_speakerTexture.height),new Vector2(0.5f, 0.5f),100);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

            NextNode("exit");
        }

        if (theNodeType == "DynamicNode")
        {
            speaker.text = theCharacter;
            speakerColor = node.GetColor();
            speaker.color = speakerColor;
            dialogue.text = theDialogue;
            dialogue.fontStyle = FontStyles.Normal;
            _speakerTexture = node.GetImage();
            speakerImage.overrideSprite = Sprite.Create(_speakerTexture, new Rect(0,0, _speakerTexture.width, _speakerTexture.height),new Vector2(0.5f, 0.5f),100);

            _indexButtonPressed = 0;
            List<Button> activeAndInactiveButtons = GameObject.FindObjectsOfType<Button>(true).ToList();
            activeAndInactiveButtons = activeAndInactiveButtons.OrderBy(go=>go.name).ToList();
            
            for (int i = 0; i < activeAndInactiveButtons.Count(); i++)
            {
                if (activeAndInactiveButtons[i].gameObject.name != "Exit")
                    activeAndInactiveButtons[i].gameObject.SetActive(false);
            }

            List<DynamicNode.DialogueOption> optionList = node.GetDynamicOptions();
            
            for (int i = 0; i < optionList.Count(); i++)
            {
                activeAndInactiveButtons[i].gameObject.SetActive(true);
                activeAndInactiveButtons[i].GetComponentInChildren<TMP_Text>().text = optionList[i].dialogue;
            }

            // Show relevant buttons 
            yield return new WaitUntil(() => _indexButtonPressed != 0);
            
            NextNode(optionList[_indexButtonPressed - 1].option);
            
            // Hide all buttons
            for (int i = 0; i < activeAndInactiveButtons.Count(); i++)
            {
                if (activeAndInactiveButtons[i].gameObject.name != "Exit")
                    activeAndInactiveButtons[i].gameObject.SetActive(false);
            }
        }
        
        if (theNodeType == "HideNode")
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
                Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                newSprite.name = tex.name;
                newImage.sprite = newSprite;
                newImage.preserveAspect = true;
                imageObject.transform.SetParent(images.transform);
                index++;
            }

            NextNode("exit");
        }

        if (theNodeType == "JumpNode")
        {
            string jumpLabel = graph.current.GetString();
            
            foreach (BaseNode n in graph.nodes)
            {
                if (n.GetNodeType() == "LabelNode")
                {
                    if (n.GetString() == jumpLabel)
                    {
                        graph.current = n;
                        break;
                    }
                }                
            }

            StopLastCoroutine();
            _parser = StartCoroutine(ParseNode());
        }
        
        if (theNodeType == "LabelNode")
        {
            NextNode("exit");
        }
        
        if (theNodeType == "MusicNode")
        {
            // Add Fadein and Fadeout to Unity player
            PlayMusic(node.GetAudioSource());
            
            NextNode("exit");
        }

        if (theNodeType == "NarrateNode")
        {
            speaker.text = "";
            speakerColor = node.GetColor();
            speaker.color = speakerColor;
            dialogue.text = theDialogue;
            dialogue.fontStyle = FontStyles.Italic;
            _speakerTexture = node.GetImage();
            speakerImage.overrideSprite = Sprite.Create(_speakerTexture, new Rect(0,0, _speakerTexture.width ,_speakerTexture.height),new Vector2(0.5f, 0.5f),100);
            
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

            NextNode("exit");
        }
        
        if (theNodeType == "PauseNode")
        {
            if (node.GetSeconds() == 0)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            }
            else
                yield return new WaitForSeconds(node.GetSeconds());

            NextNode("exit");
        }
        
        if (theNodeType == "QueueMusicNode")
        {
            QueueAudio(node.GetAudioSource());

            NextNode("exit");
        }

        if (theNodeType == "ReturnNode")
        {
#if UNITY_EDITOR
            // Returns to the start
            foreach (BaseNode n in graph.nodes)
            {
                if (n.GetNodeType() == "StartNode")
                {
                    graph.current = n;
                    break;
                }
            }

            StopLastCoroutine();
            _parser = StartCoroutine(ParseNode());
#else
            ExitApplication();
#endif
        }

        if (theNodeType == "SceneNode")
        {
            DestroyChildren(images.transform);
            _textures.Clear();
            _positions.Clear();
            _sceneTexture = node.GetImage();
            sceneImage.overrideSprite = Sprite.Create(_sceneTexture, new Rect(0,0, _sceneTexture.width, _sceneTexture.height),new Vector2(0.5f, 0.5f),100);
            NextNode("exit");
        }

        if (theNodeType == "SoundNode")
        {
            PlaySound(node.GetAudioSource());
            NextNode("exit");
        }

        if (theNodeType == "ShowNode")
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
                    tempWidth = tex.width * 1.8f;
                    tempHeight = tex.height * 1.8f;
                }
                else if (_resolutionIndex == 1)
                {
                    tempWidth = tex.width * 1.5f;
                    tempHeight = tex.height * 1.5f;
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
                Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                newSprite.name = tex.name;
                newImage.sprite = newSprite;
                imageObject.transform.SetParent(images.transform);
                index++;
            }

            NextNode("exit");
        }

        if (theNodeType == "StartNode")
        {
            NextNode("exit");
        }

        if (theNodeType == "StopMusicNode")
        {
            // Add Fadeout to Unity player
            StopAudio();
            
            NextNode("exit");
        }

        if (theNodeType == "TransitionNode")
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