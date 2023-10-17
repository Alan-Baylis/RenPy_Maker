using UnityEngine;

namespace RenPy_Maker
{
    [CreateNodeMenu("Nodes/Character")]
    public class CharacterNode : BaseNode
    {
        public bool enabled = true;

        public string character;
        [HideInInspector] public string previousName;
        public Texture2D image;
        public Color color = Color.white;
        [HideInInspector] public bool errorStatus;

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

        public override string GetCharacter()
        {
            return character;
        }

        public override Color GetColor()
        {
            return color;
        }

        public override string GetNodeType()
        {
            return "CharacterNode";
        }

        public override Texture2D GetImage()
        {
            return image;
        }

        public override void SetError()
        {
            errorStatus = true;
        }

        public override string GetError()
        {
            errorStatus = false;

            if (!RenpyMaker.CheckForAlphaNumeric(character))
            {
                errorStatus = true;
                return "Character name must start with a letter and use alphanumeric characters";
            }

            if (string.IsNullOrEmpty(character))
            {
                errorStatus = true;
                return "No character name";
            }

            if (image == null)
            {
                errorStatus = true;
                return "No image texture";
            }

            return "No Error";
        }
    }
}