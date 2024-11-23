﻿using AsyncAcademy.Data;
using AsyncAcademy.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AsyncAcademy.Utils
{
    public class noto
    {
        public noto() {}


        public void SetViewData(ViewDataDictionary viewData, int notoCount)
        {
            if (notoCount > 0)
            {
                viewData["BellIcon"] = "fa-solid fa-bell";
                viewData["BellNum"] = notoCount.ToString();
            }
            else
            {
                viewData["BellIcon"] = "fa-regular fa-bell";
                viewData["BellNum"] = String.Empty;
            }
        }

        public List<object> NotoData(AsyncAcademyContext context, Submission notification)
        {
            var assignment = context.Assignment.Where(a => a.Id == notification.AssignmentId).First() as Assignment;
            var course = context.Course.Where(c => c.Id == assignment.CourseId).First() as Course;
            string assignmentName = assignment.Title;
            string courseNum = course.CourseNumber;
            var score = notification.PointsGraded;
            var max = assignment.MaxPoints;
            List<object> result = [courseNum, assignmentName, score, max];
            
            return result;
        }
    }
}