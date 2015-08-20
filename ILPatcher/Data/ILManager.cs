using ILPatcher.Utility;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ILPatcher.Data
{
	public class ILManager : ISaveToFile
	{
		private DataStruct dataStruct;
		private static NameCompressor nc = NameCompressor.Instance;

		private AnyArray<OperandInfo> MemberList;

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
		}

		public ILManager(DataStruct dataStruct)
		{
			MemberList = new AnyArray<OperandInfo>();
			this.dataStruct = dataStruct;
		}

		// SAVE METHODS ******************************************************

		/// <summary>Saves the current reference list of this ILM into a xml parent node.
		/// If an entry is not resolved it will copy the raw data read back again.</summary>
		/// <param name="xNode">The parent node for the new reference nodes</param>
		public bool Save(XmlNode xNode)
		{
			XmlNode xM = xNode.InsertCompressedElement(SST.MethodReference);
			XmlNode xF = xNode.InsertCompressedElement(SST.FieldReference);
			XmlNode xT = xNode.InsertCompressedElement(SST.TypeReference);
			XmlNode xCS = xNode.InsertCompressedElement(SST.CallSite);

			bool allOk = true;

			for (int i = 0; i < MemberList.Length; i++)
			{
				OperandInfo oi = MemberList[i];
				switch (oi.oit)
				{
				case OperandInfoT.ParameterDefinition:
				case OperandInfoT.ParameterReference:
					Log.Write(Log.Level.Warning, $"PT resolving is obsolete: {oi}");
					//todo reenable
					if (oi.resolved)
						Reference(((ParameterReference)oi.operand).ParameterType);
					else
					{
						Log.Write(Log.Level.Error, "ParameterDef/Ref cannot be raw saved");
						allOk = false;
					}
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
					Log.Write(Log.Level.Warning, $"VD resolving is obsolete: {oi}");
					//todo reenable
					if (oi.resolved)
						Reference(((VariableReference)oi.operand).VariableType);
					else
					{
						Log.Write(Log.Level.Info, "VariableDef/Ref cannot be raw saved");
						allOk = false;
					}
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
						XmlNode xElem = xCS.InsertCompressedElement(i);
						xElem.CreateAttribute(SST.Name, ((CallSite)MemberList[i].operand).FullName);
						Log.Write(Log.Level.Error, $"Unhandled CallSite: {oi}");
						allOk = false;
					}
					else
						xCS.AppendClonedChild(oi.rawData);
					break;
				default:
					Log.Write(Log.Level.Error, $"Not saved Member Entry: {oi}");
					allOk = false;
					break;
				}
			}

			MergeDoubleElements();

			return allOk;
		}

		/// <summary>Inserts the xml formatted reference for a MethodReference into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="mr">The MethodReference which will be xml formatted</param>
		/// <param name="val">The ID of the MethodReference in the ILM list</param>
		private void GenMChild(XmlNode xGroup, MethodReference mr, int val)
		{
			XmlNode xElem = xGroup.InsertCompressedElement(val);
			StringBuilder strb = new StringBuilder();

			xElem.CreateAttribute(SST.Type, Reference(mr.DeclaringType).ToBaseAlph());
			xElem.CreateAttribute(SST.Name, mr.Name);

			#region GENERICS
			if (mr.IsGenericInstance)
			{
				GenericInstanceMethod git = mr as GenericInstanceMethod;
				if (git == null)
					Log.Write(Log.Level.Error, $"GenericInstance Type \"{git.FullName}\" couldn't be converted.");
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
					xElem.CreateAttribute(SST.Generics, strb.ToString());
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
				xElem.CreateAttribute(SST.Generics, strb.ToString());
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
			xElem.CreateAttribute(SST.Return, strb.ToString());
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
				xElem.CreateAttribute(SST.Parameter, strb.ToString());
			}
			#endregion
		}

		/// <summary>Inserts the xml formatted reference for a FieldReference into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="fr">The FieldReference which will be xml formatted</param>
		/// <param name="val">The ID of the FieldReference in the ILM list</param>
		private void GenFChild(XmlNode xGroup, FieldReference fr, int val)
		{
			XmlNode xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.Type, Reference(fr.FieldType).ToBaseAlph());
			xElem.CreateAttribute(SST.Name, fr.Name);
			xElem.CreateAttribute(SST.Module, Reference(fr.DeclaringType).ToBaseAlph());
		}

		/// <summary>Inserts the xml formatted reference for a TypeReference into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="tr">The TypeReference which will be xml formatted</param>
		/// <param name="val">The ID of the TypeReference in the ILM list</param>
		private void GenTChild(XmlNode xGroup, TypeReference tr, int val)
		{
			XmlNode xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.Name, tr.Name);

			StringBuilder strb = new StringBuilder();
			if (tr.IsGenericInstance)
			{
				// This is a instanced form of a Type, it can still have unspecified (generic) Parameters
				// but also specified Parameters like a int32
				if (tr.HasGenericParameters) // debug
					Console.WriteLine("Woah");
				GenericInstanceType git = tr as GenericInstanceType;
				if (git == null)
					Log.Write(Log.Level.Error, $"GenericInstance Type \"{tr.FullName}\" couldn't be converted.");
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
					xElem.CreateAttribute(SST.Generics, strb.ToString());
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
				xElem.CreateAttribute(SST.Generics, strb.ToString());
			}

			if (tr.IsNested)
				xElem.CreateAttribute(SST.NestedIn, Reference(tr.DeclaringType).ToBaseAlph());
			else
			{
				xElem.CreateAttribute(SST.Module, tr.Scope.Name);
				if (!tr.IsGenericParameter)
					xElem.CreateAttribute(SST.Namespace, tr.Namespace);
				else
					Log.Write(Log.Level.Error, "A GenericParameter(Type) has been passed. It will be dismissed.");
			}
		}

		/// <summary>Inserts the xml formatted reference for an ArrayType into a xml parent</summary>
		/// <param name="xGroup">The xml parent for the new generated element</param>
		/// <param name="at">The ArrayType which will be xml formatted</param>
		/// <param name="val">The ID of the ArrayType in the ILM list</param>
		private void GenAChild(XmlNode xGroup, ArrayType at, int val)
		{
			XmlNode xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.Type, Reference(at.ElementType).ToBaseAlph());
			if (at.IsVector)
				xElem.CreateAttribute(SST.Array, "0");
			else
				xElem.CreateAttribute(SST.Array, at.Dimensions.Count.ToString());
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
				Log.Write(Log.Level.Error, $"Not Listed OperandType: {t.Name}");
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
		public bool Load(XmlNode xNode)
		{
			Clear();

			bool allOk = true;

			foreach (XmlNode xElem in xNode.ChildNodes)
			{
				// TODO: rework load scheme
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
						allOk = false;
						Log.Write(Log.Level.Careful, $"Unknown Resolving Node: {xElem.Name}");
						continue;
					}
					oi.rawData = xItem;
					MemberList[xItem.Name.ToBaseInt()] = oi;
				}
			}
			return allOk;
		}

		/// <summary>Looks for a reference with the given ID. If the reference is unresolved,
		/// it will try to resolve it. When no match exists it will return null.<summary>
		/// <param name="idNum">The ID to be searched.</param>
		/// <returns>The referenced object if it exists, otherwise null.</returns>
		public object Resolve(int idNum)
		{
			if (idNum >= MemberList.Length) { Log.Write(Log.Level.Error, $"Resolve number {idNum} is not in Range."); return null; }
			OperandInfo oi = MemberList[idNum];

			if (oi.Status == ResolveStatus.Resolved) return oi.operand;
			if (oi.Status != ResolveStatus.Unresolved) return null;

			MemberList[idNum].Status = ResolveStatus.UnkownError;

			if (oi.oit == OperandInfoT.MethodReference)
				return ResMElement(oi);
			else if (oi.oit == OperandInfoT.FieldReference)
				return ResFElement(oi);
			else if (oi.oit == OperandInfoT.TypeReference)
				return ResTElement(oi);

			Log.Write(Log.Level.Error, $"Resolve element {idNum} is failed init.");
			oi.Status = ResolveStatus.ReferenceNotFound;
			return null;
		}

		/// <summary>Looks for a MethodReference matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.MethodReference</summary>
		/// <param name="oi">The OperandInfo specifying the searched Method.</param>
		/// <returns>Returns the MethodReference if found, otherwise null</returns>
		private object ResMElement(OperandInfo oi)
		{
			XmlNode xDataNode = oi.rawData;

			string name = xDataNode.GetAttribute(SST.Name);
			string type = xDataNode.GetAttribute(SST.Type);
			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
			{
				oi.Status = ResolveStatus.SaveFileError;
				Log.Write(Log.Level.Error, $"\"{xDataNode.Name}\" has no Name or Parenttype");
				return null;
			}

			bool isGeneric;
			bool isGenericInstance;
			string genericvalstr;
			string[] genericValues;

			// 1] search in
			#region search_in
			TypeDefinition typdef = Resolve(type.ToBaseInt()) as TypeDefinition;
			if (typdef == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, $"Type \"{name}\" couldn't be resolved"); return null; }
			#endregion

			// 2] search generics
			#region search_generics
			if (xDataNode.GetAttribute(SST.Generics, out genericvalstr))
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
					if (!Enum.TryParse(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, $"OperandInfoType \"{t.Name}\" not found"); }
					oi.operand = metdef;
					oi.Status = ResolveStatus.Resolved;
					return metdef;
				}
			}
			#endregion

			Log.Write(Log.Level.Error, $"MethodDefinition \"{name}\" couldn't be found in Type \"{typdef.Name}\"");
			return null;
		}

		/// <summary>Looks for a FieldReference matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.FieldReference</summary>
		/// <param name="oi">The OperandInfo specifying the searched Field.</param>
		/// <returns>Returns the FieldReference if found, otherwise null</returns>
		private object ResFElement(OperandInfo oi)
		{
			XmlNode xDataNode = oi.rawData;

			string FieldType = xDataNode.GetAttribute(SST.Type);
			string Name = xDataNode.GetAttribute(SST.Name);
			string DeclaringType = xDataNode.GetAttribute(SST.Module);

			if (string.IsNullOrEmpty(FieldType) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(DeclaringType))
			{ oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, $"\"{xDataNode.Name}\" has no FieldType/Name/DeclaringType"); }

			TypeReference TypDefFT = Resolve(FieldType.ToBaseInt()) as TypeReference;
			if (TypDefFT == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, $"FieldType \"{FieldType}\" not found"); return null; }

			TypeDefinition TypDefDT = Resolve(DeclaringType.ToBaseInt()) as TypeDefinition;
			if (TypDefDT == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, $"DeclaringType \"{DeclaringType}\" not found "); return null; }

			foreach (FieldDefinition field in TypDefDT.Fields)
			{
				if (field.Name == Name && field.FieldType.FullName == TypDefFT.FullName) // TODO: instead of fullname, make DeepTypeCompare(Type a,Type b) | hint: use cecilhelper
				{
					Type t = field.GetType();
					if (!Enum.TryParse(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, $"OperandInfoType \"{t.Name}\" was not found"); }
					oi.operand = field;
					oi.Status = ResolveStatus.Resolved;
					return field;
				}
			}

			Log.Write(Log.Level.Error, $"FieldDefinition \"{Name}\" coundn't be found in Type \"{DeclaringType}\"");
			return null;
		}

		/// <summary>Looks for a TypeReference matching the given OperandInfo.
		/// OperandInfo.oit must be OperandInfoT.TypeReference</summary>
		/// <param name="oi">The OperandInfo specifying the searched Type.</param>
		/// <returns>Returns the TypeReference if found, otherwise null</returns>
		private object ResTElement(OperandInfo oi)
		{
			XmlNode xDataNode = oi.rawData;

			Mono.Collections.Generic.Collection<TypeDefinition> searchCollection;

			string name = xDataNode.GetAttribute(SST.Name);
			if (string.IsNullOrEmpty(name)) { oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, $"\"{xDataNode.Name}\" has No Name"); return null; }

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
			string nestedin = xDataNode.GetAttribute(SST.NestedIn);
			if (string.IsNullOrEmpty(nestedin)) // type is module subtype
			{
				isNested = false;

				string module = xDataNode.GetAttribute(SST.Module);
				if (string.IsNullOrEmpty(module)) { oi.Status = ResolveStatus.SaveFileError; return null; }

				namesp = xDataNode.GetAttribute(SST.Namespace);
				if (string.IsNullOrEmpty(namesp))
				{
					noNamespaceGiven = true;
					Log.Write(Log.Level.Careful, "No Namespace defined! Will use first matching Item(!)");
				}

				ModuleDefinition ModDef = null;
				if (dataStruct.AssemblyDefinition.MainModule.Name == module)
					ModDef = dataStruct.AssemblyDefinition.MainModule;
				else
				{
					try
					{
						// fix if ns not found
						foreach (AssemblyNameReference anr in dataStruct.AssemblyDefinition.MainModule.AssemblyReferences)
							if (anr.Name == module)
							{
								AssemblyDefinition AssDef = dataStruct.AssemblyDefinition.MainModule.AssemblyResolver.Resolve(anr);
								ModDef = AssDef.MainModule;
								break;
							}
						if (ModDef == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, $"ModuleDefinition \"{module}\" not found"); return null; }
					}
					catch
					{
						oi.Status = ResolveStatus.ReferenceNotFound;
						Log.Write(Log.Level.Error, $"Assembly \"{module}\" is missing");
						return null;
					}
				}

				searchCollection = ModDef.Types;
			}
			else // type is nested
			{
				isNested = true;

				TypeDefinition nestintyp = Resolve(nestedin.ToBaseInt()) as TypeDefinition;
				if (nestintyp == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, $"Nestparent \"{nestedin}\" not found"); return null; }

				searchCollection = nestintyp.NestedTypes;
			}
			#endregion

			// 2] search generics
			#region search_generics
			if (xDataNode.GetAttribute(SST.Generics, out genericvalstr))
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
					if (!Enum.TryParse(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, $"OperandInfoType \"{t.Name}\" was not found"); }
					oi.operand = typdef;
					oi.Status = ResolveStatus.Resolved;
					return typdef;
				}
			}
			#endregion

			Log.Write(Log.Level.Error, $"TypeDefinition \"{name}\" coundn't be found in the Module");
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
					Log.Write(Log.Level.Error, $"Operand \"{opc.Name}\" must not be null with OpCode.OperandType \"{opc.OperandType}\"");
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

			Log.Write(Log.Level.Error, $"Operand Type \"{t.Name}\" with OperandValue \"{val}\" could not be created");
			return null;
		}

		/// <summary>Clears the current reference table</summary>
		public void Clear()
		{
			MemberList.Clear();
		}
	}

	class OperandInfo
	{
		public object operand;
		public OperandInfoT oit;
		public XmlNode rawData;
		public bool resolved { get { return operand != null && Status == ResolveStatus.Resolved; } }
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
}