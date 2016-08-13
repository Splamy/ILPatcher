using System;

namespace ILPatcher.Interface
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class EditorAttributes : Attribute
	{
		public string EditorName { get; }

		public EditorAttributes(string editorName)
		{
			EditorName = editorName;
        }
	}
}
