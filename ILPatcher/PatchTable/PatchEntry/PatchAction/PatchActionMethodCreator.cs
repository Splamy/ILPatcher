﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILPatcher
{
	public class PatchActionMethodCreator : PatchAction
	{
		public override PatchActionType PatchActionType { get { return PatchActionType.ILMethodCreator; } protected set { } }
		private PatchStatus _PatchStatus = PatchStatus.Unset;
		public override PatchStatus PatchStatus { get { return _PatchStatus; } protected set { _PatchStatus = value; } }

		private MethodDefinition NMetDef;
		private TypeDefinition TpyDef;

		public override bool Execute()
		{
			throw new NotImplementedException();
		}

		public override bool Load(System.Xml.XmlNode input)
		{
			throw new NotImplementedException();
		}

		public override bool Save(System.Xml.XmlNode output)
		{
			throw new NotImplementedException();
		}
	}
}
