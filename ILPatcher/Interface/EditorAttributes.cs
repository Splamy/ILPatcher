using System;

namespace ILPatcher.Interface
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	class EditorAttributes : Attribute
	{
		/// <summary><para>Indicates whether the Editor needs a new page (false) or can be inlined into an other control (true)</para>
		/// <para>Default is false</para></summary>
		public bool Inline { get; set; } = false;
		public string EditorName { get; }

		public EditorAttributes(string editorName)
		{
			EditorName = editorName;
        }
	}
}
