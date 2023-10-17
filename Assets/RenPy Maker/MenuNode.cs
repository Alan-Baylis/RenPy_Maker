using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace RenPy_Maker
{
    [NodeWidth(210)]
    [CreateNodeMenu("Nodes/Menu")]
    public class MenuNode : BaseNode
    {
        [Input] public int entry;

        public bool enabled = true;

        [TextArea] public string dialogue = "";
        public Texture2D image;
        public Sprite sprite;
        public string character = "";
        public Color color = Color.white;
        [HideInInspector] public bool initialized;
        [HideInInspector] [SerializeField] public int _characterIndex;
        [HideInInspector] public bool errorStatus;
        [HideInInspector] public List<MenuOption> menuOptionList = new List<MenuOption>();
        private int jumpIndex;
        private bool evaluated;
        private List<string> labels = new List<string>();

        private int _nodeId;

        public override bool GetEnabledStatus()
        {
            return enabled;
        }

        public override void SetNodeId(int id)
        {
            _nodeId = id;
        }

        public override int GetNodeId()
        {
            return _nodeId;
        }

        public int GetCharacterIndex()
        {
            return _characterIndex;
        }

        public void SetCharacterIndex(int res)
        {
            _characterIndex = res;
        }

        public override string GetNodeType()
        {
            return "MenuNode";
        }

        public override string GetCharacter()
        {
            return character;
        }

        public override void SetCharacter(string characterName)
        {
            character = characterName;
        }

        public override string GetDialogue()
        {
            return dialogue;
        }

        public override Sprite GetSprite()
        {
            return sprite;
        }

        public override Texture2D GetImage()
        {
            return image;
        }

        public override Color GetColor()
        {
            return color;
        }

        public override List<MenuOption> GetMenuOptions()
        {
            return menuOptionList;
        }

        [System.Serializable]
        public class MenuOption
        {
            public string dialogue;
            public string option;

            public MenuOption(string theDialogue, string theOption)
            {
                dialogue = theDialogue;
                option = theOption;
            }
        }

        public override void SetJumpIndex(int index)
        {
            jumpIndex = index;
        }

        public override int GetJumpIndex()
        {
            return jumpIndex;
        }

        public override void AddToLabelList(string newLabel)
        {
            labels.Add(newLabel);
        }

        public override List<string> GetLabelList()
        {
            return labels;
        }

        public override void ClearLabelList()
        {
            labels.Clear();
        }

        public override bool GetEvaluated()
        {
            return evaluated;
        }

        public override void SetEvaluated(bool flag)
        {
            evaluated = flag;
        }

        public override void SetError()
        {
            errorStatus = true;
        }

        public override string GetError()
        {
            errorStatus = false;

            foreach (NodePort p in Inputs)
                if (!p.IsConnected)
                    errorStatus = true;

            foreach (NodePort p in Outputs)
                if (!p.IsConnected)
                    errorStatus = true;

            foreach (NodePort p in DynamicInputs)
                if (!p.IsConnected)
                    errorStatus = true;

            foreach (NodePort p in DynamicOutputs)
                if (!p.IsConnected)
                    errorStatus = true;

            if (errorStatus)
                return "Unconnected ports";

            if (!RenpyMaker.CheckForAlphaNumeric(character))
            {
                errorStatus = true;
                return "Character name must start with a letter and use alphanumeric characters";
            }

            foreach (MenuOption option in menuOptionList)
            {
                if (!RenpyMaker.CheckForAlphaNumeric(option.option))
                {
                    errorStatus = true;
                    return "Outputs must start with a letter and use alphanumeric characters";
                }
            }

            foreach (NodePort p in DynamicOutputs)
            {
                if (p != null)
                {
                    BaseNode nextNode = p.Connection.node as BaseNode;
                    if (nextNode != null && nextNode == this)
                    {
                        errorStatus = true;
                        return "Cannot connect outputs to self";
                    }
                }
            }

            if (string.IsNullOrEmpty(character))
            {
                errorStatus = true;
                return "No character name";
            }

            if (string.IsNullOrEmpty(dialogue))
            {
                errorStatus = true;
                return "No dialogue text";
            }

            if (image == null)
            {
                errorStatus = true;
                return "No image texture";
            }

            if (menuOptionList.Count == 0)
            {
                errorStatus = true;
                return "No output buttons";
            }

            return "No Error";
        }
    }
}