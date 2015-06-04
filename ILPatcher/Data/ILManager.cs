// ILNode does not belong here, since it is more Interface related
// then data related
using ILPatcher.Interface.General; // extract ILNode
// The main-reference is only used for the assembly definition
// this isn't nice...
using ILPatcher.Interface.Main; // remove when static removed
using ILPatcher.Utility;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ILPatcher.Data
{
	class ILManager
	{
		private AnyArray<OperandInfo> MemberList;

		private Dictionary<string, ILNode> ModuleList;

		private static Dictionary<string, OpCode> _OpCodeLookup;
		public static Dictionary<string, OpCode> OpCodeLookup
		{
			get
			{
				if (_OpCodeLookup == null)
				{
					_OpCodeLookup = new Dictionary<string, OpCode>();
					FieldInfo[] fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
					foreach (FieldInfo fi in fields)
					{
						if (fi.FieldType == typeof(OpCode))
						{
							OpCode oc = (OpCode)fi.GetValue(null);
							_OpCodeLookup.Add(oc.Name, oc);
						}
					}
				}
				return _OpCodeLookup;
			}
			protected set { }
		}

		private static ILManager instance; // TODO remove static
		public static ILManager Instance
		{
			get { if (instance == null) instance = new ILManager(); return instance; }
			protected set { }
		}

		private static NameCompressor nc = NameCompressor.Instance;

		private ILManager()
		{
			MemberList = new AnyArray<OperandInfo>();

			ModuleList = new Dictionary<string, ILNode>();
		}

		// SAVE METHODS ******************************************************

		/// <summary>Saves the current reference list of this ILM into a xml parent node.
		/// If an entry is not resolved it will copy the raw data read back again.</summary>
		/// <param name="xNode">The parent node for the new reference nodes</param>
		public void Save(XmlNode xNode)
		{
			NameCompressor nc = NameCompressor.Instance;
			XmlElement xM = xNode.InsertCompressedElement(SST.MethodReference);
			XmlElement xF = xNode.InsertCompressedElement(SST.FieldReference);
			XmlElement xT = xNode.InsertCompressedElement(SST.TypeReference);
			XmlElement xCS = xNode.InsertCompressedElement(SST.CallSite);

			for (int i = 0; i < MemberList.Length; i++)
			{
				OperandInfo oi = MemberList[i];
				switch (oi.oit)
				{
				case OperandInfoT.ParameterDefinition:
				case OperandInfoT.ParameterReference:
					Log.Write(Log.Level.Warning, "PT resolving is obsolete: ", oi.ToString());
					//todo reenable
					if (oi.resolved)
						Reference(((ParameterReference)oi.operand).ParameterType);
					else
						Log.Write(Log.Level.Error, "ParameterDef/Ref cannot be raw saved");
					break;
				case OperandInfoT.MethodDefinition:
				case OperandInfoT.MethodReference:
				case OperandInfoT.GenericInstanceMethod:
					if (oi.resolved)
						GenMChild(xM, (MethodReference)oi.operand, i);
					else
						xM.AppendClonedChild(oi.rawData);
					break;
				case OperandInfoT.FieldDefinition:
				case OperandInfoT.FieldReference:
					if (oi.resolved)
						GenFChild(xF, (FieldReference)MemberList[i].operand, i);
					else
						xF.AppendClonedChild(oi.rawData);
					break;
				case OperandInfoT.TypeDefinition:
				case OperandInfoT.TypeReference:
				case OperandInfoT.GenericInstanceType:
				case OperandInfoT.GenericParameter:
					if (oi.resolved)
						GenTChild(xT, (TypeReference)MemberList[i].operand, i);
					else
						xT.AppendClonedChild(oi.rawData);
					break;
				case OperandInfoT.VariableDefinition:
				case OperandInfoT.VariableReference:
					Log.Write(Log.Level.Warning, "VD resolving is obsolete: ", oi.ToString());
					//todo reenable
					if (oi.resolved)
						Reference(((VariableReference)oi.operand).VariableType);
					else
						Log.Write(Log.Level.Info, "VariableDef/Ref cannot be raw saved");
					break;
				case OperandInfoT.ArrayType:
					if (oi.resolved)
						GenAChild(xT, (ArrayType)MemberList[i].operand, i);
					else
						xT.AppendClonedChild(oi.rawData);
					break;
				case OperandInfoT.CallSite:
					if (oi.resolved)
					{
						XmlElement xElem = xCS.InsertCompressedElement(i);
						xElem.CreateAttribute(SST.NAME, ((CallSite)MemberList[i].operand).FullName);
						Log.Write(Log.Level.Error, "Unhandled CallSite: ", oi.ToString());
					}
					else
						xCS.AppendClonedChild(oi.rawData);
					break;
				default:
					Log.Write(Log.Level.Error, "Not saved Member Entry: ", oi.oit.ToString());
					break;
				}
			}
		}

		/// <summary>Inserts the xml formatted reference for a MethodReference into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="mr">The MethodReference which will be xml formatted</param>
		/// <param name="val">The ID of the MethodReference in the ILM list</param>
		private void GenMChild(XmlElement xGroup, MethodReference mr, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);
			StringBuilder strb = new StringBuilder();

			xElem.CreateAttribute(SST.TYPE, Reference(mr.DeclaringType).ToBaseAlph());
			xElem.CreateAttribute(SST.NAME, mr.Name);

			#region GENERICS
			if (mr.IsGenericInstance)
			{
				GenericInstanceMethod git = mr as GenericInstanceMethod;
				if (git == null)
					Log.Write(Log.Level.Error, "GenericInstance Type couldn't be converted: ", git.FullName);
				else
				{
					strb.Clear();
					for (int i = 0; i < git.GenericArguments.Count; i++) // && git.GenericArguments[i] != null
						if (git.GenericArguments[i].IsGenericParameter)
						{
							strb.Append('$'); // Instance $ ?
							strb.Append(git.GenericArguments[i].Name);
						}
						else
						{
							strb.Append(Reference(git.GenericArguments[i]).ToBaseAlph());
						}
					xElem.CreateAttribute(SST.GENERICS, strb.ToString());
				}
			}
			else if (mr.HasGenericParameters)
			{
				strb.Clear();
				for (int i = 0; i < mr.GenericParameters.Count; i++)
				{
					strb.Append('%'); // Definition % ?
					strb.Append(mr.GenericParameters[i].Name);
					//strb.Append(Reference(mr.GenericParameters[i].Type).ToBaseAlph()); // type is of GenericParameterType enum
					strb.Append(' ');
				}
				xElem.CreateAttribute(SST.GENERICS, strb.ToString());
			}
			#endregion

			#region RETURN
			strb.Clear();
			if (mr.ReturnType.IsGenericParameter)
			{
				strb.Append('$'); // Definition % ?
				strb.Append(mr.ReturnType.Name);
			}
			else
				strb.Append(Reference(mr.ReturnType).ToBaseAlph());
			xElem.CreateAttribute(SST.RETURN, strb.ToString());
			#endregion

			#region PARAMETER
			if (mr.Parameters.Count > 0)
			{
				strb.Clear();
				for (int i = 0; i < mr.Parameters.Count; i++)
				{
					if (mr.Parameters[i].ParameterType.IsGenericParameter)
					{
						strb.Append('%'); // Definition % ?
						strb.Append(mr.Parameters[i].ParameterType.Name); // should be ignorable, but look for a better way to store
					}
					else
					{
						strb.Append(Reference(mr.Parameters[i].ParameterType).ToBaseAlph());
					}
					strb.Append(' ');
				}
				xElem.CreateAttribute(SST.PARAMETER, strb.ToString());
			}
			#endregion
		}

		/// <summary>Inserts the xml formatted reference for a FieldReference into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="fr">The FieldReference which will be xml formatted</param>
		/// <param name="val">The ID of the FieldReference in the ILM list</param>
		private void GenFChild(XmlElement xGroup, FieldReference fr, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.TYPE, Reference(fr.FieldType).ToBaseAlph());
			xElem.CreateAttribute(SST.NAME, fr.Name);
			xElem.CreateAttribute(SST.MODULE, Reference(fr.DeclaringType).ToBaseAlph());
		}

		/// <summary>Inserts the xml formatted reference for a TypeReference into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="tr">The TypeReference which will be xml formatted</param>
		/// <param name="val">The ID of the TypeReference in the ILM list</param>
		private void GenTChild(XmlElement xGroup, TypeReference tr, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.NAME, tr.Name);

			StringBuilder strb = new StringBuilder();
			if (tr.IsGenericInstance)
			{
				// This is a instanced form of a Type, it can still have unspecified (generic) Parameters
				// but also specified Parameters like a int32
				if (tr.HasGenericParameters) // debug
					Console.WriteLine("Woah");
				GenericInstanceType git = tr as GenericInstanceType;
				if (git == null)
					Log.Write(Log.Level.Error, "GenericInstance Type couldn't be converted: ", tr.FullName);
				else
				{
					for (int i = 0; i < git.GenericArguments.Count && git.GenericArguments[i] != null; i++)
					{
						if (git.GenericArguments[i].IsGenericParameter)
						{
							// this is a unspecified gen Paramater, we have to take it as-is
							strb.Append('$'); // Instance $
							strb.Append(git.GenericArguments[i].Name);
						}
						else
						{
							// this is a specified gen Paramater and thus can be ILM referenced
							strb.Append(Reference(git.GenericArguments[i]).ToBaseAlph());
						}
						strb.Append(' ');
					}
					xElem.CreateAttribute(SST.GENERICS, strb.ToString());
				}
			}
			else if (tr.HasGenericParameters)
			{
				// This is the Definition on a Type with Generic parameters
				for (int i = 0; i < tr.GenericParameters.Count; i++)
				{
					strb.Append('%'); // Definition %
					strb.Append(tr.GenericParameters[i].Name);
					strb.Append(' ');
				}
				xElem.CreateAttribute(SST.GENERICS, strb.ToString());
			}

			if (tr.IsNested)
				xElem.CreateAttribute(SST.NESTEDIN, Reference(tr.DeclaringType).ToBaseAlph());
			else
			{
				xElem.CreateAttribute(SST.MODULE, tr.Scope.Name);
				if (!tr.IsGenericParameter)
					xElem.CreateAttribute(SST.NAMESPACE, tr.Namespace);
				else
					Log.Write(Log.Level.Error, "A GenericParameter(Type) has been passed. It will be dismissed.");
			}
		}

		/// <summary>Inserts the xml formatted reference for an ArrayType into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="at">The ArrayType which will be xml formatted</param>
		/// <param name="val">The ID of the ArrayType in the ILM list</param>
		private void GenAChild(XmlElement xGroup, ArrayType at, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.TYPE, Reference(at.ElementType).ToBaseAlph());
			if (at.IsVector)
				xElem.CreateAttribute(SST.ARRAY, "0");
			else
				xElem.CreateAttribute(SST.ARRAY, at.Dimensions.Count.ToString());
		}

		/// <summary>Looks for the given object in the ILM list and returns its ID,
		/// if the object doesn't exists the method saves it and returns the new ID</summary>
		/// <param name="_operand">The object to be referenced</param>
		/// <returns>Unique ID for each object</returns>
		public int Reference(object _operand)
		{
			if (_operand == null)
			{
				Log.Write(Log.Level.Warning, "Can't reference <null>");
				return -1;
			}

			for (int i = 0; i < MemberList.Length; i++)
			{
				if (!MemberList[i].resolved)
				{
					object res = Resolve(i);
					if (res != null && res.ToString() == _operand.ToString())
						return i;
				}
				else
				{
					if (MemberList[i].operand.ToString() == _operand.ToString())
						return i;
				}
			}

			Type t = _operand.GetType();
			OperandInfoT _oit;
			if (!Enum.TryParse<OperandInfoT>(t.Name, out _oit))
			{
				Log.Write(Log.Level.Error, "Not Listed OperandType: ", t.Name);
				return -1;
			}
			int len = MemberList.Length;
			MemberList[len] = new OperandInfo() { oit = _oit, operand = _operand, Status = ResolveStatus.Resolved };
			return len;
		}

		// LOAD METHODS ******************************************************

		/// <summary>Loads the reference table from a node and itserts the unresolved
		/// references into the ILM list. Those can then be resoved on demand</summary>
		/// <param name="xNode">The reference table containing node</param>
		public void Load(XmlNode xNode)
		{
			Clear();

			foreach (XmlNode xElem in xNode.ChildNodes)
			{
				if (xElem.Name == nc[SST.PatchCluster])
					continue;
				foreach (XmlNode xItem in xElem.ChildNodes)
				{
					OperandInfo oi = new OperandInfo();
					if (xElem.Name == nc[SST.MethodReference])
						oi.oit = OperandInfoT.MethodReference;
					else if (xElem.Name == nc[SST.FieldReference])
						oi.oit = OperandInfoT.FieldReference;
					else if (xElem.Name == nc[SST.TypeReference])
						oi.oit = OperandInfoT.TypeReference;
					else // CalliSite
					{
						Log.Write(Log.Level.Careful, "Unknown Resolving Node: ", xElem.Name);
						continue;
					}
					oi.rawData = xItem as XmlElement;
					MemberList[xItem.Name.ToBaseInt()] = oi;
				}
			}
		}

		/// <summary>Looks for a reference with the given ID. If the reference is unresolved,
		/// it will try to resolve it. When no match exists it will return null.<summary>
		/// <param name="idNum">The ID to be searched.</param>
		/// <returns>The referenced object if it exists, otherwise null.</returns>
		public object Resolve(int idNum)
		{
			if (idNum >= MemberList.Length) { Log.Write(Log.Level.Error, "Resolve number ", idNum.ToString(), " is not in Range."); return null; }
			OperandInfo oi = MemberList[idNum];

			if (oi.Status == ResolveStatus.Resolved) return oi.operand;
			if (oi.Status != ResolveStatus.Unresolved) return null;

			XmlElement xDataNode = oi.rawData;
			MemberList[idNum].Status = ResolveStatus.UnkownError;

			if (oi.oit == OperandInfoT.MethodReference)
				return ResMElement(oi);
			else if (oi.oit == OperandInfoT.FieldReference)
				return ResFElement(oi);
			else if (oi.oit == OperandInfoT.TypeReference)
				return ResTElement(oi);

			Log.Write(Log.Level.Error, "Resolve element ", idNum.ToString(), " is failed init.");
			oi.Status = ResolveStatus.ReferenceNotFound;
			return null;
		}

		/// <summary>Looks for a MethodReference matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.MethodReference</summary>
		/// <param name="oi">The OperandInfo specifying the searched Method.</param>
		/// <returns>Returns the MethodReference if found, otherwise null</returns>
		private object ResMElement(OperandInfo oi)
		{
			XmlElement xDataNode = oi.rawData;

			string name = xDataNode.GetAttribute(SST.NAME);
			string type = xDataNode.GetAttribute(SST.TYPE);
			if (name == string.Empty || type == string.Empty) { oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, xDataNode.Name, " - No Name or Parenttype"); return null; }

			bool isGeneric;
			bool isGenericInstance;
			string genericvalstr;
			string[] genericValues;

			// 1] search in
			#region search_in
			TypeDefinition typdef = Resolve(type.ToBaseInt()) as TypeDefinition;
			if (typdef == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, name, "-Type couldn't be resolved"); return null; }
			#endregion

			// 2] search generics
			#region search_generics
			if (xDataNode.GetAttribute(SST.GENERICS, out genericvalstr))
			{
				genericValues = genericvalstr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				isGeneric = genericValues.Length != 0;
				if (isGeneric) isGenericInstance = !genericvalstr.Contains('%'); // TDOD: check if this is true
				else isGenericInstance = false;
			}
			else
			{
				isGeneric = false;
				isGenericInstance = false;
				genericValues = null;
			}
			#endregion

			// 3] compare
			#region compare
			foreach (MethodReference metdef in typdef.Methods)
			{
				if (metdef.Name == name)
				{
					if (isGeneric)
					{
						if (isGenericInstance)
						{
							// TODO: same as ResTElement
						}
						else
						{
							if (metdef.GenericParameters.Count != genericValues.Length) continue;
						}
					}

					Type t = metdef.GetType();
					if (!Enum.TryParse<OperandInfoT>(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, "OperandInfoType ", t.Name, " couldn't found"); }
					oi.operand = metdef;
					oi.Status = ResolveStatus.Resolved;
					return metdef;
				}
			}
			#endregion

			Log.Write(Log.Level.Error, "MethodDefinition ", name, " couldn't be found in Type ", typdef.Name);
			return null;
		}

		/// <summary>Looks for a FieldReference matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.FieldReference</summary>
		/// <param name="oi">The OperandInfo specifying the searched Field.</param>
		/// <returns>Returns the FieldReference if found, otherwise null</returns>
		private object ResFElement(OperandInfo oi)
		{
			XmlElement xDataNode = oi.rawData;

			string FieldType = xDataNode.GetAttribute(SST.TYPE);
			string Name = xDataNode.GetAttribute(SST.NAME);
			string DeclaringType = xDataNode.GetAttribute(SST.MODULE);

			if (FieldType == string.Empty || Name == string.Empty || DeclaringType == string.Empty)
			{ oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, xDataNode.Name, " - No FieldType/Name/DeclaringType"); }

			TypeReference TypDefFT = Resolve(FieldType.ToBaseInt()) as TypeReference;
			if (TypDefFT == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "FieldType not found ", FieldType); return null; }

			TypeDefinition TypDefDT = Resolve(DeclaringType.ToBaseInt()) as TypeDefinition;
			if (TypDefDT == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "DeclaringType not found ", DeclaringType); return null; }

			foreach (FieldDefinition field in TypDefDT.Fields)
			{
				if (field.Name == Name && field.FieldType.FullName == TypDefFT.FullName) // TODO: insteal of fullname, make DeepTypeCompare(Type a,Type b)
				{
					Type t = field.GetType();
					if (!Enum.TryParse<OperandInfoT>(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, "OperandInfoType ", t.Name, " was not found"); }
					oi.operand = field;
					oi.Status = ResolveStatus.Resolved;
					return field;
				}
			}

			Log.Write(Log.Level.Error, "FieldDefinition ", Name, " coundn't be found in Type ", DeclaringType);
			return null;
		}

		/// <summary>Looks for a TypeReference matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.TypeReference</summary>
		/// <param name="oi">The OperandInfo specifying the searched Type.</param>
		/// <returns>Returns the TypeReference if found, otherwise null</returns>
		private object ResTElement(OperandInfo oi)
		{
			XmlElement xDataNode = oi.rawData;

			Mono.Collections.Generic.Collection<TypeDefinition> searchCollection;

			string name = xDataNode.GetAttribute(SST.NAME);
			if (name == string.Empty) { oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, xDataNode.Name, " - No Name"); return null; }

			string namesp = string.Empty;
			// TODO enable namespace comparison, atm the algorithm will always take the first element.

			bool isGeneric;
			bool isGenericInstance;
			bool isNested;
			bool noNamespaceGiven = false;
			string genericvalstr;
			string[] genericValues;

			// 1] search in
			#region search_in
			string nestedin = xDataNode.GetAttribute(SST.NESTEDIN);
			if (nestedin == string.Empty) // type is module subtype
			{
				isNested = false;

				string module = xDataNode.GetAttribute(SST.MODULE);
				if (module == string.Empty) { oi.Status = ResolveStatus.SaveFileError; return null; }

				namesp = xDataNode.GetAttribute(SST.NAMESPACE);
				if (namesp == string.Empty)
				{
					noNamespaceGiven = true;
					Log.Write(Log.Level.Careful, "No Namespace defined! Will use first matching Item(!)");
				}

				ModuleDefinition ModDef = null;
				if (MainPanel.MainAssemblyDefinition.MainModule.Name == module)
					ModDef = MainPanel.MainAssemblyDefinition.MainModule;
				else
				{
					try
					{
						// fix if ns not found
						foreach (AssemblyNameReference anr in MainPanel.MainAssemblyDefinition.MainModule.AssemblyReferences)
							if (anr.Name == module)
							{
								AssemblyDefinition AssDef = MainPanel.MainAssemblyDefinition.MainModule.AssemblyResolver.Resolve(anr);
								ModDef = AssDef.MainModule;
								break;
							}
						if (ModDef == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "ModuleDefinition not found ", module); return null; }
					}
					catch
					{
						oi.Status = ResolveStatus.ReferenceNotFound;
						Log.Write(Log.Level.Error, "An Assembly is missing: ", module);
						return null;
					}
				}

				searchCollection = ModDef.Types;
			}
			else // type is nested
			{
				isNested = true;

				TypeDefinition nestintyp = Resolve(nestedin.ToBaseInt()) as TypeDefinition;
				if (nestintyp == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "Nestparent not found ", nestedin); return null; }

				searchCollection = nestintyp.NestedTypes;
			}
			#endregion

			// 2] search generics
			#region search_generics
			if (xDataNode.GetAttribute(SST.GENERICS, out genericvalstr))
			{
				genericValues = genericvalstr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				isGeneric = genericValues.Length != 0;
				if (isGeneric) isGenericInstance = !genericvalstr.Contains('%'); // TDOD: check if this is true
				else isGenericInstance = false;
			}
			else
			{
				isGeneric = false;
				isGenericInstance = false;
				genericValues = null;
			}
			#endregion

			// 3] compare
			#region compare
			foreach (TypeDefinition typdef in searchCollection)
			{
				// TODO: find out what !-beginning params mean

				if (typdef.Name == name && (isNested || noNamespaceGiven || typdef.Namespace == namesp))
				{
					if (isGeneric)
					{
						if (isGenericInstance)
						{
							// TODO: either find one or create new (better not new, because new will be a diferent object from the cecil existing)
							// also deref elements not starting with $ and check if they match
							// another problem is that this part will never be reached because GITs aren't stored in the searchCollection
							// think about a solution later
							continue;
						}
						else
						{
							if (typdef.GenericParameters.Count != genericValues.Length) continue;
						}
					}

					Type t = typdef.GetType();
					if (!Enum.TryParse<OperandInfoT>(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, "OperandInfoType ", t.Name, " was not found"); }
					oi.operand = typdef;
					oi.Status = ResolveStatus.Resolved;
					return typdef;
				}
			}
			#endregion

			Log.Write(Log.Level.Error, "TypeDefinition ", name, " coundn't be found in the Module");
			return null;
		}

		/// <summary>Looks for a ArrayType matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.ArrayType</summary>
		/// <param name="oi">The ArrayType specifying the searched Array.</param>
		/// <returns>Returns the ArrayType if found, otherwise null</returns>
		private object ResAElement(OperandInfo oi)
		{
			// TODO: uhm yes
			Log.Write(Log.Level.Warning, "Array deref not implemented");
			return null;
		}

		/// <summary>Looks for doubled elements in the ILM list and removes all but one (MethodStump)</summary>
		public void MergeDoubleElements()
		{
			/*
			 * Doubled elements shouldn't appear unless the Resolving Function
			 * fails to find a M/F/T which actually exists.
			 * In this case we ignore the problem, merge both entries
			 * and print a warning to get detailed info from feedback.
			 */

			//TODO
		}

		// STRUCTUREVIEW MODULES *********************************************

		/// <summary>Calls the InitTree method in a seperate Thread and waits for it to finish.
		/// This will prevent the interface from freezing.</summary>
		/// <param name="AssDef">The AssemblyDefinition which should be loaded into the searchlist</param>
		/// <param name="SubResolveDepth">When the given AssemblyDefinition uses references to other Assemblys
		/// the method will add then recursivly to the given depth</param>
		public void InitTreeHalfAsync(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			System.Threading.Thread t = new System.Threading.Thread(() => InitTree(AssDef, SubResolveDepth));
			t.Start();
			while (t.IsAlive)
			{
				System.Windows.Forms.Application.DoEvents();
				System.Threading.Thread.Sleep(1);
			}
		}

		/// <summary>Creates an ILNode-Tree representing the structure of the given Assembly
		/// and stores it in the ModuleList Dictionary with the AssemblyDefinition name as key.</summary>
		/// <param name="AssDef">The AssemblyDefinition which should be loaded into the searchlist</param>
		/// <param name="SubResolveDepth">When the given AssemblyDefinition uses references to other Assemblys
		/// the method will add then recursivly to the given depth</param>
		public void InitTree(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			if (AssDef == null) return;
			if (ModuleList.ContainsKey(AssDef.Name.Name)) return;

			ILNode ilParent = new ILNode(AssDef.Name.Name, AssDef.FullName, AssDef, StructureView.structure); // StructureView.Module
			ModuleList.Add(AssDef.Name.Name, ilParent);

			foreach (ModuleDefinition ModDef in AssDef.Modules)
			{
				ILNode tnModDef = ilParent.Add(ModDef.Name, ModDef.Name, ModDef, StructureView.structure);
				DefaultAssemblyResolver dar = ModDef.AssemblyResolver as DefaultAssemblyResolver;
				Array.ForEach(dar.GetSearchDirectories(), x => dar.RemoveSearchDirectory(x));
				dar.AddSearchDirectory(System.IO.Path.GetDirectoryName(MainPanel.AssemblyPath));

				// Subresolving references
				foreach (AssemblyNameReference anr in ModDef.AssemblyReferences)
				{
					try
					{
						AssemblyDefinition AssSubRef = ModDef.AssemblyResolver.Resolve(anr);
						tnModDef.Add(anr.Name, AssSubRef.FullName, AssSubRef, StructureView.structure);
						if (SubResolveDepth > 0)
							InitTree(AssSubRef, SubResolveDepth - 1);
					}
					catch { Log.Write(Log.Level.Warning, "AssemblyReference \"", anr.Name, "\" couldn't be found for \"", ModDef.Name, "\""); }
				}

				Dictionary<string, ILNode> nsDict = new Dictionary<string, ILNode>();
				foreach (TypeDefinition TypDef in ModDef.Types)
				{
					string nsstr = TypDef.Namespace;
					ILNode tnAssemblyContainer;
					if (!nsDict.ContainsKey(nsstr))
					{
						string displaystr = nsstr == string.Empty ? "<Default Namespace>" : nsstr;
						tnAssemblyContainer = ilParent.Add(displaystr, displaystr, new NamespaceHolder(displaystr), StructureView.namesp);
						nsDict.Add(nsstr, tnAssemblyContainer);
					}
					else
						tnAssemblyContainer = nsDict[nsstr];

					ILNode tnTypDef = tnAssemblyContainer.Add(TypDef.Name, TypDef.FullName, TypDef, StructureView.classes);
					LoadSubItemsRecursive(tnTypDef, TypDef);
				}
			}
			ilParent.Sort();
		}

		/// <summary>Traverses the Assembly recursivly and adds the new ILnodes to the given ILNode</summary>
		/// <param name="parentNode">The parent ILNode for the new subelements</param>
		/// <param name="TypDef">The TypeDefinition to read</param>
		private void LoadSubItemsRecursive(ILNode parentNode, TypeDefinition TypDef)
		{
			#region Functions
			//if (ViewElements.HasFlag(StructureView.functions))
			foreach (MethodDefinition MetDef in TypDef.Methods)
			{
				StringBuilder strb = new StringBuilder();
				StringBuilder strbfn = new StringBuilder();
				strb.Append(MetDef.Name); strbfn.Append(MetDef.Name);
				strb.Append('('); strbfn.Append('(');
				foreach (ParameterDefinition ParDef in MetDef.Parameters)
				{
					strbfn.Append(ParDef.ParameterType.FullName);
					strb.Append(ParDef.ParameterType.Name);
					strb.Append(','); strbfn.Append(',');
				}
				if (MetDef.Parameters.Count > 0)
				{
					strb.Remove(strb.Length - 1, 1); strbfn.Remove(strb.Length - 1, 1);
				}
				strb.Append(") : "); strbfn.Append(") : ");
				strbfn.Append(MetDef.ReturnType.FullName);
				strb.Append(MetDef.ReturnType.Name);
				parentNode.Add(strb.ToString(), strbfn.ToString(), MetDef, StructureView.methods);
			}
			#endregion

			#region Fields
			//if (ViewElements.HasFlag(StructureView.fields))
			foreach (FieldDefinition FieDef in TypDef.Fields)
			{
				StringBuilder strb = new StringBuilder();
				StringBuilder strbfn = new StringBuilder();
				strb.Append(FieDef.Name); strbfn.Append(FieDef.Name);
				strb.Append(" : "); strbfn.Append(" : ");

				strbfn.Append(FieDef.FieldType.FullName);
				strb.Append(FieDef.FieldType.Name);

				parentNode.Add(strb.ToString(), strbfn.ToString(), FieDef, StructureView.fields);
			}
			#endregion

			#region SubClasses
			foreach (TypeDefinition SubTypDef in TypDef.NestedTypes)
			{
				ILNode tnSubTypDef = parentNode.Add(SubTypDef.Name, SubTypDef.Name, SubTypDef, StructureView.classes);
				LoadSubItemsRecursive(tnSubTypDef, SubTypDef);
			}
			#endregion
		}

		/// <summary>Returns a collection of all loaded ILNode Assemblys</summary>
		/// <returns>Returns a ILNode Assembly collection</returns>
		public ICollection<ILNode> getAllNodes()
		{
			return ModuleList.Values;
		}

		/// <summary>Searches for the Cecil Typ/Met/Fld/... matching the seatch path in the loaded ILNode ModuleList</summary>
		/// <param name="path">A path of the form "asseblyname.namespace.class" or "-.namespace.class.method" to search all assemblys</param>
		/// <returns>Returns the Cecil object if found, otherwise null</returns>
		public object FindTypeByName(string path)
		{
			string[] pathbreaks = path.Split(new[] { '.', '/' });

			if (pathbreaks.Length == 0)
			{
				Log.Write(Log.Level.Warning, "FindTypeByName path is empty");
				return null;
			}

			if (pathbreaks[0] == "-")
			{
				foreach (ILNode child in ModuleList.Values)
				{
					object res = FindTypeByNameRecursive(child, pathbreaks, 1);
					if (res != null) return res;
				}
			}
			else if (ModuleList.ContainsKey(pathbreaks[0]))
			{
				return FindTypeByNameRecursive(ModuleList[pathbreaks[0]], pathbreaks, 1);
			}
			return null;
		}

		/// <summary>Traverses the loaded ILNode children and searches the current path index in its children</summary>
		/// <param name="searchnode">The ILNode with the children for the current path index</param>
		/// <param name="path">The array of all path parts</param>
		/// <param name="index">The current path index</param>
		/// <returns></returns>
		private object FindTypeByNameRecursive(ILNode searchnode, string[] path, int index)
		{
			if (index >= path.Length) return null;

			foreach (ILNode child in searchnode.Children)
			{
				if (child.Name == path[index])
				{
					if (index == path.Length - 1) return child.Value;
					else return FindTypeByNameRecursive(child, path, index + 1);
				}
			}
			return null;
		}

		// SUPPORT FUNCTIONS *************************************************

		/// <summary>Creates a new Instruction depending on the OpCodes' OperandType.
		/// GenInstruction will automatically use the correct Instruction.Create for each object type
		/// and tries to parse, if it is a string</summary>
		/// <param name="opc">The OpCode for the new Instruction</param>
		/// <param name="val">The operand for the new Instruction</param>
		/// <returns>Returns the new Instruction if successful, otherwise null.</returns>
		public static Instruction GenInstruction(OpCode opc, object val)
		{
			Type t = null;
			if (opc.OperandType != OperandType.InlineNone)
			{
				if (val != null)
					t = val.GetType();
				else
				{
					Log.Write(Log.Level.Error, "Operand <" + opc.Name + "> must not be null with this OpCode.OperandType: ", opc.OperandType.ToString());
					return null;
				}
			}
			switch (opc.OperandType)
			{
			case OperandType.InlineArg:
			case OperandType.ShortInlineArg:
				if (typeof(ParameterDefinition).IsAssignableFrom(t))
					return Instruction.Create(opc, (ParameterDefinition)val);
				break;
			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				if (typeof(Instruction).IsAssignableFrom(t))
					return Instruction.Create(opc, (Instruction)val);
				break;
			case OperandType.InlineField:
				if (typeof(FieldReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (FieldReference)val);
				break;
			case OperandType.InlineI:
				Int32 val_Int32;
				if (t == typeof(Int32))
					return Instruction.Create(opc, (Int32)val);
				else if (t == typeof(string) && Int32.TryParse((string)val, out val_Int32))
					return Instruction.Create(opc, val_Int32);
				break;
			case OperandType.InlineI8:
				Int64 val_Int64;
				if (t == typeof(Int64))
					return Instruction.Create(opc, (Int64)val);
				else if (t == typeof(string) && Int64.TryParse((string)val, out val_Int64))
					return Instruction.Create(opc, val_Int64);
				break;
			case OperandType.InlineMethod:
				if (typeof(MethodReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (MethodReference)val);
				break;
			case OperandType.InlineNone:
				return Instruction.Create(opc);
			case OperandType.InlineR:
				Double val_Double;
				if (t == typeof(Double))
					return Instruction.Create(opc, (Double)val);
				else if (t == typeof(string) && Double.TryParse((string)val, out val_Double))
					return Instruction.Create(opc, val_Double);
				break;
			case OperandType.InlineString:
				if (t == typeof(string))
					return Instruction.Create(opc, (string)val);
				break;
			case OperandType.InlineSwitch:
				if (t.IsAssignableFrom(typeof(Instruction[])))
					return Instruction.Create(opc, (Instruction[])val);
				break;
			case OperandType.InlineTok:
				if (typeof(TypeReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (TypeReference)val);
				else if (typeof(MethodReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (MethodReference)val);
				else if (typeof(FieldReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (FieldReference)val);
				break;
			case OperandType.InlineType:
				if (typeof(TypeReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (TypeReference)val);
				break;
			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				if (typeof(VariableDefinition).IsAssignableFrom(t))
					return Instruction.Create(opc, (VariableDefinition)val);
				break;
			case OperandType.ShortInlineI:
				if (opc == OpCodes.Ldc_I4_S)
				{
					SByte val_SByte;
					if (t == typeof(SByte))
						return Instruction.Create(opc, (SByte)val);
					else if (t == typeof(string) && SByte.TryParse((string)val, out val_SByte))
						return Instruction.Create(opc, val_SByte);
				}
				else
				{
					Byte val_Byte;
					if (t == typeof(Byte))
						return Instruction.Create(opc, (Byte)val);
					else if (t == typeof(string) && Byte.TryParse((string)val, out val_Byte))
						return Instruction.Create(opc, val_Byte);
				}
				break;
			case OperandType.ShortInlineR:
				Single val_Single;
				if (t == typeof(Single))
					return Instruction.Create(opc, (Single)val);
				else if (t == typeof(string) && Single.TryParse((string)val, out val_Single))
					return Instruction.Create(opc, val_Single);
				break;
			case OperandType.InlinePhi:
			case OperandType.InlineSig:
			default:
				if (typeof(CallSite).IsAssignableFrom(t))
					return Instruction.Create(opc, (CallSite)val);
				break;
			}

			Log.Write(Log.Level.Error, "Operand Type Could not be created. TypeName: ", t.Name, " OperandValue: ", val.ToString());
			return null;
		}

		/// <summary>Clears the current reference table</summary>
		public void Clear()
		{
			MemberList.Length = 0;
		}

		/// <summary>Clears the current reference table and the loaded ILNode Assemblys</summary>
		public void ClearAll() // check if necessary
		{
			MemberList.Length = 0;
			ModuleList.Clear();
		}
	}

	class OperandInfo
	{
		public object operand;
		public OperandInfoT oit;
		public XmlElement rawData;
		public bool resolved { get { return operand != null && Status == ResolveStatus.Resolved; } protected set { } }
		public ResolveStatus Status;
		public override string ToString()
		{
			MemberReference mref = operand as MemberReference;
			if (mref != null)
				return oit.ToString() + " " + mref.Name;
			else
				return oit.ToString() + " " + operand.ToString();
		}
	}

	enum ResolveStatus
	{
		Unresolved,
		Resolved,
		SaveFileError,
		ReferenceNotFound,
		UnkownError,
	}

	enum OperandInfoT
	{
		ParameterDefinition,
		ParameterReference, //(abstract)
		MethodDefinition,
		MethodReference,
		GenericInstanceMethod,
		FieldDefinition,
		FieldReference,
		TypeDefinition,
		TypeReference,
		GenericInstanceType,
		GenericParameter,
		VariableDefinition,
		VariableReference, //(abstract)
		CallSite,
		ArrayType,
	}

	public class ILNode
	{
		private readonly List<ILNode> _children = new List<ILNode>();

		public string Name { private set; get; }
		public string FullName { private set; get; }
		public object Value { private set; get; }
		public StructureView Flags { private set; get; }

		public ILNode(string name, string fullname, object value, StructureView flags)
		{
			Value = value;
			Name = name;
			FullName = fullname;
			Flags = flags;
		}

		public ILNode this[int i]
		{
			get { return _children[i]; }
		}

		public ILNode Parent { get; private set; }

		public ReadOnlyCollection<ILNode> Children
		{
			get { return _children.AsReadOnly(); }
		}

		public ILNode Add(string name, string fullname, object value, StructureView flags)
		{
			var node = new ILNode(name, fullname, value, flags) { Parent = this };
			_children.Add(node);
			return node;
		}

		public bool Remove(ILNode node)
		{
			return _children.Remove(node);
		}

		public override string ToString()
		{
			return FullName;
		}

		public void Sort()
		{
			_children.Sort((x, y) =>
			{
				sbyte strucdiff = x.Flags - y.Flags;
				return strucdiff != 0 ? strucdiff : x.Name.CompareTo(y.Name);
			});
			_children.ForEach(x => x.Sort());
		}
	}
}