using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using XNode;

namespace RenPy_Maker
{
	public abstract class BaseNode : Node
	{
		public virtual void SetNodeId(int newId)
		{
		}

		public virtual int GetNodeId()
		{
			return 0;
		}

		public virtual string GetStorageName()
		{
			return null;
		}

		public virtual void SetStorageName(string name)
		{
		}

		public virtual string GetPersonalityName()
		{
			return null;
		}

		public virtual void SetPersonalityName(string name)
		{
		}

		public virtual string GetDescription()
		{
			return null;
		}

		public virtual void SetDescription(string name)
		{
		}

		public virtual string GetItemName()
		{
			return null;
		}

		public virtual bool GetEnabledStatus()
		{
			return false;
		}

		public virtual int GetIndex()
		{
			return 0;
		}

		public virtual float GetFadein()
		{
			return 1.0f;
		}

		public virtual float GetFadeout()
		{
			return 1.0f;
		}

		public virtual AudioClip GetAudioSource()
		{
			return null;
		}

		public virtual VideoClip GetMainVideo()
		{
			return null;
		}

		public virtual VideoClip GetMaskVideo()
		{
			return null;
		}

		public virtual string GetTransition()
		{
			return null;
		}

		public virtual string GetPosition()
		{
			return null;
		}

		public virtual string GetNodeType()
		{
			return null;
		}

		public virtual string GetCharacter()
		{
			return null;
		}

		public virtual void SetCharacter(string characterName)
		{
		}

		public virtual string GetString()
		{
			return null;
		}

		public virtual string GetChannel()
		{
			return null;
		}

		public virtual List<MenuNode.MenuOption> GetMenuOptions()
		{
			return null;
		}

		public virtual Texture2D GetImage()
		{
			return null;
		}

		public virtual Texture2D GetIdleImage()
		{
			return null;
		}

		public virtual Texture2D GetHoverImage()
		{
			return null;
		}

		public virtual string GetDialogue()
		{
			return null;
		}

		public virtual Color GetColor()
		{
			return Color.white;
		}

		public virtual float GetSeconds()
		{
			return 1.0f;
		}

		public virtual Sprite GetSprite()
		{
			return null;
		}

		public virtual bool GetState()
		{
			return false;
		}

		public virtual void SetState(bool state)
		{
		}

		public virtual void SetJumpIndex(int newIndex)
		{
		}

		public virtual int GetJumpIndex()
		{
			return 1;
		}

		public virtual void AddToLabelList(string label)
		{
		}

		public virtual List<string> GetLabelList()
		{
			return null;
		}

		public virtual void ClearLabelList()
		{
		}

		public virtual bool GetEvaluated()
		{
			return false;
		}

		public virtual void SetEvaluated(bool flag)
		{
		}

		public virtual string GetError()
		{
			return null;
		}

		public virtual void SetError()
		{
		}

		public override object GetValue(NodePort port)
		{
			return null;
		}
	}
}