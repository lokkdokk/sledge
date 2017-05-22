﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.Common;
using Vg = Sledge.BspEditor.Primitives.MapData.Visgroup;

namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    public partial class VisgroupEditForm : Form
    {
        private readonly List<Vg> _visgroups;
        private readonly List<Vg> _deleted; 

        public VisgroupEditForm(MapDocument doc)
        {
            InitializeComponent();
            _visgroups = new List<Vg>(doc.Map.Data.Get<Vg>().Select(x => (Vg) x.Clone()));
            _deleted = new List<Vg>();
            UpdateVisgroups();
        }

        public void PopulateChangeLists(MapDocument doc, List<Vg> newVisgroups, List<Vg> changedVisgroups, List<Vg> deletedVisgroups)
        {
            foreach (var g in _visgroups)
            {
                var dg = doc.Map.Data.Get<Vg>().FirstOrDefault(x => x.ID == g.ID);
                if (dg == null) newVisgroups.Add(g);
                else if (dg.Name != g.Name || dg.Colour != g.Colour) changedVisgroups.Add(g);
            }
            deletedVisgroups.AddRange(_deleted.Where(x => doc.Map.Data.Get<Vg>().Any(y => y.ID == x.ID)));
        }

        private void UpdateVisgroups()
        {
            VisgroupPanel.Update(_visgroups);
        }

        private void SelectionChanged(object sender, long? visgroupId)
        {
            ColourPanel.Enabled = RemoveButton.Enabled = GroupName.Enabled = visgroupId.HasValue;
            ColourPanel.BackColor = SystemColors.Control;
            if (visgroupId.HasValue)
            {
                var visgroup = _visgroups.First(x => x.ID == visgroupId.Value);
                GroupName.Text = visgroup.Name;
                ColourPanel.BackColor = visgroup.Colour;
            }
            else
            {
                GroupName.Text = "";
            }
        }

        private long GetNewID()
        {
            var ids = _visgroups.Select(x => x.ID).Union(_deleted.Select(x => x.ID)).ToList();
            return Math.Max(1, ids.Any() ? ids.Max() + 1 : 1);
        }

        private void AddGroup(object sender, EventArgs e)
        {
            var newGroup = new Vg
            {
                                   ID = GetNewID(),
                                   Colour = Colour.GetRandomLightColour(),
                                   Name = "New Group",
                                   Visible = true
                               };
            _visgroups.Add(newGroup);
            UpdateVisgroups();
            VisgroupPanel.SetSelectedVisgroup(newGroup.ID);
            GroupName.SelectAll();
            GroupName.Focus();
        }

        private void RemoveGroup(object sender, EventArgs e)
        {
            var id = VisgroupPanel.GetSelectedVisgroup();
            if (!id.HasValue) return;
            var vg = _visgroups.First(x => x.ID == id.Value);
            _visgroups.Remove(vg);
            _deleted.Add(vg);
            UpdateVisgroups();
        }

        private void GroupNameChanged(object sender, EventArgs e)
        {
            var id = VisgroupPanel.GetSelectedVisgroup();
            if (!id.HasValue) return;
            var vg = _visgroups.First(x => x.ID == id.Value);
            if (vg.Name == GroupName.Text) return;
            vg.Name = GroupName.Text;
            VisgroupPanel.UpdateVisgroupName(id.Value, GroupName.Text);
        }

        private void ColourClicked(object sender, EventArgs e)
        {
            var id = VisgroupPanel.GetSelectedVisgroup();
            if (!id.HasValue) return;
            var vg = _visgroups.First(x => x.ID == id.Value);
            using (var cp = new ColorDialog {Color = vg.Colour})
            {
                if (cp.ShowDialog() == DialogResult.OK)
                {
                    vg.Colour = cp.Color;
                    VisgroupPanel.UpdateVisgroupColour(id.Value, cp.Color);
                }
            }
        }

        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}