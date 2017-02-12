using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public partial class S5CutsceneEditorMain : Form, ILuaDebuggerPlugin
    {
        protected LuaState LS;
        bool freeFlightActive = false;
        int cameraControl = 1;
        float speed = 20;
        S5CameraInfo Camera;
        Cutscene myCutscene;
        Flight selectedFlight;
        FlightPoint selectedFlightPoint = null;
        string tmpPath;
        Properties propertyWindow = new Properties();

        public S5CutsceneEditorMain()
        {
            InitializeComponent();
            btnFlightNrDown.Tag = 1;
            btnFlightNrUp.Tag = -1;

            tmpPath = Path.GetTempPath() + "luaDebugger-yoq/cutsceneEditor";
            try
            {
                Directory.CreateDirectory(tmpPath);
                File.WriteAllText(tmpPath + "/cutscenenames.xml", "<?xml version=\"1.0\" encoding=\"utf-8\" ?><root><Name>Preview</Name></root>");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }



        public void ShowInState(LuaState luaState, Control parent)
        {
            this.Location = new Point(parent.Location.X + parent.Width / 2 - Width / 2, parent.Location.Y + parent.Height / 2 - Height / 2);
            
            this.Show();

            if (this.LS != luaState)
            {
                this.LS = luaState;
                luaState.OnStateRemoved += luaState_OnStateRemoved;
                LS.EvaluateLua(@"
                Cutscene_Preview_Finished = function() 
                    Camera.SetControlMode(1); 
                end
                Cutscene_Preview_Cancel = function() 
                    Camera.SetControlMode(1); 
                end");
                myCutscene = new Cutscene();
                lvCut.Items.Clear();
                cbFlights.Items.Clear();
                cbFlights_SelectedIndexChanged(gbPreviewCutscene, new EventArgs());
                selectedFlight = null;
                selectedFlightPoint = null;
                freeFlightActive = false;
            }
        }

        void luaState_OnStateRemoved(object sender, StateRemovedEventArgs e)
        {
            this.Hide();
            this.LS = null;
            tmrUpdateCamera.Stop();
        }

        private void tmrUpdateCamera_Tick(object sender, EventArgs e)
        {
            if (joyStickCutsceneEditor.CurrentAction != JoyStick.Action.None)
            {
                float yawRad = (float)(Camera.YawAngle * Math.PI / 180);
                float pitchRad = (float)(Camera.PitchAngle * Math.PI / 180);
                Point3D newPos = Camera.Point3D;
                if (joyStickCutsceneEditor.CurrentAction > JoyStick.Action.None)
                {
                    Camera.YawAngle += joyStickCutsceneEditor.Yaw * cameraControl;
                    Camera.PitchAngle += joyStickCutsceneEditor.Pitch * cameraControl;
                    newPos = Camera.Point3D.MoveBy(pitchRad, yawRad, joyStickCutsceneEditor.Speed * speed);
                }
                else
                    switch (joyStickCutsceneEditor.CurrentAction)
                    {
                        case JoyStick.Action.Leftward:
                            newPos = Camera.Point3D.MoveBy(0, (float)(yawRad + Math.PI / 2f), speed);
                            break;
                        case JoyStick.Action.Rightward:
                            newPos = Camera.Point3D.MoveBy(0, (float)(yawRad - Math.PI / 2f), speed);
                            break;
                        case JoyStick.Action.Upward:
                            newPos.Z += speed;
                            break;
                        case JoyStick.Action.Downward:
                            newPos.Z -= speed;
                            break;
                    }
                Camera.Point3D = newPos;
                Camera.WriteToMemory();
            }
        }

        private void btnFreeFlight_Click(object sender, EventArgs e)
        {
            if (!freeFlightActive)
            {
                LS.EvaluateLua(@"
                    gvCamera.DefaultFlag = 0;
                    Camera.ScrollUpdateZMode(3);
                    Display.SetFarClipPlaneMinAndMax(1000000, 0);
                    Camera.ZoomSetDistance(0);
                    Game.GUIActivate(0);
                    Camera.RotSetFlipBack(0);
                    Camera.SetControlMode(1);
                    Display.SetRenderSky(1);
                ");
                Camera = S5CameraInfo.GetCurrentCamera();
                Camera.PosZ += 4000;
                Camera.WriteToMemory();
                tmrUpdateCamera.Start();
                joyStickCutsceneEditor.Enabled = true;

            }
            else
            {
                LS.EvaluateLua(@"
                    Camera_InitParams();
                    Game.GUIActivate(1);
                    Camera.SetControlMode(0);
                ");
                tmrUpdateCamera.Stop();
                joyStickCutsceneEditor.Enabled = false;
            }
            freeFlightActive = !freeFlightActive;
        }

        private void cbInvertCameraControl_CheckedChanged(object sender, EventArgs e)
        {
            cameraControl = cbInvertCameraControl.Checked ? -1 : 1;
        }

        private void cbFlights_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedFlight = cbFlights.SelectedItem as Flight;
            lvCut.Items.Clear();

            if (selectedFlight != null)
            {
                RedrawPointsList();

                btnFlightNrDown.Enabled = true;
                btnFlightNrUp.Enabled = true;
                btnAddPointAbove.Enabled = true;
                btnAddPointBelow.Enabled = true;
                btnRemoveFlight.Enabled = true;
                btnSaveCutscene.Enabled = true;

                gbPreviewCutscene.Enabled = true;
            }
            else
            {
                btnFlightNrDown.Enabled = false;
                btnFlightNrUp.Enabled = false;
                btnAddPointAbove.Enabled = false;
                btnAddPointBelow.Enabled = false;
                btnRemoveFlight.Enabled = false;
                btnSaveCutscene.Enabled = false;
                gbPreviewCutscene.Enabled = false;
            }

            btnPlaySelected.Enabled = false;
            btnReplace.Enabled = false;
        }

        private void RedrawPointsList()
        {
            lvCut.Items.Clear();
            foreach (FlightPoint fp in selectedFlight.FlightPoints)
            {
                ListViewItem point = new ListViewItem(new string[] 
                { 
                    fp.ID.ToString(), 
                    "Edit",
                    fp.LuaCallback
                });
                point.Tag = fp;
                lvCut.Items.Add(point);
            }
        }

        private void btnAddFlight_Click(object sender, EventArgs e)
        {
            if (tbFlightName.Text == "")
            {
                MessageBox.Show("Flight name needs at least 1 character!");
                return;
            }
            Flight newFlight = new Flight(tbFlightName.Text);
            myCutscene.Flights.Add(newFlight);
            cbFlights.Items.Add(newFlight);
            tbFlightName.Clear();
            cbFlights.SelectedItem = newFlight;
        }

        private void btnRemoveFlight_Click(object sender, EventArgs e)
        {
            if (selectedFlight != null)
            {
                myCutscene.Flights.Remove(selectedFlight);
                cbFlights.Items.Remove(selectedFlight);
                selectedFlight = null;
                cbFlights_SelectedIndexChanged(cbFlights, e);
            }

        }

        private void MoveFlightOrder(object sender, EventArgs e)
        {
            int oldIndex = cbFlights.SelectedIndex;
            int newIndex = oldIndex + (int)(sender as Button).Tag;
            if (newIndex >= 0 && newIndex < cbFlights.Items.Count)
            {
                Flight previousFlight = cbFlights.Items[newIndex] as Flight;
                cbFlights.Items[newIndex] = cbFlights.Items[oldIndex];
                cbFlights.Items[oldIndex] = previousFlight;

                myCutscene.Flights[newIndex] = myCutscene.Flights[oldIndex];
                myCutscene.Flights[oldIndex] = previousFlight;
                cbFlights.SelectedIndex = newIndex;
            }
        }

        private void tbFlightName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnAddFlight_Click(tbFlightName, e);
        }

        private void btnAddPointAbove_Click(object sender, EventArgs e)
        {
            var selectedPoints = lvCut.SelectedIndices;
            if (selectedPoints.Count > 0)
            {
                int newIndex = selectedPoints[0];
                if (newIndex >= 0)
                    InsertPointAt(newIndex);
                else
                    InsertPointAt(0);
            }
            else
                InsertPointAt(0);
        }

        private void btnAddPointBelow_Click(object sender, EventArgs e)
        {
            var selectedPoints = lvCut.SelectedIndices;
            if (selectedPoints.Count > 0)
            {
                int newIndex = selectedPoints[0] + 1;
                InsertPointAt(newIndex);
            }
            else
                InsertPointAt(lvCut.Items.Count);
        }

        private void InsertPointAt(int index)
        {
            if (!freeFlightActive)
            {
                MessageBox.Show("Please activate free flight mode!");
                return;
            }
            FlightPoint newFlightPoint = new FlightPoint(Camera, selectedFlight.GetNextPointID());

            ListViewItem point = new ListViewItem(new string[] 
            { 
                newFlightPoint.ID.ToString(), 
                "Edit",
                newFlightPoint.LuaCallback
            });
            point.Tag = newFlightPoint;
            lvCut.Items.Insert(index, point);
            selectedFlight.FlightPoints.Insert(index, newFlightPoint);
        }

        private void btnPlayCutscene_Click(object sender, EventArgs e)
        {
            if (!VerifyCutscene())
                return;
            myCutscene.SaveCutscene(tmpPath + "/cutscene_preview.xml");
            StartPreviewCutscene();
        }

        private void btnPlayFlight_Click(object sender, EventArgs e)
        {
            if (!VerifyCutscene())
                return;
            myCutscene.SaveCutscene(tmpPath + "/cutscene_preview.xml", myCutscene.Flights.IndexOf(selectedFlight));
            StartPreviewCutscene();
        }

        private void btnPlaySelected_Click(object sender, EventArgs e)
        {
            if (!VerifyCutscene())
                return;
            myCutscene.SaveCutscene(tmpPath + "/cutscene_preview.xml",
                myCutscene.Flights.IndexOf(selectedFlight),
                selectedFlight.FlightPoints.IndexOf(selectedFlightPoint));

            StartPreviewCutscene();
        }

        private bool VerifyCutscene()
        {
            foreach (Flight f in myCutscene.Flights)
            {
                if (f.FlightPoints.Count < 2)
                {
                    MessageBox.Show("Flight " + f.Name + " needs at least 2 points");
                    return false;
                }
            }
            return true;
        }

        private void StartPreviewCutscene()
        {
            LS.RunDelegateSafely((MethodInvoker)delegate { S5Direct.ReloadCutscenes(tmpPath); });
            LS.EvaluateLua("Cutscene.Start(\"Preview\")");
        }

        private void lvCut_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedFlightPoint = lvCut.SelectedItems.Count > 0 ? lvCut.SelectedItems[0].Tag as FlightPoint : null;
            btnPlaySelected.Enabled = (selectedFlightPoint != null && lvCut.SelectedIndices[0] != lvCut.Items.Count - 1);
            btnReplace.Enabled = selectedFlightPoint != null;
        }

        private void lvCut_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedFlightPoint == null)
                return;
            if (e.Location.X < chJumpTo.Width)
            {
                Camera.Point3D = selectedFlightPoint.CamPos.Position;
                Camera.PitchAngle = selectedFlightPoint.CamPitch;
                Camera.YawAngle = selectedFlightPoint.CamYaw;
                Camera.WriteToMemory();
            }
            else if (e.Location.X < (chJumpTo.Width + chProperties.Width))
            {
                propertyWindow.ShowProperties(selectedFlightPoint);
            }
            else if (e.Location.X < (chLuaCall.Width + chJumpTo.Width + chProperties.Width))
            {
                selectedFlightPoint.LuaCallback = Interaction.InputBox("Lua Callback Function Name", "S5Cutscene Editor", selectedFlightPoint.LuaCallback);
                lvCut.SelectedItems[0].SubItems[2].Text = selectedFlightPoint.LuaCallback;
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            FlightPoint newPoint = new FlightPoint(Camera, selectedFlight.GetNextPointID());
            int oldIndex = selectedFlight.FlightPoints.IndexOf(selectedFlightPoint);
            selectedFlight.FlightPoints[oldIndex] = newPoint;
            lvCut.SelectedItems[0].Tag = newPoint;
            lvCut.SelectedItems[0].SubItems[0].Text = newPoint.ID.ToString();
            selectedFlightPoint = newPoint;
        }

        private void tbSpeed_Scroll(object sender, EventArgs e)
        {
            speed = tbSpeed.Value;
        }

        private void btnSaveCutscene_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                myCutscene.SaveCutscene(sfd.FileName);
                XElement cs = myCutscene.serialize();
                XDocument d = new XDocument(new XElement("root", cs));
                d.Save(sfd.FileName + ".save");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Cutscene save (*.xml.save)|*.xml.save|All files (*.*)|*.*";
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //IFormatter fo = new BinaryFormatter();
                XDocument d = XDocument.Load(od.FileName);
                Cutscene cs = Cutscene.deserialize(d.Element("root"));
                if (cs==null)
                {
                    MessageBox.Show("Error reading file!");
                    return;
                }
                myCutscene = cs;
                cbFlights.Items.Clear();
                foreach (Flight f in cs.Flights)
                {
                    cbFlights.Items.Add(f);
                }
                selectedFlight = null;
                selectedFlightPoint = null;
                lvCut.Items.Clear();
            }
        }

        private void S5CutsceneEditorMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void lvCut_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int selectedIndex = lvCut.SelectedItems[0].Index;
                selectedFlight.FlightPoints.RemoveAt(selectedIndex);
                lvCut.Items.RemoveAt(selectedIndex);
            }
        }
    }
}
