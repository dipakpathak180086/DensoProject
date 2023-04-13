﻿using DENSO_PRINTING_BL;
using DENSO_PRINTING_COMMON;
using DENSO_PRINTING_PL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DENSO_PRINTING_APP
{
    public partial class frmSpoolingScaning : Form
    {

        #region Variables

        private BL_SPOOLING _blObj = null;
        private PL_SPOOLING _plObj = null;
        private Common _comObj = null;
        private string _runningPartNo = string.Empty;
        private DataTable dtBindGrid = new DataTable();
        #endregion

        #region Form Methods

        public frmSpoolingScaning()
        {
            try
            {
                InitializeComponent();
                //_blObj = new BL_BOX_LABEL_PRINTING();

            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }
        }

        private void frmModelMaster_Load(object sender, EventArgs e)
        {
            try
            {
                //  this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                DisableFields();
                BindView();


                txtOldPartNo.Focus();
                //dgv.Rows.Add(new object[] { "Line-04","3N1HA949532-4410 500","A949528-9860","A949528-9860","DIPAK PATHAK","A949528-9860"
                //    , "A949528-9860",true,true,true,"4-Mar-23","04:33:33" });
                //     dgv.Rows.Add(new object[] { "Line-04","3N1HA949532-4410 500","A949528-9860","A949528-9860","DIPAK PATHAK","A949528-9860"
                //    , "A949528-9860",true,true,true,"4-Mar-23","04:33:33" });
                //for (int i = 0; i < dgv.ColumnCount; i++)
                //{
                //    this.dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //    //this.dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //    //this.dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                //}
            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }
        }

        #endregion

        #region Button Event
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {

                Clear();


            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Methods

        private void Clear()
        {
            try
            {
                _runningPartNo = "";
                lblMessage.BackColor = Color.Transparent;
                lblMessage.Text = "";
                //for (int i = dgv.Rows.Count-1; i >= 0; i--)
                //{
                //    dgv.Rows.RemoveAt(i);
                //}
                for (int i = dgvLoc.Rows.Count - 1; i >= 0; i--)
                {
                    dgvLoc.Rows.RemoveAt(i);
                }
                txtOldPartNo.Text = txtNewPartNo.Text = txtFeederPartNo.Text = txtLotNo.Text = txtTMName.Text = txtFinalPart.Text = "";
                chkFinalLoading.Checked = chkSoolSelection.Checked = chkSpoolLoading.Checked = false;
                chkFinalLoading.BackColor = chkSoolSelection.BackColor = chkSpoolLoading.BackColor = Color.Yellow;
                DisableFields();
                txtOldPartNo.Focus();

            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }
        }
        void BindView()
        {
            try
            {
                _blObj = new BL_SPOOLING();
                _plObj = new PL_SPOOLING();
                _plObj.DbType = "BIND_VIEW";
                _plObj.Line = GlobalVariable.mLine;
                DataTable dt = _blObj.BL_ExecuteTask(_plObj);
                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt.DefaultView;
                    for (int i = 0; i < dgv.ColumnCount; i++)
                    {
                        this.dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                }
            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }
        }
        void lblShowMessage(string msg = "", int ictr = -1)
        {
            if (ictr == 1)
            {
                lblMessage.BackColor = Color.LightGreen;
                lblMessage.Text = msg;
            }
            else if (ictr == 2)
            {
                lblMessage.BackColor = Color.Red;
                lblMessage.Text = msg;

            }
            else
            {
                lblMessage.BackColor = Color.Transparent;
                lblMessage.Text = msg;
            }
        }
        bool ValidatePartNo(string partNo)
        {
            bool bReturn = false;
            try
            {
                _blObj = new BL_SPOOLING();
                _plObj = new PL_SPOOLING();
                _plObj.DbType = "VALIDATE_PART";
                _plObj.OldPartNo = partNo;
                DataTable dt = _blObj.BL_ExecuteTask(_plObj);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns[0].ColumnName == "RESULT")
                    {
                        lblShowMessage(dt.Rows[0]["RESULT"].ToString(), 2);
                        PlayValidationSound();
                        ShowAccessScreen();
                        bReturn = false;
                    }
                    else
                    {
                        _runningPartNo = dt.Rows[0]["RUNNING_PART"].ToString();

                        bReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }

            return bReturn;

        }
        bool ValidatePartNoAndLot(string partNo,string lotNo)
        {
            bool bReturn = false;
            try
            {
                _blObj = new BL_SPOOLING();
                _plObj = new PL_SPOOLING();
                _plObj.DbType = "VALIDATE_PART_LOT";
                _plObj.Line = GlobalVariable.mLine;
                _plObj.OldPartNo = partNo;
                _plObj.Lot = lotNo;
                DataTable dt = _blObj.BL_ExecuteTask(_plObj);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns[0].ColumnName == "RESULT")
                    {
                        lblShowMessage(dt.Rows[0]["RESULT"].ToString(), 2);
                        PlayValidationSound();
                        ShowAccessScreen();
                        bReturn = false;
                    }
                    else
                    {
                       // _runningPartNo = dt.Rows[0]["RUNNING_PART"].ToString();

                        bReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }

            return bReturn;

        }
        void PlayValidationSound()
        {
            try
            {
                Application.DoEvents();
                SoundPlayer simpleSound = new SoundPlayer(Application.StartupPath + @"\Sound\Beep1.wav");
                simpleSound.Play();
                Thread.Sleep(3000);
                simpleSound.Stop();

            }
            catch (Exception)
            {

                throw;
            }
        }
        void ShowAccessScreen()
        {
            frmAccessPassword oFrmLogin = new frmAccessPassword();
            oFrmLogin.ShowDialog();
            if (GlobalVariable.mAccessUser != "" && oFrmLogin.IsCancel == true)
            {
                lblShowMessage();
            }
        }
        void BindLocationView(string partNo)
        {

            try
            {
                DataTable dtFinal = new DataTable();
                dtFinal.Columns.Add("LocationName");
                dtFinal.Columns.Add("LocationNo");
                _blObj = new BL_SPOOLING();
                _plObj = new PL_SPOOLING();
                _plObj.DbType = "BIND_LOC_VIEW";
                _plObj.OldPartNo = _runningPartNo;
                DataTable dt = _blObj.BL_ExecuteTask(_plObj);
                if (dt.Rows.Count > 0)
                {

                    //dgvLoc.DataSource = dt.DefaultView;
                    for (int i = 0; i < 6; i++)
                    {
                        DataRow dr = dtFinal.NewRow();
                        if (i == 0)
                        {
                            dr["LocationName"] = "Line 2 Back"; //Location1
                        }
                        if (i == 1)
                        {
                            dr["LocationName"] = "Line 2-3 Mid";//Location2
                        }
                        if (i == 2)
                        {
                            dr["LocationName"] = "Line 3-4 Mid";//Location3
                        }
                        if (i == 3)
                        {
                            dr["LocationName"] = "Line 4 Front";//Location4
                        }
                        if (i == 4)
                        {
                            dr["LocationName"] = "Line 5 Back";//Location5
                        }
                        if (i == 5)
                        {
                            dr["LocationName"] = "Line 5 Front";//Location6
                        }
                        dtFinal.Rows.Add(dr);
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Location1"].ToString() != "")
                        {
                            dtFinal.Rows[0][1] = dt.Rows[i]["Location1"].ToString(); //Location1
                        }
                        if (dt.Rows[i]["Location2"].ToString() != "")
                        {
                            dtFinal.Rows[1][1] = dt.Rows[i]["Location2"].ToString(); //Location2
                        }
                        if (dt.Rows[i]["Location3"].ToString() != "")
                        {
                            dtFinal.Rows[2][1] = dt.Rows[i]["Location3"].ToString(); //Location3
                        }
                        if (dt.Rows[i]["Location4"].ToString() != "")
                        {
                            dtFinal.Rows[3][1] = dt.Rows[i]["Location4"].ToString(); //Location4
                        }
                        if (dt.Rows[i]["Location5"].ToString() != "")
                        {
                            dtFinal.Rows[4][1] = dt.Rows[i]["Location5"].ToString(); //Location5
                        }
                        if (dt.Rows[i]["Location6"].ToString() != "")
                        {
                            dtFinal.Rows[5][1] = dt.Rows[i]["Location6"].ToString(); //Location6
                        }
                    }
                    dgvLoc.DataSource = dtFinal.DefaultView;
                    for (int i = 0; i < dgvLoc.ColumnCount; i++)
                    {
                        this.dgvLoc.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        if (i == dgvLoc.ColumnCount - 1)
                        {
                            this.dgvLoc.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                lblShowMessage(ex.Message, 2);
            }



        }
        void DisableFields()
        {
            txtOldPartNo.Enabled = true;
            txtNewPartNo.Enabled = txtFeederPartNo.Enabled = txtLotNo.Enabled = txtTMName.Enabled = txtFinalPart.Enabled = false;
        }


        private bool ValidateInput()
        {
            try
            {

                if (txtOldPartNo.Text.Length == 0)
                {
                    GlobalVariable.MesseageInfo(lblMessage, "Barcode can't be blank!!!", 2);
                    this.txtOldPartNo.Focus();
                    PlayValidationSound();return false;
                }
                if (!txtOldPartNo.Text.Contains(";"))
                {
                    GlobalVariable.MesseageInfo(lblMessage, "Invalid Barcode!!!", 2);
                    this.txtOldPartNo.Focus();
                    PlayValidationSound();return false;
                }
                if (txtOldPartNo.Text.Trim().Split(';').Length != 7)
                {
                    GlobalVariable.MesseageInfo(lblMessage, "Invalid Barcode!!!", 2);
                    this.txtOldPartNo.Focus();
                    PlayValidationSound();return false;
                }
                if (char.IsLetter(txtOldPartNo.Text.Trim().Split(';')[txtOldPartNo.Text.Trim().Split(';').Length - 1], 0) == false)
                {
                    GlobalVariable.MesseageInfo(lblMessage, "Invalid Barcode!!!", 2);
                    this.txtOldPartNo.Focus();
                    PlayValidationSound();return false;
                }
                if (dgv.Rows.Count == 2)
                {
                    dgv.Rows.Clear();
                    dtBindGrid.Rows.Clear();
                    //lblBinCount.Text = "0";
                }


                return true;
            }
            catch (Exception ex) { throw ex; }
        }


        #endregion

        #region Label Event

        #endregion

        #region DataGridView Events


        #endregion

        #region TextBox Event
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    _runningPartNo = "";
                    lblShowMessage();
                    if (txtOldPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Old Part can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (ValidatePartNo(txtOldPartNo.Text.Trim()))
                    {
                        BindLocationView(txtOldPartNo.Text.Trim());

                        txtOldPartNo.Enabled = false;
                        txtNewPartNo.Enabled = true;
                        txtNewPartNo.Focus();
                    }
                    else
                    {
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                    }


                }
                catch (Exception ex)
                {

                    lblShowMessage(ex.Message, 2);
                }
                finally
                {

                }
            }
        }

        private void txtNewPartNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    lblShowMessage();
                    if (txtOldPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Old Part No can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtNewPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "New Part No can't be blank!!!", 2);
                        this.txtNewPartNo.SelectAll();
                        this.txtNewPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (ValidatePartNo(txtNewPartNo.Text.Trim()))
                    {
                        txtNewPartNo.Enabled = false;
                        chkSoolSelection.Checked = true;
                        txtFeederPartNo.Enabled = true;
                        txtFeederPartNo.Focus();
                    }
                    else
                    {
                        this.txtNewPartNo.SelectAll();
                        this.txtNewPartNo.Focus();
                    }


                }
                catch (Exception ex)
                {

                    lblShowMessage(ex.Message, 2);
                }
                finally
                {

                }
            }

        }

        private void txtFeederPartNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    lblShowMessage();
                    if (txtOldPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Old Part No can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtNewPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "New Part No can't be blank!!!", 2);
                        this.txtNewPartNo.SelectAll();
                        this.txtNewPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtFeederPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Feeder Part No can't be blank!!!", 2);
                        this.txtFeederPartNo.SelectAll();
                        this.txtFeederPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    string sfeeder = txtFeederPartNo.Text.Substring(1);
                    if (!sfeeder.EndsWith("-"))
                    {
                        sfeeder = sfeeder + "-";
                    }
                    if (sfeeder == _runningPartNo)
                    {
                        txtFeederPartNo.Enabled = false;
                        chkSpoolLoading.Checked = true;
                        txtLotNo.Enabled = true;
                        txtLotNo.Focus();
                    }
                    else
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Invalid Feeder Part No", 2);
                        this.txtFeederPartNo.SelectAll();
                        this.txtFeederPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }


                }
                catch (Exception ex)
                {

                    lblShowMessage(ex.Message, 2);
                }
                finally
                {

                }
            }
        }

        private void txtLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    lblShowMessage();
                    if (txtOldPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Old Part No can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtNewPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "New Part No can't be blank!!!", 2);
                        this.txtNewPartNo.SelectAll();
                        this.txtNewPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtFeederPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Feeder Part No can't be blank!!!", 2);
                        this.txtFeederPartNo.SelectAll();
                        this.txtFeederPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtLotNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Lot No can't be blank!!!", 2);
                        this.txtLotNo.SelectAll();
                        this.txtLotNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (!ValidatePartNoAndLot(txtNewPartNo.Text.Trim(),txtLotNo.Text.Trim()))
                    {
                        this.txtLotNo.SelectAll();
                        this.txtLotNo.Focus();
                        return;
                    }

                    txtLotNo.Enabled = false;
                    txtTMName.Enabled = true;
                    txtTMName.Focus();




                }
                catch (Exception ex)
                {

                    lblShowMessage(ex.Message, 2);
                }
                finally
                {

                }
            }
        }

        private void txtTMName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    lblShowMessage();
                    if (txtOldPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Old Part No can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtNewPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "New Part No can't be blank!!!", 2);
                        this.txtNewPartNo.SelectAll();
                        this.txtNewPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtFeederPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Feeder Part No can't be blank!!!", 2);
                        this.txtFeederPartNo.SelectAll();
                        this.txtFeederPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtLotNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Lot No can't be blank!!!", 2);
                        this.txtLotNo.SelectAll();
                        this.txtLotNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtTMName.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "TM can't be blank!!!", 2);
                        this.txtTMName.SelectAll();
                        this.txtTMName.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }

                    txtTMName.Enabled = false;
                    txtFinalPart.Enabled = true;
                    txtFinalPart.Focus();




                }
                catch (Exception ex)
                {

                    lblShowMessage(ex.Message, 2);
                }
                finally
                {

                }
            }
        }

        private void txtFinalPart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    lblShowMessage();
                    if (txtOldPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Old Part No can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtNewPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "New Part No can't be blank!!!", 2);
                        this.txtOldPartNo.SelectAll();
                        this.txtOldPartNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtFeederPartNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Feeder Part No can't be blank!!!", 2);
                        this.txtFeederPartNo.SelectAll();
                        this.txtFeederPartNo.Focus();

                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtLotNo.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Lot No can't be blank!!!", 2);
                        this.txtLotNo.SelectAll();
                        this.txtLotNo.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtTMName.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "TM can't be blank!!!", 2);
                        this.txtTMName.SelectAll();
                        this.txtTMName.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtFinalPart.Text.Length == 0)
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Final Part No can't be blank!!!", 2);
                        this.txtFinalPart.SelectAll();
                        this.txtFinalPart.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    if (txtFinalPart.Text.Trim() != txtNewPartNo.Text.Trim())
                    {
                        GlobalVariable.MesseageInfo(lblMessage, "Final part should be equal to new part !!!", 2);
                        this.txtFinalPart.SelectAll();
                        this.txtFinalPart.Focus();
                        PlayValidationSound();
                        ShowAccessScreen();
                        return;
                    }
                    _blObj = new BL_SPOOLING();
                    _plObj = new PL_SPOOLING();
                    _plObj.DbType = "SAVE";
                    _plObj.Line = GlobalVariable.mLine;
                    _plObj.OldPartNo = txtOldPartNo.Text.Trim();
                    _plObj.NewPartNo = txtNewPartNo.Text.Trim();
                    _plObj.FeederPartNo = txtFeederPartNo.Text.Trim();
                    _plObj.Lot = txtLotNo.Text.Trim();
                    _plObj.TMBarcode = txtTMName.Text.Trim();
                    _plObj.FinalPartNo = txtFinalPart.Text.Trim();
                    _plObj.CreatedBy = GlobalVariable.UserName;
                    _plObj.SpoolType = GlobalVariable.mSpoolType;
                    DataTable dt = _blObj.BL_ExecuteTask(_plObj);
                    if (dt.Rows.Count > 0)
                    {
                        BindView();
                        txtFinalPart.Enabled = false;
                        chkFinalLoading.Checked = true;

                    }





                }
                catch (Exception ex)
                {

                    lblShowMessage(ex.Message, 2);
                }
                finally
                {

                }
            }
        }













        #endregion

        private void chkSoolSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSoolSelection.Checked)
            {
                chkSoolSelection.BackColor = Color.Green;
            }
            else
            {
                chkSoolSelection.BackColor = Color.Yellow;
            }
        }

        private void chkSpoolLoading_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSpoolLoading.Checked)
            {
                chkSpoolLoading.BackColor = Color.Green;
            }
            else
            {
                chkSpoolLoading.BackColor = Color.Yellow;
            }
        }

        private void chkFinalLoading_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFinalLoading.Checked)
            {
                chkFinalLoading.BackColor = Color.Green;
            }
            else
            {
                chkFinalLoading.BackColor = Color.Yellow;
            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
