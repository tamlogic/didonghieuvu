using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;

namespace ManageEmployee.Helpers
{
    public class StringHelpers
    {
     
        public static string GenerateCaseId(string lastId, string parentId)
        {
            int intLastId = 0;
            try
            {
                if (string.IsNullOrEmpty(parentId))
                {
                    intLastId = String.IsNullOrEmpty(lastId) ? 0 : Convert.ToInt32(lastId);
                    return (intLastId + 1).ToString();
                }
                else
                {
                    if (string.IsNullOrEmpty(lastId))
                    {
                        return string.Format("{0}.1", parentId);
                    }

                    IList<string> arrLastId = lastId.Split(".");

                    intLastId = Convert.ToInt32(arrLastId[arrLastId.Count - 1]);

                    return string.Format("{0}.{1}", parentId, intLastId + 1);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            //try
            //{
            //    try
            //    {
            //        var parseLastId = lastId.Split(".");
            //        if (parseLastId.Any())
            //        {
            //            newId = int.Parse(parseLastId[parseLastId.Length - 1]) + 1;
            //        }
            //        else
            //        {
            //            newId = int.Parse(lastId) + 1;
            //        }
            //    }
            //    catch (Exception)
            //    {

            //        newId = int.Parse(lastId) + 1;
            //    }
            //    if (!String.IsNullOrEmpty(parentId))
            //    {
            //        if (newId < 10000)
            //        {
            //            return string.Format("{0}.{1}", parentId, newId.ToString("0000"));
            //        }
            //        else
            //        {
            //            return string.Format("{0}.{1}", parentId, newId.ToString());
            //        }
            //    }
            //    else
            //    {
            //        if (newId < 10000)
            //        {
            //            return string.Format("{0}",  newId.ToString("0000"));
            //        }
            //        else
            //        {
            //            return string.Format("{0}",  newId.ToString());
            //        }
            //    }
             
            //}
            //catch (Exception ex)
            //{
            //    return string.Format("{0}.0001",parentId);
            //}
        }

        public static string GetStringWithMaxLength(string input, int maxLength)
        {
            try
            {
                if(!string.IsNullOrEmpty(input))
                {
                    if (input.Length > maxLength)
                        return input.Substring(0, maxLength) + " ... ";
                }
                return input;
            }
            catch(Exception ex)
            {
                return input;
            }
        }

    }
}
