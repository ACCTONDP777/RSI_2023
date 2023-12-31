﻿using RSI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Models.Manager
{
    public static class H_Form_ApproveManager
    {
        public static bool Approve(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Approve(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static bool insert_Approve(string emp_no, string comment, string rsi_no, string part_type, string phase_id, string tomanager)
        {
            return H_Form_ApproveEntityDAL.insert_Approve(emp_no, comment, rsi_no, part_type, phase_id, tomanager);
        }

        public static int count_Approve(string rsi_no, string part_type)
        {
            return H_Form_ApproveEntityDAL.count_Approve(rsi_no, part_type);
        }

        public static bool Insert_Sourcer_Log(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Insert_Sourcer_Log(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static bool Approve_SourcerForRecordeSN(string emp_no, string comment, string rsi_no, string part_type, string phase_id, string sn)
        {
            return H_Form_ApproveEntityDAL.Approve_SourcerForRecordeSN(emp_no, comment, rsi_no, part_type, phase_id, sn);
        }

        public static bool Approve_SourcerForRecordeNoSN(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Approve_SourcerForRecordeNoSN(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static void Approve_PMConfirm(string emp_no, string comment, string rsi_no, string phase_id)
        {
            H_Form_ApproveEntityDAL.Approve_PMConfirm(emp_no, comment, rsi_no, phase_id);
        }

        public static void Approve_SourcerConfirm(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            H_Form_ApproveEntityDAL.Approve_SourcerConfirm(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static bool Reject(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Reject(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static bool Reject_Sourcer(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Reject_Sourcer(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static void Reject_Return(string emp_no, string comment, string rsi_no, string part_type, string phase_id, string now_phase)
        {
            H_Form_ApproveEntityDAL.Reject_Return(emp_no, comment, rsi_no, part_type, phase_id, now_phase);
        }
        public static bool Reject_Sourcer_Log(string emp_no, string comment, string rsi_no, string part_type, string phase_id, int[] snarr)
        {
            return H_Form_ApproveEntityDAL.Reject_Sourcer_Log(emp_no, comment, rsi_no, part_type, phase_id, snarr);
        }

        public static bool Update_Assigner(string rsi_no, string part_type, string phase_id, string emp_no)
        {
            return H_Form_ApproveEntityDAL.Update_Assigner(rsi_no, part_type, phase_id, emp_no);
        }

        public static void Update_AssignerForSourcer(string rsi_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_AssignerForSourcer(rsi_no, part_type);
        }

        public static void Update_AssignerForReturn(string rsi_no, string part_type, string phase_id, string emp_no)
        {
            H_Form_ApproveEntityDAL.Update_AssignerForReturn(rsi_no, part_type, phase_id, emp_no);
        }
        public static bool Update_Price_Group(string price_group, string rsi_no, string form_no)
        {
            return H_Form_ApproveEntityDAL.Update_Price_Group(price_group, rsi_no, form_no);
        }

        public static void Return_Sourcer(string rsi_no, string part_type, string phase_id, string comment, string emp_no)
        {
            H_Form_ApproveEntityDAL.Return_Sourcer(rsi_no, part_type, phase_id, comment, emp_no);
        }

        public static void ReassignForRDDefineAuth(string bu, string reassign, string mtl_part, string rsi_no, string part_type, string phase_id, string sn, string emp_no)
        {
            H_Form_ApproveEntityDAL.ReassignForRDDefineAuth(bu, reassign, mtl_part, rsi_no, part_type, phase_id, sn, emp_no);
        }

        public static void ReassignApprove(string bu, string reassign, string mtl_part, string rsi_no, string part_type, string phase_id, string emp_no)
        {
            H_Form_ApproveEntityDAL.ReassignApprove(bu, reassign, mtl_part, rsi_no, part_type, phase_id, emp_no);
        }

        public static void Update_AssignerForSourcerReturn(string rsi_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_AssignerForSourcerReturn(rsi_no, part_type);
        }

        public static void ReassignProductSourcerMember(string reassign, string rsi_no, string part_type, string phase_id, string bu, string emp_no)
        {
            H_Form_ApproveEntityDAL.ReassignProductSourcerMember(reassign, rsi_no, part_type, phase_id, bu, emp_no);
        }

        public static void Update_IsApproved_Y(string rsi_no, string emp_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_IsApproved_Y(rsi_no, emp_no, part_type);
        }

        public static void Update_IsApproved_Y_MOH(string rsi_no, string emp_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_IsApproved_Y_MOH(rsi_no, emp_no, part_type);
        }

        public static void Update_IsAssigner_IsApproved_N(string rsi_no, string part_type, string sn, string reassign)
        {
            H_Form_ApproveEntityDAL.Update_IsAssigner_IsApproved_N(rsi_no, part_type, sn, reassign);
        }

        public static void Update_IsApproved_N_byPartType(string rsi_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_IsApproved_N_byPartType(rsi_no, part_type);
        }

        public static void Update_Phaseid_byPartType(string rsi_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_Phaseid_byPartType(rsi_no, part_type);
        }

        public static void Update_Sourcer_Assigner(string rsi_no, string part_type)
        {
            H_Form_ApproveEntityDAL.Update_Sourcer_Assigner(rsi_no, part_type);
        }

        public static bool Approve_Sourcer(string emp_no, string comment, string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Approve_Sourcer(emp_no, comment, rsi_no, part_type, phase_id);
        }

        public static bool Update_Approve_byPass(string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Update_Approve_byPass(rsi_no, part_type, phase_id);
        }

        public static bool Update_Approve_byPass_forRej(string rsi_no, string part_type, string phase_id)
        {
            return H_Form_ApproveEntityDAL.Update_Approve_byPass_forRej(rsi_no, part_type, phase_id);
        }

        //public static bool Check_Reassign_Status(string rsi_no, string part_type, string emp_no)
        //{
        //    return H_Form_ApproveEntityDAL.Check_Reassign_Status(rsi_no, part_type, emp_no);
        //}

        public static bool RDCheckUpdate(string rsi_no, string emp_no)
        {
            return H_Form_ApproveEntityDAL.RDCheckUpdate(rsi_no, emp_no);
        }

        public static void Update_mtl_group_forSpecial(string rsi_no)
        {
            H_Form_ApproveEntityDAL.Update_mtl_group_forSpecial(rsi_no);
        }

        public static bool Insert_b2b_status(string rsi_no, string part_type)
        {
            return H_Form_ApproveEntityDAL.Insert_b2b_status(rsi_no, part_type);
        }
    }
}