﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionItemNode : CustomFolderNode
	{
		ISolution     solution;
		ISolutionFileItem item;
		
		public ISolutionFileItem SolutionItem {
			get {
				return item;
			}
		}
		
		public FileName FileName {
			get {
				return item.FileName;
			}
		}
		
		public SolutionItemNode(ISolutionFileItem item)
		{
			sortOrder = 2;
			canLabelEdit = true;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionItemNode";
			
			this.solution = item.ParentSolution;
			this.item = item;
			this.Text = Path.GetFileName(FileName);
			SetIcon(IconService.GetImageForFile(FileName));
		}
		
		public override void ActivateItem()
		{
			FileService.OpenFile(FileName);
		}
		
		#region Drag & Drop
		public override DataObject DragDropDataObject {
			get {
				return new DataObject(this);
			}
		}
		
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			return ((ExtTreeNode)Parent).GetDragDropEffect(dataObject, proposedEffect);
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			((ExtTreeNode)Parent).DoDragDrop(dataObject, effect);
		}
		#endregion
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			((ISolutionFolderNode)Parent).Folder.Items.Remove(item);
			base.Remove();
		}
		
		public override bool EnablePaste {
			get {
				return ((ExtTreeNode)Parent).EnablePaste;
			}
		}
		
		public override void Paste()
		{
			((ExtTreeNode)Parent).Paste();
		}
		
		public override bool EnableCopy {
			get {
				return true;
			}
		}
		
		public override void Copy()
		{
			DoPerformCut = true;
			SD.Clipboard.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, false));
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			SD.Clipboard.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, true));
		}
		#endregion
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName == null) {
				return;
			}
			if (!FileService.CheckFileName(newName)) {
				return;
			}
			
			string newFileName = Path.Combine(Path.GetDirectoryName(this.FileName), newName);
			if (!FileService.RenameFile(this.FileName, newFileName, false)) {
				return;
			}
			solution.Save();
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
