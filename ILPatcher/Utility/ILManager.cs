using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	class ILManager
	{
		//private Dictionary<int, OperandInfo> MemberList;
		private AnyArray<OperandInfo> MemberList;
		//private int MemberCount;

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

		private static ILManager instance;
		public static ILManager Instance
		{
			get { if (instance == null) instance = new ILManager(); return instance; }
			protected set { }
		}

		private static NameCompressor nc = NameCompressor.Instance;

		public ILManager()
		{
			//MemberList = new Dictionary<int, OperandInfo>();
			MemberList = new AnyArray<OperandInfo>();
			//MemberCount = 0;
			ModuleList = new Dictionary<string, ILNode>();
		}

		// SAVE METHODS ******************************************************

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
				switch (MemberList[i].oit)
				{
					case OperandInfoT.ParameterDefinition:
					case OperandInfoT.ParameterReference:
						Log.Write(Log.Level.Warning, "PT resolving is obsolete: ", MemberList[i].ToString());
						if (oi.resolved)
							Reference(((ParameterReference)oi.operand).ParameterType);
						else
							Log.Write(Log.Level.Info, "ParameterDef/Ref cannot be raw saved");
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
					case OperandInfoT.ArrayType:
						if (oi.resolved)
							GenAChild(xT, (ArrayType)MemberList[i].operand, i);
						else
							xT.AppendClonedChild(oi.rawData);
						break;
					case OperandInfoT.CallSite:
						if (oi.resolved)
							xCS.CreateAttribute(i, ((CallSite)MemberList[i].operand).FullName);
						else
							xCS.AppendClonedChild(oi.rawData);
						break;
					default:
						Log.Write(Log.Level.Error, "Not saved Member Entry: ", MemberList[i].oit.ToString());
						break;
				}
			}
		}

		public void GenMChild(XmlElement xGroup, MethodReference mr, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);

			if (mr.IsGenericInstance)
			{
				GenericInstanceMethod git = mr as GenericInstanceMethod;
				if (git == null)
					Log.Write(Log.Level.Error, "GenericInstance Type couldn't be converted: ", git.FullName);
				else
				{
					for (int i = 0; i < git.GenericArguments.Count && git.GenericArguments[i] != null; i++)
						if (git.GenericArguments[i].IsGenericParameter)
							xElem.CreateAttribute(i, git.GenericArguments[i].Name);
						else
							xElem.CreateAttribute(i, Reference(git.GenericArguments[i]).ToBaseAlph());
				}
			}

			if (mr.HasGenericParameters)
			{
				StringBuilder strb = new StringBuilder();
				for (int i = 0; i < mr.GenericParameters.Count; i++)
				{
					strb.Append(Reference(mr.GenericParameters[i].Type).ToBaseAlph());
					strb.Append(' ');
				}
				xElem.CreateAttribute(SST.GENERICS, strb.ToString());
			}

			if (mr.ReturnType.IsGenericParameter)
				xElem.CreateAttribute(SST.RETURN, mr.ReturnType.Name);
			else
				xElem.CreateAttribute(SST.RETURN, Reference(mr.ReturnType).ToBaseAlph());
			xElem.CreateAttribute(SST.TYPE, Reference(mr.DeclaringType).ToBaseAlph());
			xElem.CreateAttribute(SST.NAME, mr.Name);

			for (int i = 0; i < mr.Parameters.Count; i++)
				if (mr.Parameters[i].ParameterType.IsGenericParameter)
					xElem.CreateAttribute(i, mr.Parameters[i].ParameterType.Name);
				else
					xElem.CreateAttribute(i, Reference(mr.Parameters[i].ParameterType).ToBaseAlph());
		}

		public void GenFChild(XmlElement xGroup, FieldReference fr, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.TYPE, Reference(fr.FieldType).ToBaseAlph());
			xElem.CreateAttribute(SST.NAME, fr.Name);
			xElem.CreateAttribute(SST.MODULE, Reference(fr.DeclaringType).ToBaseAlph());
		}

		public void GenTChild(XmlElement xGroup, TypeReference tr, int val)
		{
			XmlElement xElem = xGroup.InsertCompressedElement(val);
			xElem.CreateAttribute(SST.TYPE, tr.Name);
			StringBuilder strb = new StringBuilder();
			if (tr.IsGenericInstance)
			{
				GenericInstanceType git = tr as GenericInstanceType;
				if (git == null)
					Log.Write(Log.Level.Error, "GenericInstance Type could nt be converted: ", tr.FullName);
				else
				{
					for (int i = 0; i < git.GenericArguments.Count && git.GenericArguments[i] != null; i++)
						if (git.GenericArguments[i].IsGenericParameter)
							xElem.CreateAttribute(i, git.GenericArguments[i].Name);
						else
							xElem.CreateAttribute(i, Reference(git.GenericArguments[i]).ToBaseAlph());
				}
			}
			if (tr.IsNested)
				xElem.CreateAttribute(SST.NESTEDIN, Reference(tr.DeclaringType).ToBaseAlph());
			else
			{
				xElem.CreateAttribute(SST.MODULE, tr.Module.Name);
				if (!tr.IsGenericParameter)
					xElem.CreateAttribute(SST.NAMESPACE, tr.Namespace);
			}
		}

		public void GenAChild(XmlElement xGroup, ArrayType at, int val)
		{
			XmlElement xElem = xGroup.OwnerDocument.CreateElement(val.ToBaseAlph());
			xElem.CreateAttribute(SST.TYPE, Reference(at.ElementType).ToBaseAlph());
			if (at.IsVector)
				xElem.CreateAttribute(SST.ARRAY, "0");
			else
				xElem.CreateAttribute(SST.ARRAY, at.Dimensions.Count.ToString());
		}

		public int Reference(object _operand)
		{
			// GenericInstanceMethod
			/*if (_operand.ToString().Contains("!0"))
			{
				Console.WriteLine("blub");
			}*/
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


			//foreach (KeyValuePair<int, OperandInfo> entry in MemberList)

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

		public void Load(XmlNode xNode)
		{
			Clear();

			foreach (XmlNode xElem in xNode.ChildNodes)
			{
				//skip kown nodes
				foreach (XmlNode xItem in xElem.ChildNodes)
				{
					OperandInfo oi = new OperandInfo();
					if (xElem.Name == "MethodReference")
						oi.oit = OperandInfoT.MethodReference;
					else if (xElem.Name == "FieldReference")
						oi.oit = OperandInfoT.FieldReference;
					else if (xElem.Name == "TypeReference")
						oi.oit = OperandInfoT.TypeReference;
					else // CalliSite
					{
						//Log.Write("Unknown Resolving Node: ", xElem.Name);
						continue;
					}
					oi.rawData = xItem as XmlElement;
					MemberList[xItem.Name.ToBaseInt()] = oi;
				}
			}
		}

		public object Resolve(int idNum)
		{
			if (idNum >= MemberList.Length) { Log.Write(Log.Level.Error, "Resolve number ", idNum.ToString(), " is not in Range."); return null; }
			OperandInfo oi = MemberList[idNum];

			if (oi.Status == ResolveStatus.Resolved) return oi.operand;
			if (oi.Status != ResolveStatus.Unresolved) return null;

			XmlElement xDataNode = oi.rawData;
			MemberList[idNum].Status = ResolveStatus.UnkownError;

			// TODO get generic parameters, written in ToBaseAlph() = (a, b, c...)

			#region MetRef
			if (oi.oit == OperandInfoT.MethodReference)
			{
				//generics here

				//generic return
				//beginswith !

				string name = xDataNode.GetAttribute(SST.NAME);
				string type = xDataNode.GetAttribute(SST.TYPE);

				if (name == string.Empty || type == string.Empty) { oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, xDataNode.Name, " - No Name or Parenttype"); return null; }

				TypeDefinition typdef = Resolve(type.ToBaseInt()) as TypeDefinition;
				if (typdef == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, name, "-Type coudn't be resolved"); return null; }

				foreach (MethodReference metdef in typdef.Methods)
				{
					if (metdef.Name == name)
					{
						Type t = metdef.GetType();
						if (!Enum.TryParse<OperandInfoT>(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, "OperandInfoType ", t.Name, " was not found"); }
						oi.operand = metdef;
						oi.Status = ResolveStatus.Resolved;
						return metdef;
					}
				}

				Log.Write(Log.Level.Error, "MethodDefinition ", name, " coundn't be found in Type ", typdef.Name);
				return null;
			}
			#endregion
			#region FldRef
			else if (oi.oit == OperandInfoT.FieldReference)
			{
				string FieldType = xDataNode.GetAttribute(SST.TYPE);
				string Name = xDataNode.GetAttribute(SST.NAME);
				string DeclaringType = xDataNode.GetAttribute(SST.TYPE);

				if (FieldType == string.Empty || Name == string.Empty || DeclaringType == string.Empty)
				{ oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, xDataNode.Name, " - No FieldType/Name/DeclaringType"); }

				TypeReference TypDefFT = Resolve(FieldType.ToBaseInt()) as TypeReference;
				if (TypDefFT == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "FieldType not found ", FieldType); return null; }

				TypeDefinition TypDefDT = Resolve(DeclaringType.ToBaseInt()) as TypeDefinition;
				if (TypDefDT == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "DeclaringType not found ", DeclaringType); return null; }

				foreach (FieldDefinition field in TypDefDT.Fields)
				{
					if (field.Name == Name && field.FieldType == TypDefFT)
					{
						Type t = field.GetType();
						if (!Enum.TryParse<OperandInfoT>(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, "OperandInfoType ", t.Name, " was not found"); }
						oi.operand = field;
						oi.Status = ResolveStatus.Resolved;
						return field;
					}
				}
				Log.Write(Log.Level.Error, "FieldDefinition ", Name, " coundn't be found in Type ", DeclaringType);
			}
			#endregion
			#region TypRef
			else if (oi.oit == OperandInfoT.TypeReference)
			{
				string namesp = xDataNode.GetAttribute(SST.NAMESPACE);
				if (namesp == string.Empty) Log.Write(Log.Level.Careful, "No Namespace defined! Will use first matching Item(!)");

				/*List<string> values = new List<string>();
				bool attok = true;
				int attcnt = 0;
				while (attok)
				{
					namesp = xDataNode.GetAttribute(attcnt.ToBaseAlph());
					//xNSAtt = xDataNode.Attributes[];
					if (namesp != string.Empty)
					{
						values.Add(namesp);
						attcnt++;
					}
					else
						attok = false;
				}*/

				string module = xDataNode.GetAttribute(SST.MODULE);
				if (module == string.Empty) { MemberList[idNum].Status = ResolveStatus.SaveFileError; return null; }
				//values.Add(module);

				ModuleDefinition ModDef = null;
				if (MainPanel.AssemblyDef.MainModule.Name == module)
					ModDef = MainPanel.AssemblyDef.MainModule;
				else
				{
					AssemblyDefinition AssDef = MainPanel.AssemblyDef.MainModule.AssemblyResolver.Resolve(namesp); // fix if ns not found
					foreach (ModuleDefinition moddef in AssDef.Modules)
						if (moddef.Name == module)
						{
							ModDef = moddef;
							break;
						}
				}
				if (ModDef == null) { oi.Status = ResolveStatus.ReferenceNotFound; Log.Write(Log.Level.Error, "ModuleDefinition not found ", module); return null; }

				string name = xDataNode.GetAttribute(SST.TYPE); // change to name
				if (name == string.Empty) { oi.Status = ResolveStatus.SaveFileError; Log.Write(Log.Level.Error, xDataNode.Name, " - No Name"); return null; }

				foreach (TypeDefinition typdef in ModDef.Types)
				{
					if (typdef.Name == name)
					{
						Type t = typdef.GetType();
						if (!Enum.TryParse<OperandInfoT>(t.Name, out oi.oit)) { Log.Write(Log.Level.Warning, "OperandInfoType ", t.Name, " was not found"); }
						oi.operand = typdef;
						oi.Status = ResolveStatus.Resolved;
						return typdef;
					}
				}
				Log.Write(Log.Level.Error, "TypeDefinition ", name, " coundn't be found in Module ", module);
			}
			#endregion

			oi.Status = ResolveStatus.ReferenceNotFound;
			return null;
		}

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

		// LOAD MODULES ******************************************************

		public void InitTreeHalfAsync(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			System.Threading.Thread t = new System.Threading.Thread(() => InitTree(AssDef, SubResolveDepth));
			t.Start();
			while (t.IsAlive)
			{
				Application.DoEvents();
				System.Threading.Thread.Sleep(1);
			}
		}

		public void InitTree(AssemblyDefinition AssDef, int SubResolveDepth = 0)
		{
			if (AssDef == null) return;
			if (ModuleList.ContainsKey(AssDef.Name.Name)) return;

			ILNode ilParent = new ILNode(AssDef.Name.Name, AssDef.FullName, AssDef, StructureView.none); // StructureView.Module
			ModuleList.Add(AssDef.Name.Name, ilParent);

			foreach (ModuleDefinition ModDef in AssDef.Modules)
			{
				ILNode tnModDef = ilParent.Add(ModDef.Name, ModDef.Name, ModDef, StructureView.none);
				DefaultAssemblyResolver dar = ModDef.AssemblyResolver as DefaultAssemblyResolver;
				Array.ForEach(dar.GetSearchDirectories(), x => dar.RemoveSearchDirectory(x));
				dar.AddSearchDirectory(System.IO.Path.GetDirectoryName(MainPanel.AssemblyPath));


				if (SubResolveDepth > 0) // Subresolving references
					foreach (AssemblyNameReference anr in ModDef.AssemblyReferences)
					{
						AssemblyDefinition AssSubRef = ModDef.AssemblyResolver.Resolve(anr);
						InitTree(AssSubRef, SubResolveDepth - 1);
					}

				foreach (TypeDefinition TypDef in ModDef.Types)
				{
					ILNode tnTypDef = tnModDef.Add(TypDef.Name, TypDef.FullName, TypDef, StructureView.classes);
					LoadSubItemsRecursive(tnTypDef, TypDef);
				}
			}
		}

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
				parentNode.Add(strb.ToString(), strbfn.ToString(), MetDef, StructureView.functions);
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

		public ICollection<ILNode> getAllNodes()
		{
			return ModuleList.Values;
		}

		// SUPPORT FUNCTIONS *************************************************

		public static Instruction GenInstructionObsolete(OpCode opc, object val)
		{
			Instruction retIntr = null;
			//OperandType
			//retIntr.OpCode.OperandType.
			Type t = null;
			if (val != null)
				t = val.GetType();

			//dummy table for all stuff like
			//to-be-added before instructions for jumps
			//to-be-created local var
			Instruction dummy = Instruction.Create(OpCodes.Nop);

			switch (opc.OperandType)
			{
				case OperandType.InlineArg: // (uint16) Argument/Parameter
					//TODO String/Int interpreter
					if (t.IsAssignableFrom(typeof(ParameterDefinition)))
						retIntr = Instruction.Create(opc, (ParameterDefinition)val);
					break;
				case OperandType.InlineBrTarget: // (int32) Instruction target
					//TODO String/Int interpreter
					if (t == typeof(string))
						retIntr = Instruction.Create(opc, dummy); // TODO add to dummylist
					else if (t == typeof(Int32))
						retIntr = Instruction.Create(opc, dummy); // TODO add to dummylist
					else if (t.IsAssignableFrom(typeof(Instruction)))
						retIntr = Instruction.Create(opc, (Instruction)val);
					break;
				case OperandType.InlineField: // Field 
					if (t.IsAssignableFrom(typeof(FieldReference)))
						retIntr = Instruction.Create(opc, (FieldReference)val);
					break;
				case OperandType.InlineI: //Int32
					if (t == typeof(string))
						retIntr = Instruction.Create(opc, Int32.Parse((string)val));
					else if (t == typeof(Int32))
						retIntr = Instruction.Create(opc, (Int32)val);
					break;
				case OperandType.InlineI8: //Int64
					if (t.IsAssignableFrom(typeof(string)))
						retIntr = Instruction.Create(opc, Int64.Parse((string)val));
					else if (t.IsAssignableFrom(typeof(Int64)))
						retIntr = Instruction.Create(opc, (Int64)val);
					break;
				case OperandType.InlineMethod: // Methode
					if (t.IsAssignableFrom(typeof(MethodReference)))
						retIntr = Instruction.Create(opc, (MethodReference)val);
					break;
				case OperandType.InlineNone: // None
					retIntr = Instruction.Create(opc);
					break;
				case OperandType.InlinePhi: // ----
					break;
				case OperandType.InlineR: // Double
					if (t.IsAssignableFrom(typeof(string)))
						retIntr = Instruction.Create(opc, double.Parse((string)val));
					else if (t.IsAssignableFrom(typeof(double)))
						retIntr = Instruction.Create(opc, (double)val);
					break;
				case OperandType.InlineSig:
					if (t.IsAssignableFrom(typeof(CallSite)))
						retIntr = Instruction.Create(opc, (CallSite)val);
					break;
				case OperandType.InlineString: // String
					if (t.IsAssignableFrom(typeof(string)))
						retIntr = Instruction.Create(opc, (string)val);
					break;
				case OperandType.InlineSwitch: // Instruction[]
					if (t == typeof(Instruction[]))
						retIntr = Instruction.Create(opc, (Instruction[])val);
					break;
				case OperandType.InlineTok: // ???
					Log.Write(Log.Level.Careful, "Fuck, a Token! I have no idea what to do with dat...");
					break;
				case OperandType.InlineType: // Type
					if (t.IsAssignableFrom(typeof(TypeReference)))
						retIntr = Instruction.Create(opc, (TypeReference)val);
					break;
				case OperandType.InlineVar: // (uint16) Local Variable
					//TODO String/Int interpreter
					if (t.IsAssignableFrom(typeof(VariableDefinition)))
						retIntr = Instruction.Create(opc, (VariableDefinition)val);
					break;
				case OperandType.ShortInlineArg: // (uint8) Argument/Parameter
					//TODO String/Int interpreter
					if (t.IsAssignableFrom(typeof(ParameterDefinition)))
						retIntr = Instruction.Create(opc, (ParameterDefinition)val);
					break;
				case OperandType.ShortInlineBrTarget:  // (int8) Instruction target
					if (t.IsAssignableFrom(typeof(string)))
						retIntr = Instruction.Create(opc, dummy); // TODO add to dummylist
					else if (t.IsAssignableFrom(typeof(Int32)))
						retIntr = Instruction.Create(opc, dummy); // TODO add to dummylist
					else if (t.IsAssignableFrom(typeof(Instruction)))
						retIntr = Instruction.Create(opc, (Instruction)val);
					break;
				case OperandType.ShortInlineI: // SByte
					if (t.IsAssignableFrom(typeof(string)))
						retIntr = Instruction.Create(opc, sbyte.Parse((string)val));
					else if (t.IsAssignableFrom(typeof(sbyte)))
						retIntr = Instruction.Create(opc, (sbyte)val);
					break;
				case OperandType.ShortInlineR: // Float
					if (t.IsAssignableFrom(typeof(string)))
						retIntr = Instruction.Create(opc, float.Parse((string)val));
					else if (t.IsAssignableFrom(typeof(sbyte)))
						retIntr = Instruction.Create(opc, (float)val);
					break;
				case OperandType.ShortInlineVar:// (uint8) Local Variable
					//TODO String/Int interpreter
					if (t.IsAssignableFrom(typeof(VariableDefinition)))
						retIntr = Instruction.Create(opc, (VariableDefinition)val);
					break;
				default:
					break;
			}
			return retIntr;
		}

		public static Instruction GenInstruction(OpCode opc, object val)
		{
			if (opc.OperandType == OperandType.InlineNone)
				return Instruction.Create(opc);
			else
			{
				Type t = val.GetType();
				if (typeof(TypeReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (TypeReference)val);
				else if (typeof(CallSite).IsAssignableFrom(t))
					return Instruction.Create(opc, (CallSite)val);
				else if (typeof(MethodReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (MethodReference)val);
				else if (typeof(FieldReference).IsAssignableFrom(t))
					return Instruction.Create(opc, (FieldReference)val);
				else if (t == typeof(string))
					return Instruction.Create(opc, (string)val);
				else if (t == typeof(sbyte))
					return Instruction.Create(opc, (sbyte)val);
				else if (t == typeof(byte))
					return Instruction.Create(opc, (byte)val);
				else if (t == typeof(int))
					return Instruction.Create(opc, (int)val);
				else if (t == typeof(long))
					return Instruction.Create(opc, (long)val);
				else if (t == typeof(float))
					return Instruction.Create(opc, (float)val);
				else if (t == typeof(double))
					return Instruction.Create(opc, (double)val);
				else if (typeof(Instruction).IsAssignableFrom(t))
					return Instruction.Create(opc, (Instruction)val);
				else if (t.IsAssignableFrom(typeof(Instruction[])))
					return Instruction.Create(opc, (Instruction[])val);
				else if (typeof(VariableDefinition).IsAssignableFrom(t))
					return Instruction.Create(opc, (VariableDefinition)val);
				else if (typeof(ParameterDefinition).IsAssignableFrom(t))
					return Instruction.Create(opc, (ParameterDefinition)val);
				else
				{
					Log.Write(Log.Level.Error, "Operand Type Could not be cloned. TypeName: ", t.Name, " OperandValue: ", val.ToString());
					return null;
				}
			}
		}

		public void Clear()
		{
			MemberList.Length = 0;
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
		ParameterReference,
		MethodDefinition,
		MethodReference,
		GenericInstanceMethod,
		FieldDefinition,
		FieldReference,
		TypeDefinition,
		TypeReference,
		GenericInstanceType,
		GenericParameter,
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
	}
}