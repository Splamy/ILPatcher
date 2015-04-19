﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILPatcher
{
	public partial class EditorCluster : UserControl
	{
		private PatchCluster patchcluster;
		public PatchCluster patchCluster
		{
			get
			{
				if (patchcluster == null)
					patchcluster = new PatchCluster();
				patchcluster.ClusterName = txtPatchClusterName.Text;
				return patchcluster;
			}
			set { patchcluster = value; }
		}

		Action<PatchCluster> callbackAdd;

		public EditorCluster(Action<PatchCluster> _cbAdd)
		{
			InitializeComponent();

			callbackAdd = _cbAdd;
		}

		private void btnILMethodFixed_Click(object sender, EventArgs e)
		{
			((SwooshPanel)Parent).PushPanel(new EditorILPattern(Add), "PatchAction: ILMethodFixed");
		}

		private void btnMethodCreator_Click(object sender, EventArgs e)
		{
			((SwooshPanel)Parent).PushPanel(new EditorMethodCreator(Add), "PatchAction: MethodCreator");
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			callbackAdd(patchCluster);
			((SwooshPanel)Parent).SwooshBack();
		}

		public void Add(PatchAction pa)
		{
			patchCluster.Add(pa);
			LoadCluster(patchCluster);
		}

		public void LoadCluster(PatchCluster loadpe)
		{
			mClusterList.ClearItems();
			patchCluster = loadpe;
			if (loadpe == null)
			{
				txtPatchClusterName.Text = "DafaultName_" + new Random().Next(1000);
			}
			else
			{
				txtPatchClusterName.Text = loadpe.ClusterName;
				foreach (PatchAction pa in loadpe.ActionList)
					mClusterList.AddItem(new PatchActionItem(pa));
			}
			mClusterList.InvalidateBuffer();
		}

		private void EditorCluster_Resize(object sender, EventArgs e)
		{
			const int space = 5;
			const int labelspace = 100 + 2 * space;

			txtPatchClusterName.Width = Width - (labelspace + space);
			mClusterList.Width = Width - (labelspace + space);
			mClusterList.Height = Height - (mClusterList.Top + space);

			btnOK.Top = Height - (btnOK.Height + space);
		}

		private void btnEditPatchAction_Click(object sender, EventArgs e)
		{
			if (mClusterList.SelectedIndex >= 0)
			{
				PatchAction pa = patchCluster.ActionList[mClusterList.SelectedIndex];
				switch (pa.PatchActionType)
				{
				case PatchActionType.ILMethodFixed:
					EditorILPattern eilp = new EditorILPattern(Add);
					eilp.LoadPatchAction((PatchActionILMethodFixed)pa);
					((SwooshPanel)Parent).PushPanel(eilp, "PatchAction: ILMethodFixed");
					break;
				case PatchActionType.ILMethodDynamic:
					Log.Write(Log.Level.Info, "ILMethodDynamic not implemented");
					return;
				case PatchActionType.ILDynamicScan:
					Log.Write(Log.Level.Info, "ILDynamicScan not implemented");
					return;
				case PatchActionType.AoBRawScan:
					Log.Write(Log.Level.Info, "AoBRawScan not implemented");
					return;
				default:
					return;
				}
			}
		}

		private void ReloadTable()
		{
			LoadCluster(patchCluster);
		}


	}
}